using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
using agame;
namespace agame.GUI{
public enum TextAlignment
    {
        Left,
        Middle,
        Right
    }
public class GuiSingleLineTextBox:GuiElement
{
    public Color color;
    public Color textcolor;
    public TextInput input;
    public static Texture2D keyboardSelectTexture;
    public static Texture2D textbarTexture;
    public TimeSpan lastUpdate=TimeSpan.Zero;
    public TimeSpan lastFlash=TimeSpan.Zero;
    public TextAlignment alignment=TextAlignment.Left;
    public bool allowTypeOutOfBounds=false;
    private int tooBigRepeatCount=0;
    public string Content{get{return this.input.value;}set{this.input.value=value;}}
    public GuiSingleLineTextBox(Texture2D _texture,int left,int top, int width, int height,Color tint,Color textcolor):base(_texture,left,top,width,height)
    {
        
        this.textcolor=textcolor;
        this.color=tint;
        this.input=new TextInput("");
    }
    public override void OnKeyPress(Keys key, bool shift, bool alt, bool ctrl, bool capsLock)
    {
        if ((Game1.currentTime.TotalGameTime - lastFlash).TotalSeconds > 0.999)
            {
                if(tooBigRepeatCount<10){
                    tooBigRepeatCount=0;
                }
            }
        this.lastUpdate=Game1.currentTime.TotalGameTime;
        string lastvalue=this.input.value;
        int lastcursorPos=this.input.cursorPos;
        int lastselectedBetweenPos=this.input.selectedBetweenPos;
        this.input.TypeKey(key,shift,alt,ctrl,capsLock);
        if(allowTypeOutOfBounds==false){
            if (Game1._font.MeasureString(input.value).X / 2 > (this.width - 12))
            {
                input.cursorPos=lastcursorPos;
                input.selectedBetweenPos=lastselectedBetweenPos;
                this.input.value=lastvalue;
                this.lastFlash=Game1.currentTime.TotalGameTime;
                tooBigRepeatCount+=1;

            }
        }
            if (lastvalue != Content)
            {
                ((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
            }
    }
    public override void FocusParents()
    {
        base.FocusParents();
        this.lastUpdate=Game1.currentTime.TotalGameTime;
    }

    public override bool MouseDown(int screenx, int screeny, int buttonid)
    {
        if(buttonid==1){
            this.lastUpdate=Game1.currentTime.TotalGameTime;
            if(this.alignment==TextAlignment.Left){
            input.cursorPos=input.FindIndexClosestTo(Game1._font,(screenx-this.left-6)*2);
            }else if(this.alignment==TextAlignment.Right){
            input.cursorPos=input.FindIndexClosestTo(Game1._font,(int)(screenx-this.left-this.width+6+Game1._font.MeasureString(input.value).X/2)*2);
            }else{
            input.cursorPos=input.FindIndexClosestTo(Game1._font,(int)(screenx-this.left-this.width/2+Game1._font.MeasureString(input.value).X/4)*2);
            }
            input.selectedBetweenPos=input.cursorPos;
        }
        return base.MouseDown(screenx, screeny, buttonid);
    }

    public override bool Drag(int screenx, int screeny,int Dx,int Dy)
    {
        if(this.mouseDownOn){
            this.lastUpdate=Game1.currentTime.TotalGameTime;
            if(this.alignment==TextAlignment.Left){
            input.selectedBetweenPos=input.FindIndexClosestTo(Game1._font,(screenx-this.left-6)*2);
            }else if(this.alignment==TextAlignment.Right){
            input.selectedBetweenPos=input.FindIndexClosestTo(Game1._font,(int)(screenx-this.left-this.width+6+Game1._font.MeasureString(input.value).X/2)*2);
            }else{
            input.selectedBetweenPos=input.FindIndexClosestTo(Game1._font,(int)(screenx-this.left-this.width/2+Game1._font.MeasureString(input.value).X/4)*2);
            }
        }
        //((GuiFrame)this.getRootParent()).OnUpdate(this.id,this);
        return true;
    }
    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        input.cursorPos=Math.Min(input.cursorPos,input.value.Length);
        input.selectedBetweenPos=Math.Min(input.selectedBetweenPos,input.value.Length);
        if(tooBigRepeatCount<10){
            DrawBox(gameTime,_spriteBatch,color);
            }
            else
            {
                DrawBox(gameTime,_spriteBatch,color*Color.Red);
            }
        int offset;
            if (this.alignment == TextAlignment.Left)
            {
                offset=6;
            }else if (this.alignment == TextAlignment.Right)
            {
                offset=(int)(this.width-6-font.MeasureString(this.input.value).X/2);
            }else
            {
                offset=(int)(this.width/2-font.MeasureString(this.input.value).X/4);
            }
            if (tooBigRepeatCount >= 10)
            {
                if(input.value.Length>0){
                    input.value=input.value.Substring(0,input.value.Length-1);
                    input.cursorPos=Math.Min(input.cursorPos,input.value.Length);
                    input.selectedBetweenPos=Math.Min(input.selectedBetweenPos,input.value.Length);
                }
                else
                {
                    tooBigRepeatCount=0;
                }
            }
        _spriteBatch.DrawString(
                font, 
                input.value,
                new Vector2(this.left+offset, this.top+this.height/2-font.LineSpacing/4), 
                textcolor,
                0,
                Vector2.Zero,
                Vector2.One/2,
                SpriteEffects.None,
                0
            );

        _spriteBatch.Draw(keyboardSelectTexture, new Rectangle(left+(int)font.MeasureString(input.value.Substring(0,Math.Min(input.cursorPos,input.selectedBetweenPos))).X/2+offset, top+5, (int)Math.Ceiling(font.MeasureString(input.value.Substring(Math.Min(input.cursorPos,input.selectedBetweenPos),Math.Max(input.cursorPos,input.selectedBetweenPos)-Math.Min(input.cursorPos,input.selectedBetweenPos))).X/2), height-10), Color.White);
        if(focused){
                if ((gameTime.TotalGameTime - lastFlash).TotalSeconds < 0.999)
                {
                    _spriteBatch.Draw(textbarTexture, new Rectangle(left+(int)font.MeasureString(input.value.Substring(0,input.cursorPos)).X/2-1+offset, top+4, 2, height-8), Color. Red *(float)Math.Min(1,Math.Sin(2*Math.PI*(gameTime.TotalGameTime - this.lastUpdate).TotalSeconds+0.001)+0.5));
                }
                else
                {
                    _spriteBatch.Draw(textbarTexture, new Rectangle(left+(int)font.MeasureString(input.value.Substring(0,input.cursorPos)).X/2-1+offset, top+4, 2, height-8), Color.White*(float)Math.Min(1,Math.Sin(2*Math.PI*(gameTime.TotalGameTime - this.lastUpdate).TotalSeconds+0.001)+0.5));
                }
            /*if (Math.Sin(2*Math.PI*(gameTime.TotalGameTime - this.lastUpdate).TotalSeconds+0.1)>0)
            {
                _spriteBatch.Draw(textbarTexture, new Rectangle(left+(int)font.MeasureString(input.value.Substring(0,input.cursorPos)).X/2-1+offset, top+4, 2, height-8), Color.White);
            }*/
            
        }
        
        
    }
}
}