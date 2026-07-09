using Microsoft.Xna.Framework.Graphics;
using agame.IDs;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace agame.Rooms.Tile;
class TileInfo
{
    public static Texture2D[] textures=new Texture2D[TileID.count];
    public static Texture2D[] lightTextures=new Texture2D[TileID.count];
    public static Texture2D GetTexture(int id)
    {
        return textures[id];
    }
    public static Texture2D GetLightTexture(int id)
    {
        return lightTextures[id];
    }
    public static bool IsEmpty(int id)
    {
        return id==0;
    }
    public static bool IsOpaque(int id)
    {
        return TileID.Sets.Opaque[id];
    }
    public static bool IsSolid(int id)
    {
        return TileID.Sets.Solid[id];
    }
    public static bool IsPlatform(int id)
    {
        return TileID.Sets.Platform[id];
    }
    public static bool IsSolidSlant(int id, int dir)
    {
        if (id < 0)
        {
            return false;
        }
        switch (dir)
        {
            case 0://bottom right
            return TileID.Sets.SlantBR[id];
            case 1://bottom left
            return TileID.Sets.SlantBL[id];
            case 2://top right
            return TileID.Sets.SlantTR[id];
            case 3://top left
            return TileID.Sets.SlantTL[id];
        }
        return false;
    }

    public static bool IsFullSolid(int id)
    {
        return TileID.Sets.FullSolid[id];
    }
    public static bool IsGlowing(int id)
    {
        return false;//id==2;
    }
    public static bool HasCustomShape(int id)
    {
        return TileID.Sets.CustomShape[id];
    }
    public static Rectangle[] GetCustomShape(int id)
    {
        return TileID.Sets.CustomShapes[id];
    }
}