using System;
using System.Drawing;
using System.IO;
using System.Text;
using StateOfWarUtility;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Console;

public static class __Main__
{
    static void SrfTest()
    {
        if(!Srf.ValidateJpg("./Test/res/x.jpg"))
            throw new FileLoadException("Input is not a jpg file.");
        if(!Srf.ValidateSrf("./Test/res/x.srf"))
            throw new FileLoadException("Input is not a srf file.");
        var xjpg = File.ReadAllBytes("./Test/res/x.jpg");
        var xsrf = File.ReadAllBytes("./Test/res/x.srf");
        
        File.WriteAllBytes("./Test/res/M00.srf", Srf.ToSrf(xjpg));
        File.WriteAllBytes("./Test/res/M00.jpg", Srf.ToJpg(xsrf));
        
        Bitmap bitmap = (Bitmap)Bitmap.FromFile("./Test/res/M00.jpg");
        bitmap.Save("./Test/res/jpgCompatiabiliyTest.jpg");
    }
    
    static void MapTest(string path)
    {
        if(!Map.Validate(path))
            throw new FileLoadException(path + " is not a map file.");
        Map map = new Map(path);
        
        WriteLine(string.Format("size ({0}, {1}) init ({0}, {1})",
            map.headerInfo.width, map.headerInfo.height, map.headerInfo.initViewX, map.headerInfo.initViewY));
        
        for(int y=0; y<map.height; y++)
        {
            for(int x=0; x<map.width; x++)
            {
                int v = 0;
                if(map[y, x].ground == TileGround.Blocked) v += 1;
                if(map[y, x].air == TileAir.Blocked) v += 2;
                if(map[y, x].turret == TileTurret.Blocked) v += 4;
                Write(v.ToString());
            }
            WriteLine();
        }
        
        for(int y=0; y<map.height; y++)
        {
            for(int x=0; x<map.width; x++)
            {
                var t = map[y, x];
                t.turret = x % 2 == 0 ? TileTurret.Blocked : TileTurret.Passed;
                t.ground = y % 2 == 0 ? TileGround.Blocked : TileGround.Passed;
                t.air = y >= map.height / 2 ? TileAir.Blocked : TileAir.Passed;
            }
        }
        
        map.Save("./Test/res/M00.map");
    }
    
    static void EdtTest(string path)
    {
        if(!Edt.Validate(path))
            throw new FileLoadException(path + " is not a edt file.");
        
        Edt edt = new Edt(path);
        
        var header = edt.headerInfo;
        WriteLine("Time limit : " + header.hasTimeLimit);
        if(edt.headerInfo.hasTimeLimit)
        {
            WriteLine("  Type: {0} length: {1}", header.timeLimitType, header.timeLimit);
        }
        WriteLine("Player:");
        WriteLine("  Money " + header.pMoney);
        WriteLine("  Research " +header.pResearch);
        WriteLine(string.Format("  Airforce {0} {1} {2} {3} {4}",header.pBomber,header.pMeteor,header.pCarrier,header.pTripler,header.pFighter));
        WriteLine(string.Format("  Turret {0} {1} {2} {3} {4}",header.pTurretDefence,header.pTurretAntiair,header.pTurretIon,header.pTurretLed,header.pTurretCluster));
        WriteLine(string.Format("  Disk {0} : {1}",header.pDisk,header.pDiskAttack));
        WriteLine("Enemy:");
        WriteLine("  Money " +header.nMoney);
        WriteLine("  Research " +header.nResearch);
        WriteLine(string.Format("  Airforce {0} {1} {2} {3} {4}",header.nBomber,header.nMeteor,header.nCarrier,header.nTripler,header.nFighter));
        WriteLine(string.Format("  Turret {0} {1} {2} {3} {4}",header.nTurretDefence,header.nTurretAntiair,header.nTurretIon,header.nTurretLed,header.nTurretCluster));
        WriteLine(string.Format("  Disk {0} : {1}",header.nDisk,header.nDiskAttack));
        
        for(int i=0; i<edt.buildings.count; i++)
        {
            var b = edt.buildings[i];
            WriteLine(string.Format("{0} | {1} : {2} {3} {4} {5} {6} : {7} {8} {9} {10} {11} | {12} | {13} | {14} {15} | {16}",
                b.type, b.level,
                b.production0, b.production1, b.production2, b.production3, b.production4,
                b.upgrade0, b.upgrade1, b.upgrade2, b.upgrade3, b.upgrade4,
                b.owner, b.satellite, b.x, b.y, b.health));
        }
        
        for(int i=0; i<edt.units.count; i++)
        {
            var b = edt.units[i];
            WriteLine(string.Format("{0} | {1} | {2} {3}", b.type, b.owner, b.x, b.y));
        }
        
        var building = edt.buildings.Add(3);
        building.type = BuildingType.BotFactory;
        building.level = 0;
        building.production0 = UnitType.Achilles;
        building.upgrade0 = 0;
        building.owner = Owner.Player;
        building.x = 50;
        building.y = 50;
        building.satellite = false;
        
        var rm = edt.buildings.Remove(1);
        
        WriteLine(string.Format("removed: {0} | {1} : {2} {3} {4} {5} {6} : {7} {8} {9} {10} {11} | {12} | {13} | {14} {15} | {16}",
            rm.type, rm.level,
            rm.production0, rm.production1, rm.production2, rm.production3, rm.production4,
            rm.upgrade0, rm.upgrade1, rm.upgrade2, rm.upgrade3, rm.upgrade4,
            rm.owner, rm.satellite, rm.x, rm.y, rm.health));
        
        var unit = edt.units.Add(1);
        unit.type = UnitType.Codiak;
        unit.owner = Owner.Player;
        unit.x = 12;
        unit.y = 15;
        
        var ru = edt.units.Remove(2);
        
        WriteLine(string.Format("{0} | {1} | {2} {3}", ru.type, ru.owner, ru.x, ru.y));
        
        edt.Save("./Test/res/M01.edt");
    }
    
    public static void Main()
    {
        SrfTest();
        MapTest("./Test/res/x.map");
        EdtTest("./Test/res/x.edt");
        
    }
    
}
