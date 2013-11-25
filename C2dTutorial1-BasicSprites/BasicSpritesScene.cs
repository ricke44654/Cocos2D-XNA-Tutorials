using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial1_BasicSprites
{
    /// <summary>
    /// A Cocos2D-XNA scene that displays the background and sprite layers.
    /// </summary>
    internal class BasicSpritesScene : CCScene
    {
        // Identifies the sprite layer in the scene so we can access it easily in the Update method of the game
        public const int SpriteLayerTag = 1;

        public BasicSpritesScene()
        {
            // Create the background layer and add it to the scene
            var backgroundLayer = new BackgroundLayer();
            AddChild(backgroundLayer, 0);

            // Create the sprite layer and add it to the scene
            var spritesLayer = new BasicSpritesLayer();
            AddChild(spritesLayer, 5, SpriteLayerTag);
        }
    }
}
