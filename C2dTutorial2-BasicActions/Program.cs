using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace C2dTutorial2_BasicActions
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BasicActionsGame game = new BasicActionsGame())
            {
                game.Run();
            }
        }
    }


}

