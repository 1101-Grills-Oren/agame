using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame.Rendering;
namespace agame.GUI;

public class GuiImageButton:GuiButton
{
    public Texture2D image;
    public GuiImageButton(Texture2D _texture,Texture2D image,int left,int top, int width, int height,Color tint):base(_texture,left,top,width,height,tint)
    {
        this.image=image;
        this.color=tint;
    }

    public override void Draw(GameTime gameTime,SpriteBatch _spriteBatch,SpriteFont font,ScissorStack stack)
    {
        DrawBox(gameTime,_spriteBatch,color);
        _spriteBatch.Draw(image, new Rectangle(left+width/2-image.Width/2, top+height/2-image.Height/2, image.Width, image.Height), Color.White);
    }
}
