using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Windows;
using System.Collections.Generic;

namespace agame.GUI{
    public class XnaKeyMapper
    {
        public static string GetTypedCharBase(Keys key,bool shift){
            switch (key)
            {
                case Keys.A: return "a";
                case Keys.B: return "b";
                case Keys.C: return "c";
                case Keys.D: return "d";
                case Keys.E: return "e";
                case Keys.F: return "f";
                case Keys.G: return "g";
                case Keys.H: return "h";
                case Keys.I: return "i";
                case Keys.J: return "j";
                case Keys.K: return "k";
                case Keys.L: return "l";
                case Keys.M: return "m";
                case Keys.N: return "n";
                case Keys.O: return "o";
                case Keys.P: return "p";
                case Keys.Q: return "q";
                case Keys.R: return "r";
                case Keys.S: return "s";
                case Keys.T: return "t";
                case Keys.U: return "u";
                case Keys.V: return "v";
                case Keys.W: return "w";
                case Keys.X: return "x";
                case Keys.Y: return "y";
                case Keys.Z: return "z";
                
                case Keys.D0: return shift?")":"0";
                case Keys.D1: return shift?"!":"1";
                case Keys.D2: return shift?"@":"2";
                case Keys.D3: return shift?"#":"3";
                case Keys.D4: return shift?"$":"4";
                case Keys.D5: return shift?"%":"5";
                case Keys.D6: return shift?"^":"6";
                case Keys.D7: return shift?"&":"7";
                case Keys.D8: return shift?"*":"8";
                case Keys.D9: return shift?"(":"9";
                case Keys.NumPad0: return "0";
                case Keys.NumPad1: return "1";
                case Keys.NumPad2: return "2";
                case Keys.NumPad3: return "3";
                case Keys.NumPad4: return "4";
                case Keys.NumPad5: return "5";
                case Keys.NumPad6: return "6";
                case Keys.NumPad7: return "7";
                case Keys.NumPad8: return "8";
                case Keys.NumPad9: return "9";
                
                case Keys.Multiply: return "*";
                case Keys.Add: return "+";
                case Keys.Subtract: return "-";
                case Keys.Decimal: return ".";
                case Keys.Divide: return "/";
                case Keys.Space: return " ";

                case Keys.OemPeriod: return shift?">":".";
                case Keys.OemTilde: return shift?"~":"`";
                case Keys.OemSemicolon: return shift?":":";";
                case Keys.OemQuestion: return shift?"?":"/";
                case Keys.OemOpenBrackets: return shift?"{":"[";
                case Keys.OemCloseBrackets: return shift?"}":"]";
                case Keys.OemMinus: return shift?"_":"-";
                case Keys.OemPlus: return shift?"+":"=";
                case Keys.OemComma: return shift?"<":",";
                case Keys.OemQuotes: return shift?"\"":"'";
                case Keys.OemPipe: return shift?"|":"\\";
                }
            return "";
        }
        public static bool IsTypedChar(Keys key)
        {
            return GetTypedCharBase(key,false).Length!=0;
        }
        public static string GetTypedChar(Keys key, bool shift, bool capsLock)
        {
            string x=GetTypedCharBase(key,shift);
            if ((shift || capsLock) && !(shift && capsLock))
            {
                return x.ToUpper();
            }
            return x;
        }
    }
    
public class TextInput
{
    public String value{get{return _value;} set{
        if(this._value!=value){
            this._value=value;
            valueDirty=true;
        }
        }}
    private String _value;
    public bool valueDirty;
    public int cursorPos;
    public int selectedBetweenPos;
    public TextInput(String defaultvalue)
    {
        this.value=defaultvalue;
        cursorPos=0;
        selectedBetweenPos=0;
    }
    [STAThread]
    public void copy()
        {
            Clipboard.SetText(value.Substring(Math.Min(cursorPos,selectedBetweenPos),Math.Max(cursorPos,selectedBetweenPos)-Math.Min(cursorPos,selectedBetweenPos)));
        }
    [STAThread]
    public void paste()
        {
            
            value=value.Substring(0,Math.Min(cursorPos,selectedBetweenPos))+Clipboard.GetText()+value.Substring(Math.Max(cursorPos,selectedBetweenPos));
            cursorPos=Math.Min(cursorPos,selectedBetweenPos)+Clipboard.GetText().Length;
            selectedBetweenPos=cursorPos;
        }
    public void TypeKey(Keys key, bool shiftDown,bool altDown, bool ctrlDown, bool capsLock)
    {
        if(!ctrlDown){
            if (XnaKeyMapper.IsTypedChar(key))
            {
                value=value.Substring(0,Math.Min(cursorPos,selectedBetweenPos))+value.Substring(Math.Max(cursorPos,selectedBetweenPos));
                cursorPos=Math.Min(cursorPos,selectedBetweenPos);
                string chr=XnaKeyMapper.GetTypedChar(key,shiftDown,capsLock);
                value=value.Insert(cursorPos,chr);
                cursorPos+=chr.Length;
                selectedBetweenPos=cursorPos;
            }

            else if (key == Keys.Left)
            {
                cursorPos=Math.Max(0,cursorPos-1);
                if (!shiftDown)
                {
                    selectedBetweenPos=cursorPos;
                }
            }else if (key == Keys.Right)
            {
                cursorPos=Math.Min(value.Length,cursorPos+1);
                if (!shiftDown)
                {
                    selectedBetweenPos=cursorPos;
                }
            }else if (key == Keys.Back)
                {
                    int selectedAmount=Math.Max(cursorPos,selectedBetweenPos)-Math.Min(cursorPos,selectedBetweenPos);
                    if(selectedAmount>0){
                    value=value.Substring(0,Math.Min(cursorPos,selectedBetweenPos))+value.Substring(Math.Max(cursorPos,selectedBetweenPos));
                    cursorPos=Math.Min(cursorPos,selectedBetweenPos);
                    }
                    else
                    {
                        if (cursorPos != 0)
                        {
                            value=value.Substring(0,cursorPos-1)+value.Substring(cursorPos);
                            cursorPos-=1;
                        }
                    }
                    selectedBetweenPos=cursorPos;
                }
            }
            else
            {
                if (key == Keys.C)
                {
                    copy();
                }
                if (key == Keys.V)
                {
                    paste();
                }
                if (key == Keys.X)
                {
                    copy();
                    value=value.Substring(0,Math.Min(cursorPos,selectedBetweenPos))+value.Substring(Math.Max(cursorPos,selectedBetweenPos));
                    cursorPos=Math.Min(cursorPos,selectedBetweenPos);
                }
            }
        
        {
                
        }

        //Char.
        //Keys.A
        //Keys.Z
        //Keys.NumPad0
        //Keys.Divide
        //Keys.OemSemicolon
    
    }
    private int FindIndexClosestToBetween(SpriteFont font, int xpos, int ind1, int ind2)
    {
        if (ind1 == ind2)
        {
            return ind1;
        }
        int ind3=(int)((ind1+ind2)/2);
        float len1=font.MeasureString(this.value.Substring(0,ind1)).X;
        float len3=font.MeasureString(this.value.Substring(0,ind3)).X;
        float len2=font.MeasureString(this.value.Substring(0,ind2)).X;
        if (ind2 == (ind1 + 1))
        {
            if (xpos < ((len1 + len2) / 2))
            {
                return ind1;
            }
            else
            {
                return ind2;
            }
        }
        if (xpos < len3)
        {
            return FindIndexClosestToBetween(font,xpos,ind1,ind3);
        }
        else
        {
            return FindIndexClosestToBetween(font,xpos,ind3,ind2);
        }
    }
    public int FindIndexClosestTo(SpriteFont font, int xpos)
    {
        return FindIndexClosestToBetween(font,xpos,0,this.value.Length);
    }
}
}