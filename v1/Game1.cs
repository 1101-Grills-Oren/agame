using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using agame.Rendering;
using agame.GUI;
using agame.IDs;
using agame.Rooms.Tile;
using Penumbra;
using System.Collections.Generic;
using agame.Rooms;
using agame.Physics;
using System.Reflection;
using agame.RoomEditor;
using agame.Sound;
using Microsoft.Xna.Framework.Audio;
namespace agame;
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    public SpriteBatch _spriteBatch{get;private set;}
    private Texture2D _guiElementBoxSelectedTexture;
    private Texture2D _guiElementBoxTexture;
    private Texture2D _guiElementTabBoxTexture;
    private Texture2D _guiElementTabBoxSelectedTexture;
    public Texture2D _lightBarrierTexture;
    public static SpriteFont _font{get;private set;}
    public GuiElement[] elements=[];
    public MouseState lastMouseState=new MouseState(0, 0, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
    public KeyboardState lastKeyState;
    public ScissorStack ScissorStack{get;private set;}
    public static GameTime currentTime;
    public PenumbraComponent penumbra;
    public List<RoomInstance> rooms=new List<RoomInstance>();
    public static Game1 instance;
    public static RasterizerState _rasterizerState = new RasterizerState
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
            
        };
    const int GWL_STYLE = -16;
    const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
    const uint WS_SIZEBOX = 0x00040000;
    const uint WS_MAXIMIZEBOX = 0x00010000;
    const uint WS_MINIMIZEBOX = 0x00020000;

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    public RenderTarget2D sceneRenderTarget;
    public RenderTarget2D lightTarget;
    public RenderTarget2D finalLightTarget;
    public RenderTarget2D opaquesTarget;
    public Effect lightSpreadEffect;
    public Effect lightSpreadEffectb;
    public Effect lightSpreadEffectc;
    public Effect finalizeLightingEffect;
    public List<Particle> particles=new List<Particle>();
    public Point ScreenPos;
    public Entity e=new Player(18,36,new Vector2(400,400));
    public Random random=new Random(398476);
    public Rectangle ScreenRect
    {
        get
        {
            return new Rectangle(ScreenPos,Window.ClientBounds.Size);
            //return new Rectangle(lastMouseState.Position-new Point(50,50),new Point(100,100));
        }
    }
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        this.ScissorStack=new ScissorStack(_graphics.GraphicsDevice);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        lastKeyState = Keyboard.GetState();
        penumbra = new PenumbraComponent(this){
                AmbientColor = new Color(new Vector3(0.0f))
            };
        Components.Add(penumbra);
        instance=this;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
        this.Window.AllowUserResizing=true;
    }
    /*RenderTarget2D guiRenderTarget;
    RenderTarget2D sceneRenderTarget;
    Effect combineRenderTargetsPostEffect;*/
    private void OnClientSizeChanged(object sender, System.EventArgs e)
        {
            // This triggers on every size change
            int width = Window.ClientBounds.Width;
            int height = Window.ClientBounds.Height;

            Console.WriteLine($"Window resized to: {width}x{height}");
            sceneRenderTarget.Dispose();
            finalLightTarget.Dispose();
            lightTarget.Dispose();
            opaquesTarget.Dispose();
            sceneRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
            finalLightTarget = new RenderTarget2D(
                GraphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
            lightTarget = new RenderTarget2D(
                GraphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
            opaquesTarget = new RenderTarget2D(
                    GraphicsDevice,
                    width,
                    height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );
            sceneRenderTarget = new RenderTarget2D(
                    GraphicsDevice,
                    width,
                    height,
                    false,
                    SurfaceFormat.Color,
                    DepthFormat.None
                );
            finalizeLightingEffect.Parameters[1].SetValue(sceneRenderTarget);
            finalizeLightingEffect.Parameters[0].SetValue(finalLightTarget);
            lightSpreadEffectb.Parameters[3].SetValue(opaquesTarget);
            lightSpreadEffectb.Parameters[2].SetValue(lightTarget);
            lightSpreadEffectb.Parameters[0].SetValue((float)1/width);
            lightSpreadEffectb.Parameters[1].SetValue((float)1/height);
            
            lightSpreadEffectc.Parameters[3].SetValue(opaquesTarget);
            lightSpreadEffectc.Parameters[2].SetValue(finalLightTarget);
            lightSpreadEffectc.Parameters[0].SetValue((float)1/width);
            lightSpreadEffectc.Parameters[1].SetValue((float)1/height);
            Components.Remove(penumbra);
            penumbra = new PenumbraComponent(this){
                AmbientColor = new Color(new Vector3(0.0f))
            };
            Components.Add(penumbra);
            foreach(RoomInstance inst in this.rooms){
            inst.AddLights(penumbra);
            }
            penumbra.Lights.Add(((Player)this.e).light);
            elements[0].Resize(20,20,width-40,height-40);
        }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        /*sceneRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        guiRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );*/
        sceneRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        finalLightTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        lightTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        opaquesTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None
            );
        finalizeLightingEffect = Content.Load<Effect>("combine");
        finalizeLightingEffect.Parameters[1].SetValue(sceneRenderTarget);
        finalizeLightingEffect.Parameters[0].SetValue(finalLightTarget);
        
        lightSpreadEffectb = Content.Load<Effect>("lightDistortGen2");
        lightSpreadEffectc = Content.Load<Effect>("lightDistortGen2");
        lightSpreadEffectb.Parameters[3].SetValue(opaquesTarget);
        lightSpreadEffectc.Parameters[3].SetValue(opaquesTarget);
        lightSpreadEffectb.Parameters[0].SetValue((float)1/Window.ClientBounds.Width);
        lightSpreadEffectb.Parameters[1].SetValue((float)1/Window.ClientBounds.Height);
        lightSpreadEffectc.Parameters[0].SetValue((float)1/Window.ClientBounds.Width);
        lightSpreadEffectc.Parameters[1].SetValue((float)1/Window.ClientBounds.Height);

        // TODO: use this.Content to load your game content here
        _guiElementBoxSelectedTexture = Content.Load<Texture2D>("guiElementBoxSelected"); 
        GuiElement._defaultselectedtexture=_guiElementBoxSelectedTexture;
        _guiElementBoxTexture = Content.Load<Texture2D>("guiElementBox");
        _lightBarrierTexture = Content.Load<Texture2D>("lightBarrier");
        _guiElementTabBoxTexture = Content.Load<Texture2D>("guiElementTabBox");
        _guiElementTabBoxSelectedTexture = Content.Load<Texture2D>("guiElementTabBoxSelected");
        elements=[new GameEditorGui(_guiElementBoxTexture,_guiElementTabBoxTexture,_guiElementTabBoxSelectedTexture,20,20,400,400)];
        
        _font = Content.Load<SpriteFont>("font");
        GuiSingleLineTextBox.keyboardSelectTexture = new Texture2D(GraphicsDevice, 1, 1);
        GuiSingleLineTextBox.keyboardSelectTexture.SetData(new[] { new Color(30,30,150,150) });
        GuiSingleLineTextBox.textbarTexture = new Texture2D(GraphicsDevice, 1, 1);
        GuiSingleLineTextBox.textbarTexture.SetData(new[] { Color.White });
        TileInfo.textures[0] = new Texture2D(GraphicsDevice, 1, 1);
        TileInfo.textures[0].SetData(new[] { Color.Transparent });
        /*for(int i = 1; i < TileID.count; i++)
        {
            Console.WriteLine(i);
            TileInfo.textures[i] = Content.Load<Texture2D>("tile_"+i);
            if(TileInfo.IsGlowing(i)){
                Console.WriteLine(i);
                TileInfo.lightTextures[i] = Content.Load<Texture2D>("tile_"+i+"_glow");
            }
        }*/
        for(int i = 0; i < ParticleID.count; i++)
        {
            ParticleID.Textures[i] = RoomEditLogic.LoadTexture("particle_"+i);
        }
        RoomEditLogic.TryLoadData("Map");
        RoomInstance inst=new RoomInstance(0, new Vector2(300,300));
        this.rooms.Add(inst);
        inst.AddLights(penumbra);
        penumbra.Lights.Add(((Player)this.e).light);
        SoundID.Load("0.ogg",0);
    }

    int soundCount=0;
    protected override void Update(GameTime gameTime)
    {
        if(IsActive){
        currentTime=gameTime;
        MouseState mouseState = Mouse.GetState();
        KeyboardState keyState = Keyboard.GetState();
        // Update sprite position to follow the mouse cursor
        
        
        int mouseX = mouseState.X;
        int mouseY = mouseState.Y;
        int mouseDX = mouseState.X-lastMouseState.X;
        int mouseDY = mouseState.Y-lastMouseState.Y;
        bool button1Down=(mouseState.LeftButton == ButtonState.Pressed);
        bool button2Down=(mouseState.RightButton == ButtonState.Pressed);
        bool button3Down=(mouseState.MiddleButton == ButtonState.Pressed);
        bool button1WasDown=(lastMouseState.LeftButton == ButtonState.Pressed);
        bool button2WasDown=(lastMouseState.RightButton == ButtonState.Pressed);
        bool button3WasDown=(lastMouseState.MiddleButton == ButtonState.Pressed);
        bool tabDown= keyState.IsKeyDown(Keys.Tab);
        bool tabWasDown= lastKeyState.IsKeyDown(Keys.Tab);
        bool shiftDown= keyState.IsKeyDown(Keys.LeftShift)||keyState.IsKeyDown(Keys.RightShift);
        bool ctrlDown= keyState.IsKeyDown(Keys.LeftControl)||keyState.IsKeyDown(Keys.RightControl);
        bool altDown= keyState.IsKeyDown(Keys.LeftAlt)||keyState.IsKeyDown(Keys.RightAlt);
        bool rightarrowkeypressed=((!keyState.IsKeyDown(Keys.Right))&&lastKeyState.IsKeyDown(Keys.Right));
        bool leftarrowkeypressed=((!keyState.IsKeyDown(Keys.Left))&&lastKeyState.IsKeyDown(Keys.Left));
        bool uparrowkeypressed=((!keyState.IsKeyDown(Keys.Up))&&lastKeyState.IsKeyDown(Keys.Up));
        bool downarrowkeypressed=((!keyState.IsKeyDown(Keys.Down))&&lastKeyState.IsKeyDown(Keys.Down));
        bool enterkeypressed=((!keyState.IsKeyDown(Keys.Enter))&&lastKeyState.IsKeyDown(Keys.Enter));
        if(!button1WasDown&&button1Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._MouseDown(mouseX,mouseY,1);
                }
            }
        }
        if(!button2WasDown&&button2Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._MouseDown(mouseX,mouseY,2);
                }
            }
        }
        if(!button3WasDown&&button3Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._MouseDown(mouseX,mouseY,3);
                }
            }
        }
        if(button1WasDown&&!button1Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._Click(mouseX,mouseY,1);
                elements[i]._MouseUp(mouseX,mouseY,1);
                }
            }
        }
        if(button2WasDown&&!button2Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._Click(mouseX,mouseY,2);
                elements[i]._MouseUp(mouseX,mouseY,2);
                }
            }
        }
        if(button3WasDown&&!button3Down){
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].Unfocus();
                elements[i]._Click(mouseX,mouseY,3);
                elements[i]._MouseUp(mouseX,mouseY,3);
                }
            }
        }
        if (mouseDX != 0 || mouseDY != 0)
        {
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i]._Drag(mouseX,mouseY,mouseDX,mouseDY);
                }
            }
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i]._MouseMove(mouseX,mouseY,mouseDX,mouseDY);
                }
            }
        }
        if (tabWasDown && !tabDown)
        {
            if(shiftDown){
                for(int i=0;i<elements.Length;i++){
                    if(elements[i].visible){
                    elements[i].OnUnTab();
                    }
                }
            }
            else
            {
                for(int i=0;i<elements.Length;i++){
                    if(elements[i].visible){
                    elements[i].OnTab();
                    }
                }
            }
        }
        if (rightarrowkeypressed)
        {
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].OnRightArrowKey();
                }
            }
        }
        if (leftarrowkeypressed)
        {
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].OnLeftArrowKey();
                }
            }
        }
        if (enterkeypressed)
        {
            for(int i=0;i<elements.Length;i++){
                if(elements[i].visible){
                elements[i].OnEnter();
                }
            }
        }
        
        // Exit the game if the right mouse button is pressed
        

        // TODO: Add your update logic here
        if (keyState.IsKeyDown(Keys.P)&&keyState.IsKeyDown(Keys.E)&&(!lastKeyState.IsKeyDown(Keys.E)))
        {
            elements[0].visible=!elements[0].visible;
            elements[0].Unfocus();
        }
        foreach(Keys key in keyState.GetPressedKeys())
        {
            if (lastKeyState.IsKeyUp(key))
            {
                for(int i=0;i<elements.Length;i++){
                    if(elements[i].visible){
                    elements[i].OnKeyPress(key,shiftDown,altDown,ctrlDown,keyState.CapsLock);
                    }
                }
                
            }
        }
        
        
        lastMouseState=mouseState;
        lastKeyState=keyState;
        e.Update(rooms);
        List<RoomInstance> t=new List<RoomInstance>();
        foreach(RoomInstance inst in this.rooms){
            t.Add(inst);
        }
        foreach(RoomInstance inst in t){
            inst.Update();
        }
        if (this.rooms.Count == 0)
        {
            this.rooms.Add(new RoomInstance(0, new Vector2(400,400)));
        }
        }
        List<Particle> particlesb=new List<Particle>(particles);
        foreach(Particle particle in particlesb){
            particle.Update(rooms);
            if (particle.dead)
            {
                particles.Remove(particle);
                Console.WriteLine("Particle ded");
            }
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if(IsActive){
        
        //draw base light shape
        
        GraphicsDevice.SetRenderTarget(lightTarget);
        penumbra.BeginDraw();
        
        GraphicsDevice.Clear(Color.White);
        penumbra.Draw(gameTime);
        
        
        GraphicsDevice.SetRenderTarget(opaquesTarget);
        GraphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,samplerState:SamplerState.PointClamp);
        foreach(RoomInstance inst in this.rooms){
            inst.Draw(_spriteBatch,true);
        }
        _spriteBatch.End();
        GraphicsDevice.SetRenderTarget(sceneRenderTarget);
        GraphicsDevice.Clear(Color.DarkGray);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,samplerState:SamplerState.PointClamp);
        foreach(RoomInstance inst in this.rooms){
        inst.Draw(_spriteBatch,false);
        }
        foreach(Particle particle in particles){
            particle.Draw(_spriteBatch);
        }
        e.Draw(_spriteBatch);
        //IDs.RoomID.Rooms[inst.id].Draw((int)inst.position.X,(int)inst.position.Y,_spriteBatch);
        
        //e.Draw(_spriteBatch);
        _spriteBatch.End();



        GraphicsDevice.ScissorRectangle=new Rectangle(0,0,GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height);
        ScissorStack.device=GraphicsDevice;
        this.ScissorStack.batch=_spriteBatch;
        // TODO: Add your drawing code here
        /*GraphicsDevice.SetRenderTarget(finalLightTarget);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:lightSpreadEffect,samplerState:SamplerState.PointClamp);
        _spriteBatch.Draw(lightTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();*/



        for(int i=0;i<2;i++){
        GraphicsDevice.SetRenderTarget(finalLightTarget);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:lightSpreadEffectb,samplerState:SamplerState.PointClamp);
        _spriteBatch.Draw(lightTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(lightTarget);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:lightSpreadEffectc,samplerState:SamplerState.PointClamp);
        _spriteBatch.Draw(finalLightTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();
        }

        GraphicsDevice.SetRenderTarget(finalLightTarget);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:lightSpreadEffectb,samplerState:SamplerState.PointClamp);
        _spriteBatch.Draw(lightTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();
        



        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:finalizeLightingEffect,samplerState:SamplerState.PointClamp);
        _spriteBatch.Draw(finalLightTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend);
        for(int i=0;i<elements.Length;i++){
            if(elements[i].visible){
            elements[i].Draw(gameTime,_spriteBatch,_font,ScissorStack);
            }
        }
        _spriteBatch.DrawString(_font,e.Position.ToString(),new Vector2(1000,50),Color.Black);
        _spriteBatch.DrawString(_font,e.Velocity.ToString(),new Vector2(1000,150),Color.Black);
        _spriteBatch.DrawString(_font,((int)(1/(gameTime.ElapsedGameTime.TotalSeconds))).ToString(),new Vector2(1000,250),Color.Black);
        Console.WriteLine("fps "+((int)(1/(gameTime.ElapsedGameTime.TotalSeconds))).ToString());
        _spriteBatch.End();
        }
        /*GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.CornflowerBlue);        
        _spriteBatch.Begin(rasterizerState:_rasterizerState,blendState:BlendState.AlphaBlend,effect:combineRenderTargetsPostEffect);
        _spriteBatch.End();
        base.Draw(gameTime);*/
    }
}
