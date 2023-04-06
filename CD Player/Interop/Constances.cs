using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop
{
    public class Constances
    {
        public const int NSECTORS = 1;// 13;
        public const int UNDERSAMPLING = 1;
        public const int CB_CDDASECTOR = 2368;
        public const int CB_QSUBCHANNEL = 16;
        public const int CB_CDROMSECTOR = 2048;
        public const int CB_AUDIO = (CB_CDDASECTOR - CB_QSUBCHANNEL);

        public const uint IOCTL_CDROM_READ_TOC = 0x00024000;
        public const uint IOCTL_STORAGE_CHECK_VERIFY = 0x002D4800;
        public const uint IOCTL_CDROM_RAW_READ = 0x0002403E;
        public const uint IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804;
        public const uint IOCTL_STORAGE_EJECT_MEDIA = 0x002D4808;
        public const uint IOCTL_STORAGE_LOAD_MEDIA = 0x002D480C;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_ALL = 0x10000000;

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_SHARE_DELETE = 0x00000004;

        //CreationDisposition constants
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        public const int MAXIMUM_NUMBER_TRACKS = 100;
    }
}
