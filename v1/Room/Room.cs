
using System;
using System.Collections.Generic;
using agame.IDs;
using agame.Physics;
using agame.Rooms.Tile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using agame.Rendering;
namespace agame.Rooms
{
    public class Room
    {
        public static int screenTileSize=20;
        public static int tileSize=20;
        // Do not change after init
        private int _width;
        // Do not change after init
        private int _height;
        public int Width{get{return _width;}}
        public int Height{get{return _height;}}
        public int[] tiles;
        private float[] darkness;
        public List<Light> lights=new List<Light>();
        public List<List<Vector2>> shadowHulls=new List<List<Vector2>>();
        public RoomLinkInfo[] linkedRooms;
        public Room(int width, int height)
        {
            this._width=width;
            this._height=height;
            this.tiles=new int[width*height];
            this.darkness=new float[width*height*4];
            
        }
        public Room(int width, int height,int[] tiles)
        {
            this._width=width;
            this._height=height;
            
            if (tiles.Length != width * height)
            {
                throw new System.Exception("Error! Room requires "+width*height+" tiles, got "+tiles.Length);
            }
            this.tiles=tiles;
            this.darkness=new float[width*height*4];
            for(int i = 0; i < width*height*4; i++)
            {
                this.darkness[i]=-1;
            }
            
        GenHulls();
            
        }
        public void SetTile(int x, int y,int tile)
        {
            if ((x >= 0) && (y >= 0) && (y < Height) && (x < Width))
            {
                tiles[x+y*Width]=tile;
            }
        }
        public int GetTile(int x, int y)
        {
            if ((x >= 0) && (y >= 0) && (y < Height) && (x < Width))
            {
                return tiles[x+y*Width];
            }
            return 0;
        }
        private int GetTileInt(int x, int y)
        {
            return tiles[x+y*Width];
        }
        
