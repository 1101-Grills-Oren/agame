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

public class ParticleEditGui:GuiFrame
{
    public ParticleEditGui(Texture2D _texture,int left,int top, int width, int height):base(_texture,left,top,width,height,true,Color.AliceBlue,"")
    {
        //Particle select
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+10,width-20,20,Color.AliceBlue,Color.White){id=1});
        //add new
        this.Add(new GuiButton(_texture,left+width-20,top+10,10,20,Color.Green){id=2,visible=false});
        //remove
        this.Add(new GuiButton(_texture,left+width-20,top+10,10,20,Color.Red){id=3,visible=false});
        //width
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+50,(width-20)/2,20,Color.AliceBlue,Color.White){id=4,visible=false});
        //height
        this.Add(new GuiSingleLineTextBox(_texture,left+10+(width-20)/2,top+50,(width-20)/2,20,Color.AliceBlue,Color.White){id=5,visible=false});
        //behavior
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+70,width-20,20,Color.AliceBlue,Color.White){id=6,visible=false});
        //numvariants
        this.Add(new GuiSingleLineTextBox(_texture,left+10,top+90,width-20,20,Color.AliceBlue,Color.White){id=7,visible=false});
        
        
    }
    public override void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
        this.children[0].Resize(left+10,top+10,width-((((GuiSingleLineTextBox)this.children[0]).Content.Length==0)?20:30),20);
        this.children[1].Resize(left+width-20,top+10,10,20);
        this.children[2].Resize(left+width-20,top+10,10,20);
        this.children[3].Resize(left+10,top+50,(width-20)/2,20);
        this.children[4].Resize(left+10+(width-20)/2,top+50,(width-20)/2,20);
        this.children[5].Resize(left+10,top+70,width-20,20);
        this.children[6].Resize(left+10,top+90,width-20,20);
        
    }
    private int CurrentParticle=-1;
    public override void OnUpdate(int id,GuiElement obj)
    {
        
        switch(id){
            case 1:
                int pid=IDMaps.GetParticleFromName(((GuiSingleLineTextBox)obj).Content);
                CurrentParticle=pid;
                if (((GuiSingleLineTextBox)obj).Content.Length == 0)
                {
                    this.children[1].visible=false;
                    this.children[2].visible=false;
                    this.children[3].visible=false;
                    this.children[4].visible=false;
                    this.children[5].visible=false;
                    this.children[6].visible=false;
                    this.children[0].Resize(left+10,top+10,width-(20),20);
                }else if ((pid>=0))
                {
                    CurrentParticle=pid;
                    this.children[1].visible=false;
                    this.children[2].visible=true;
                    this.children[3].visible=true;
                    this.children[4].visible=true;
                    this.children[5].visible=true;
                    this.children[6].visible=true;
                    ((GuiSingleLineTextBox)this.children[3]).Content=ParticleID.Width[CurrentParticle].ToString();
                    ((GuiSingleLineTextBox)this.children[4]).Content=ParticleID.Height[CurrentParticle].ToString();
                    ((GuiSingleLineTextBox)this.children[5]).Content=ParticleID.Behavior[CurrentParticle].ToString();
                    ((GuiSingleLineTextBox)this.children[6]).Content=ParticleID.NumVariants[CurrentParticle].ToString();
                    this.children[0].Resize(left+10,top+10,width-(30),20);
                }
                else
                {
                    CurrentParticle=pid;
                    this.children[1].visible=true;
                    this.children[2].visible=false;
                    this.children[3].visible=false;
                    this.children[4].visible=false;
                    this.children[5].visible=false;
                    this.children[6].visible=false;
                    this.children[0].Resize(left+10,top+10,width-(30),20);
                }
                
                break;
            case 2:
                RoomEditLogic.AddParticleToList(((GuiSingleLineTextBox)this.children[0]).Content);
                this.OnUpdate(1,this.children[0]);
                break;
            case 3:
                RoomEditLogic.RemoveParticleFromList(CurrentParticle);
                this.OnUpdate(1,this.children[0]);
                break;
                
            case 4:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    if(i>0){
                    ParticleID.Width[CurrentParticle]=i;
                    }
                    else
                    {
                        ((GuiSingleLineTextBox)obj).Content="0";
                    }
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=(ParticleID.Width[CurrentParticle]).ToString();
                }
                break;
            case 5:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    if(i>0){
                    ParticleID.Height[CurrentParticle]=i;
                    }
                    else
                    {
                        ((GuiSingleLineTextBox)obj).Content="0";
                    }
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=(ParticleID.Height[CurrentParticle]).ToString();
                }
                break;
            case 6:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    if(i>0){
                    ParticleID.Behavior[CurrentParticle]=i;
                    }
                    else
                    {
                        ((GuiSingleLineTextBox)obj).Content="0";
                    }
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=(ParticleID.Behavior[CurrentParticle]).ToString();
                }
                break;
            case 7:
                try{
                    int i=StringToInt(((GuiSingleLineTextBox)obj).Content);
                    if(i>0){
                    ParticleID.NumVariants[CurrentParticle]=i;
                    }
                    else
                    {
                        ((GuiSingleLineTextBox)obj).Content="0";
                    }
                }
                catch
                {
                    ((GuiSingleLineTextBox)obj).Content=(ParticleID.NumVariants[CurrentParticle]).ToString();
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
        if (CurrentParticle != -1)
        {
            double time=Game1.currentTime.TotalGameTime.TotalSeconds;
            Texture2D t=ParticleID.Textures[CurrentParticle];
            _spriteBatch.Draw(t,new Rectangle(left+10,top+110,t.Width/ParticleID.NumVariants[CurrentParticle],t.Height),new Rectangle(t.Width/ParticleID.NumVariants[CurrentParticle]*((int)time%ParticleID.NumVariants[CurrentParticle]),0,t.Width/ParticleID.NumVariants[CurrentParticle],t.Height),Color.White);
        }
    }
}
