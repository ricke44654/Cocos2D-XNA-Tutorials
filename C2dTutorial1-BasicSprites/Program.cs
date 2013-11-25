using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace C2dTutorial1_BasicSprites
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BasicSpritesGame game = new BasicSpritesGame())
            {
                game.Run();
            }
        }
    }


}

