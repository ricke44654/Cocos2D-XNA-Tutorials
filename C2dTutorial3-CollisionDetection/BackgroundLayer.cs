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

        public override void Draw()
        {
            base.Draw();

            // Show the collision grid if necessary
            if (CollisionGame.ShowCollisionGrid)
            {
                // Get a reference to the collision grid
                var grid = CollisionGame.Grid;
                var winSize = CCDirector.SharedDirector.WinSize;

                CCDrawingPrimitives.Begin();

                for (int row = 1; row <= grid.GridRows; row++)
                    CCDrawingPrimitives.DrawLine(new CCPoint(0, row * grid.GridCellY), new CCPoint(winSize.Width, row * grid.GridCellY), new CCColor4B(Color.Gray));

                for (int col = 1; col <= grid.GridColumns; col++)
                    CCDrawingPrimitives.DrawLine(new CCPoint(col * grid.GridCellX, 0), new CCPoint(col * grid.GridCellX, winSize.Height), new CCColor4B(Color.Gray));

                CCDrawingPrimitives.End();
            }
        }
    }
}

