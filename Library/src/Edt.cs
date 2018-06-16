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
    
    
    public struct Unit
    {
        internal static readonly List<byte> template = new List<byte> {
            0xE0,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x01,0x00,0x00,0x00,
            0x50,0x02,0x00,0x00,
            0xF0,0x01,0x00,0x00 };
        internal const int length = 0x14;
        [Location(0x4)] public UnitType type;
        [Location(0x8)] public Owner owner;
        [Location(0xC)] public uint x;
        [Location(0x10)] public uint y;
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
            get => (Unit)data.GrabData(unitsBegin + index * Unit.length, typeof(Unit));
            set => data.Set(unitsBegin + index * Unit.length, value, typeof(Unit));
        }
        
        public void Add(int before, Unit val)
        {
            if(before < 0 || before >= count)
                throw new InvalidOperationException("cannot insert the element before the specific position.");
            data.InsertRange(unitsBegin + before * Unit.length, Unit.template);
            data.Set(unitsBegin + before * Unit.length, val, typeof(Unit));
            count++;
        }
        
        public Unit Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            var v = (Unit)data.GrabData(unitsBegin + index * Unit.length, typeof(Unit));
            data.RemoveRange(unitsBegin + index * Unit.length, Unit.length);
            count--;
            return v;
        }
    }
    
    // ================================================================================================================
    // Building section.
    // ================================================================================================================
    
    
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
            get => (Building)data.GrabData(buildingsBegin + index * Building.length, typeof(Building));
            set => data.Set(buildingsBegin + index * Building.length, value, typeof(Building));
        }
        
        public void Add(int before, Building val)
        {
            if(before < 0 || before >= count)
                throw new InvalidOperationException("cannot insert the element before the specific position.");
            
            data.InsertRange(buildingsBegin + before * Building.length, Building.template);
            data.Set(buildingsBegin + before * Building.length, val, typeof(Building));
            count++;
        }
        
        public Building Remove(int index)
        {
            if(index < 0 || index >= count)
                throw new InvalidOperationException("cannot remove the element that does not exist.");
            
            var v = (Building)data.GrabData(buildingsBegin + index * Building.length, typeof(Building));
            data.RemoveRange(buildingsBegin + index * Building.length, Building.length);
            count--;
            return v;
        }
    }
    
    // ================================================================================================================
    // Edt control and global settings section.
    // ================================================================================================================
    
    
    public struct EdtInfo
    {
        public static readonly byte[] edtHeader = new byte[]{0x04, 0x00, 0x8E, 0x26, 0x06, 0x00, 0x00, 0x00};
        internal const int length = 0x178;
        
        // The p prefix and the n prefix:
        // p is player.
        // n is enemy.
        
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
    }
    
    public sealed class Edt : ByteFile
    {
        public EdtInfo headerInfo
        {
            get => (EdtInfo)data.GrabData(0, typeof(EdtInfo));
            set => data.Set(0, value, typeof(EdtInfo));
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
