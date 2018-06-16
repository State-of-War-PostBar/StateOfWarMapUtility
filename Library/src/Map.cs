using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StateOfWarUtility
{
    public enum TileGround : uint
    {
        Passed = 0x00000000,
        Blocked = 0x00FFFFFD,
    }
    
    public enum TileAir : uint
    {
        Passed = 0x00000000,
        Blocked = 0x00FFFFFD,
    }
    
    public enum TileTurret : uint
    {
        Passed = 0x00000000,
        Blocked = 0x00010000,
    }
    
    
    public struct Tile
    {
        internal const int length = 0x11;
        [Location(0x0)] public byte x;
        [Location(0x1)] public byte y;
        [Location(0x2)] public TileGround ground;
        [Location(0x6)] public TileAir air;
        [Location(0xA)] public TileTurret turret;
    }
    
    
    public struct MapInfo
    {
        internal static readonly IReadOnlyList<byte> mapHeader = new byte[]{0x04,0x56,0x45,0x52,0x37,0x00,0x03,0x00,0x00,0xC0,0x04,0x00,0x00,0x40,0x00,0x00,0x00,0x40,0x00,0x00,0x00};
        public const int length = 0x15;
        [Location(0x05)] public uint initViewX;
        [Location(0x09)] public uint initViewY;
        [Location(0x0D)] public uint width;
        [Location(0x11)] public uint height;
    }
    
    /// <summary>
    /// This is an in-place maintainer.
    /// edit the byte arrays as needed.
    /// </summary>
    public sealed class Map : ByteFile
    {
        const int headerLength = 0x15;
        
        public MapInfo headerInfo
        {
            get => (MapInfo)data.GrabData(0, typeof(MapInfo));
            set => data.Set(0, value, typeof(MapInfo));
        }
        
        int tileOffset(int x, int y) => headerLength + (x + y * (int)headerInfo.width) * Tile.length;
        
        public Tile this[int x, int y]
        {
            get => (Tile)data.GrabData(tileOffset(x, y), typeof(Tile));
            set => data.Set(tileOffset(x, y), value, typeof(Tile));
        }
        
        public int width { get => (int)headerInfo.width; }
        public int height { get => (int)headerInfo.height; }
        
        
        public Map(string filePath) : this(File.ReadAllBytes(filePath)) { }
        public Map(byte[] raw) : base(raw) { }
    }
    
}
