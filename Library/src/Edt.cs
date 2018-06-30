using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StateOfWarUtility
{
    public interface Unit
    {
        UnitType type { get; set; }
        Owner owner { get; set; }
        uint x { get; set; }
        uint y { get; set; }
    }
    
    // ================================================================================================================
    // Unit section.
    // ================================================================================================================
    
    
    public class BattleUnit : Unit
    {
        internal static readonly List<byte> template = new List<byte>() {
            0xE0,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x50,0x02,0x00,0x00,
            0xF0,0x01,0x00,0x00 };
        
        internal static int length { get => template.Count; }
        
        [Location(0x4)] public UnitType type { get; set; }
        [Location(0x8)] public Owner owner { get; set; }
        [Location(0xC)] public uint x { get; set; }
        [Location(0x10)] public uint y { get; set; }
        
        internal BattleUnit() => Access(0, template);
        internal BattleUnit(int begin, List<byte> arr) => Access(begin, arr);        
        internal void Access(int begin, List<byte> arr) => arr.GrabData(begin, this);
        internal void AppendTo(List<byte> arr)
        {
            arr.AddRange(template);
            arr.Set(arr.Count - length, this);
        }
        
        public BattleUnit Clone() => (BattleUnit)MemberwiseClone();
        
        internal static bool CheckHeader(int begin, List<byte> arr) => template.Slice(0, 4).SameAs(arr.Slice(begin, 4));
    }
    
    public class UnitManager : IEnumerable<BattleUnit>
    {
        readonly List<BattleUnit> data = new List<BattleUnit>();
        
        internal UnitManager() { }
        public int count { get => data.Count;}
        
        public BattleUnit this[int index] { get => data[index]; }
        
        public BattleUnit Add() => Add(count);
        public BattleUnit Add(int before)
        {
            if(before < 0 || before > count)
                throw new InvalidOperationException(
                    string.Format("cannot insert the element before {0}, array length is {1}.", before, count));
            BattleUnit unit = new BattleUnit();
            data.Insert(before, unit);
            return unit;
        }
        
        public BattleUnit Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            BattleUnit rm = data[index];
            data.RemoveAt(index);
            return rm;
        }
        
        public IEnumerator<BattleUnit> GetEnumerator() => data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
    
    // ================================================================================================================
    // Building section.
    // ================================================================================================================
    
    
    public class Building
    {
        internal static readonly List<byte> template = new List<byte>() {
            0x7B,0x00,0x00,0x00,0x64,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x58,0x34,0x00,0x00,
            0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,
            0x3A,0x00,0x00,0x00,0x35,0x00,0x00,0x00,
            0x63,0x00,0x00,0x00 };
        internal static int length { get => template.Count; }
        
        [Location(0x04)] public UnitType type { get; set; }
        [Location(0x08)] public uint level { get; set; }
        [Location(0x0C)] public UnitType production0 { get; set; }
        [Location(0x10)] public UnitType production1 { get; set; }
        [Location(0x14)] public UnitType production2 { get; set; }
        [Location(0x18)] public UnitType production3 { get; set; }
        [Location(0x1C)] public UnitType production4 { get; set; }
        [Location(0x20)] public uint upgrade0 { get; set; }
        [Location(0x24)] public uint upgrade1 { get; set; }
        [Location(0x28)] public uint upgrade2 { get; set; }
        [Location(0x2C)] public uint upgrade3 { get; set; }
        [Location(0x30)] public uint upgrade4 { get; set; }
        [Location(0x38)] public Owner owner { get; set; }
        [Location(0x3C)] public bool satellite { get; set; }
        [Location(0x40)] public uint x { get; set; }
        [Location(0x44)] public uint y { get; set; }
        [Location(0x48)] public uint health { get; set; }
        
        internal Building() => Access(0, template);
        internal Building(int begin, List<byte> arr) => Access(begin, arr);        
        internal void Access(int begin, List<byte> arr) => arr.GrabData(begin, this);
        internal void AppendTo(List<byte> arr)
        {
            arr.AddRange(template);
            arr.Set(arr.Count - length, this);
        }
        
        public Building Clone() => (Building)MemberwiseClone();
        
        internal static bool CheckHeader(int begin, List<byte> arr) => template.Slice(0, 4).SameAs(arr.Slice(begin, 4));
    }
    
    
    public class BuildingManager : IEnumerable<Building>
    {
        readonly List<Building> data = new List<Building>();
        
        internal BuildingManager() { }
        
        public int count { get => data.Count; }
        
        public Building this[int index] { get => data[index]; }
        
        public Building Add() => Add(count);
        public Building Add(int before)
        {
            if(before < 0 || before > count)
                throw new InvalidOperationException(
                    string.Format("cannot insert the element before {0}, array length is {1}.", before, count));
            Building building = new Building();
            data.Insert(before, building);
            return building;
        }
        
        public Building Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            Building rm = data[index];
            data.RemoveAt(index);
            return rm;
        }
        
        public IEnumerator<Building> GetEnumerator() => data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }
    
    // ================================================================================================================
    // Edt control and global settings section.
    // ================================================================================================================
    
    
    public class EdtInfo
    {
        public static readonly List<byte> edtHeader = new List<byte> {
            0x04,0x00,0x8E,0x26,0x01,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x03,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x0F,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x02,0x00,0x00,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x03,0x00,0x00,0x00
            ,0x0D,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x04,0x00,0x00,0x00,0x0C,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x05,0x00,0x00,0x00,0x0B,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x01,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x01,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00,0x00
            ,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x90,0x01,0x00,0x00,0x09,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x21,0x00,0x00,0x00,0x01,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            ,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 };
        
        public static readonly List<byte> edtTail = new List<byte>() {0xE8, 0x1D, 0x00, 0x00};
        
        internal static int length { get => edtHeader.Count; }
        
        [Location(0x00)] public ushort number;
        
        [Location(0x08)] public uint pMoney;
        [Location(0x0C)] public uint nMoney;
        
        [Location(0x94)] public uint pResearch;
        [Location(0x98)] public uint nResearch;
        
        [Location(0x14)] public uint pBomber;
        [Location(0x18)] public uint nBomber;
        [Location(0x20)] public uint pMeteor;
        [Location(0x24)] public uint nMeteor;
        [Location(0x2C)] public uint pCarrier;
        [Location(0x30)] public uint nCarrier;
        [Location(0x38)] public uint pTripler;
        [Location(0x3C)] public uint nTripler;
        [Location(0x44)] public uint pFighter;
        [Location(0x48)] public uint nFighter;
        
        [Location(0x5C)] public bool pTurretDefence;
        [Location(0x5D)] public bool pTurretAntiair;
        [Location(0x5E)] public bool pTurretIon;
        [Location(0x5F)] public bool pTurretLed;
        [Location(0x60)] public bool pTurretCluster;
        
        [Location(0x66)] public bool nTurretDefence;
        [Location(0x67)] public bool nTurretAntiair;
        [Location(0x68)] public bool nTurretIon;
        [Location(0x69)] public bool nTurretLed;
        [Location(0x6A)] public bool nTurretCluster;
        
        [Location(0x7C)] public uint pDiskAttack;
        [Location(0x80)] public uint nDiskAttack;
        
        [Location(0x88)] public DiskRebuildType pDisk;
        [Location(0x8C)] public DiskRebuildType nDisk;
        
        [Location(0x104)] public bool hasTimeLimit;
        [Location(0x108)] public uint timeLimit;
        [Location(0x10C)] public TimeLimitType timeLimitType;
        
        internal EdtInfo() => Access(0, edtHeader);
        internal EdtInfo(int begin, List<byte> arr) => Access(begin, arr);        
        internal void Access(int begin, List<byte> arr) => arr.GrabData(begin, this);
        internal void AppendTo(List<byte> arr)
        {
            arr.AddRange(edtHeader);
            arr.Set(arr.Count - length, this);
        }
    }
    
    public sealed class Edt
    {
        public readonly EdtInfo headerInfo = new EdtInfo();
        public readonly UnitManager units = new UnitManager();
        public readonly BuildingManager buildings = new BuildingManager();
        
        public Edt(string path) : this(File.ReadAllBytes(path)) { }
        public Edt(byte[] data) : this(new List<byte>(data)) { }
        public Edt(List<byte> data) => FromBytes(data);
        
        public void FromBytes(List<byte> data)
        {
            headerInfo.Access(0, data);
            int cur = EdtInfo.length;
            while(cur < data.Count && Building.CheckHeader(cur, data))
            {
                buildings.Add().Access(cur, data);
                cur += Building.length;
            }
            while(cur < data.Count && BattleUnit.CheckHeader(cur, data))
            {
                units.Add().Access(cur, data);
                cur += BattleUnit.length;
            }
        }
        
        public List<byte> ToBytes()
        {
            var data = new List<byte>();
            headerInfo.AppendTo(data);
            foreach(var i in buildings)
                i.AppendTo(data);
            foreach(var i in units)
                i.AppendTo(data);
            data.AddRange(EdtInfo.edtTail);
            return data;
        }
        
        public void Save(string path) => File.WriteAllBytes(path, ToBytes().ToArray());
        
        public static bool Validate(string path)
        {
            try
            {
                var data = File.ReadAllBytes(path);
                // var head = data.Slice(0, EdtInfo.edtHeader.Count);
                var tail = data.Slice(data.Length - EdtInfo.edtTail.Count, EdtInfo.edtTail.Count);
                // return EdtInfo.edtHeader.SameAs(head) || EdtInfo.edtTail.SameAs(tail);
                return true;
            }
            catch(Exception) { return false; }
        }
    }
    
    
}
