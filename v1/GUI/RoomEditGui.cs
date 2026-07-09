using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using agame;
using agame.Rendering;
using agame.IDs;
using agame.Rooms.Tile;
using agame.Rooms;
using agame.RoomEditor;
using System;
namespace agame.GUI;

public class RoomEditGui:GuiFrame
{
    public RoomEditGui(Texture2D _texture,int left,int top, int width, int height):base(_texture,left,top,width,height,true,Color.AliceBlue,"")
    {
        GuiElement element=new GuiSingleLineTextBox(_texture,left+10,top+10,width-30,20,Color.AliceBlue,Color.White);
        element.id=1;
        this.Add(element);
        element=new GuiSingleLineTextBox(_texture,left+width-80,top+10,50,10,Color.AliceBlue,Color.Red){alignment=TextAlignment.Middle};
        element.id=5;
        element.visible=false;
        this.Add(element);
        element=new GuiSingleLineTextBox(_texture,left+width-80,top+20,50,10,Color.AliceBlue,Color.Blue){alignment=TextAlignment.Middle};
        element.id=6;
        element.visible=false;
        this.Add(element);
        element=new GuiButton(_texture,left+width-30,top+10,10,20,Color.Green);
        element.id=4;
        element.visible=false;
        this.Add(element);
        element=new GuiSingleLineTextBox(_texture,left+10,top+30,width-40,20,Color.AliceBlue,Color.White);
        element.id=2;
        this.Add(element);
        element=new GuiButton(_texture,left+10,top+54,width-20,height-94,Color.Black){triggerOnDrag=true};//edit box
        element.id=3;
        this.Add(element);
        element=new GuiButton(_texture,left+width-20,top+10,10,20,Color.Black);
        element.id=7;
        element.visible=true;
        this.Add(element);

        element=new GuiButton(_texture,left+10,top+height-30,10,10,Color.Red);//left
        element.id=10;
        this.Add(element);
        element=new GuiButton(_texture,left+30,top+height-30,10,10,Color.Red);//right
        element.id=11;
        this.Add(element);
        element=new GuiButton(_texture,left+20,top+height-40,10,10,Color.Red);//up
        element.id=12;
        this.Add(element);
        element=new GuiButton(_texture,left+20,top+height-20,10,10,Color.Red);//down
        element.id=13;
        this.Add(element);

        element=new GuiButton(_texture,left+width-30,top+10,10,20,Color.Black);
        element.id=17;
        element.visible=false;
        this.Add(element);
        
    }
    private int CurrentRoom=-1;
    private int CurrentTile=-1;
    private Vector2 offset=Vector2.Zero;
    private int lastKnownNumTiles=TileID.count;
    public void FloodFillRoom(int x, int y)
    {
        int t=RoomID.Rooms[CurrentRoom].GetTile(x,y);
        if(t!=CurrentTile){
            RoomID.Rooms[CurrentRoom].SetTile(x,y,CurrentTile);
            if(RoomID.Rooms[CurrentRoom].GetTile(x, y)==CurrentTile){
                if (RoomID.Rooms[CurrentRoom].GetTile(x+1, y) == t)
                {
                    FloodFillRoom(x+1,y);
                }
                if (RoomID.Rooms[CurrentRoom].GetTile(x-1, y) == t)
                {
                    FloodFillRoom(x-1,y);
                }
                if (RoomID.Rooms[CurrentRoom].GetTile(x, y+1) == t)
                {
                    FloodFillRoom(x,y+1);
                }
                if (RoomID.Rooms[CurrentRoom].GetTile(x, y-1) == t)
                {
                    FloodFillRoom(x,y-1);
                }
            }
        }
    }
    public override void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
        this.children[0].Resize(left+10,top+10,width-30,20);
        this.children[1].Resize(left+width-80,top+10,50,10);
        this.children[2].Resize(left+width-80,top+20,50,10);
        this.children[3].Resize(left+width-30,top+10,10,20);
        this.children[4].Resize(left+10,top+30,width-40,20);
        this.children[5].Resize(left+10,top+54,width-20,height-94);
        this.children[6].Resize(left+width-20,top+10,10,20);
        this.children[7].Resize(left+10,top+height-30,10,10);
        this.children[8].Resize(left+30,top+height-30,10,10);
        this.children[9].Resize(left+20,top+height-40,10,10);
        this.children[10].Resize(left+20,top+height-20,10,10);
        this.children[11].Resize(left+width-30,top+10,10,20);
        
    }
    public override void OnUpdate(int id,GuiElement obj)
    {
        
        if (id == 1)
        {
            int rid=IDMaps.GetRoomFromName(((GuiSingleLineTextBox)obj).Content);
            if ((rid<RoomID.count)&&(rid>=0))
            {
                CurrentRoom=rid;
                this.children[1].visible=false;
                this.children[2].visible=false;
                this.children[3].visible=false;
                this.children[11].visible=true;
                obj.width=width-40;
            }
            else
            {
                CurrentRoom=-1;
                if (((GuiSingleLineTextBox)obj).Content.Length > 0)
                {
                    this.children[1].visible=true;
                    this.children[2].visible=true;
                    this.children[3].visible=true;
                    this.children[11].visible=false;
                    obj.width=width-90;
                }
                else
                {
                    this.children[1].visible=false;
                    this.children[2].visible=false;
                    this.children[3].visible=false;
                    this.children[11].visible=false;
                    obj.width=width-30;
                }
            }
        }
        else if (id == 2)
        {
            int rid=IDMaps.GetTileFromName(((GuiSingleLineTextBox)obj).Content);
            if (rid<TileID.count)
            {
                CurrentTile=rid;
            }
            else
            {
                CurrentTile=-1;
            }
        }else if (id == 3)
        {
            if ((CurrentRoom != -1)&&(CurrentTile != -1))
            {
                int x=(int)(((GuiButton)obj).lastClickPos.GetValueOrDefault().X/Room.screenTileSize)+(int)this.offset.X;
                int y=(int)(((GuiButton)obj).lastClickPos.GetValueOrDefault().Y/Room.screenTileSize)+(int)this.offset.Y;
                if(((GuiButton)obj).clickId==1){
                    RoomID.Rooms[CurrentRoom].SetTile(x,y,CurrentTile);
                }else if(((GuiButton)obj).clickId==2){
                    FloodFillRoom(x,y);
                }
                RoomID.Rooms[CurrentRoom].GenHulls();
            }
        }else if (id == 4)
        {
            try{
                int width=StringToInt(((GuiSingleLineTextBox)this.children[1]).Content);
                int height=StringToInt(((GuiSingleLineTextBox)this.children[2]).Content);
                RoomEditLogic.AddRoomToList(((GuiSingleLineTextBox)this.children[0]).Content,width,height);
                this.OnUpdate(1,this.children[0]);
            }
            catch
            {
                Console.WriteLine("Numbers Invalid! ("+((GuiSingleLineTextBox)this.children[1]).Content+","+((GuiSingleLineTextBox)this.children[2]).Content+")");
            }
            
            
        }else if (id == 7)
        {
            foreach(RoomInstance r in Game1.instance.rooms){
                r.Remove(Game1.instance.penumbra);
            }
            Game1.instance.rooms.Clear();
            RoomInstance inst=new RoomInstance(CurrentRoom,new Vector2((int)(Game1.instance.e.Position.X-RoomID.Rooms[CurrentRoom].Width*Room.tileSize/2),(int)Game1.instance.e.Position.Y+50));
            Game1.instance.rooms.Add(inst);
            inst.AddLights(Game1.instance.penumbra);
            
            
        }else if (id == 10)
        {
            this.offset.X-=1;
        }else if (id == 11)
        {
            this.offset.X+=1;
        }else if (id == 12)
        {
            this.offset.Y-=1;
        }else if (id == 13)
        {
            this.offset.Y+=1;
        }else if (id == 17)
        {
            RoomEditLogic.RemoveRoomFromList(this.CurrentRoom);
            this.OnUpdate(1,this.children[0]);
        }
    }
    private int StringToInt(string input)
    {
        int value=0;
        foreach(char c in input)
        {
            value*=10;
            if (c == '0')
            {
                value+=0;
            }else if (c == '1')
            {
                value+=1;
            }else if (c == '2')
            {
                value+=2;
            }else if (c == '3')
            {
                value+=3;
            }else if (c == '4')
            {
                value+=4;
            }else if (c == '5')
            {
                value+=5;
            }else if (c == '6')
            {
                value+=6;
            }else if (c == '7')
            {
                value+=7;
            }else if (c == '8')
            {
                value+=8;
            }else if (c == '9')
            {
                value+=9;
            }else if (char.IsNumber(c)==false)
            {
                value/=10;
            }
            else
            {
                throw new Exception("Invalid Int!");
            }

        }
        return value;
    }
    int lastScroll=0;
    int minScrollChange=99999999;
    public override void DrawExtras(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        int vscrollchange=Game1.instance.lastMouseState.ScrollWheelValue-lastScroll;
        if (Math.Abs(vscrollchange) > 0)
        {
            minScrollChange=Math.Min(minScrollChange,Math.Abs(vscrollchange));
        }
        if (this.children[5].hovered)
        {
            if(Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)|Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift)){
                this.offset.X-=vscrollchange/minScrollChange;
            }
            else
            {
                this.offset.Y-=vscrollchange/minScrollChange;
            }
        }
        lastScroll+=vscrollchange;
        if(lastKnownNumTiles!=TileID.count){
        this.OnUpdate(0,this.children[0]);
        lastKnownNumTiles=TileID.count;
        }
        if (CurrentTile != -1)
        {
            _spriteBatch.Draw(TileInfo.GetTexture(CurrentTile),new Rectangle(left+width-30,top+30,20,20),Color.White);
        }
        if (CurrentRoom != -1)
        {
            GuiElement e=this.children[5];
            RoomID.Rooms[CurrentRoom].Draw(left+10-Room.screenTileSize*(int)offset.X,
            top+54-Room.screenTileSize*(int)offset.Y,
            _spriteBatch,
            (int)offset.X,
            (int)offset.X+(int)(e.width/Room.screenTileSize),
            (int)offset.Y,
            (int)offset.Y+(int)(e.height/Room.screenTileSize));
        }
    }
}
