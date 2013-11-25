using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial1_BasicSprites
{
    /// <summary>
    /// A sprite object that inherits from a Cocos2D-XNA sprite and adds a velocity property to control the movement.
    /// </summary>
    internal class BasicSprite : CCSprite
    {
        private CCPoint _velocity;

        public BasicSprite(string content)
            : base(content)
        {
        }

        /// <summary>
        /// Sets the velocity of the sprite.
        /// </summary>
        /// <param name="velocity"></param>
        internal void SetVelocity(CCPoint velocity)
        {
            _velocity = velocity;
        }

        /// <summary>
        /// Moves the sprite by the velocity while keeping it in the bounds of the game window.
        /// </summary>
        internal void MoveByVelocity()
        {
            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Add the velocity to the current position to get the new position for the sprite
            var newPosition = Position + _velocity;

            // By default, Cocos2D-XNA has the sprite origin in the center of the sprite.  So we determine the midpoint to help keep 
            // the sprite in the bounds of the game window.
            var halfWidth = ContentSizeInPixels.Width / 2;
            var halfHeight = ContentSizeInPixels.Height / 2;

            // See if the new position of the sprite is off the left or right side of the game window
            if (newPosition.X <= halfWidth || newPosition.X >= winSize.Width - halfWidth)
            {
                // Reverse the horizontal direction of the sprite
                _velocity.X *= -1;

                // Adjust the new position of the sprite to be against the left or right side of the game window
                if (newPosition.X <= halfWidth)
                    newPosition.X = halfWidth;
                else
                    newPosition.X = winSize.Width - halfWidth;
            }

            // See if the new position of the sprite is off the bottom or top of the game window
            if (newPosition.Y <= halfHeight || newPosition.Y >= winSize.Height - halfHeight)
            {
                // Reverse the vertical direction of the sprite
                _velocity.Y *= -1;

                // Adjust the new position of the sprite to be against the bottom or top of the game window
                if (newPosition.Y <= halfHeight)
                    newPosition.Y = halfHeight;
                else
                    newPosition.Y = winSize.Height - halfHeight;
            }

            // Move the sprite to the new position
            SetPosition(newPosition.X, newPosition.Y);
        }
    }
}
