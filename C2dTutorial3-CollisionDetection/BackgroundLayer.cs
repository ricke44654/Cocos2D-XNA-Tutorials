using System;
using Cocos2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// A Cocos2D-XNA layer that manages the background in the game.
    /// </summary>
    internal class BackgroundLayer : CCLayer
    {
        public BackgroundLayer()
        {
        }

        /// <summary>
        /// Override method to draw the background on the screen.
        /// </summary>
        public override void Draw()
        {
            // Let the base object do it's thing
            base.Draw();

            // Show the collision grid if necessary
            if (CollisionGame.ShowCollisionGrid)
            {
                // Get a reference to the collision grid and window dimensions
                var grid = CollisionGame.Grid;
                var winSize = CCDirector.SharedDirector.WinSize;

                // Start the process of drawing primitives
                CCDrawingPrimitives.Begin();

                // Use the Cocos2d-XNA drawing primitives to draw horizontal lines for each row in the collision grid
                for (int row = 1; row <= grid.GridRows; row++)
                    CCDrawingPrimitives.DrawLine(new CCPoint(0, row * grid.GridSquareY), new CCPoint(winSize.Width, row * grid.GridSquareY), new CCColor4B(Color.Gray));

                // Use the Cocos2d-XNA drawing primitives to draw vertical lines for each column in the collision grid
                for (int col = 1; col <= grid.GridColumns; col++)
                    CCDrawingPrimitives.DrawLine(new CCPoint(col * grid.GridSquareX, 0), new CCPoint(col * grid.GridSquareX, winSize.Height), new CCColor4B(Color.Gray));

                // We're finished drawing primitives
                CCDrawingPrimitives.End();
            }
        }
    }
}

