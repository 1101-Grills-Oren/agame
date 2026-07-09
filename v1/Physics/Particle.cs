
using System;
using System.Collections.Generic;
using agame.IDs;
using agame.Rooms;
using agame.Rooms.Tile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace agame.Physics
{
    public class Particle
    {
        public Particle(int id,Vector2 position)
        {
            bounds=new PhysicsBoxComponent(new Rectangle(0,0,ParticleID.Width[id],ParticleID.Height[id]),position,new Vector2(0));
            this.id=id;
            this.variant=Game1.instance.random.Next(0,ParticleID.NumVariants[id]);
            this.flags=0;
        }
        public bool dead=false;
        public int id;
        public int variant;
        public PhysicsBoxComponent bounds;
        public double rotation=0;
        public double scale=1;
        public int flags=0;
        public Vector2 Position{get{return bounds.position;}set{bounds.position=value;}}
        public Vector2 Velocity{get{return bounds.vel;}set{bounds.vel=value;}}
        public virtual void Update(List<RoomInstance> rooms)
        {
            if(!dead){
            bounds.Move(rooms);
            switch (ParticleID.Behavior[id])
            {
                case 1:
                    this.scale-=0.003;
                    this.rotation+=0.003*this.flags;
                        if (this.scale <= 0)
                        {
                            this.dead=true;
                        }
                    break;
            }
            }
        }
        public virtual void Draw(SpriteBatch _batch)
        {
            if(!dead){
            Texture2D t=ParticleID.Textures[id];
            _batch.Draw(t, new Rectangle(
                (int)(Position.X-Game1.instance.ScreenPos.X+this.bounds.rect.X+this.bounds.rect.Width/2-t.Width*scale/2/ParticleID.NumVariants[id]),
                (int)(Position.Y-Game1.instance.ScreenPos.Y+this.bounds.rect.Y+this.bounds.rect.Height/2-t.Height*scale/2),
                (int)(t.Width*scale/ParticleID.NumVariants[id]),
                (int)(t.Height*scale)),
                new Rectangle(
                    t.Width/ParticleID.NumVariants[id]*variant,0,t.Width/ParticleID.NumVariants[id],t.Height), Color.White,(float)this.rotation,new Vector2(this.bounds.rect.X+this.bounds.rect.Width/2/ParticleID.NumVariants[id],
                    this.bounds.rect.Y+this.bounds.rect.Height/2),SpriteEffects.None,0);
                }
                
            
            
        }
    }
}