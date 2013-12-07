using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial2_BasicActions
{
    /// <summary>
    /// A Cocos2D-XNA scene that displays the background and sprite layers.
    /// </summary>
    internal class BasicActionsScene : CCScene
    {
        // Identifies the action layer in the scene so we can access it easily in the Update method of the game
        public const int ActionLayerTag = 1;

        public BasicActionsScene()
        {
            // Create the background layer and add it to the scene
            var backgroundLayer = new BackgroundLayer();
            AddChild(backgroundLayer, 0);

            // Create the sprite layer and add it to the scene
            var spritesLayer = new BasicActionsLayer();
            AddChild(spritesLayer, 5, ActionLayerTag);
        }
    }
}
