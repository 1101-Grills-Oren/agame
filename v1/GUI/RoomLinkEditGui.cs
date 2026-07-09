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

public class RoomLinkEditGui:GuiFrame
{
    public RoomLinkEditGui(Texture2D _texture,int left,int top, int width, int height):base(_texture,left,top,width,height,true,Color.AliceBlue,"")
    {
        //Room select
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+10,width-20,20,Color.AliceBlue,Color.White){id=1});
        //Room link select
        this.Add(new GuiSingleLineTextBox(_texture,left+20,top+30,width-40,20,Color.AliceBlue,Color.White){id=2,visible=false});
        //room link increment
        this.Add(new GuiButton(_texture,left+10,top+30,10,10,Color.AliceBlue){id=3,visible=false});
        //room link decrement
        this.Add(new GuiButton(_texture,left+10,top+40,10,10,Color.AliceBlue){id=4,visible=false});
        //room link create new
        this.Add(new GuiButton(_texture,left+width-20,top+30,10,10,Color.Green){id=5,visible=false});
        //room link remove
        this.Add(new GuiButton(_texture,left+width-20,top+40,10,10,Color.Red){id=7,visible=false});
        //room link target select
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+50,width-20,20,Color.AliceBlue,Color.White){id=6,visible=false});
        //room link offset1x
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+70,(width-20)/2,20,Color.AliceBlue,Color.White){id=10,visible=false});
        //room link offset1y
        this.Add(new GuiSingleLineTextBox(_texture,left+10+(width-20)/2,top+70,(width-20)/2,20,Color.AliceBlue,Color.White){id=11,visible=false});
        //room link offset2x
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+90,(width-20)/2,20,Color.AliceBlue,Color.White){id=20,visible=false});
        //room link offset2y
        this.Add(new GuiSingleLineTextBox(_texture,left+10+(width-20)/2,top+90,(width-20)/2,20,Color.AliceBlue,Color.White){id=21,visible=false});
        
        
    }
    public override void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
        this.children[0].Resize(left+10,top+10,width-20,20);
        this.children[1].Resize(left+20,top+30,width-40,20);
        this.children[2].Resize(left+10,top+30,10,10);
        this.children[3].Resize(left+10,top+40,10,10);
        this.children[4].Resize(left+width-20,top+30,10,10);
        this.children[5].Resize(left+width-20,top+40,10,10);
        this.children[6].Resize(left+10,top+50,width-20,20);
        this.children[7].Resize(left+10,top+70,(width-20)/2,20);
        this.children[8].Resize(left+10+(width-20)/2,top+70,(width-20)/2,20);
        this.children[9].Resize(left+10,top+90,(width-20)/2,20);
        this.children[10].Resize(left+10+(width-20)/2,top+90,(width-20)/2,20);
        
    }
    private int CurrentRoom=-1;
    private int CurrentRoomLink=0;
    private int lastKnownNumRooms=RoomID.count;
    private Point offset=Point.Zero;
    public override void OnUpdate(int id,GuiElement obj)
    {
        
        switch(id){
            case 1:
                int rid=IDMaps.GetRoomFromName(((GuiSingleLineTextBox)obj).Content);
                if ((rid<RoomID.count)&&(rid>=0))
                {
                    CurrentRoom=rid;
                    this.children[1].visible=true;
                    this.children[2].visible=true;
                    this.children[3].visible=true;
                    this.children[4].visible=true;
                    this.children[5].visible=true;
                    OnUpdate(2,this.children[1]);
                }
                else
                {
                    this.children[1].visible=false;
                    this.children[2].visible=false;
                    this.children[3].visible=false;
                    this.children[4].visible=false;
                    this.children[5].visible=false;
                    OnUpdate(2,this.children[1]);
                }
                break;
            case 2:
                try
                {
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    CurrentRoomLink=i;
                    if(((GuiSingleLineTextBox)obj).Content.Length>0){
                        if(CurrentRoom>=0){
                            if ((i >= 0) && (i < RoomID.Rooms[CurrentRoom].linkedRooms.Length))
                            {
                                this.children[4].height=10;
                                this.children[5].visible=obj.visible;
                                this.children[6].visible=obj.visible;
                                this.children[7].visible=obj.visible;
                                this.children[8].visible=obj.visible;
                                this.children[9].visible=obj.visible;
                                this.children[10].visible=obj.visible;
                                if(RoomID.Rooms[CurrentRoom].linkedRooms[i].id!=-1)
                                ((GuiSingleLineTextBox)this.children[6]).Content=IDMaps.RoomNames[RoomID.Rooms[CurrentRoom].linkedRooms[i].id];
                                else
                                {
                                    ((GuiSingleLineTextBox)this.children[6]).Content="";
                                }
                                ((GuiSingleLineTextBox)this.children[7]).Content=((int)Math.Floor((double)RoomID.Rooms[CurrentRoom].linkedRooms[i].offset.X/Room.tileSize)).ToString();
                                ((GuiSingleLineTextBox)this.children[8]).Content=((int)Math.Floor((double)RoomID.Rooms[CurrentRoom].linkedRooms[i].offset.Y/Room.tileSize)).ToString();
                                ((GuiSingleLineTextBox)this.children[9]).Content=((int)Math.Floor((double)RoomID.Rooms[CurrentRoom].linkedRooms[i].offset2.X/Room.tileSize)).ToString();
                                ((GuiSingleLineTextBox)this.children[10]).Content=((int)Math.Floor((double)RoomID.Rooms[CurrentRoom].linkedRooms[i].offset2.Y/Room.tileSize)).ToString();
                            }
                            else
                            {
                                this.children[4].height=20;
                                this.children[5].visible=false;
                                this.children[6].visible=false;
                                this.children[7].visible=false;
                                this.children[8].visible=false;
                                this.children[9].visible=false;
                                this.children[10].visible=false;
                            }
                        }
                        else
                        {
                            this.children[4].height=20;
                            this.children[5].visible=false;
                            this.children[6].visible=false;
                            this.children[7].visible=false;
                            this.children[8].visible=false;
                            this.children[9].visible=false;
                            this.children[10].visible=false;
                        }
                    }else
                    {
                        this.children[4].height=20;
                        this.children[5].visible=false;
                        this.children[6].visible=false;
                        this.children[7].visible=false;
                        this.children[8].visible=false;
                        this.children[9].visible=false;
                        this.children[10].visible=false;
                    }
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=CurrentRoomLink.ToString();
                }
                break;
            case 3:
                ((GuiSingleLineTextBox)this.children[1]).Content=(StringToInt(((GuiSingleLineTextBox)this.children[1]).Content)+1).ToString();
                OnUpdate(2,this.children[1]);
                break;
            case 4:
                ((GuiSingleLineTextBox)this.children[1]).Content=(StringToInt(((GuiSingleLineTextBox)this.children[1]).Content)-1).ToString();
                OnUpdate(2,this.children[1]);
                break;
            case 7:
                if(CurrentRoom>=0){
                    if ((CurrentRoomLink >= 0) && (CurrentRoomLink < RoomID.Rooms[CurrentRoom].linkedRooms.Length))
                    {
                        RoomID.Rooms[CurrentRoom].linkedRooms=RoomEditLogic.PopArrayValue(RoomID.Rooms[CurrentRoom].linkedRooms,CurrentRoomLink);
                    }
                }
                OnUpdate(2,this.children[1]);
                break;
            case 5:
                if(CurrentRoom>=0){
                    RoomID.Rooms[CurrentRoom].linkedRooms=RoomEditLogic.AppendArrayValue(RoomID.Rooms[CurrentRoom].linkedRooms,new RoomLinkInfo{id=-1});
                    CurrentRoomLink=RoomID.Rooms[CurrentRoom].linkedRooms.Length-1;
                    ((GuiSingleLineTextBox)this.children[1]).Content=CurrentRoomLink.ToString();
                }
                OnUpdate(2,this.children[1]);
                break;
            case 6:
                int j=IDMaps.GetRoomFromName(((GuiSingleLineTextBox)obj).Content);
                if(j>0){
                RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].id=j;
                }
                break;
            case 10:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset.X=i*Room.tileSize;
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=((int)(RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset.X/Room.tileSize)).ToString();
                }
                break;
            case 11:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset.Y=i*Room.tileSize;
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=((int)(RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset.Y/Room.tileSize)).ToString();
                }
                break;
            case 20:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset2.X=(int)((i+0.5)*Room.tileSize);
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=((int)(RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset2.X/Room.tileSize)).ToString();
                }
                break;
            case 21:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset2.Y=(int)((i+0.5)*Room.tileSize);
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=((int)(RoomID.Rooms[CurrentRoom].linkedRooms[CurrentRoomLink].offset2.Y/Room.tileSize)).ToString();
                }
                break;
        }
        
    }
    private int StringToInt(string input)
    {
        if (input.Length == 0)
        {
            return 0;
        }
        else
        {
            if (input[0].Equals('-'))
            {
                return -StringToInt(input.Substring(1));
            }
        }
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
    public override void DrawExtras(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        if(lastKnownNumRooms!=RoomID.count){
        this.OnUpdate(0,this.children[0]);
        lastKnownNumRooms=RoomID.count;
        }
        if (CurrentRoom != -1)
        {
            GuiElement e=this.children[5];
            RoomID.Rooms[CurrentRoom].Draw(left+10-Room.screenTileSize*(int)offset.X,top+54-Room.screenTileSize*(int)offset.Y,_spriteBatch,(int)offset.X,(int)offset.X+(int)(e.width/Room.screenTileSize),(int)offset.Y,(int)offset.Y+(int)(e.height/Room.screenTileSize));
        }
    }
}
