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

public class TileEditGui:GuiFrame
{
    public TileEditGui(Texture2D _texture,int left,int top, int width, int height):base(_texture,left,top,width,height,true,Color.AliceBlue,"")
    {
        //select box
        GuiElement element=new GuiSingleLineTextBox(_texture,left+10,top+10,width-20,20,Color.AliceBlue,Color.White);
        element.id=1;
        this.Add(element);

        //add tile button
        element=new GuiButton(_texture,left+width-20,top+10,10,20,Color.Lime);
        element.id=4;
        element.visible=false;
        this.Add(element);
        //remove tile button
        element=new GuiButton(_texture,left+width-20,top+10,10,20,Color.Black);
        element.id=7;
        element.visible=false;
        this.Add(element);

        //Tile -> full solid
        element=new GuiButton(_texture,left+30,top+50,20,20,Color.Gray);
        element.id=10;
        this.Add(element);
        //Tile -> slantBR
        element=new GuiButton(_texture,left+50,top+70,20,20,Color.Gray);
        element.id=11;
        this.Add(element);
        //Tile -> slantBL
        element=new GuiButton(_texture,left+10,top+70,20,20,Color.Gray);
        element.id=12;
        this.Add(element);
        //Tile -> slantTR
        element=new GuiButton(_texture,left+50,top+30,20,20,Color.Gray);
        element.id=13;
        this.Add(element);
        //Tile -> slantTL
        element=new GuiButton(_texture,left+10,top+30,20,20,Color.Gray);
        element.id=14;
        this.Add(element);
        //Tile.opaque toggle
        element=new GuiButton(_texture,left+30,top+30,20,20,Color.Gray);
        element.id=15;
        this.Add(element);
        //Tile solid toggle
        element=new GuiButton(_texture,left+30,top+70,20,20,Color.Gray);
        element.id=16;
        this.Add(element);
        //Tile platform toggle
        element=new GuiButton(_texture,left+50,top+50,20,20,Color.Gray);
        element.id=17;
        this.Add(element);
        
        
        
    }
    public override void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
        this.children[0].Resize(left+10,top+10,width-20,20);
        this.children[1].Resize(left+width-20,top+10,10,20);
        this.children[2].Resize(left+width-20,top+10,10,20);
        this.children[3].Resize(left+30,top+50,20,20);
        this.children[4].Resize(left+50,top+70,20,20);
        this.children[5].Resize(left+10,top+70,20,20);
        this.children[6].Resize(left+50,top+30,20,20);
        this.children[7].Resize(left+10,top+30,20,20);
        this.children[8].Resize(left+30,top+30,20,20);
        this.children[9].Resize(left+30,top+70,20,20);
        this.children[10].Resize(left+50,top+50,20,20);
        
    }
    private int CurrentTile=-1;
    
    public override void OnUpdate(int id,GuiElement obj)
    {
        
        if (id == 1)
        {
            int rid=IDMaps.GetTileFromName(((GuiSingleLineTextBox)obj).Content);
            if ((rid<TileID.count)&&(rid>=0))
            {
                CurrentTile=rid;
                this.children[1].visible=false;
                this.children[2].visible=true;
                obj.width=width-30;
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
                ((GuiButton)this.children[8]).color=TileInfo.IsOpaque(CurrentTile)?Color.Lime:Color.Red;
                ((GuiButton)this.children[9]).color=TileInfo.IsSolid(CurrentTile)?Color.Lime:Color.Red;
                ((GuiButton)this.children[10]).color=TileInfo.IsPlatform(CurrentTile)?Color.Lime:Color.Red;
            }
            else
            {
                CurrentTile=-1;
                if (((GuiSingleLineTextBox)obj).Content.Length > 0)
                {
                    this.children[1].visible=true;
                    this.children[2].visible=false;
                    obj.width=width-30;
                }
                else
                {
                    this.children[1].visible=false;
                    this.children[2].visible=false;
                    obj.width=width-20;
                }
                ((GuiButton)this.children[3]).color=Color.Gray;
                ((GuiButton)this.children[4]).color=Color.Gray;
                ((GuiButton)this.children[5]).color=Color.Gray;
                ((GuiButton)this.children[6]).color=Color.Gray;
                ((GuiButton)this.children[7]).color=Color.Gray;
                ((GuiButton)this.children[8]).color=Color.Gray;
                ((GuiButton)this.children[9]).color=Color.Gray;
                ((GuiButton)this.children[10]).color=Color.Gray;
            }
        }
        else if (id == 4)
        {
            RoomEditLogic.AddTileToList(((GuiSingleLineTextBox)this.children[0]).Content);
            CurrentTile=TileID.count-1;
            this.OnUpdate(1,children[0]);
        }else if (id == 7)
        {
            RoomEditLogic.RemoveTileFromList(CurrentTile);
            foreach(Room r in RoomID.Rooms){
                for(int i = 0; i < r.Width * r.Height; i++)
                {
                    if (r.tiles[i]==CurrentTile)
                    {
                        r.tiles[i]=TileID.None;
                    }else if (r.tiles[i]>CurrentTile)
                    {
                        r.tiles[i]-=1;
                    }
                }
            }
            this.OnUpdate(1,children[0]);
            
            
        }else if (id == 10)
        {
            if(CurrentTile!=-1){

                TileID.Sets.FullSolid[CurrentTile]=!TileID.Sets.FullSolid[CurrentTile];
                TileID.Sets.SlantBL[CurrentTile]=false;
                TileID.Sets.SlantBR[CurrentTile]=false;
                TileID.Sets.SlantTL[CurrentTile]=false;
                TileID.Sets.SlantTR[CurrentTile]=false;
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
            }
        }else if (id == 12)
        {
            if(CurrentTile!=-1){

                TileID.Sets.FullSolid[CurrentTile]=false;
                TileID.Sets.SlantBL[CurrentTile]=!TileID.Sets.SlantBL[CurrentTile];
                TileID.Sets.SlantBR[CurrentTile]=false;
                TileID.Sets.SlantTL[CurrentTile]=false;
                TileID.Sets.SlantTR[CurrentTile]=false;
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
            }
        }else if (id == 11)
        {
            if(CurrentTile!=-1){

                TileID.Sets.FullSolid[CurrentTile]=false;
                TileID.Sets.SlantBL[CurrentTile]=false;
                TileID.Sets.SlantBR[CurrentTile]=!TileID.Sets.SlantBR[CurrentTile];
                TileID.Sets.SlantTL[CurrentTile]=false;
                TileID.Sets.SlantTR[CurrentTile]=false;
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
            }
        }else if (id == 14)
        {
            if(CurrentTile!=-1){

                TileID.Sets.FullSolid[CurrentTile]=false;
                TileID.Sets.SlantBL[CurrentTile]=false;
                TileID.Sets.SlantBR[CurrentTile]=false;
                TileID.Sets.SlantTL[CurrentTile]=!TileID.Sets.SlantTL[CurrentTile];
                TileID.Sets.SlantTR[CurrentTile]=false;
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
            }
        }else if (id == 13)
        {
            if(CurrentTile!=-1){

                TileID.Sets.FullSolid[CurrentTile]=false;
                TileID.Sets.SlantBL[CurrentTile]=false;
                TileID.Sets.SlantBR[CurrentTile]=false;
                TileID.Sets.SlantTL[CurrentTile]=false;
                TileID.Sets.SlantTR[CurrentTile]=!TileID.Sets.SlantTR[CurrentTile];
                ((GuiButton)this.children[3]).color=TileInfo.IsFullSolid(CurrentTile)?Color.Lime:Color.Green;
                ((GuiButton)this.children[4]).color=TileInfo.IsSolidSlant(CurrentTile,0)?Color.Lime:Color.Green;
                ((GuiButton)this.children[5]).color=TileInfo.IsSolidSlant(CurrentTile,1)?Color.Lime:Color.Green;
                ((GuiButton)this.children[6]).color=TileInfo.IsSolidSlant(CurrentTile,2)?Color.Lime:Color.Green;
                ((GuiButton)this.children[7]).color=TileInfo.IsSolidSlant(CurrentTile,3)?Color.Lime:Color.Green;
            }
        }else if (id == 15)
        {
            if(CurrentTile!=-1){
                TileID.Sets.Opaque[CurrentTile]=!TileID.Sets.Opaque[CurrentTile];
                ((GuiButton)this.children[8]).color=TileInfo.IsOpaque(CurrentTile)?Color.Lime:Color.Red;
            }
        }else if (id == 16)
        {
            if(CurrentTile!=-1){
                TileID.Sets.Solid[CurrentTile]=!TileID.Sets.Solid[CurrentTile];
                ((GuiButton)this.children[9]).color=TileInfo.IsSolid(CurrentTile)?Color.Lime:Color.Red;
            }
        }else if (id == 17)
        {
            if(CurrentTile!=-1){
                TileID.Sets.Platform[CurrentTile]=!TileID.Sets.Platform[CurrentTile];
                ((GuiButton)this.children[10]).color=TileInfo.IsPlatform(CurrentTile)?Color.Lime:Color.Red;
            }
        }
    }
    
    public override void DrawExtras(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        if (CurrentTile != -1)
        {
            _spriteBatch.Draw(TileInfo.GetTexture(CurrentTile),new Rectangle(left+10,top+50,20,20),Color.White);
        }
        
    }
}
