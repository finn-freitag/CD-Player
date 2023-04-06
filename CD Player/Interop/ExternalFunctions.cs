using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyCodeClass.Multimedia.Audio.Windows.CDRom.Interop
{
    public class ExternalFunctions
    {
        [DllImport("Kernel32.dll")]
        public extern static int FlushFileBuffers(IntPtr FileHandle);

        [DllImport("Kernel32.dll")]
        public extern static DriveTypes GetDriveType(string drive);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static IntPtr CreateFile(string FileName, uint DesiredAccess,
          uint ShareMode, IntPtr lpSecurityAttributes,
          uint CreationDisposition, uint dwFlagsAndAttributes,
          IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern int WriteFile(IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer, // also tried this.
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        IntPtr lpOverlapped);




        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(IntPtr hDevice, uint IoControlCode,
          IntPtr lpInBuffer, uint InBufferSize,
          IntPtr lpOutBuffer, uint nOutBufferSize,
          ref uint lpBytesReturned,
          IntPtr lpOverlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int CloseHandle(IntPtr hObject);
    }
}
