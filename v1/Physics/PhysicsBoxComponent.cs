using System;
using System.Collections.Generic;
using System.Windows;
using agame.Rooms;
using agame.Rooms.Tile;
using Microsoft.Xna.Framework;

namespace agame.Physics
{
    public class PhysicsBoxComponent
    {
        public PhysicsBoxComponent(Rectangle rect,Vector2 pos, Vector2 vel)
        {
            this.rect=rect;
            this.position=pos;
            this.vel=vel;
        }
        /// <summary>
        /// The bounding box, where 0,0 is the center.
        /// </summary>
        public Rectangle rect;
        /// <summary>
        /// Center of the bounding box
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// current motion, units/second
        /// </summary>
        public Vector2 vel;
        public Vector2 acceleration=new Vector2(0,120);
        public bool onGround{get;protected set;}=false;
        public void HandleXCollision(List<RoomInstance> rooms,FloatRectangle last)
        {
            FloatRectangle r= new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            if(vel.X>0){
                foreach(RoomInstance inst in rooms)
                {
                    if(inst.IsColliding(r,last)){
                    FloatRectangle newrect= inst.GetCollisionObject(r,last,1);
                    
                        position.X=newrect.X-rect.X;
                        if (acceleration.X > 0)
                        {
                            onGround=true;
                        }
                        Console.WriteLine("Collide x");
                        vel.X=0;
                    
                    }
                }
            }else if(vel.X<0){
                foreach(RoomInstance inst in rooms)
                {
                    if(inst.IsColliding(r,last)){
                    FloatRectangle newrect= inst.GetCollisionObject(r,last,3);
                    
                        position.X=newrect.X-rect.X;
                        if (acceleration.X < 0)
                        {
                            onGround=true;
                        }
                        Console.WriteLine("Collide x");
                        vel.X=0;
                    
                    }
                }
            }
        }
        public void HandleYCollision(List<RoomInstance> rooms,FloatRectangle last)
        {
            FloatRectangle r= new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            if(vel.Y>0){
                foreach(RoomInstance inst in rooms)
                {
                    if(inst.IsColliding(r,last)){
                    FloatRectangle newrect= inst.GetCollisionObject(r,last,2);
                    
                    
                        position.Y=newrect.Y-rect.Y;
                        if (acceleration.Y > 0)
                        {
                            onGround=true;
                        }
                        vel.Y=0;
                    
                    }
                }
            }else if(vel.Y<0){
                foreach(RoomInstance inst in rooms)
                {
                    if(inst.IsColliding(r,last)){
                    FloatRectangle newrect= inst.GetCollisionObject(r,last,0);
                    
                        position.Y=newrect.Y-rect.Y;
                        if (acceleration.Y < 0)
                        {
                            onGround=true;
                        }
                        vel.Y=0;
                    
                    }
                }
            }
        }
        public virtual void Move(List<RoomInstance> rooms)
        {
            FloatRectangle last=new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            //move X
            if (onGround)
            {
                this.vel/=1.2f;
            }
            if (Math.Abs(this.vel.X) < 0.1)
            {
                this.vel.X=0;
            }
            onGround=false;
            
            vel+=acceleration/60;
            this.position.X+=vel.X/60;
            HandleXCollision(rooms,last);
            //move Y
            this.position.Y+=vel.Y/60;
            HandleYCollision(rooms,last);
            
        }
    }
    public class PhysicsBoxComponent_Stepped:PhysicsBoxComponent
    {
        public PhysicsBoxComponent_Stepped(Rectangle rect,Vector2 pos, Vector2 vel):base(rect,pos,vel)
        {
        }
        public int stepHeight=5;
        public override void Move(List<RoomInstance> rooms){
            FloatRectangle last=new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            //move X
            bool wasOnGround=onGround;
            if (onGround)
            {
                this.vel/=1.05f;
            }
            if (Math.Abs(this.vel.X) < 0.1)
            {
                this.vel.X=0;
            }
            onGround=false;
            
            vel+=acceleration/60;
            this.position.X+=vel.X/60;
            FloatRectangle r= new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            //Console.WriteLine("---------------------");
            if(wasOnGround){
                if (vel.X != 0)
                {
                    FloatRectangle steppedRectangle= new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
                    foreach(RoomInstance inst in rooms)
                    {
                        FloatRectangle newrect= inst.GetCollisionObject(r,last,2);
                        if (newrect.Y >= (r.Y-stepHeight))
                        {
                            steppedRectangle.Y=Math.Min(steppedRectangle.Y,newrect.Y);
                            //Console.WriteLine("Step height - not too high");
                        }
                    }
                    if(steppedRectangle.Y!=r.Y){
                        bool isSteppedColliding=false;
                        steppedRectangle.Height-=0.5f;
                        foreach(RoomInstance inst in rooms)
                        {
                            if (inst.IsColliding(steppedRectangle,last))
                            {
                                isSteppedColliding=true;
                            }
                        }
                        if (isSteppedColliding)
                        {
                            //Console.WriteLine("Step height - invalid");
                            HandleXCollision(rooms,last);
                        }
                        else
                        {
                            //Console.WriteLine("Step height - good");
                            position.Y=steppedRectangle.Y-rect.Y;
                            onGround=true;
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Step height - no change");
                        HandleXCollision(rooms,last);
                    }
                }
            }
            else
            {
                //Console.WriteLine("Was not on ground");
                HandleXCollision(rooms,last);
            }
            last=new FloatRectangle(rect.X+position.X,rect.Y+position.Y,rect.Width,rect.Height);
            //move Y
            this.position.Y+=vel.Y/60;
            HandleYCollision(rooms,last);
        
        }
    }
}