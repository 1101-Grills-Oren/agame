using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
using agame;
namespace agame.GUI;


public class GuiButton:GuiElement
{
    public Point? lastClickPos;
    public Color color;
    public bool triggerOnDrag=false;
    public int clickId;
    public GuiButton(Texture2D _texture,int left,int top, int width, int height,Color tint):base(_texture,left,top,width,height)
    {
        this.color=tint;
    }

    public override void OnEnter()
    {
        if (this.focused)
        {
            //Console.WriteLine("click");
            ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
            clickId=0;
        }
    }

    public override bool Click(int screenx, int screeny,int buttonid)
    {
        lastClickPos=new Point(screenx-left,screeny-top);
        //Console.WriteLine("click");
        clickId=buttonid;
        ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        
        return true;
    }
    public override bool MouseDown(int screenx, int screeny, int buttonid)
    {
        clickId=buttonid;
        return base.MouseDown(screenx, screeny, buttonid);
    }

    public override bool Drag(int screenx, int screeny,int dx, int dy)
    {
        if(triggerOnDrag){
            lastClickPos=new Point(screenx-left,screeny-top);
            //Console.WriteLine("click");

            ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        }
        return true;
    }
    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        DrawBox(gameTime,_spriteBatch,color);
    }
}
