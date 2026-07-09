
using System;
using System.Collections.Generic;
using agame.Rooms;
using agame.Rooms.Tile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace agame.Physics
{
    public class Entity
    {
        public Entity(int width, int height,Vector2 position)
        {
            bounds=new PhysicsBoxComponent(new Rectangle(0,0,width,height),position,new Vector2(0));
        }
        public PhysicsBoxComponent bounds;
        public Vector2 Position{get{return bounds.position;}set{bounds.position=value;}}
        public Vector2 Velocity{get{return bounds.vel;}set{bounds.vel=value;}}
        public virtual void Update(List<RoomInstance> rooms)
        {
            bounds.Move(rooms);
        }
        public virtual void Draw(SpriteBatch _batch)
        {
            _batch.Draw(TileInfo.GetTexture(1), new Rectangle((int)Position.X-Game1.instance.ScreenPos.X+this.bounds.rect.X, (int)Position.Y-Game1.instance.ScreenPos.Y+this.bounds.rect.Y, this.bounds.rect.Width, this.bounds.rect.Height), Color.White);
        }
    }
    class Player:Entity
    {
        public Light light;
        public int TimeSinceWasOnGround=0;
        public int maxNumMidairJumps=1;
        public int numMidairJumps=1;
        public bool jumpButtonWasDown=false;
        public Player(int width, int height,Vector2 position):base(width,height,position)
        {
            bounds=new PhysicsBoxComponent_Stepped(new Rectangle(0,0,width,height),position,new Vector2(0));
            bounds.acceleration=new Vector2(0,240);
            this.light=new PointLight(){Radius=5,Scale=new Vector2(500),ShadowType=ShadowType.Solid};
        }
        public override void Update(List<RoomInstance> rooms)
        {

            bool a=Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A);
            bool d=Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D);
            if (a&(!d))
            {
                if ((bounds.vel.X > -1)&&(bounds.onGround))
                {
                    bounds.vel.X=-10;
                }
                bounds.vel.X-=bounds.onGround?8:1;
                if (bounds.onGround&&(bounds.vel.X <= -11))
                {
                    if(Game1.instance.random.Next(0,10)==0)
                    Game1.instance.particles.Add(new Particle(0,bounds.position+new Vector2(bounds.rect.Left+bounds.rect.Width*0.8f,bounds.rect.Bottom-1)){flags=Game1.instance.random.Next(-15,16)});
                }
            }
            if (d&(!a))
            {
                if ((bounds.vel.X < 1)&&(bounds.onGround))
                {
                    bounds.vel.X=10;
                }
                bounds.vel.X+=bounds.onGround?8:1;
                if (bounds.onGround&&(bounds.vel.X >= 11))
                {
                    if(Game1.instance.random.Next(0,10)==0)
                    Game1.instance.particles.Add(new Particle(0,bounds.position+new Vector2(bounds.rect.Left+bounds.rect.Width*0.2f,bounds.rect.Bottom-1)){flags=Game1.instance.random.Next(-15,16)});
                }
            }
            if (bounds.onGround)
            {
                TimeSinceWasOnGround=0;
            }
            else
            {
                TimeSinceWasOnGround++;
            }
            if (Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W)&&(TimeSinceWasOnGround<9))
            {
                bounds.vel.Y=(float)(-71*2*Math.Sqrt(2));
                TimeSinceWasOnGround=50;
                numMidairJumps=maxNumMidairJumps;
            }
            else if (Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W)&&(!jumpButtonWasDown)&&(numMidairJumps!=0))
            {
                bounds.vel.Y=(float)(-71*2*Math.Sqrt(2));
                numMidairJumps--;
            }
            jumpButtonWasDown=Game1.instance.lastKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W);
            bounds.Move(rooms);
            Game1.instance.ScreenPos=new Point((int)Math.Round(this.bounds.position.X-Game1.instance.Window.ClientBounds.Width/2),(int)Math.Round(this.bounds.position.Y-Game1.instance.Window.ClientBounds.Height/2))+this.bounds.rect.Center;
            Matrix m=Game1.instance.penumbra.Transform;
            m.Translation=new Vector3(-Game1.instance.ScreenPos.X,-Game1.instance.ScreenPos.Y,0);
            Game1.instance.penumbra.Transform=m;
            light.Position=new Vector2((int)Math.Round(this.bounds.position.X*0+Game1.instance.ScreenPos.X+Game1.instance.ScreenRect.Width/2),(int)Math.Round(this.bounds.position.Y*0+Game1.instance.ScreenPos.Y+Game1.instance.ScreenRect.Height/2));
        }
    }
}