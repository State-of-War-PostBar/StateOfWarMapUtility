using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StateOfWarUtility
{
    public enum UnitType : uint
    {
        None = 0,
        
        LAntiair = 1,
        MAntiair = 2,
        HAntiair = 3,
        LArti = 4,
        MArti = 5,
        HArti = 6,
        LArmor = 7,
        MArmor = 8,
        HArmor = 9,
        LFlame = 10,
        MFlame = 11,
        HFlame = 12,
        LSpec = 13,
        MSpec = 14,
        HSpec = 15,
        
        Gold1 = 16,
        Gold2 = 17,
        Gold3 = 18,
        Research1 = 19,
        Research2 = 20,
        Research3 = 21,
        Power1 = 22,
        Power2 = 23,
        Power3 = 24,
        
        Bomber = 30,
        Carrier = 31,
        Fighter = 32,
        Tripler = 33,
        Meteor = 34,
        
        Disk = 40,
        Codiak = 41,
        Revenger = 42,
        Cougar = 43,
        Gattling = 44,
        Achilles = 45,
        Rogon = 46,
    }
    
    public enum BuildingType : uint
    {
        None = 0,
        
        TurretDefence = 25,
        TurretAntiair = 26,
        TurretIon = 27,
        TurretLed = 28,
        TurretCluster = 29,
        Headquater = 100,
        LightFactory = 101,
        MediumFactory = 102,
        HeavyFactory = 103,
        Radar = 104,
        Mine = 105,
        ResearchStation = 106,
        Fan = 107,
        BotFactory = 108,
    }
    
    public enum Owner : uint
    {
        Player = 0,
        Enemy = 1,
        Neutral = 2,
    }
    
    // only work when there is a time limit, specified by other position.
    public enum TimeLimitType : uint
    {
        Victory = 0,
        Fail = 1,
        Reinforcement = 2,
    }
    
    // ================================================================================================================
    // Unit section.
    // ================================================================================================================
    
    
    public class Unit
    {
        internal static readonly List<byte> template = new List<byte>() {
            0xE0,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x50,0x02,0x00,0x00,
            0xF0,0x01,0x00,0x00 };
        internal static int length { get => template.Count; }
        [Location(0x4)] public UnitType type;
        [Location(0x8)] public Owner owner;
        [Location(0xC)] public uint x;
        [Location(0x10)] public uint y;
        
        internal Unit() => Access(0, template);
        internal Unit(int begin, List<byte> arr) => Access(begin, arr);        
        internal void Access(int begin, List<byte> arr) => arr.GrabData(begin, this);
        internal void AppendTo(List<byte> arr)
        {
            arr.AddRange(template);
            arr.Set(arr.Count - length, this);
        }
        
        internal static bool CheckHeader(int begin, List<byte> arr) => template.Slice(0, 4).SameAs(arr.Slice(begin, 4));
    }
    
    public class UnitManager : IEnumerable<Unit>
    {
        readonly List<Unit> data = new List<Unit>();
        
        internal UnitManager() { }
        
        public int count { get => data.Count;}
        
        public Unit this[int index] { get => data[index]; }
        
        public Unit Add() => Add(count);
        public Unit Add(int before)
        {
            if(before < 0 || before > count)
                throw new InvalidOperationException(
                    string.Format("cannot insert the element before {0}, array length is {1}.", before, count));
            Unit unit = new Unit();
            data.Insert(before, unit);
            return unit;
        }
        
        public Unit Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            Unit rm = data[index];
            data.RemoveAt(index);
            return rm;
        }
        
        public IEnumerator<Unit> GetEnumerator() => data.GetEnumerator();
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
        [Location(0x04)] public BuildingType type;
        [Location(0x08)] public uint level;
        [Location(0x0C)] public UnitType production0;
        [Location(0x10)] public UnitType production1;
        [Location(0x14)] public UnitType production2;
        [Location(0x18)] public UnitType production3;
        [Location(0x1C)] public UnitType production4;
        [Location(0x20)] public uint upgrade0;
        [Location(0x24)] public uint upgrade1;
        [Location(0x28)] public uint upgrade2;
        [Location(0x2C)] public uint upgrade3;
        [Location(0x30)] public uint upgrade4;
        [Location(0x38)] public Owner owner;
        [Location(0x3C)] public bool satellite;
        [Location(0x40)] public uint x;
        [Location(0x44)] public uint y;
        [Location(0x48)] public uint health;
        
        internal Building() => Access(0, template);
        internal Building(int begin, List<byte> arr) => Access(begin, arr);        
        internal void Access(int begin, List<byte> arr) => arr.GrabData(begin, this);
        internal void AppendTo(List<byte> arr)
        {
            arr.AddRange(template);
            arr.Set(arr.Count - length, this);
        }
        
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
        
        [Location(0x88)] public uint pDisk;
        [Location(0x8C)] public uint nDisk;
        
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
            while(cur < data.Count && Unit.CheckHeader(cur, data))
            {
                units.Add().Access(cur, data);
                cur += Unit.length;
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
                var head = data.Slice(0, EdtInfo.edtHeader.Count);
                var tail = data.Slice(data.Length - EdtInfo.edtTail.Count, EdtInfo.edtTail.Count);
                return EdtInfo.edtHeader.SameAs(head) || EdtInfo.edtTail.SameAs(tail);
            }
            catch(FileNotFoundException) { return false; }
            catch(FieldAccessException) { return false; }
            catch(AccessViolationException) { return false; }
        }
    }
    
    
}
