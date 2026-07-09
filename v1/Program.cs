using System;
namespace agame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
using var game = new agame.Game1();
game.Run();
        }
    }
}
