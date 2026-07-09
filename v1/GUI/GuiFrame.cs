using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
using agame;
namespace agame.GUI;

public class GuiFrame:GuiElement
{
    public List<GuiElement> children;
    public bool hasBack;
    public Color color;
    public String aboveText;
    public GuiFrame(Texture2D _texture,int left,int top, int width, int height,bool hasBack,Color tint,String aboveText=""):base(_texture,left,top,width,height)
    {
        children=new List<GuiElement>();
        this.hasBack=hasBack;
        this.color=tint;
        this.aboveText=aboveText;
    }
    
    public List<GuiElement> Traversible
    {
        get
        {
            List<GuiElement> list=new List<GuiElement>();
            foreach(GuiElement child in children)
            {
                if(child.visible)
                if (child.tabTraversible)
                {
                    if(child is GuiFrame)
                    {
                        foreach(GuiElement childb in ((GuiFrame)child).Traversible)
                        {
                            list.Add(childb);
                        }
                    }
                    else
                    {
                        list.Add(child);
                    }
                }
            }
            return list;
        }
    }
    public override void OnTab()
    {
        if (this.focused)
        {
            this.focused=false;
            foreach(GuiElement e in this.children)
            {
                if(e.visible)
                if(e.focused){
                    e.OnTab();
                    break;}
            }
        }
    }
    public override void OnUnTab()
    {
        if (this.focused)
        {
            this.focused=false;
            foreach(GuiElement e in this.children)
            {
                if(e.visible)
                if(e.focused){
                    e.OnUnTab();
                    break;}
            }
        }

    }
    public override void OnRightArrowKey()
    {
        if (this.focused)
        {
            foreach(GuiElement e in this.children)
            {
                if(e.visible)
                if(e.focused){
                    e.OnRightArrowKey();
                    break;}
            }
        }
        
    }
    public override void OnLeftArrowKey()
    {
        if (this.focused)
        {
            foreach(GuiElement e in this.children)
            {
                if(e.visible)
                if(e.focused){
                    e.OnLeftArrowKey();
                    break;}
            }
        }
    }
    public override bool MouseDown(int screenx, int screeny, int buttonid)
    {
        if(buttonid==1){
            this.mouseDownOn=true;
        }
        foreach(GuiElement child in children){
            if(child.visible){
                if (child._MouseDown(screenx, screeny, buttonid))
                {
                    return true;
                }
            }
                
            }
            return this.hasBack;
    }
    public override bool MouseUp(int screenx, int screeny, int buttonid)
    {
        if (buttonid == 1)
        {
            this.mouseDownOn=false;
        }
        foreach(GuiElement child in children){
            if(child.visible){
                if (child._MouseUp(screenx, screeny, buttonid))
                {
                    return true;
                }
            }
                
            }
            return this.hasBack;
    }
    public override void OnEnter()
    {
        if (this.focused)
        {
            
            foreach(GuiElement child in children){
                if(child.visible){
                    if(child.focused){
                        child.OnEnter();
                        break;
                    }
                }
                
            }
        }
    }
    public override void OnKeyPress(Keys key, bool shift, bool alt, bool ctrl, bool capsLock)
    {
        if (this.focused)
        {
            
            foreach(GuiElement child in children){
                if(child.visible){
                    if(child.focused){
                        child.OnKeyPress(key,shift,alt,ctrl,capsLock);
                        break;
                    }
                }
                
            }
        }
    }
    public override bool Click(int screenx, int screeny,int buttonid)
    {
        this.focused=true;
        foreach(GuiElement child in children){
            if(child.visible){
                if (child._Click(screenx, screeny, buttonid))
                {
                    return true;
                }
            }
                
            }
            return hasBack;
    }
    public override bool _MouseMove(int screenx, int screeny,int changex,int changey)
    {
        bool wasHovered=hovered;
        base._MouseMove(screenx,screeny,changex,changey);
        if(hovered){
            foreach(GuiElement child in children){
                if(child.visible){
                    child._MouseMove(screenx,screeny,changex,changey);
                }
            }
        }
        else if(wasHovered)
        {
            foreach(GuiElement child in children){
                child.Unhover();
            }
        }
        return false;
    }
    public override void Unfocus()
    {
        if(this.focused){
            
            foreach(GuiElement child in children){
                child.Unfocus();
            }
            this.focused=false;
            this.mouseDownOn=false;
            
        }
    }
    public override void Unhover()
    {
        if(this.hovered){
            foreach(GuiElement child in children){
                child.Unhover();
            }
            this.hovered=false;
        }
    }
    public override bool Drag(int screenx, int screeny,int changex,int changey)
    {
        foreach(GuiElement child in children){
            if(child.visible){
                if (child._Drag(screenx, screeny, changex,changey))
                {
                    return true;
                }
            }
                
            }
            return false;
    }
    public void Add(GuiElement element)
    {
        this.children.Add(element);
        element.parent=this;
        element.hasParent=true;
    }
    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        
        _spriteBatch.DrawString(
                font, 
                this.aboveText, 
                new Vector2(this.left, this.top-font.LineSpacing/2), 
                Color.White,
                0,
                Vector2.Zero,
                Vector2.One/2,
                SpriteEffects.None,
                0
            );
        stack.Push(this.left,this.top,this.width,this.height);
        if(hasBack){
            DrawBox(_texture,gameTime,_spriteBatch,color,this.left,this.top,this.width,this.height);
        }
        foreach(GuiElement child in children){
            if(child.visible){
                child.Draw(gameTime,_spriteBatch,font,stack);
            }
        }
        stack.Pop(1);
        DrawExtras(gameTime,_spriteBatch,font,stack);
    }
    public virtual void DrawExtras(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack){}
    public virtual void OnUpdate(int id,GuiElement obj)
    {
        
    }
}
