using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StateOfWarUtility
{
    public static class ListExt
    {
        public static byte[] Slice(this List<byte> lst, int begin, int len)
        {
            byte[] res = new byte[len];
            for(int i=0; i<len; i++) res[i] = lst[begin + i];
            return res;
        }
        
        public static unsafe List<byte> Set(this List<byte> lst, int begin, void* data, Type type)
        {
            // Only assign the specified offsets.
            foreach(var i in type.GetFields())
            {
                byte* d = (byte*)data;
                var attrs = i.GetCustomAttributes(typeof(FieldOffsetAttribute), false);
                if(attrs == null || attrs.Length != 1) continue;
                var attr = attrs[0] as FieldOffsetAttribute;
                if(i.FieldType == typeof(uint) || i.FieldType.IsEnum) // assume all enum is uint32.
                {
                    
                    lst[begin + attr.Value + 0] = d[attr.Value + 0];
                    lst[begin + attr.Value + 1] = d[attr.Value + 1];
                    lst[begin + attr.Value + 2] = d[attr.Value + 2];
                    lst[begin + attr.Value + 3] = d[attr.Value + 3];
                }
                else if(i.FieldType == typeof(ushort))
                {
                    lst[begin + attr.Value + 0] = d[attr.Value + 0];
                    lst[begin + attr.Value + 1] = d[attr.Value + 1];
                }
                else if(i.FieldType == typeof(byte) || i.FieldType == typeof(bool))
                {
                    lst[begin + attr.Value + 0] = d[attr.Value + 0];
                }
                else
                    throw new InvalidOperationException(i.FieldType + " not supported");
            }
            return lst;
        }
        
        public static unsafe void GrabData(this List<byte> lst, int begin, void* data, Type type)
        {
            foreach(var i in type.GetFields())
            {
                var attrs = i.GetCustomAttributes(typeof(FieldOffsetAttribute), false);
                if(attrs == null || attrs.Length != 1) continue;
                var attr = attrs[0] as FieldOffsetAttribute;
                if(i.FieldType == typeof(uint) || i.FieldType.IsEnum) // assume all enum is uint32.
                    *((uint*)((byte*)data + attr.Value)) = BitConverter.ToUInt32(lst.Slice(begin + attr.Value, 4), 0);
                else if(i.FieldType == typeof(ushort))
                    *((ushort*)((byte*)data + attr.Value)) = BitConverter.ToUInt16(lst.Slice(begin + attr.Value, 2), 0);
                else if(i.FieldType == typeof(byte) || i.FieldType == typeof(bool))
                    ((byte*)data)[attr.Value] = lst[begin + attr.Value];
                else
                    throw new InvalidOperationException(i.FieldType + " not supported");
            }
        }
    }
    
    internal static class Util
    {
        internal static byte[] ReverseIfNecessary(byte[] src)
        {
            if(!BitConverter.IsLittleEndian) Array.Reverse(src);
            return src;
        }
    }
    
    public class Ref<T>
    {
        public T value;
        public Ref(T val) { this.value = val; }
        public static implicit operator T(Ref<T> x) => x.value;
    }
    
    
    
}
