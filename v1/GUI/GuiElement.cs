
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using agame.Rendering;
using agame;
namespace agame.GUI;

public class GuiElement
{
    public Texture2D _texture;
    public static Texture2D _defaultselectedtexture;
    public Texture2D _selectedtexture;
    public int top;
    public int left;
    public int width;
    public int height;
    public bool focused=false;
    public bool hovered=false;
    public bool mouseDownOn=false;
    public int id;
    public int idb;
    public GuiFrame parent;
    public bool tabTraversible=true;
    public bool hasParent=false;
    public bool visible=true;
    public GuiElement(Texture2D _texture,int left,int top, int width, int height, int id=-1)
    {
        this._selectedtexture=_defaultselectedtexture;
        this._texture=_texture;
        this.top=top;
        this.left=left;
        this.width=width;
        this.height=height;
        this.id=id;
        this.idb=0;
    }
    public bool _Click(int screenx, int screeny,int buttonid)
    {
        
        if((screenx>=left)&&(screeny>=top)&&(screenx<=left+width)&&(screeny<=top+height)&&this.visible){
            return Click(screenx,screeny,buttonid);
        }
        else
        {
            return false;
        }
    }
    public bool _MouseDown(int screenx, int screeny,int buttonid)
    {
        if((screenx>=left)&&(screeny>=top)&&(screenx<=left+width)&&(screeny<=top+height)&&this.visible){
            return MouseDown(screenx,screeny,buttonid);
        }
        else
        {
            return false;
        }
    }
    public bool _MouseUp(int screenx, int screeny,int buttonid)
    {
        if(this.visible){
        return MouseUp(screenx,screeny,buttonid);
        }
        else return false;
    }
    public bool _Drag(int screenx, int screeny,int changex,int changey)
    {
        if(this.visible){
        if(mouseDownOn){
            return Drag(screenx,screeny,changex,changey);
        }
        else
        {
            return false;
        }
        }
        else return false;
    }
    public virtual bool _MouseMove(int screenx, int screeny,int changex,int changey)
    {
        if(this.visible){
        if(hovered){
            if((screenx<left)||(screeny<top)||(screenx>left+width)||(screeny>top+height)){
                return MouseAway(screenx,screeny);
            }
        }
        else
        {
            if((screenx>=left)&&(screeny>=top)&&(screenx<=left+width)&&(screeny<=top+height)){
                return MouseOver(screenx,screeny);
            }
        }
        return false;
        }
        else return false;
    }
    public virtual bool MouseDown(int screenx, int screeny, int buttonid)
    {
        focused=true;
        if(buttonid==1){
            mouseDownOn=true;
        }
        return true;
    }
    public virtual bool MouseUp(int screenx, int screeny, int buttonid)
    {
        if(buttonid==1){
        mouseDownOn=false;
        }
        return true;
    }
    public virtual bool Click(int screenx, int screeny,int buttonid)
    {
        focused=true;
        return true;
    }
    public virtual bool Drag(int screenx, int screeny,int changex,int changey)
    {
        return true;
    }
    public virtual bool MouseOver(int screenx, int screeny)
    {
        hovered=true;
        return true;
    }
    public virtual bool MouseAway(int screenx, int screeny)
    {
        hovered=false;
        return true;
    }
    public void DrawBox(GameTime gameTime,SpriteBatch _spriteBatch, Color tint)
    {
        if(this.focused){
            DrawBox(_selectedtexture,gameTime,_spriteBatch,tint,this.left,this.top,this.width,this.height);
        }
        else
        {
            DrawBox(_texture,gameTime,_spriteBatch,tint,this.left,this.top,this.width,this.height);
        }
    }
    public virtual void Unfocus()
    {
        this.mouseDownOn=false;
        this.focused=false;
    }
    public virtual void Unhover()
    {
        this.hovered=false;
    }
    public virtual void FocusParents()
    {
        if (this.hasParent)
        {
            this.parent.FocusParents();
            this.parent.focused=true;
        }
    }
    public virtual void OnRightArrowKey()
    {
        if (this.focused)
        {
        }
    }
    public virtual void OnEnter()
    {
        if (this.focused)
        {
        }
    }
    public virtual void OnLeftArrowKey()
    {
        if (this.focused)
        {
        }
    }
    public virtual void OnTab()
    {
        if (this.focused)
        {
            this.focused=false;
            GuiElement next=this.getNext();
            next.focused=true;
            next.FocusParents();
        }
    }
    public virtual void OnUnTab()
    {
        if (this.focused)
        {
            this.focused=false;
            GuiElement next=this.getPrevious();
            next.focused=true;
            next.FocusParents();
        }
    }
    public virtual void OnKeyPress(Keys key, bool shift, bool alt, bool ctrl, bool capsLock)
    {
        
    }
    public GuiElement getRootParent()
    {
        if (this.hasParent)
        {
            return this.parent.getRootParent();
        }
        else
        {
            return this;
        }
    }
    public GuiElement getNext()
    {
        if(this.hasParent){
            List<GuiElement> tabTraversibleValues=((GuiFrame)this.getRootParent()).Traversible;
            int ind=tabTraversibleValues.IndexOf(this);
            if (tabTraversibleValues.Count > (ind+1))
            {
                return tabTraversibleValues[ind+1];
            }
            else
            {
                return tabTraversibleValues[0];
            }
        }
        else
        {
            return this;
        }
    }
    public GuiElement getPrevious()
    {
        if(this.hasParent){
            List<GuiElement> tabTraversibleValues=((GuiFrame)this.getRootParent()).Traversible;
            int ind=tabTraversibleValues.IndexOf(this);
            if (ind>0)
            {
                return tabTraversibleValues[ind-1];
            }
            else
            {
                return tabTraversibleValues[tabTraversibleValues.Count-1];
            }
        }
        else
        {
            return this;
        }
    }
    public static void DrawBox(Texture2D _texture, GameTime gameTime,SpriteBatch _spriteBatch, Color tint, int left, int top, int width, int height)
    {
        int twidth=_texture.Width;
        int theight=_texture.Height;
        int tsw=twidth/3;
        int tsh=theight/3;
        if (width > tsw*2)
        {
            if (height > tsh*2)
            {
                _spriteBatch.Draw(_texture, new Rectangle(left, top, tsw, tsh), new Rectangle(0,0,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+tsw, top, width-tsw*2, tsh), new Rectangle(tsw,0,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width-tsw, top, tsw, tsh), new Rectangle(2*tsw,0,tsw,tsh), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+tsh, tsw, height-tsh*2), new Rectangle(0,tsh,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+tsw, top+tsh, width-tsw*2, height-tsh*2), new Rectangle(tsw,tsh,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width-tsw, top+tsh, tsw, height-tsh*2), new Rectangle(2*tsw,tsh,tsw,tsh), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+height-tsh, tsw, tsh), new Rectangle(0,2*tsh,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+tsw, top+height-tsh, width-tsw*2, tsh), new Rectangle(tsw,2*tsh,tsw,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width-tsw, top+height-tsh, tsw, tsh), new Rectangle(2*tsw,2*tsh,tsh,tsh), tint);
            }
            else
            {
                _spriteBatch.Draw(_texture, new Rectangle(left, top, tsw, height/2), new Rectangle(0,0,tsw,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+tsw, top, width-tsw*2, height/2), new Rectangle(tsw,0,tsw,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width-tsw, top, tsw, height/2), new Rectangle(2*tsw,0,tsw,height/2), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+height/2, tsw, height/2), new Rectangle(0,theight-height/2,tsw,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+tsw, top+height/2, width-2*tsw, height/2), new Rectangle(tsw,theight-height/2,tsw,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width-tsw, top+height/2, tsw, height/2), new Rectangle(2*tsw,theight-height/2,tsw,height/2), tint);
            }
        }
        else
        {
            if (height > tsh*2)
            {
                _spriteBatch.Draw(_texture, new Rectangle(left, top, width/2, tsh), new Rectangle(0,0,width/2,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width/2, top, width/2, tsh), new Rectangle(twidth-width/2,0,width/2,tsh), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+tsh, width/2, height-tsh*2), new Rectangle(0,tsh,width/2,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width/2, top+tsh, width/2, height-tsh*2), new Rectangle(twidth-width/2,tsh,width/2,tsh), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+height-tsh, width/2, tsh), new Rectangle(0,2*tsh,width/2,tsh), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width/2, top+height-tsh, width/2, tsh), new Rectangle(twidth-width/2,2*tsh,width/2,tsh), tint);
            }
            else
            {
                _spriteBatch.Draw(_texture, new Rectangle(left, top, width/2, height/2), new Rectangle(0,0,width/2,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width/2, top, width/2, height/2), new Rectangle(twidth-width/2,0,width/2,height/2), tint);

                _spriteBatch.Draw(_texture, new Rectangle(left, top+height/2, width/2, height/2), new Rectangle(0,theight-height/2,width/2,height/2), tint);
                _spriteBatch.Draw(_texture, new Rectangle(left+width/2, top+height/2, width/2, height/2), new Rectangle(twidth-width/2,theight-height/2,width/2,height/2), tint);
            }
        }
    }
    public virtual void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        if(this.visible){
            if(this.focused){
                _spriteBatch.Draw(_selectedtexture, new Rectangle(left, top, width, height), Color.White);
            }else{
                _spriteBatch.Draw(_texture, new Rectangle(left, top, width, height), Color.White);
            }
        }
    }
    public virtual void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
    }
}