        public void Draw(int xoff, int yoff, SpriteBatch batch)
        {
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    Texture2D texture=TileInfo.GetTexture(tiles[x+y*Width]);
                    int texwidth=texture.Width;
                    int texheight=texture.Height;
                    batch.Draw(texture, new Rectangle(xoff+x*screenTileSize, yoff+y*screenTileSize, screenTileSize, screenTileSize), new Rectangle(0,0,texwidth,texheight), Color.White);
                    
                }
            }
        }
        public void Draw(int xoff, int yoff, SpriteBatch batch,int tl,int tr,int tt,int tb)
        {
            for(int y = Math.Max(0,tt); y < Math.Min(Height,tb); y++)
            {
                for(int x=Math.Max(0,tl); x < Math.Min(Width,tr); x++)
                {
                    Texture2D texture=TileInfo.GetTexture(tiles[x+y*Width]);
                    int texwidth=texture.Width;
                    int texheight=texture.Height;
                    batch.Draw(texture, new Rectangle(xoff+x*screenTileSize, yoff+y*screenTileSize, screenTileSize, screenTileSize), new Rectangle(0,0,texwidth,texheight), Color.White);
                    
                }
            }
        }
        public void GenHulls()
        {

            shadowHulls.Clear();
            foreach(Vector2[] hull in ShadowHullLoader.GetHulls(this.tiles, Width, Height))
            {
                List<Vector2> n=new List<Vector2>(hull);
                n.RemoveAt(n.Count-1);
                shadowHulls.Add(
                    n
                );
            }
            /*int[] HullIds=new int[this.Width*this.Height];
            int numEmptyHulls=0;
            int numHulls=0;
            for(int i = 0; i < this.Width * this.Height; i++)
            {
                HullIds[i]=-1;
            }
            /*for(int x = 0; x < this.Width ; x++)
            {
                for(int y = 0; y < this.Height ; y++)
                {
                    
                    if(TileInfo.IsOpaque(GetTileInt(x,y))){
                        if(TileInfo.IsSolidSlant(GetTileInt(x,y),0)){//bottom right
                            Console.WriteLine("adding slant shadow at "+x+", "+y);
                            List<Vector2> l=new List<Vector2>();
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                            shadowHulls.Add(l);
                        }
                        if(TileInfo.IsSolidSlant(GetTileInt(x,y),1)){//bottom left
                            List<Vector2> l=new List<Vector2>();
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                            shadowHulls.Add(l);
                        }
                        if(TileInfo.IsSolidSlant(GetTileInt(x,y),2)){
                            List<Vector2> l=new List<Vector2>();
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                            
                            shadowHulls.Add(l);
                        }
                        if(TileInfo.IsSolidSlant(GetTileInt(x,y),3)){
                            List<Vector2> l=new List<Vector2>();
                            l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                            l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                            shadowHulls.Add(l);
                        }
                    }
                }
            }* /
            for(int x = 0; x < this.Width ; x++)
            {
                for(int y = 0; y < this.Height ; y++)
                {
                    if (HullIds[x+y*Width] == -1)
                    {
                        if(TileInfo.IsOpaque(GetTileInt(x,y))&&TileInfo.IsFullSolid(GetTileInt(x,y))){
                            HullIds[x+y*Width]=numHulls;
                            numHulls+=1;
                        }
                        else
                        {
                            HullIds[x+y*Width]=-2-numEmptyHulls;
                            numEmptyHulls+=1;
                        }
                        HullSpread(HullIds,x,y);
                    }
                }
            }
            //Draw hulls for debugging
            for(int y = 0; y < this.Height ; y++)
            {
                String line="";
                for(int x = 0; x < this.Width ; x++)
                {
                    if (HullIds[x + y * Width] > -2)
                    {
                        line+=HullIds[x + y * Width]+" ";
                    }
                    else
                    {
                        line+="  ";
                    }
                }
                Console.WriteLine(line);
            }
            bool[] hullsCreated=new bool[Width*Height*4];
            for(int y = 0; y < this.Height ; y++)
            {
                for(int x = 0; x < this.Width ; x++)
                {
                    if (GetHullIdAt(HullIds, x, y) >= 0)
                    {
                        if (!hullsCreated[4 * x + 4 * Width * y])
                        {
                            if (GetHullIdAt(HullIds, x, y - 1) != GetHullIdAt(HullIds, x, y))
                            {
                                List<Vector2> l=new List<Vector2>();
                                WalkHull(HullIds,hullsCreated,x,y,GetHullIdAt(HullIds,x,y),l,0,0.5f);
                                int id=GetHullIdAt(HullIds,x,y);
                                for(int y2 = 0; y2 < this.Height ; y2++)
                                {
                                    for(int x2 = 0; x2 < this.Width ; x2++)
                                    {
                                        if (GetHullIdAt(HullIds, x2, y2) == id)
                                        {
                                            if(TileInfo.IsSolidSlant(GetTileInt(x2,y2),0)){//bottom right
                                                //List<Vector2> l=new List<Vector2>();
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                                                int ind=l.FindIndex(0,1,(Vector2 n)=>{return n==new Vector2(screenTileSize*x2,screenTileSize*(y2));});
                                                if (ind != -1)
                                                {
                                                    l.RemoveAt(ind);
                                                    
                                                }
                                            }
                                            if(TileInfo.IsSolidSlant(GetTileInt(x2,y2),1)){//bottom left
                                                //List<Vector2> l=new List<Vector2>();
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                                                l.Remove(new Vector2(screenTileSize*(x2+1),screenTileSize*(y2)));
                                            }
                                            if(TileInfo.IsSolidSlant(GetTileInt(x2,y2),2)){
                                                //List<Vector2> l=new List<Vector2>();
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y+1)));
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                                                l.Remove(new Vector2(screenTileSize*(x2),screenTileSize*(y2+1)));
                                            }
                                            if(TileInfo.IsSolidSlant(GetTileInt(x2,y2),3)){
                                                //List<Vector2> l=new List<Vector2>();
                                                //l.Add(new Vector2(screenTileSize*(x+1),screenTileSize*(y)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y)));
                                                //l.Add(new Vector2(screenTileSize*x,screenTileSize*(y+1)));
                                                
                                                l.Remove(new Vector2(screenTileSize*(x2+1),screenTileSize*(y2+1)));
                                            }
                                        }
                                    }
                                }
                                shadowHulls.Add(l);
                            }
                        }
                    }
                }
            }
            /*for(int i = 0; i < Width*Height*4 ; i++)
            {
                hullsCreated[i]=false;
            }
            for(int y = 0; y < this.Height ; y++)
            {
                for(int x = 0; x < this.Width ; x++)
                {
                    if (GetHullIdAt(HullIds, x, y) >= 0)
                    {
                        if (!hullsCreated[4 * x + 4 * Width * y])
                        {
                            if (GetHullIdAt(HullIds, x, y - 1) != GetHullIdAt(HullIds, x, y))
                            {
                                List<Vector2> l=new List<Vector2>();
                                WalkHull(HullIds,hullsCreated,x,y,GetHullIdAt(HullIds,x,y),l,0,0.2f);
                                shadowHulls.Add(l);
                            }
                        }
                    }
                }
            }* /
            */
        }
        private void WalkHull(int[] HullIds,bool[] hullsCreated,int x, int y, int targetHull,List<Vector2> list,int dir,float distFromEdge)
        {
            if (hullsCreated[x * 4 + y * Width * 4 + dir] == true)
            {
                return;
            }
            hullsCreated[x*4+y*Width*4+dir]=true;
            bool up=GetHullIdAt(HullIds,x,y-1)==targetHull;
            bool down=GetHullIdAt(HullIds,x,y+1)==targetHull;
            bool left=GetHullIdAt(HullIds,x-1,y)==targetHull;
            bool right=GetHullIdAt(HullIds,x+1,y)==targetHull;
            if ((!up) && (!down) && (!left) && (!right))
            {
                list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                return;
            }
            switch (dir)
            {
                case 0://up
                    if (left == true)
                    {
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x-1,y,targetHull,list,3,distFromEdge);
                    }
                    else if (up==true)
                    {
                        //list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y-1,targetHull,list,0,distFromEdge);
                    }
                    else if (right==true)
                    {
                        hullsCreated[x*4+y*Width*4+1]=true;
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x+1,y,targetHull,list,1,distFromEdge);
                    }
                    else
                    {
                        hullsCreated[x*4+y*Width*4+1]=true;
                        hullsCreated[x*4+y*Width*4+2]=true;
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y+1,targetHull,list,2,distFromEdge);
                    }
                    break;
                case 1://right
                    if (up == true)
                    {
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y-1,targetHull,list,0,distFromEdge);
                    }
                    else if (right==true)
                    {
                        //list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x+1,y,targetHull,list,1,distFromEdge);
                    }
                    else if (down==true)
                    {
                        hullsCreated[x*4+y*Width*4+2]=true;
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y+1,targetHull,list,2,distFromEdge);
                        
                    }
                    else
                    {
                        hullsCreated[x*4+y*Width*4+2]=true;
                        hullsCreated[x*4+y*Width*4+3]=true;
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x-1,y,targetHull,list,3,distFromEdge);
                    }
                    break;
                case 2://down
                    if (right == true)
                    {
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x+1,y,targetHull,list,1,distFromEdge);
                    }
                    else if (down==true)
                    {
                        //list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y+1,targetHull,list,2,distFromEdge);
                    }
                    else if (left==true)
                    {
                        hullsCreated[x*4+y*Width*4+3]=true;
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x-1,y,targetHull,list,3,distFromEdge);
                    }
                    else
                    {
                        hullsCreated[x*4+y*Width*4+3]=true;
                        hullsCreated[x*4+y*Width*4+0]=true;
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y-1,targetHull,list,0,distFromEdge);
                    }
                    break;
                case 3://left
                    if (down == true)
                    {
                        list.Add(new Vector2((x+0.5f+distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y+1,targetHull,list,2,distFromEdge);
                    }
                    else if (left==true)
                    {
                        //list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x-1,y,targetHull,list,3,distFromEdge);
                    }
                    else if (up==true)
                    {
                        hullsCreated[x*4+y*Width*4+0]=true;
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        WalkHull(HullIds,hullsCreated,x,y-1,targetHull,list,0,distFromEdge);
                    }
                    else
                    {
                        
                        list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f+distFromEdge)*screenTileSize));
                        if (!hullsCreated[x*4+y*Width*4+1])
                        {
                            list.Add(new Vector2((x+0.5f-distFromEdge)*screenTileSize,(y+0.5f-distFromEdge)*screenTileSize));
                        }
                        hullsCreated[x*4+y*Width*4+0]=true;
                        hullsCreated[x*4+y*Width*4+1]=true;
                        
                        WalkHull(HullIds,hullsCreated,x+1,y,targetHull,list,1,distFromEdge);
                    }
                    break;
            }
        }
        private int GetHullIdAt(int[] HullIds,int x, int y)
        {
            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height))
            {
                return HullIds[x+y*Width];
            }
            return -1;
        }
        private void HullSpread(int[] HullIds, int x, int y)
        {
            Console.WriteLine("Gen ("+x+", "+y+")");
            if (x > 0)
            {
                if (HullIds[(x - 1) + y * Width] == -1)
                {
                    if (TileInfo.IsOpaque(tiles[x + y * Width]) == TileInfo.IsOpaque(tiles[x - 1 + y * Width]))
                    {
                        HullIds[(x - 1) + y * Width]=HullIds[x + y * Width];
                        HullSpread(HullIds,x-1,y);
                    }
                }
            }
            if (x < (Width-1))
            {
                if (HullIds[(x + 1) + y * Width] == -1)
                {
                    if (TileInfo.IsOpaque(tiles[x + y * Width]) == TileInfo.IsOpaque(tiles[x + 1 + y * Width]))
                    {
                        HullIds[(x + 1) + y * Width]=HullIds[x + y * Width];
                        HullSpread(HullIds,x+1,y);
                    }
                }
            }
            if (y > 0)
            {
                if (HullIds[x + (y-1) * Width] == -1)
                {
                    if (TileInfo.IsOpaque(tiles[x + y * Width]) == TileInfo.IsOpaque(tiles[x + (y-1) * Width]))
                    {
                        HullIds[x + (y-1) * Width]=HullIds[x + y * Width];
                        HullSpread(HullIds,x,y-1);
                    }
                }
            }
            if (y < (Height-1))
            {
                if (HullIds[x + (y+1) * Width] == -1)
                {
                    if (TileInfo.IsOpaque(tiles[x + y * Width]) == TileInfo.IsOpaque(tiles[x + (y+1) * Width]))
                    {
                        HullIds[x + (y+1) * Width]=HullIds[x + y * Width];
                        HullSpread(HullIds,x,y+1);
                    }
                }
            }
        }
        
    }
    public struct RoomLinkInfo
    {
        public Point offset;
        public int id;
        public Point offset2;
        public bool Valid
        {
            get
            {
                if ((RoomID.count <= this.id)||(this.id<0))
                {
                    return false;
                }
                Rectangle r=new Rectangle(offset.X,offset.Y,RoomID.Rooms[this.id].Width*Room.tileSize,RoomID.Rooms[this.id].Height*Room.tileSize);
                return r.Contains(this.offset2);
            }
        }
    }
    public class RoomInstance
    {
        public int id;
        public Vector2 position;
        public List<Light> lights=new List<Light>();
        public List<Hull> hulls=new List<Hull>();
        public Room Room
        {
            get
            {
                return RoomID.Rooms[id];
            }
        }
        public RoomInstance(int id, Vector2 position)
        {
            this.id=id;
            this.position=position;
            foreach(Light light in this.Room.lights){
                if(light is PointLight){
                    this.lights.Add(new PointLight()
                    {
                        Scale = light.Scale,
                        Color = light.Color,
                        CastsShadows = light.CastsShadows,
                        ShadowType = light.ShadowType,
                        Enabled=light.Enabled,
                        Origin=light.Origin,
                        Position=light.Position+position,
                        Radius=light.Radius,
                        Rotation=light.Rotation,
                        Intensity=light.Intensity
                        
                    });
                }else if(light is Spotlight spotlight){
                    this.lights.Add(new Spotlight()
                    {
                        Scale = light.Scale,
                        Color = light.Color,
                        CastsShadows = light.CastsShadows,
                        ShadowType = light.ShadowType,
                        Enabled=light.Enabled,
                        Origin=light.Origin,
                        Position=light.Position+position,
                        Radius=light.Radius,
                        Rotation=light.Rotation,
                        ConeDecay=spotlight.ConeDecay,
                        Intensity=spotlight.Intensity
                    });
                }else if(light is TexturedLight texturedLight){
                    this.lights.Add(new TexturedLight()
                    {
                        Scale = light.Scale,
                        Color = light.Color,
                        CastsShadows = light.CastsShadows,
                        ShadowType = light.ShadowType,
                        Enabled=light.Enabled,
                        Origin=light.Origin,
                        Position=light.Position+position,
                        Radius=light.Radius,
                        Rotation=light.Rotation,
                        Texture=texturedLight.Texture
                        
                    });
                }
            }
            foreach(List<Vector2> hullInfo in this.Room.shadowHulls)
            {
                List<Vector2> altHullInfo=new List<Vector2>();
                foreach(Vector2 hullPoint in hullInfo)
                {altHullInfo.Add(hullPoint+position);}
                this.hulls.Add(new Hull(altHullInfo));
            }
        }
        public bool OnScreen
        {
            get
            {
                return Game1.instance.ScreenRect.Intersects(this.Rectangle);
            }
        }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(this.position.ToPoint(),new Point(Room.screenTileSize * this.Room.Width, Room.screenTileSize * this.Room.Height));
            }
        }
        public void Update()
        {
            Rectangle r1=Game1.instance.ScreenRect;
            r1.Inflate(800,800);
            
            if(r1.Intersects(this.Rectangle))
            {
                for(int i = 0; i < this.Room.linkedRooms.Length; i++)
                {
                    RoomLinkInfo info=this.Room.linkedRooms[i];
                    if(info.Valid){
                        Point c=this.position.ToPoint()+info.offset2;
                        Rectangle r=new Rectangle(this.position.ToPoint()+info.offset,new Point(RoomID.Rooms[info.id].Width*Room.screenTileSize,RoomID.Rooms[info.id].Height*Room.screenTileSize));
                        
                        if(r1.Intersects(r))
                        {
                            bool isAlreadyPresent=false;
                            foreach(RoomInstance inst in Game1.instance.rooms)
                            {
                                if (inst.Rectangle.Contains(c))
                                {
                                    isAlreadyPresent=true;
                                }
                            }
                            if (isAlreadyPresent == false)
                            {
                                RoomInstance inst=new RoomInstance(info.id,info.offset.ToVector2()+this.position);
                                inst.AddLights(Game1.instance.penumbra);
                                Game1.instance.rooms.Add(inst);
                                Console.WriteLine("Created Room Instance with Id "+info.id);
                            }
                        }
                    }
                }
            }
            else
            {
                Game1.instance.rooms.Remove(this);
                this.Remove(Game1.instance.penumbra);
                Console.WriteLine("Removed Room Instance with Id "+this.id);
            }
        }
        public void AddLights(PenumbraComponent penumbra)
        {
            foreach(Light light in this.lights){
                penumbra.Lights.Add(light);
            }
            foreach(Hull hull in this.hulls){
                penumbra.Hulls.Add(hull);
            }
        }
        public void Remove(PenumbraComponent penumbra)
        {
            foreach(Light light in this.lights){
                penumbra.Lights.Remove(light);
            }
            Console.WriteLine(penumbra.Hulls.Count);
            foreach(Hull hull in this.hulls){
                penumbra.Hulls.Remove(hull);
            }
            Console.WriteLine(penumbra.Hulls.Count);
        }
        public void Draw(SpriteBatch batch,bool onlyOpaques)
        {
            for(int y = 0; y < this.Room.Height; y++)
            {
                for(int x = 0; x < this.Room.Width; x++)
                {
                    if((!onlyOpaques)||(TileInfo.IsOpaque(this.Room.tiles[x+y*this.Room.Width]))){
                        Texture2D texture=TileInfo.GetTexture(this.Room.tiles[x+y*this.Room.Width]);
                        int texwidth=texture.Width;
                        int texheight=texture.Height;
                        batch.Draw(texture, new Rectangle((int)position.X+x*Room.screenTileSize-Game1.instance.ScreenPos.X, (int)position.Y+y*Room.screenTileSize-Game1.instance.ScreenPos.Y, Room.screenTileSize, Room.screenTileSize), new Rectangle(0,0,texwidth,texheight), Color.White);
                    }
                }
            }
            //Draw points of each hull, for debugging.
            /*
            if (!onlyOpaques)
            {
                foreach(Penumbra.Hull hull in this.hulls){
                    foreach(Vector2 point in hull.Points){
                        batch.Draw(TileInfo.GetTexture(1), new Rectangle((int)point.X-Game1.instance.ScreenPos.X-2, (int)point.Y-Game1.instance.ScreenPos.Y-2, 4, 4), Color.Green);
                    }
                }
            }*/
            

        }
        public bool IsColliding(Rectangle rectangle)
        {
            int left=rectangle.Left-(int)this.position.X;
            int top=rectangle.Top-(int)this.position.Y;
            int width=rectangle.Width;
            int height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                
                return false;
            }
            for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
            {
                for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                {
                    int t=Room.GetTile(x,y);
                    if(TileInfo.IsSolid(t)){
                        if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                        {
                            if(TileInfo.IsFullSolid(t)){
                                    return true;
                                
                            }
                            else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                            {
                                
                                    if((x+y+1)*Room.tileSize<(left+top+width+height))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,1))//Bottom left slant
                            {
                                
                                    if((x-y)*Room.tileSize>(left-top-height))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                            {
                                
                                    if((x-y)*Room.tileSize<(left-top+width))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,3))//Top left slant
                            {
                                
                                    if((x+y+1)*Room.tileSize>(left+top))
                                    return true;
                                
                            }
                        }
                    }
                }
            }
            
            return false;
        }
        public bool IsColliding(FloatRectangle rectangle)
        {
            float left=rectangle.Left-this.position.X;
            float top=rectangle.Top-this.position.Y;
            float width=rectangle.Width;
            float height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                
                return false;
            }
            for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
            {
                for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                {
                    int t=Room.GetTile(x,y);
                    if(TileInfo.IsSolid(t)){
                        if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                        {
                            if(TileInfo.IsFullSolid(t)){
                                    return true;
                                
                            }
                            else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                            {
                                
                                    if((x+y+1)*Room.tileSize<(left+top+width+height))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,1))//Bottom left slant
                            {
                                
                                    if((x-y)*Room.tileSize>(left-top-height))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                            {
                                
                                    if((x-y)*Room.tileSize<(left-top+width))
                                    return true;
                                
                            }else if(TileInfo.IsSolidSlant(t,3))//Top left slant
                            {
                                
                                    if((x+y+1)*Room.tileSize>(left+top))
                                    return true;
                                
                            }
                        }
                    }
                }
            }
            
            return false;
        }
        public bool IsColliding(FloatRectangle rectangle,FloatRectangle last)
        {
            float left=rectangle.Left-this.position.X;
            float top=rectangle.Top-this.position.Y;
            float width=rectangle.Width;
            float height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                
                return false;
            }
            for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
            {
                for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                {
                    int t=Room.GetTile(x,y);
                    if(TileInfo.IsSolid(t)){
                        if(!TileInfo.IsPlatform(t)){
                            if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                            {
                                if(TileInfo.IsFullSolid(t)){
                                        return true;
                                    
                                }
                                else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                {
                                    
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                        return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,1))//Bottom left slant
                                {
                                    
                                        if((x-y)*Room.tileSize>(left-top-height))
                                        return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                {
                                    
                                        if((x-y)*Room.tileSize<(left-top+width))
                                        return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,3))//Top left slant
                                {
                                    
                                        if((x+y+1)*Room.tileSize>(left+top))
                                        return true;
                                    
                                }
                            }
                        }
                        else
                        {
                            
                            if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                            {
                                if(TileInfo.IsFullSolid(t)){
                                        return (last.Bottom<=(y*Room.tileSize+(int)position.Y));
                                    
                                }
                                else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                {
                                    
                                        //if((x+y+1)*Room.tileSize<(left+top+width+height))
                                        //return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,1))//Bottom left slant
                                {
                                    
                                        //if((x-y)*Room.tileSize>(left-top-height))
                                        //return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                {
                                    
                                        //if((x-y)*Room.tileSize<(left-top+width))
                                        //return true;
                                    
                                }else if(TileInfo.IsSolidSlant(t,3))//Top left slant
                                {
                                    
                                        //if((x+y+1)*Room.tileSize>(left+top))
                                        //return true;
                                    
                                }
                            }
                        }
                    }
                }
            }
            
            return false;
        }
        public FloatRectangle GetCollisionObject(FloatRectangle rectangle,int dir)
        {
            float left=rectangle.Left-this.position.X;
            float top=rectangle.Top-this.position.Y;
            float width=rectangle.Width;
            float height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                return rectangle;
            }
            
            //moving up,right,down,left
            float current=0;
            switch (dir)
            {
                case 0://moving up
                    current=top;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(y+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,((left+width)>((x+1)*Room.tileSize))?(y+1)*Room.tileSize:(left+width-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((left)<(x*Room.tileSize))?(y+1)*Room.tileSize:((x+y+1)*Room.tileSize-left));
                                    }
                                }
                            }
                        }
                    }
                break;
                case 1://moving right
                    current=left+width;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((top+height)>((y+1)*Room.tileSize))?(x)*Room.tileSize:((x+y+1)*Room.tileSize-top-height));
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,((top)<((y)*Room.tileSize))?(x)*Room.tileSize:((x-y)*Room.tileSize+top));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 2://moving down
                    current=top+height;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Min(current,(y)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((left+width)>((x+1)*Room.tileSize))?(y)*Room.tileSize:((x+y+1)*Room.tileSize-left-width));
                                    }else if(TileInfo.IsSolidSlant(t,2))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,(y)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(left<((x)*Room.tileSize))?(y)*Room.tileSize:(left-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,y*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 3://moving left
                    current=left;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(x+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,((top+height)>((y+1)*Room.tileSize))?(x+1)*Room.tileSize:((x-y)*Room.tileSize+top+height));
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((top)<((y)*Room.tileSize))?(x+1)*Room.tileSize:((x+y+1)*Room.tileSize-top));
                                    }
                                }
                            }
                        }
                    }
                break;
            }


            switch (dir)
            {
                case 0:
                    //top=current
                    return new FloatRectangle(rectangle.X,current+(int)position.Y,rectangle.Width,rectangle.Height);
                case 1:
                    //right=current
                    return new FloatRectangle(current+(int)position.X-rectangle.Width,rectangle.Y,rectangle.Width,rectangle.Height);
                case 2:
                    //bottom=current
                    return new FloatRectangle(rectangle.X,current+(int)position.Y-rectangle.Height,rectangle.Width,rectangle.Height);
                case 3:
                    //left=current
                    return new FloatRectangle(current+(int)position.X,rectangle.Y,rectangle.Width,rectangle.Height);
            }
            return rectangle;
        }
        public FloatRectangle GetCollisionObject(FloatRectangle rectangle,FloatRectangle last,int dir)
        {
            float left=rectangle.Left-this.position.X;
            float top=rectangle.Top-this.position.Y;
            float width=rectangle.Width;
            float height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                return rectangle;
            }
            
            //moving up,right,down,left
            float current=0;
            switch (dir)
            {
                case 0://moving up
                    current=top;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)&&(!TileInfo.IsPlatform(t))){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(y+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,((left+width)>((x+1)*Room.tileSize))?(y+1)*Room.tileSize:(left+width-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((left)<(x*Room.tileSize))?(y+1)*Room.tileSize:((x+y+1)*Room.tileSize-left));
                                    }
                                }
                            }
                        }
                    }
                break;
                case 1://moving right
                    current=left+width;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)&&(!TileInfo.IsPlatform(t))){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((top+height)>((y+1)*Room.tileSize))?(x)*Room.tileSize:((x+y+1)*Room.tileSize-top-height));
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,((top)<((y)*Room.tileSize))?(x)*Room.tileSize:((x-y)*Room.tileSize+top));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 2://moving down
                    current=top+height;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        if (TileInfo.IsPlatform(t))
                                        {
                                            if(last.Bottom<=(y*Room.tileSize+(int)position.Y)){
                                                current=Math.Min(current,(y)*Room.tileSize);
                                            }
                                        }
                                        else{
                                            current=Math.Min(current,(y)*Room.tileSize);
                                        }
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0)&&(!TileInfo.IsPlatform(t)))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((left+width)>((x+1)*Room.tileSize))?(y)*Room.tileSize:((x+y+1)*Room.tileSize-left-width));
                                    }else if(TileInfo.IsSolidSlant(t,2)&&(!TileInfo.IsPlatform(t)))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,(y)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1)&&(!TileInfo.IsPlatform(t)))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(left<((x)*Room.tileSize))?(y)*Room.tileSize:(left-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,3)&&(!TileInfo.IsPlatform(t)))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,y*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 3://moving left
                    current=left;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)&&(!TileInfo.IsPlatform(t))){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(x+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,((top+height)>((y+1)*Room.tileSize))?(x+1)*Room.tileSize:((x-y)*Room.tileSize+top+height));
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((top)<((y)*Room.tileSize))?(x+1)*Room.tileSize:((x+y+1)*Room.tileSize-top));
                                    }
                                }
                            }
                        }
                    }
                break;
            }


            switch (dir)
            {
                case 0:
                    //top=current
                    return new FloatRectangle(rectangle.X,current+(int)position.Y,rectangle.Width,rectangle.Height);
                case 1:
                    //right=current
                    return new FloatRectangle(current+(int)position.X-rectangle.Width,rectangle.Y,rectangle.Width,rectangle.Height);
                case 2:
                    //bottom=current
                    return new FloatRectangle(rectangle.X,current+(int)position.Y-rectangle.Height,rectangle.Width,rectangle.Height);
                case 3:
                    //left=current
                    return new FloatRectangle(current+(int)position.X,rectangle.Y,rectangle.Width,rectangle.Height);
            }
            return rectangle;
        }
        public Rectangle GetCollisionObject(Rectangle rectangle,int dir)
        {
            int left=rectangle.Left-(int)this.position.X;
            int top=rectangle.Top-(int)this.position.Y;
            int width=rectangle.Width;
            int height=rectangle.Height;
            int roomWidth= this.Room.Width*Room.tileSize;
            int roomHeight= this.Room.Height*Room.tileSize;
            if (((left + width) < 0) || ((top + height) < 0)|| ((left) > roomWidth)|| ((top) >  roomHeight))
            {
                return rectangle;
            }
            
            //moving up,right,down,left
            int current=0;
            switch (dir)
            {
                case 0://moving up
                    current=top;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(y+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,((left+width)>((x+1)*Room.tileSize))?(y+1)*Room.tileSize:(left+width-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,(y+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((left)<(x*Room.tileSize))?(y+1)*Room.tileSize:((x+y+1)*Room.tileSize-left));
                                    }
                                }
                            }
                        }
                    }
                break;
                case 1://moving right
                    current=left+width;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((top+height)>((y+1)*Room.tileSize))?(x)*Room.tileSize:((x+y+1)*Room.tileSize-top-height));
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,((top)<((y)*Room.tileSize))?(x)*Room.tileSize:((x-y)*Room.tileSize+top));
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,(x)*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 2://moving down
                    current=top+height;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Min(current,(y)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Min(current,((left+width)>((x+1)*Room.tileSize))?(y)*Room.tileSize:((x+y+1)*Room.tileSize-left-width));
                                    }else if(TileInfo.IsSolidSlant(t,2))//top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Min(current,(y)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Min(current,(left<((x)*Room.tileSize))?(y)*Room.tileSize:(left-(x-y)*Room.tileSize));
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Min(current,y*Room.tileSize);
                                    }
                                }
                            }
                        }
                    }
                break;
                case 3://moving left
                    current=left;
                    for(int y = (int)(Math.Max(top/Room.tileSize,0)); y <= (int)(Math.Min((top + height)/Room.tileSize, roomHeight/Room.tileSize))+1;y++)
                    {
                        for(int x = (int)(Math.Max(left/Room.tileSize,0)); x <= (int)(Math.Min((left + width)/Room.tileSize, roomWidth/Room.tileSize))+1;x++)
                        {
                            int t=Room.GetTile(x,y);
                            if(TileInfo.IsSolid(t)){
                                if(rectangle.Intersects(new Rectangle(x*Room.tileSize+(int)position.X,y*Room.tileSize+(int)position.Y,Room.tileSize,Room.tileSize)))
                                {
                                    if(TileInfo.IsFullSolid(t)){
                                        current=Math.Max(current,(x+1)*Room.tileSize);
                                    }
                                    else if(TileInfo.IsSolidSlant(t,0))//Bottom right slant
                                    {
                                        if((x+y+1)*Room.tileSize<(left+top+width+height))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,2))//Top right slant
                                    {
                                        if((x-y)*Room.tileSize<(left-top+width))
                                            current=Math.Max(current,(x+1)*Room.tileSize);
                                    }else if(TileInfo.IsSolidSlant(t,1))//bottom left slant
                                    {
                                        if((x-y)*Room.tileSize>(left-top-height))
                                            current=Math.Max(current,((top+height)>((y+1)*Room.tileSize))?(x+1)*Room.tileSize:((x-y)*Room.tileSize+top+height));
                                    }else if(TileInfo.IsSolidSlant(t,3))//top left slant
                                    {
                                        if((x+y+1)*Room.tileSize>(left+top))
                                            current=Math.Max(current,((top)<((y)*Room.tileSize))?(x+1)*Room.tileSize:((x+y+1)*Room.tileSize-top));
                                    }
                                }
                            }
                        }
                    }
                break;
            }


            switch (dir)
            {
                case 0:
                    //top=current
                    return new Rectangle(rectangle.X,current+(int)position.Y,rectangle.Width,rectangle.Height);
                case 1:
                    //right=current
                    return new Rectangle(current+(int)position.X-rectangle.Width,rectangle.Y,rectangle.Width,rectangle.Height);
                case 2:
                    //bottom=current
                    return new Rectangle(rectangle.X,current+(int)position.Y-rectangle.Height,rectangle.Width,rectangle.Height);
                case 3:
                    //left=current
                    return new Rectangle(current+(int)position.X,rectangle.Y,rectangle.Width,rectangle.Height);
            }
            return rectangle;
        }


    }
}
