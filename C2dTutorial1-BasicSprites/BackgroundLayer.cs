using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial1_BasicSprites
{
    /// <summary>
    /// A Cocos2D-XNA layer that manages the background in the game.
    /// </summary>
    internal class BackgroundLayer : CCLayer
    {
        public BackgroundLayer()
        {
            // Load the background image into a sprite
            var backgroundImage = new CCSprite("Images/Background");

            // Get the dimensions of the game window
            var screenSize = CCDirector.SharedDirector.WinSize;

            // Set the position of the background sprite to the center of the screen
            backgroundImage.SetPosition(screenSize.Width / 2, screenSize.Height / 2);

            // Add the background sprite to the layer
            AddChild(backgroundImage, 0);
        }
    }
}
