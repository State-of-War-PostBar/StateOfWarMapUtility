using System;
using System.IO;
using System.Collections.Generic;
using System.Text;


namespace StateOfWarUtility
{
    public class ByteFile
    {
        public readonly List<byte> data;
        public ByteFile(string filePath) : this(File.ReadAllBytes(filePath)) { }
        public ByteFile(byte[] raw) { data = new List<byte>(raw); }
        public void Save(string path) => File.WriteAllBytes(path, data.ToArray());
    }
}
