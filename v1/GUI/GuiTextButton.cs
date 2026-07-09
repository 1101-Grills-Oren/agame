using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
namespace agame.GUI;

public class GuiTextButton:GuiButton
{
    public string text;
    public Color textColor;
    public TextAlignment alignment=TextAlignment.Middle;
    public GuiTextButton(Texture2D _texture,string text,int left,int top, int width, int height,Color tint, Color textColor):base(_texture,left,top,width,height,tint)
    {
        this.text=text;
        this.color=tint;
        this.textColor=textColor;
    }

    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        DrawBox(gameTime,_spriteBatch,color);
        /*switch (this.alignment)
        {
            case TextAlignment.Left:
                _spriteBatch.DrawString(Game1._font)
                break;
        }*/
        
    }
}
