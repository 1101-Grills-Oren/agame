
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using agame;
using agame.RoomEditor;
namespace agame.GUI;

public class GameEditorGui:GuiFrame
{
    
    public GameEditorGui(Texture2D _texture,Texture2D _tabtexture,Texture2D _textureselected,int left,int top, int width, int height):base(_texture,left,top,width,height,true,Color.AliceBlue,"")
    {
        GuiElement element=new GuiButton(_tabtexture,left+12,top+10,30,20,Color.AliceBlue);
        element.id=1;
        element._selectedtexture=_textureselected;
        this.Add(element);
        element=new GuiButton(_tabtexture,left+42,top+10,30,20,Color.AliceBlue);
        element.id=2;
        element._selectedtexture=_textureselected;
        this.Add(element);
        element=new GuiButton(_tabtexture,left+72,top+10,30,20,Color.AliceBlue);
        element.id=3;
        element._selectedtexture=_textureselected;
        this.Add(element);
        
        element=new RoomEditGui(_texture,left+10,top+30,width-20,height-40);
        element.id=4;
        this.Add(element);
        element.hasParent=false;
        
        element=new TileEditGui(_texture,left+10,top+30,width-20,height-40);
        element.id=5;
        element.visible=false;
        this.Add(element);
        element.hasParent=false;

        element=new RoomLinkEditGui(_texture,left+10,top+30,width-20,height-40);
        element.id=6;
        element.visible=false;
        this.Add(element);
        element.hasParent=false;
        element=new GuiButton(_texture,left+152,top+10,20,20,Color.AliceBlue);
        element.id=7;
        this.Add(element);
        element=new GuiButton(_tabtexture,left+102,top+10,30,20,Color.AliceBlue);
        element.id=8;
        element._selectedtexture=_textureselected;
        this.Add(element);
        element=new ParticleEditGui(_texture,left+10,top+30,width-20,height-40){visible=false};
        element.id=9;
        element._selectedtexture=_textureselected;
        this.Add(element);
        element.hasParent=false;
        
    }
    public override void Resize(int left, int top, int width, int height)
    {
        this.left=left;
        this.top=top;
        this.width=width;
        this.height=height;
        this.children[0].Resize(left+12,top+10,30,20);
        this.children[1].Resize(left+42,top+10,30,20);
        this.children[2].Resize(left+72,top+10,30,20);
        this.children[3].Resize(left+10,top+30,width-20,height-40);
        this.children[4].Resize(left+10,top+30,width-20,height-40);
        this.children[5].Resize(left+10,top+30,width-20,height-40);
        this.children[6].Resize(left+152,top+10,20,20);
        this.children[7].Resize(left+102,top+10,30,20);
        this.children[8].Resize(left+10,top+30,width-20,height-40);
        
    }
    
    public override void OnUpdate(int id,GuiElement obj)
    {
        if (id == 1)
        {
            this.children[3].visible=true;
            this.children[4].visible=false;
            this.children[5].visible=false;
            this.children[8].visible=false;
        }
        if (id == 2)
        {
            this.children[3].visible=false;
            this.children[4].visible=true;
            this.children[5].visible=false;
            this.children[8].visible=false;
        }
        if (id == 3)
        {
            this.children[3].visible=false;
            this.children[4].visible=false;
            this.children[5].visible=true;
            this.children[8].visible=false;
        }
        if (id == 7)
        {
            RoomEditLogic.TrySaveData("Map");
        }
        if (id == 8)
        {
            this.children[3].visible=false;
            this.children[4].visible=false;
            this.children[5].visible=false;
            this.children[8].visible=true;
        }
    }
}
