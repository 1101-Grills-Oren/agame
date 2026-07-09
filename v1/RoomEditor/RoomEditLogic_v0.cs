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
public class RoomEditLogic_v00000000
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
        RoomID.count+=1;
        
        IDMaps.RoomNames=AppendArrayValue(IDMaps.RoomNames,name);
        RoomID.Rooms=AppendArrayValue(RoomID.Rooms,new Room(width,height){linkedRooms=[]});
    }
    public static void RemoveRoomFromList(int id)
    {
        RoomID.count-=1;
        
        IDMaps.RoomNames=PopArrayValue(IDMaps.RoomNames,id);
        RoomID.Rooms=PopArrayValue(RoomID.Rooms,id);
        foreach(Room r in RoomID.Rooms)
        {
            for(int i=0;i<r.linkedRooms.Length;i++)
            {
                if (r.linkedRooms[i].id > id)
                {
                    r.linkedRooms[i].id-=1;
                }else if (r.linkedRooms[i].id == id)
                {
                    r.linkedRooms[i].id=-1;
                }
            }
        }
        List<RoomInstance> roomInstancesAlt=new List<RoomInstance>(Game1.instance.rooms);
        foreach(RoomInstance inst in roomInstancesAlt)
        {
            if (inst.id == id)
            {
                Game1.instance.rooms.Remove(inst);
            }else if(inst.id > id)
            {
                inst.id-=1;
            }
        }
    }
    public static void AddTileToList(String name)
    {
        TileID.count+=1;
        
        IDMaps.TileNames=AppendArrayValue(IDMaps.TileNames,name);
        TileID.Sets.FullSolid=AppendArrayValue(TileID.Sets.FullSolid,false);
        TileID.Sets.Opaque=AppendArrayValue(TileID.Sets.Opaque,false);
        TileID.Sets.SlantBL=AppendArrayValue(TileID.Sets.SlantBL,false);
        TileID.Sets.SlantBR=AppendArrayValue(TileID.Sets.SlantBR,false);
        TileID.Sets.SlantTL=AppendArrayValue(TileID.Sets.SlantTL,false);
        TileID.Sets.SlantTR=AppendArrayValue(TileID.Sets.SlantTR,false);
        TileID.Sets.Solid=AppendArrayValue(TileID.Sets.Solid,false);
        TileID.Sets.Platform=AppendArrayValue(TileID.Sets.Platform,false);
        TileID.NumVariants=AppendArrayValue(TileID.NumVariants,1);
        TileInfo.textures=AppendArrayValue(TileInfo.textures,LoadTexture("tile_"+name));
    }
    public static void RemoveTileFromList(int id)
    {
        TileID.count-=1;
        IDMaps.TileNames=PopArrayValue(IDMaps.TileNames,id);
        TileID.Sets.FullSolid=PopArrayValue(TileID.Sets.FullSolid,id);
        TileID.Sets.Opaque=PopArrayValue(TileID.Sets.Opaque,id);
        TileID.Sets.SlantBL=PopArrayValue(TileID.Sets.SlantBL,id);
        TileID.Sets.SlantBR=PopArrayValue(TileID.Sets.SlantBR,id);
        TileID.Sets.SlantTL=PopArrayValue(TileID.Sets.SlantTL,id);
        TileID.Sets.SlantTR=PopArrayValue(TileID.Sets.SlantTR,id);
        TileID.Sets.Solid=PopArrayValue(TileID.Sets.Solid,id);
        TileID.Sets.Platform=PopArrayValue(TileID.Sets.Platform,id);
        TileID.NumVariants=PopArrayValue(TileID.NumVariants,id);
        TileInfo.textures[id].Dispose();
        TileInfo.textures=PopArrayValue(TileInfo.textures,id);
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
    public static string TileToInfo(int id)
    {
        int tileType=(TileInfo.IsFullSolid(id)?5:(TileInfo.IsSolidSlant(id,0)?3:(TileInfo.IsSolidSlant(id,1)?1:(TileInfo.IsSolidSlant(id,2)?9:(TileInfo.IsSolidSlant(id,3)?7:0)))));
        return IDMaps.TileNames[id]+"="+id+"("+(TileInfo.IsOpaque(id)?"O":"T")+(TileInfo.IsSolid(id)&&(!(tileType==0))?"F":"B")+
        ((tileType==0)?(5):(tileType-(TileInfo.IsPlatform(id)?1:0)))
        +");";
    }
    public struct TileData
    {
        public string name;
        public int id;
        public bool opaque;
        public bool solid;
        public bool full;
        public bool slantBR;
        public bool slantBL;
        public bool slantTR;
        public bool slantTL;
        public bool platform;
    }
    public static TileData InfoToTile(string info)
    {
        string name=info.Split("=")[0];
        info=info.Split("=")[1];
        string id=info.Split("(")[0];
        info=info.Split("(")[1];
        bool opaque=info[0]=='O';
        bool solid=info[1]=='F';
        bool full=(info[2]=='5')||(info[2]=='4');
        bool slantBR=(info[2]=='3')||(info[2]=='2');
        bool slantBL=(info[2]=='1')||(info[2]=='0');
        bool slantTR=(info[2]=='9')||(info[2]=='8');
        bool slantTL=(info[2]=='7')||(info[2]=='6');
        bool platform=(info[2]=='0')||(info[2]=='2')||(info[2]=='4')||(info[2]=='6')||(info[2]=='8');
        return new TileData{name=name,id=int.Parse(id),opaque=opaque,solid=solid,full=full,slantBR=slantBR,slantBL=slantBL,slantTR=slantTR,slantTL=slantTL,platform=platform};
    }
    public static string LinkToInfo(RoomLinkInfo info)
    {
        return "("+info.id+"."+(int)(info.offset.X/Room.tileSize)+"."+(int)(info.offset.Y/Room.tileSize)+"."+(int)Math.Floor((double)info.offset2.X/Room.tileSize)+"."+(int)Math.Floor((double)info.offset2.Y/Room.tileSize)+")";
    }
    public static RoomLinkInfo InfoToLink(string info)
    {
        return new RoomLinkInfo{id=int.Parse(info.Split(".")[0].Substring(1)),offset=new Point(int.Parse(info.Split(".")[1])*Room.tileSize,int.Parse(info.Split(".")[2])*Room.tileSize),offset2=new Point((int)((int.Parse(info.Split(".")[3])+0.5)*Room.tileSize),(int)((int.Parse(info.Substring(0,info.Length-1).Split(".")[4])+0.5)*Room.tileSize))};
    }
    public static string RoomToInfo(int id)
    {
        string output=IDMaps.RoomNames[id]+"="+id+"("+RoomID.Rooms[id].Width+","+RoomID.Rooms[id].Height+")";
        for(int y = 0; y < RoomID.Rooms[id].Height; y++)
        {
            output+="\n|";
            for(int x = 0; x < RoomID.Rooms[id].Width; x++)
            {
                output+=RoomID.Rooms[id].GetTile(x,y).ToString();
                if(x < (RoomID.Rooms[id].Width - 1))
                {
                    output+=" ";
                }
            }
        }
        output+="\n[";
        if (RoomID.Rooms[id].linkedRooms.Length == 0)
        {
            return output+"];";
        }
        foreach(RoomLinkInfo link in RoomID.Rooms[id].linkedRooms)
        {
            output+=LinkToInfo(link)+",";
        }
        return output+"];";
    }
    public static Room InfoToRoom(string info,out int id,out string name)
    {
        name=info.Split("=")[0];
        info=info.Split("=")[1];
        id=int.Parse(info.Split("(")[0]);
        info=info.Substring(info.IndexOf("(")+1).Trim();
        int width=int.Parse(info.Split(")")[0].Split(",")[0]);
        int height=int.Parse(info.Split(")")[0].Split(",")[1]);
        info=info.Substring(info.IndexOf(")")+1).Trim();
        string[] lines=info.Split("\n");
        Room r= new Room(width,height);
        RoomLinkInfo[] links=new RoomLinkInfo[CountChars(info,',')];
        int hplaced=0;
        int lplaced=0;
        foreach(string line in lines)
        {
            if (line[0] == '|')
            {
                int x=0;
                string[] t=line.Substring(1).Split(' ');
                foreach(string n in t){
                    r.SetTile(x,hplaced,int.Parse(n));
                    x+=1;
                }
                hplaced+=1;
            }else if (line[0] == '[')
            {
                string[] lines2=line.Substring(1,line.Length-2).Split(",");
                foreach(string line2 in lines2)
                {
                    if (lplaced < links.Length)
                    {

                        links[lplaced]=InfoToLink(line2);
                        lplaced+=1;
                    }
                }
            }
        }
        r.linkedRooms=links;
        return r;
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
    public static string GetTileInfos()
    {
        String v="";
        for(int i = 0; i < TileID.count; i++)
        {
            v+=TileToInfo(i)+"\n";
        }
        return v;
    }
    public static string GetRoomInfos()
    {
        String v="";
        for(int i = 0; i < RoomID.count; i++)
        {
            v+=RoomToInfo(i)+"\n";
        }
        return v;
    }
    public static string GetRoomDatas()
    {
        return GetTileInfos()+"-----\n"+GetRoomInfos();
    }
    public static void LoadRoomDatas(string datas)
    {
        string n=datas.Split("-----")[0].Trim();
        string[] tiledatas=n.Substring(0,n.Length-1).Split(";");
        TileID.count=tiledatas.Length;
        TileID.Sets.FullSolid=new bool[tiledatas.Length];
        TileID.Sets.Solid=new bool[tiledatas.Length];
        TileID.Sets.Opaque=new bool[tiledatas.Length];
        TileID.Sets.SlantBL=new bool[tiledatas.Length];
        TileID.Sets.SlantBR=new bool[tiledatas.Length];
        TileID.Sets.SlantTL=new bool[tiledatas.Length];
        TileID.Sets.SlantTR=new bool[tiledatas.Length];
        TileID.Sets.Platform=new bool[tiledatas.Length];
        TileInfo.textures=new Texture2D[tiledatas.Length];
        IDMaps.TileNames=new string[tiledatas.Length];
        foreach(string tiledata in tiledatas)
        {
            TileData d=InfoToTile(tiledata.Trim());
            IDMaps.TileNames[d.id]=d.name;
            TileID.Sets.Solid[d.id]=d.solid;
            TileID.Sets.Opaque[d.id]=d.opaque;
            TileID.Sets.FullSolid[d.id]=d.full;
            TileID.Sets.SlantTR[d.id]=d.slantTR;
            TileID.Sets.SlantTL[d.id]=d.slantTL;
            TileID.Sets.SlantBR[d.id]=d.slantBR;
            TileID.Sets.SlantBL[d.id]=d.slantBL;
            TileID.Sets.Platform[d.id]=d.platform;
            if(d.id>0)
                TileInfo.textures[d.id]=LoadTexture("tile_"+d.name);
            else{
                TileInfo.textures[0] = new Texture2D(Game1.instance.GraphicsDevice, 1, 1);
                TileInfo.textures[0].SetData(new[] { Color.Transparent });
            }
        }
        n=datas.Split("-----")[1].Trim();
        string[] roomdatas=n.Substring(0,n.Length-1).Split(";");
        RoomID.count=roomdatas.Length;
        RoomID.Rooms=new Room[roomdatas.Length];
        IDMaps.RoomNames=new string[roomdatas.Length];
        foreach(string roomdata in roomdatas)
        {
            Room r=InfoToRoom(roomdata.Trim(),out int id, out string name);
            IDMaps.RoomNames[id]=name;
            RoomID.Rooms[id]=r;
            r.GenHulls();
        }
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
            LoadRoomDatas(c);
        }
    }
    public static void TrySaveData(string path)
    {
        string pngPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Content\\"+path+".roomtxt");
        // Load PNG from file stream (bypasses Content Pipeline)
        using (FileStream stream = new FileStream(pngPath, FileMode.Create))
        {
            string c=GetRoomDatas();
            foreach(char x in c){
                stream.WriteByte((byte)x);
            }
        }
    }
}