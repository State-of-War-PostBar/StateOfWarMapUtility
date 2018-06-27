using System;
using System.Reflection;
using System.Collections;
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
        
        
        internal struct PropertyInfo
        {
            public FieldInfo field;
            public int offset;
            public TypeCode type;
        }
        
        internal class TypeInfo : IEnumerable<PropertyInfo>
        {
            public readonly List<PropertyInfo> info = new List<PropertyInfo>();
            
            public TypeInfo(Type type)
            {
                foreach(var i in type.GetFields())
                {
                    var attrs = i.GetCustomAttributes(typeof(Location), false);
                    if(attrs == null) continue;
                    Location attr = null;
                    foreach(var k in attrs)
                    {
                        attr = k as Location;
                        if(attr != null) break;
                    }
                    if(attr == null) continue;
                    
                    TypeCode code = TypeCode.Object;
                    if(i.FieldType == typeof(uint) || i.FieldType.IsEnum) // assume all enum is uint32.
                        code = TypeCode.UInt32;
                    else if(i.FieldType == typeof(ushort))
                        code = TypeCode.UInt16;
                    else if(i.FieldType == typeof(byte))
                        code = TypeCode.Byte;
                    else if(i.FieldType == typeof(bool))
                        code = TypeCode.Boolean;
                    else
                        throw new InvalidOperationException(i.FieldType + " not supported.");
                    
                    info.Add(new PropertyInfo() { field = i, offset = attr.offset, type = code });
                }
            }
            
            public IEnumerator<PropertyInfo> GetEnumerator() => info.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
        }
        
        static readonly Dictionary<Type, TypeInfo> typeInfoCache = new Dictionary<Type, TypeInfo>(); 
        
        static TypeInfo GetTypeInfo(Type type)
        {
            TypeInfo info = null;
            typeInfoCache.TryGetValue(type, out info);
            if(info != null) return info;
            info = new TypeInfo(type);
            typeInfoCache.Add(type, info);
            return info;
        }
        
        public static IList<byte> Set(this IList<byte> lst, int begin, object data)
        {
            // Only assign the specified offsets.
            foreach(var i in GetTypeInfo(data.GetType()))
            {
                byte[] sec = null;
                switch(i.type)
                {
                    case TypeCode.UInt32: sec = BitConverter.GetBytes((uint)i.field.GetValue(data)); break;
                    case TypeCode.UInt16: sec = BitConverter.GetBytes((ushort)i.field.GetValue(data)); break;
                    case TypeCode.Byte: sec = BitConverter.GetBytes((byte)i.field.GetValue(data)); break;
                    case TypeCode.Boolean: sec = BitConverter.GetBytes((bool)i.field.GetValue(data)); break;
                    default: break;
                }
                
                // Assert sec != null.
                for(int x = 0; x < sec.Length; x++)
                    lst[begin + i.offset + x] = sec[x];
            }
            return lst;
        }
        
        public static void GrabData(this IReadOnlyList<byte> lst, int begin, object data)
        {
            foreach(var i in GetTypeInfo(data.GetType()))
            {
                switch(i.type)
                {
                    case TypeCode.UInt32: i.field.SetValue(data, BitConverter.ToUInt32(lst.Slice(begin + i.offset, 4), 0)); break;
                    case TypeCode.UInt16: i.field.SetValue(data, BitConverter.ToUInt16(lst.Slice(begin + i.offset, 2), 0)); break;
                    case TypeCode.Byte: i.field.SetValue(data, lst[begin + i.offset]); break;
                    case TypeCode.Boolean: i.field.SetValue(data, lst[begin + i.offset] == 1); break;
                    default: throw new InvalidOperationException(i.field.FieldType + " not supported");
                }
            }
        }
        
        public static IEnumerable<T> Cast<T>(this Array x)
        {
            foreach(var item in x)
                yield return (T)item;
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
