using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace StateOfWarUtility
{
    public static class Srf
    {
        static readonly byte[] jpgHeader = new byte[]{0xFF,0xD8,0xFF,0xE0,0x00,0x10,0x4A,0x46,0x49,0x46,0x00,0x01,0x01,0x01,0x01,0x2C,0x01,0x2C,0x00,0x00,0xFF,0xFE,0x00,0x0F,0x44,0x72,0x61,0x67,0x6F,0x6F,0x6E,0x4B,0x69,0x6C,0x6C,0x65,0x72};
        static readonly byte[] srfHeader = new byte[]{0x41,0x03,0xCD,0xB7,0x9D,0x1C,0x40,0x1C,0x81,0x41,0x21,0x64,0xB2,0x30,0x3F,0xAF,0xD2,0xA8,0xC8,0x18,0x07,0xE0,0xC2,0x35,0x82,0x0F,0x7D,0x73,0x8D,0xB8,0xF6,0x99,0xB1,0xD7,0x58,0x34,0xD4,0xC4,0x9B,0x13,0x31,0x54,0x10,0xCC,0x24,0x29,0xE3,0xFD,0x00,0x64,0xC0,0x00,0x00,0x00,0x01};
        
        static int LocateHeaderEnd(byte[] arr)
        {
            // Find 0xFF, 0xDB sequence for header locating.
            for(int i=1; i<arr.Length; i++)
            {
                bool able = true;
                if(arr[i-1] != 0xFF) able = false;
                if(arr[i-0] != 0xDB) able = false;
                if(able) return i-1;
            }
            throw new ArgumentException();
        }
        
        public static byte[] ToSrf(byte[] src)
        {
            var lst = new List<byte>(src);
            lst.RemoveRange(0, LocateHeaderEnd(src));
            lst.InsertRange(0, srfHeader);
            return lst.ToArray();
        }
        
        public static byte[] ToJpg(byte[] src)
        {
            var lst = new List<byte>(src);
            lst.RemoveRange(0, LocateHeaderEnd(src));
            lst.InsertRange(0, jpgHeader);
            return lst.ToArray();
        }
        
        public static byte[] GetHeader(byte[] src)
        {
            int len = LocateHeaderEnd(src);
            var res = new byte[len];
            for(int i=0; i<len; i++) res[i] = src[i];
            return res;
        }
        
        public static bool ValidateJpg(string path)
        {
            try
            {
                LocateHeaderEnd(File.ReadAllBytes(path));
                return true;
            }
            catch(FileNotFoundException) { return false; }
            catch(FieldAccessException) { return false; }
            catch(AccessViolationException) { return false; }
        }
        
        public static bool ValidateSrf(string path)
        {
            try { return LocateHeaderEnd(File.ReadAllBytes(path)) == srfHeader.Length; }
            catch(FileNotFoundException) { return false; }
            catch(FieldAccessException) { return false; }
            catch(AccessViolationException) { return false; }
        }
    }
    
}
