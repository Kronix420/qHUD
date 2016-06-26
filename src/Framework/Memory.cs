namespace qHUD.Framework
{
    using Enums;
    using Models;
    using Poe;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class Memory : IDisposable
    {
        public readonly int BaseAddress;
        private bool closed;
        public Offsets offsets;
        private IntPtr procHandle;

        public Memory(Offsets offs, int pId)
        {
            offsets = offs;
            Process = Process.GetProcessById(pId);
            BaseAddress = Process.MainModule.BaseAddress.ToInt32();
            Open();
        }

        public Process Process { get; }

        public void Dispose()
        {
            Close();
        }

        public bool IsInvalid()
        {
            return Process.HasExited || closed;
        }

        public int ReadInt(int addr)
        {
            return BitConverter.ToInt32(ReadMem(addr, 4), 0);
        }

        public int ReadInt(int addr, params int[] _offsets)
        {
            int num = ReadInt(addr);
            return _offsets.Aggregate(num, (current, num2) => ReadInt(current + num2));
        }

        public float ReadFloat(int addr)
        {
            return BitConverter.ToSingle(ReadMem(addr, 4), 0);
        }

        public uint ReadUInt(int addr)
        {
            return BitConverter.ToUInt32(ReadMem(addr, 4), 0);
        }

        public string ReadString(int addr, int length = 256, bool replaceNull = true)
        {
            if (addr <= 65536 && addr >= -1)
            {
                return string.Empty;
            }
            string @string = Encoding.ASCII.GetString(ReadMem(addr, length));
            return replaceNull ? RTrimNull(@string) : @string;
        }

        private static string RTrimNull(string text)
        {
            int num = text.IndexOf('\0');
            return num > 0 ? text.Substring(0, num) : String.Empty;
        }

        public string ReadStringU(int addr, int length = 256, bool replaceNull = true)
        {
            if (addr <= 65536 && addr >= -1)
            {
                return string.Empty;
            }
            byte[] mem = ReadMem(addr, length);
            if (mem[0] == 0 && mem[1] == 0)
                return string.Empty;
            string @string = Encoding.Unicode.GetString(mem);
            return replaceNull ? RTrimNull(@string) : @string;
        }

        public byte ReadByte(int addr)
        {
            return ReadBytes(addr, 1).FirstOrDefault();
        }

        public byte[] ReadBytes(int addr, int length)
        {
            return ReadMem(addr, length);
        }

        private void Open()
        {
            procHandle = WinApi.OpenProcess(Process, ProcessAccessFlags.All);
        }

        private void Close()
        {
            if (closed) return;
            closed = true;
            WinApi.CloseHandle(procHandle);
        }

        private byte[] ReadMem(int addr, int size)
        {
            var array = new byte[size];
            WinApi.ReadProcessMemory(procHandle, (IntPtr)addr, array);
            return array;
        }

        public int[] FindPatterns(params Pattern[] patterns)
        {
            byte[] exeImage = ReadBytes(BaseAddress, 0x2000000); //33mb
            var address = new int[patterns.Length];

            for (int iPattern = 0; iPattern < patterns.Length; iPattern++)
            {
                Pattern pattern = patterns[iPattern];
                byte[] patternData = pattern.Bytes;
                int patternLength = patternData.Length;

                for (int offset = 0; offset < exeImage.Length - patternLength; offset += 4)
                {
                    if (!CompareData(pattern, exeImage, offset)) continue;
                    address[iPattern] = offset;
                    break;
                }
            }
            return address;
        }

        private static bool CompareData(Pattern pattern, byte[] data, int offset)
        {
            return !pattern.Bytes.Where((t, i) => pattern.Mask[i] == 'x' && t != data[offset + i]).Any();
        }
    }
}