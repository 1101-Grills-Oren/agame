using Microsoft.Xna.Framework;

namespace agame.IDs;
class TileID
{
    private class Registerer
    {
        public static int numRegistered=0;
        public static void RegisterBackground(){}
    }
    public class Sets
    {
        public static bool[] Solid=[false,true,false,true,true,true,true];
        public static bool[] FullSolid=[false,true,false,false,false,false,false];
        public static bool[] SlantBL=[false,false,false,true,false,false,false];
        public static bool[] SlantBR=[false,false,false,false,true,false,false];
        public static bool[] SlantTL=[false,false,false,false,false,true,false];
        public static bool[] SlantTR=[false,false,false,false,false,false,true];
        public static bool[] Opaque=[false,true,false,false,false,false,false];
        public static bool[] Platform=[false,false,false,false,false,false,false];
        public static bool[] CustomShape=[false,false,false,false,false,false,false];
        public static Rectangle[][] CustomShapes=[null,null,null,null,null,null,null];
    }
    public static int[] NumVariants=[1,1,1,1,1,1,1];
    public const int None=0;
    public const int FullBricks=1;
    public const int FullBackgroundBricks=2;
    public const int BricksSlantBL=3;
    public const int BricksSlantBR=4;
    public const int BricksSlantTL=5;
    public const int BricksSlantTR=6;
    public static int count=7;
    
}