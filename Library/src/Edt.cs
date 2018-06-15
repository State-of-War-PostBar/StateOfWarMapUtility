using System;
using System.IO;
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
        Revanger = 42,
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
        TurretIonCannon = 27,
        TurretLedStorm = 28,
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
    
    [StructLayout(LayoutKind.Explicit)]
    public struct Unit
    {
        internal static readonly List<byte> template = new List<byte> {
            0xE0,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x50,0x02,0x00,0x00,
            0xF0,0x01,0x00,0x00 };
        internal const int length = 0x14;
        [FieldOffset(0x4)] public UnitType type;
        [FieldOffset(0x8)] public Owner owner;
        [FieldOffset(0xC)] public uint x;
        [FieldOffset(0x10)] public uint y;
    }
    
    public class UnitManager
    {
        readonly BuildingManager mgr;
        readonly List<byte> data;
        internal UnitManager(List<byte> data, BuildingManager mgr)
        {
            this.data = data;
            this.mgr = mgr;
            
            int cur = unitsBegin;
            while(cur < data.Count && data[cur] == Unit.template[0])
            {
                cur += Unit.length;
                count++;
            }
        }
        
        public int count { get; internal set; }
        
        internal int unitsBegin { get => mgr.buildingsBegin + mgr.count * Building.length; }
        
        public Unit this[int index]
        {
            get
            {
                Unit v;
                unsafe { data.GrabData(unitsBegin + index * Unit.length, &v, typeof(Unit)); }
                return v;
            }
            
            set { unsafe { data.Set(unitsBegin + index * Unit.length, &value, typeof(Unit)); } }
        }
        
        public void Add(int before, Unit val)
        {
            if(before < 0 || before >= count)
                throw new InvalidOperationException("cannot insert the element before the specific position.");
            
            data.InsertRange(unitsBegin + before * Unit.length, Unit.template);
            unsafe { data.Set(unitsBegin + before * Unit.length, &val, typeof(Unit)); }
            count++;
        }
        
        public Unit Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            
            Unit v;
            unsafe { data.GrabData(unitsBegin + index * Unit.length, &v, typeof(Unit)); }
            data.RemoveRange(unitsBegin + index * Unit.length, Unit.length);
            
            count--;
            return v;
        }
    }
    
    // ================================================================================================================
    // Building section.
    // ================================================================================================================
    
    [StructLayout(LayoutKind.Explicit)]
    public struct Building
    {
        internal static readonly List<byte> template = new List<byte> {
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
        
        internal const int length = 0x4C;
        [FieldOffset(0x04)] public BuildingType type;
        [FieldOffset(0x08)] public uint level;
        [FieldOffset(0x0C)] public UnitType production0;
        [FieldOffset(0x10)] public UnitType production1;
        [FieldOffset(0x14)] public UnitType production2;
        [FieldOffset(0x18)] public UnitType production3;
        [FieldOffset(0x1C)] public UnitType production4;
        [FieldOffset(0x20)] public uint upgrade0;
        [FieldOffset(0x24)] public uint upgrade1;
        [FieldOffset(0x28)] public uint upgrade2;
        [FieldOffset(0x2C)] public uint upgrade3;
        [FieldOffset(0x30)] public uint upgrade4;
        [FieldOffset(0x38)] public Owner owner;
        [FieldOffset(0x3C)] public bool satellite;
        [FieldOffset(0x40)] public uint x;
        [FieldOffset(0x44)] public uint y;
        [FieldOffset(0x48)] public uint health;        
    }
    
    
    public class BuildingManager
    {
        readonly List<byte> data;
        
        internal BuildingManager(List<byte> data)
        {
            this.data = data;
            
            int cur = EdtInfo.length;
            while(cur < data.Count && data[cur] == Building.template[0])
            {
                cur += Building.length;
                count++;
            }
        }
        
        
        public int count { get; internal set; }
        
        internal int buildingsBegin { get => EdtInfo.length; }
        
        public Building this[int index]
        {
            get
            {
                Building v;
                unsafe { data.GrabData(buildingsBegin + index * Building.length, &v, typeof(Building)); }
                return v;
            }
            
            set { unsafe { data.Set(buildingsBegin + index * Building.length, &value, typeof(Building)); } }
        }
        
        public void Add(int before, Building val)
        {
            if(before < 0 || before >= count)
                throw new InvalidOperationException("cannot insert the element before the specific position.");
            
            data.InsertRange(buildingsBegin + before * Building.length, Building.template);
            unsafe { data.Set(buildingsBegin + before * Building.length, &val, typeof(Building)); }
            count++;
        }
        
        public Building Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            
            Building v;
            unsafe { data.GrabData(buildingsBegin + index * Building.length, &v, typeof(Building)); }
            data.RemoveRange(buildingsBegin + index * Building.length, Building.length);
            
            count--;
            return v;
        }
    }
    
    // ================================================================================================================
    // Edt control and global settings section.
    // ================================================================================================================
    
    [StructLayout(LayoutKind.Explicit)]
    public struct EdtInfo
    {
        public static readonly byte[] edtHeader = new byte[]{0x04, 0x00, 0x8E, 0x26, 0x06, 0x00, 0x00, 0x00};
        internal const int length = 0x178;
        
        // The p prefix and the n prefix:
        // p is player.
        // n is enemy.
        
        [FieldOffset(0x08)] public uint pMoney;
        [FieldOffset(0x0C)] public uint nMoney;
        
        [FieldOffset(0x94)] public uint pResearch;
        [FieldOffset(0x98)] public uint nResearch;
        
        [FieldOffset(0x14)] public uint pBomber;
        [FieldOffset(0x18)] public uint nBomber;
        [FieldOffset(0x20)] public uint pMeteor;
        [FieldOffset(0x24)] public uint nMeteor;
        [FieldOffset(0x2C)] public uint pCarrier;
        [FieldOffset(0x30)] public uint nCarrier;
        [FieldOffset(0x38)] public uint pTripler;
        [FieldOffset(0x3C)] public uint nTripler;
        [FieldOffset(0x44)] public uint pFighter;
        [FieldOffset(0x48)] public uint nFighter;
        
        [FieldOffset(0x5C)] public bool pTurretDefence;
        [FieldOffset(0x5D)] public bool pTurretAntiair;
        [FieldOffset(0x5E)] public bool pTurretIon;
        [FieldOffset(0x5F)] public bool pTurretLed;
        [FieldOffset(0x60)] public bool pTurretCluster;
        
        [FieldOffset(0x66)] public bool nTurretDefence;
        [FieldOffset(0x67)] public bool nTurretAntiair;
        [FieldOffset(0x68)] public bool nTurretIon;
        [FieldOffset(0x69)] public bool nTurretLed;
        [FieldOffset(0x6A)] public bool nTurretCluster;
        
        [FieldOffset(0x7C)] public uint pDiskAttack;
        [FieldOffset(0x80)] public uint nDiskAttack;
        
        [FieldOffset(0x88)] public uint pDisk;
        [FieldOffset(0x8C)] public uint nDisk;
        
        [FieldOffset(0x104)] public bool hasTimeLimit;
        [FieldOffset(0x108)] public uint timeLimit;
        [FieldOffset(0x10C)] public TimeLimitType timeLimitType;
    }
    
    public sealed class Edt : ByteFile
    {
        public EdtInfo headerInfo
        {
            get
            {
                EdtInfo v;
                unsafe { data.GrabData(0, &v, typeof(EdtInfo)); }
                return v;
            }
            
            set { unsafe { data.Set(0, &value, typeof(EdtInfo)); } }
        }
        
        public readonly UnitManager units;
        public readonly BuildingManager buildings;
        
        public Edt(string filePath) : this(File.ReadAllBytes(filePath)) { }
        public Edt(byte[] raw) : base(raw)
        {
            buildings = new BuildingManager(data);
            units = new UnitManager(data, buildings);
        }
    }
    
    
}
