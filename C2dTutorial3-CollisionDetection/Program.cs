using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace C2dTutorial3_CollisionDetection
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CollisionGame game = new CollisionGame())
            {
                game.Run();
            }
        }
    }


}

