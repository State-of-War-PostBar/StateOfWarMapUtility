using System;

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
        
        TurretDefence = 25,
        TurretAntiair = 26,
        TurretIon = 27,
        TurretLed = 28,
        TurretCluster = 29,
        
        Bomber = 30,
        Carrier = 31,
        Fighter = 32,
        Tripler = 33,
        Meteor = 34,
        
        Disk = 40,
        Codiak = 41,
        Avenger = 42,
        Cougar = 43,
        Gattling = 44,
        Achilles = 45,
        Rogon = 46,
        
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
    
    public enum DiskRebuildType : uint
    {
        Enabled = 0,
        Disabled = 1,
    }
    
    public static partial class EnumExt
    {
        public static bool IsNothing(this UnitType x) => x == UnitType.None;
        public static bool IsTank(this UnitType x) => 1 <= (uint)x && (uint)x <= 15;
        public static bool IsResources(this UnitType x) => 16 <= (uint)x && (uint)x <= 24;
        public static bool IsTurret(this UnitType x) => 25 <= (uint)x && (uint)x <= 29;
        public static bool IsAirforce(this UnitType x) => 30 <= (uint)x && (uint)x <= 34;
        public static bool IsDisk(this UnitType x) => x == UnitType.Disk;
        public static bool IsBot(this UnitType x) => 41 <= (uint)x && (uint)x <= 45;
        public static bool IsRogon(this UnitType x) => x == UnitType.Rogon;
        
        /// <summary>
        /// Contains: Heavy, Medium, Light factory, and BOt factory. 
        /// </summary>
        public static bool IsFactory(this UnitType x) => (101 <= (uint)x && (uint)x <= 103) || x == UnitType.BotFactory;
        
        /// <summary>
        /// Contains: Factories and headquaters.
        /// </summary>
        public static bool IsProductionBuilding(this UnitType x) => x.IsFactory() || x == UnitType.Headquater;
        
        /// <summary>
        /// Contains: Mine, research station and fan.
        /// </summary>
        public static bool IsResourcesBuilding(this UnitType x) => 105 <= (uint)x && (uint)x <= 107;
        
        /// <summary>
        /// Contains all buildings and turrets.
        /// </summary>
        public static bool IsBuilding(this UnitType x) => x.IsTurret() || (100 <= (uint)x && (uint)x <= 108);
        
        /// <summary>
        /// Contains all non-tanks land units.
        /// </summary>
        public static bool IsHighTech(this UnitType x) => x.IsBot() || x.IsRogon();
        
        /// <summary>
        /// Contains all battile units except for disks.
        /// </summary>
        public static bool IsLandUint(this UnitType x) => x.IsTank() || x.IsHighTech();
        
        /// <summary>
        /// Contains all things that can is a battle unit in game.
        /// </summary>
        public static bool IsBattleUnit(this UnitType x) => x.IsLandUint() || x.IsDisk();
        
        /// <summary>
        /// Contains all things that can be produced at Factory and headquater.
        /// </summary>
        public static bool IsProduction(this UnitType x) => x.IsLandUint() || x.IsAirforce();
    }
}
