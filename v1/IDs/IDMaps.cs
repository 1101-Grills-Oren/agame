using System;
using System.Reflection;
using agame.Rooms;

namespace agame.IDs;
class IDMaps
{
    public static string[] GetIds(Type t,int max)
    {
        string[] i=new string[max];
        foreach(FieldInfo j in t.GetFields())
        {
            if(j.Attributes.HasFlag(FieldAttributes.Literal)){
                int v=(int)j.GetRawConstantValue();
                if(v<max)
                i[v]=j.Name;
            }
            
        }
        return i;
    }
    public static string[] TileNames=GetIds((new TileID()).GetType(),TileID.count);
    public static string[] RoomNames=GetIds((new RoomID()).GetType(),RoomID.count);
    public static string[] ParticleNames=GetIds((new ParticleID()).GetType(),ParticleID.count);
    public static string[] SoundNames=GetIds((new SoundID()).GetType(),SoundID.count);
    public static int GetTileFromName(String name)
    {
        return Array.IndexOf(TileNames,name);
    }
    public static int GetRoomFromName(String name)
    {
        return Array.IndexOf(RoomNames,name);
    }
    public static int GetParticleFromName(String name)
    {
        return Array.IndexOf(ParticleNames,name);
    }
    public static int GetSoundFromFromName(String name)
    {
        return Array.IndexOf(SoundNames,name);
    }
}