using agame.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


namespace agame.IDs;
class SoundID
{
    
    public static SoundEffect[] effects=[null];
    public static int count=1;
    public static void Load(string fname,int id)
    {
        effects[id]=SoundSystem.LoadSound(fname);
    }
    
    
}