using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EasyCodeClass.Multimedia.Audio;
using EasyCodeClass.Multimedia.Audio.Wave;
using EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop;
using System.Runtime.InteropServices;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom
{
    public delegate void TrackReaded(TrackReadEventArgs e);

    public class CompactDisc : ICloneable
    {
        const int OPEN_EXISTING = 3;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint IOCTL_STORAGE_EJECT_MEDIA = 2967560;

        [DllImport("kernel32")]
        private static extern IntPtr CreateFile
            (string filename, uint desiredAccess,
             uint shareMode, IntPtr securityAttributes,
             int creationDisposition, int flagsAndAttributes,
             IntPtr templateFile);

        [DllImport("kernel32")]
        private static extern int DeviceIoControl
            (IntPtr deviceHandle, uint ioControlCode,
             IntPtr inBuffer, int inBufferSize,
             IntPtr outBuffer, int outBufferSize,
             ref int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32")]
        private static extern int CloseHandle(IntPtr handle);

        public static CompactDisc[] GetAllDiscs()
        {
            List<CompactDisc> discs = new List<CompactDisc>();
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                DriveInfo di = new DriveInfo(drive);
                if(di.DriveType == DriveType.CDRom)
                {
                    discs.Add(new CompactDisc(drive[0]));
                }
            }
            return discs.ToArray();
        }

        public readonly char DriveLetter = 'c';

        public readonly string Name = "";

        protected CompactDisc()
        {

        }

        protected CompactDisc(char drive)
        {
            DriveLetter = drive;
            try
            {
                DriveInfo di = new DriveInfo(drive + ":\\\\");
                Name = di.VolumeLabel;
            }
            catch { }
        }

        public void GetData(TrackReaded trackReaded/*, out int InvaildDataErrors*/)
        {
            WaveFormat format = new WaveFormat();
            format.SampleRate = 44100;
            format.bitsPerSample = BitsPerSample.WAV16;
            format.Channels = 2;

            // Contains code from https://www.c-sharpcorner.com/UploadFile/moosestafa/converting-cda-to-wav/

            if(ExternalFunctions.GetDriveType(DriveLetter + ":\\") == DriveTypes.DRIVE_CDROM)
            {
                uint dummy = 0;
                IntPtr cdHandle = ExternalFunctions.CreateFile("\\\\.\\" + DriveLetter + ':', Constances.GENERIC_READ, Constances.FILE_SHARE_READ, IntPtr.Zero, Constances.OPEN_EXISTING, 0, IntPtr.Zero);
                bool isReady = ExternalFunctions.DeviceIoControl(cdHandle, Constances.IOCTL_STORAGE_CHECK_VERIFY, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero) == 1;
                if (isReady)
                {
                    try
                    {
                        CDROM_TOC TOC = new CDROM_TOC();
                        IntPtr pointer = Marshal.AllocHGlobal((IntPtr)(Marshal.SizeOf(TOC)));
                        Marshal.StructureToPtr(TOC, pointer, false);
                        uint BytesRead = 0;
                        bool tocValid = ExternalFunctions.DeviceIoControl(cdHandle, Constances.IOCTL_CDROM_READ_TOC, IntPtr.Zero, 0, pointer, (uint)Marshal.SizeOf(TOC), ref BytesRead, IntPtr.Zero) != 0;
                        Marshal.PtrToStructure(pointer, TOC);
                        if (tocValid)
                        {
                            uint TrackCount = TOC.LastTrack;
                            byte[][] TrackData = new byte[TrackCount][];
                            int errors = 0;
                            for (int track = 0; track < TrackCount; track++)
                            {
                                long offset = 0;
                                //MemoryStream ms = new MemoryStream();
                                //BinaryWriter bw = new BinaryWriter(ms);
                                TRACK_DATA td = TOC.TrackData[track];
                                int StartSector = (td.Address_1 * 60 * 75 + td.Address_2 * 75 + td.Address_3) - 150;
                                td = TOC.TrackData[track + 1];
                                int EndSector = (td.Address_1 * 60 * 75 + td.Address_2 * 75 + td.Address_3) - 151;
                                uint TrackSize = (uint)(EndSector - StartSector) * Constances.CB_AUDIO;
                                TrackData[track] = new byte[TrackSize];
                                byte[] SectorData = new byte[Constances.CB_AUDIO * Constances.NSECTORS];
                                for (int sector = StartSector; sector < EndSector; sector += Constances.NSECTORS)
                                {
                                    RAW_READ_INFO rri = new RAW_READ_INFO();
                                    rri.TrackMode = TRACK_MODE_TYPE.CDDA;
                                    rri.SectorCount = (uint)1;
                                    rri.DiskOffset = sector * Constances.CB_CDROMSECTOR;
                                    Marshal.StructureToPtr(rri, pointer, false);
                                    int size = Marshal.SizeOf(SectorData[0]) * SectorData.Length;
                                    IntPtr pointer2 = Marshal.AllocHGlobal(size);
                                    SectorData.Initialize();
                                    int i = ExternalFunctions.DeviceIoControl(cdHandle, Constances.IOCTL_CDROM_RAW_READ, pointer, (uint)Marshal.SizeOf(rri), pointer2, (uint)Constances.NSECTORS * Constances.CB_AUDIO, ref BytesRead, IntPtr.Zero);
                                    if (i == 0)
                                    {
                                        errors++;
                                        break;
                                    }
                                    Marshal.PtrToStructure(pointer, rri);
                                    Marshal.Copy(pointer2, SectorData, 0, SectorData.Length);
                                    Marshal.FreeHGlobal(pointer2);
                                    Array.Copy(SectorData, 0, TrackData[track], offset, BytesRead);
                                    offset += BytesRead;
                                }
                                //bw.Write(TrackData[track]);
                                //bw.Close();

                                WaveData wd = new WaveData();
                                wd.format = (WaveFormat)format.Clone();
                                wd.format.parent = wd;

                                int dataLength = TrackData[track].Length;
                                int readedLength = 0;
                                List<List<long>> dat = new List<List<long>>();

                                BinaryReader reader = new BinaryReader(new MemoryStream(TrackData[track]));

                                for (int c = 0; c < format.Channels; c++)
                                {
                                    dat.Add(new List<long>());
                                }

                                while (dataLength > readedLength)
                                {
                                    for (int c = 0; c < format.Channels; c++)
                                    {
                                        if (format.bitsPerSample == BitsPerSample.WAV16)
                                        {
                                            dat[c].Add(reader.ReadInt16());
                                            readedLength += 2;
                                        }
                                        if (format.bitsPerSample == BitsPerSample.WAV32)
                                        {
                                            dat[c].Add(reader.ReadInt32());
                                            readedLength += 4;
                                        }
                                        if (format.bitsPerSample == BitsPerSample.WAV64)
                                        {
                                            dat[c].Add(reader.ReadInt64());
                                            readedLength += 8;
                                        }
                                    }
                                }

                                int checkLength = dat[0].Count;
                                foreach (List<long> l in dat)
                                {
                                    if (checkLength != l.Count) throw new InvalidCastException();
                                }

                                wd.data = dat;

                                trackReaded(new TrackReadEventArgs(wd, track));
                            }
                            PREVENT_MEDIA_REMOVAL pmr = new PREVENT_MEDIA_REMOVAL();
                            pmr.PreventMediaRemoval = 0;
                            pointer = Marshal.AllocHGlobal((IntPtr)(Marshal.SizeOf(pmr)));
                            Marshal.StructureToPtr(pmr, pointer, false);
                            ExternalFunctions.DeviceIoControl(cdHandle, Constances.IOCTL_STORAGE_MEDIA_REMOVAL, pointer, (uint)Marshal.SizeOf(pmr), IntPtr.Zero, 0, ref dummy, IntPtr.Zero);
                            Marshal.PtrToStructure(pointer, pmr);
                            Marshal.FreeHGlobal(pointer);

                            ExternalFunctions.CloseHandle(cdHandle);

                            // Save as Wavedata

                            /*int tracks = TrackData.GetUpperBound(0) + 1;

                            WaveData[] data = new WaveData[tracks];
                            for(int i = 0; i < data.Length; i++)
                            {
                                data[i] = new WaveData();
                                data[i].format = (WaveFormat)format.Clone();
                                data[i].format.parent = data[i];

                                int dataLength = TrackData[i].Length;
                                int readedLength = 0;
                                List<List<long>>  dat = new List<List<long>>();

                                BinaryReader reader = new BinaryReader(new MemoryStream(TrackData[i]));

                                for (int c = 0; c < format.Channels; c++)
                                {
                                    dat.Add(new List<long>());
                                }

                                while (dataLength > readedLength)
                                {
                                    for (int c = 0; c < format.Channels; c++)
                                    {
                                        if (format.bitsPerSample == BitsPerSample.WAV16)
                                        {
                                            dat[c].Add(reader.ReadInt16());
                                            readedLength += 2;
                                        }
                                        if (format.bitsPerSample == BitsPerSample.WAV32)
                                        {
                                            dat[c].Add(reader.ReadInt32());
                                            readedLength += 4;
                                        }
                                        if (format.bitsPerSample == BitsPerSample.WAV64)
                                        {
                                            dat[c].Add(reader.ReadInt64());
                                            readedLength += 8;
                                        }
                                    }
                                }

                                int checkLength = dat[0].Count;
                                foreach (List<long> l in dat)
                                {
                                    if (checkLength != l.Count) throw new InvalidCastException();
                                }

                                data[i].data = dat;
                            }

                            InvaildDataErrors = errors;
                            return data;*/
                        }
                        else
                        {
                            ExternalFunctions.CloseHandle(cdHandle);
                            throw new InvalidDataException();
                        }
                    }
                    catch
                    {
                        ExternalFunctions.CloseHandle(cdHandle);
                        throw new Exception();
                    }
                }
                else
                {
                    ExternalFunctions.CloseHandle(cdHandle);
                    throw new DriveNotFoundException();
                }
            }
            else
            {
                throw new DriveNotFoundException();
            }
        }

        public void Eject() // https://stackoverflow.com/questions/1449410/programatically-ejecting-and-retracting-the-cd-drive-in-vb-net-or-c-sharp
        {
            string path = "\\\\.\\" + DriveLetter + ":";
            IntPtr handle = CreateFile(path, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
                                       IntPtr.Zero, OPEN_EXISTING, 0,
                                       IntPtr.Zero);
            if ((long)handle == -1)
            {
                throw new IOException("Unable to open drive " + DriveLetter);
            }
            int dummy = 0;
            DeviceIoControl(handle, IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0,
                            IntPtr.Zero, 0, ref dummy, IntPtr.Zero);
            CloseHandle(handle);
        }

        public object Clone()
        {
            return new CompactDisc(DriveLetter);
        }
    }

    public class TrackReadEventArgs
    {
        public WaveData track;
        public int trackNumber;

        public TrackReadEventArgs(WaveData Track, int TrackNumber)
        {
            this.track = Track;
            this.trackNumber = TrackNumber;
        }
    }
}
