using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// A Cocos2D-XNA scene that displays the background and sprite layers.
    /// </summary>
    public class CollisionScene : CCScene
    {
        #region Constructors

        public CollisionScene()
        {
            // Create the background layer and add it to the scene
            var backgroundLayer = new BackgroundLayer();
            AddChild(backgroundLayer);

            // Create the game object layer and add it to the scene
            var gameObjectLayer = new GameObjectLayer();
            AddChild(gameObjectLayer);
        }

        #endregion
    }
}
