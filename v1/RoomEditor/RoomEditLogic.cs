using System;
using System.IO;
using agame.IDs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Emit;
using System.Reflection;
using agame.Rooms;
using agame.Rooms.Tile;
using System.Collections.Generic;
namespace agame.RoomEditor;
class RoomEditLogic
{
    public static Texture2D LoadTexture(string name){
        string pngPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content\\"+name+".png");
        // Load PNG from file stream (bypasses Content Pipeline)
        try{
            using (FileStream stream = new FileStream(pngPath, FileMode.Open))
            {
                Texture2D texture = Texture2D.FromStream(Game1.instance.GraphicsDevice, stream);
                return texture;
            }
        }
        catch
        {
            Console.WriteLine(pngPath);
            Texture2D texture = new Texture2D(Game1.instance.GraphicsDevice,2,2);
            texture.SetData(new[] { Color.Black,Color.BlueViolet,Color.BlueViolet,Color.Black});
            return texture;
        }
    }
    public static void AddRoomToList(String name,int width, int height)
    {
        RoomEditLogic_v00000001.AddRoomToList(name,width,height);
    }
    public static void RemoveRoomFromList(int id)
    {
        RoomEditLogic_v00000001.RemoveRoomFromList(id);
    }
    public static void AddTileToList(String name)
    {
        RoomEditLogic_v00000001.AddTileToList(name);
    }
    public static void RemoveTileFromList(int id)
    {
        RoomEditLogic_v00000001.RemoveTileFromList(id);
    }
    public static void AddParticleToList(String name)
    {
        RoomEditLogic_v00000001.AddParticleToList(name);
    }
    public static void RemoveParticleFromList(int id)
    {
        RoomEditLogic_v00000001.RemoveParticleFromList(id);
    }
    public static T[] AppendArrayValue<T>(T[] array,T value)
    {
        T[] newArr=new T[array.Length+1];
        for(int i = 0; i < array.Length; i++)
        {
            newArr[i]=array[i];
        }
        newArr[array.Length]=value;
        return newArr;
    }
    public static T[] PopArrayValue<T>(T[] array,int id)
    {
        T[] newArr=new T[array.Length-1];
        for(int i = 0; i < id; i++)
        {
            newArr[i]=array[i];
        }
        for(int i = id+1; i < array.Length; i++)
        {
            newArr[i-1]=array[i];
        }
        return newArr;
    }
    public static int CountChars(String a, char b)
    {
        int v=0;
        foreach(char c in a)
        {
            if(c==b)
            v+=1;
        }
        return v;
    }
    
    
    
    
    public static void TryLoadData(string path)
    {
        string pngPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content\\"+path+".roomtxt");
        // Load PNG from file stream (bypasses Content Pipeline)
        using (FileStream stream = new FileStream(pngPath, FileMode.Open))
        {
            string c="";
            for(int i = 0; i < stream.Length; i++)
            {
                c+=((char)(stream.ReadByte())).ToString();
            }
            Console.WriteLine(c);
            stream.Close();
            switch (c.Substring(0, 9))
            {
                case "v00000001":
                RoomEditLogic_v00000001.TryLoadData(path);
                break;
                default:
                RoomEditLogic_v00000000.TryLoadData(path);
                break;
            }
        }
    }
    public static void TrySaveData(string path)
    {
        RoomEditLogic_v00000001.TrySaveData(path);
    }
}