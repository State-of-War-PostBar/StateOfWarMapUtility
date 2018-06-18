using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StateOfWarUtility
{
    public class Location : Attribute
    {
        public int offset;
        public Location(int offset) => this.offset = offset;
    }
    
    public static class ListExt
    {
        public static bool SameAs(this IReadOnlyList<byte> lst, IReadOnlyList<byte> other)
        {
            if(lst.Count != other.Count) return false;
            for(int i=0; i<lst.Count; i++)
            {
                if(lst[i] != other[i]) return false;
            }
            return true;
        }
        
        public static byte[] Slice(this IReadOnlyList<byte> lst, int begin, int len)
        {
            byte[] res = new byte[len];
            for(int i=0; i<len; i++) res[i] = lst[begin + i];
            return res;
        }
        
        public static unsafe IList<byte> Set(this IList<byte> lst, int begin, ValueType data, Type type)
        {
            // Only assign the specified offsets.
            foreach(var i in type.GetFields())
            {
                var attrs = i.GetCustomAttributes(typeof(Location), false);
                if(attrs == null || attrs.Length != 1) continue;
                var attr = attrs[0] as Location;
                byte[] sec = null;
                if(i.FieldType == typeof(uint) || i.FieldType.IsEnum) // assume all enum is uint32.
                {
                    sec = BitConverter.GetBytes((uint)i.GetValue(data));
                }
                else if(i.FieldType == typeof(ushort))
                {
                    sec = BitConverter.GetBytes((ushort)i.GetValue(data));
                }
                else if(i.FieldType == typeof(byte))
                {
                    sec = BitConverter.GetBytes((byte)i.GetValue(data));
                }
                else if(i.FieldType == typeof(bool))
                {
                    sec = BitConverter.GetBytes((bool)i.GetValue(data));
                }
                else
                    throw new InvalidOperationException(i.FieldType + " not supported");
                
                // Assert sec != null.
                for(int x = 0; x < sec.Length; x++)
                {
                    lst[begin + x] = sec[x];
                }
            }
            return lst;
        }
        
        public static unsafe ValueType GrabData(this IReadOnlyList<byte> lst, int begin, Type type)
        {
            ValueType data = (ValueType)Activator.CreateInstance(type);
            foreach(var i in type.GetFields())
            {
                var attrs = i.GetCustomAttributes(typeof(Location), false);
                if(attrs == null || attrs.Length != 1) continue;
                var attr = attrs[0] as Location;
                if(i.FieldType == typeof(uint) || i.FieldType.IsEnum) // assume all enum is uint32.
                    i.SetValue(data, BitConverter.ToUInt32(lst.Slice(begin + attr.offset, 4), 0));
                else if(i.FieldType == typeof(ushort))
                    i.SetValue(data, BitConverter.ToUInt16(lst.Slice(begin + attr.offset, 2), 0));
                else if(i.FieldType == typeof(byte))
                    i.SetValue(data, lst[begin + attr.offset]);
                else if(i.FieldType == typeof(bool))
                    i.SetValue(data, lst[begin + attr.offset] == 0 ? false : true);
                else
                    throw new InvalidOperationException(i.FieldType + " not supported");
            }
            return data;
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
