using System;

namespace AsteroidsApp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Asteroids())
                game.Run();
        }
    }
}
