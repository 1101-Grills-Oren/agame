using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
namespace agame.GUI;

public class GuiSlider:GuiElement
{
    public Color color;
    public float min;
    public float max;
    public float value{get{return Math.Min(max,Math.Max(min,internalvalue));}}
    private float internalvalue;
    public int sliderheight;
    public GuiSlider(Texture2D _texture,int left,int top, int width, int height,Color tint,float min, float max):base(_texture,left,top,width,height)
    {
        this.internalvalue=min;
        this.min=min;
        this.max=max;
        this.color=tint;
        this.sliderheight=this.height-4;
    }
    
    public override bool MouseDown(int screenx, int screeny, int buttonid)
    {
        internalvalue=Math.Min(max,Math.Max(min,internalvalue));
        return base.MouseDown(screenx,screeny,buttonid);
    }
    
    public override void OnRightArrowKey()
    {
        if (this.focused)
        {
            internalvalue=Math.Min(max,Math.Max(min,internalvalue+5*(max-min)/(width-12)));
            ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        }
    }
    public override void OnLeftArrowKey()
    {
        if (this.focused)
        {
            internalvalue=Math.Min(max,Math.Max(min,internalvalue-5*(max-min)/(width-12)));
            ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        }
    }

    public override bool Drag(int screenx, int screeny,int changex,int changey)
    {
        internalvalue=internalvalue+changex*(max-min)/(width-12);
        ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        return true;
    }
    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        
        DrawBox(base._texture,gameTime,_spriteBatch,color,this.left,this.top+(height-sliderheight)/2,this.width,sliderheight);
        if(this.focused){
            DrawBox(_selectedtexture,gameTime,_spriteBatch,color,this.left+(int)Math.Round(this.value/(max-min)*(this.width-12))+2,this.top,8,this.height);
        }
        else
        {
            DrawBox(base._texture,gameTime,_spriteBatch,color,this.left+(int)Math.Round(this.value/(max-min)*(this.width-12))+2,this.top,8,this.height);
        }
    }
}
