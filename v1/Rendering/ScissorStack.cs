using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace agame.Rendering;

public class ScissorStack
{
    public ScissorStack(GraphicsDevice device)
    {
        this.stack=new List<Rectangle>();
        this.stackCount=0;
        ScissorStack.device=device;
    }
    public bool IsEmpty
    {
        get
        {
            return this.stackCount==0;
        }
    }
    public static GraphicsDevice device;
    
    private List<Rectangle> stack;
    private int stackCount=0;
    public SpriteBatch batch;

    public void Push(int left, int top, int width, int height)
    {
        batch.End();
        batch.Begin(rasterizerState:Game1._rasterizerState,blendState:BlendState.AlphaBlend);
        stack.Add(Rectangle.Intersect(new Rectangle(left,top,width,height),device.ScissorRectangle));
        device.ScissorRectangle=stack[stackCount];
        stackCount+=1;
        
    }
    public void Pop(int count)
    {
        batch.End();
        batch.Begin(rasterizerState:Game1._rasterizerState,blendState:BlendState.AlphaBlend);
        for(int i=0;i<count;i++){
        stackCount-=1;
        stack.RemoveAt(stackCount);}
        if(stackCount>0){
            device.ScissorRectangle=stack[stackCount-1];
        }
        else
        {
            device.ScissorRectangle=device.Viewport.Bounds;
        }
        
    }
}
