using System;
using System.Collections.Generic;
using System.Linq;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace C2dTutorial2_BasicActions
{
    public class BasicActionsLayer : CCLayer
    {
        private Dictionary<string, CCSprite> _sprites;           // Contains the list of all the sprites in the sprite layer
        private Random _rand;                                    // Your friendly neighborhood random number generator
        private CCSprite _activeSprite;                          // Contains a reference to the sprite that will receive the actions
        private int _curSpriteNumber;                            // Contains the current sprite number being changed

        public BasicActionsLayer()
        {
            // Initialize the sprite list
            _sprites = new Dictionary<string, CCSprite>();

            // Initialize the random number generator
            _rand = new Random();

            // Start with two sprites on the sprite layer
            for (int i = 0; i < 2; i++)
                CreateSprite();
        }

        /// <summary>
        /// Creates a new sprite and adds it to the sprite list & screen.
        /// </summary>
        internal void CreateSprite()
        {
            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Get the new number of sprites on the screen and setup an id & image for the sprite (cycles between the 3 available images)
            var numSprites = _sprites.Count + 1;
            var spriteId = "sprite" + numSprites;
            var spriteImage = "Images/Sprite" + ((numSprites % 3) + 1);

            // Create a new instance of the sprite, then setup the position, velocity, & zorder
            var newSprite = new CCSprite(spriteImage);
            newSprite.SetPosition(_rand.Next(150, (int)(winSize.Width - 150f)), _rand.Next(150, (int)(winSize.Height - 150f)));

            // Add the new sprite to the sprite list
            _sprites.Add(spriteId, newSprite);

            // Add the sprite to the layer
            AddChild(newSprite);

            // Set the active sprite to the newly added sprite
            _curSpriteNumber = numSprites - 1;
            SetActiveSprite(true);
        }

        /// <summary>
        /// Deletes the last added sprite from the sprite list & screen.
        /// </summary>
        internal void DeleteSprite()
        {
            // Make sure we have some sprites
            if (_sprites.Any())
            {
                // Get the id of the last sprite in the sprite list
                var spriteId = "sprite" + _sprites.Count;

                // Get the sprite to delete
                var deleteSprite = _sprites[spriteId];

                // Remove the sprite from the sprite list
                _sprites.Remove(spriteId);

                // Remove the sprite from the layer
                RemoveChild(deleteSprite, true);

                // Set the active sprite to the newly added sprite
                _curSpriteNumber = _sprites.Count - 1;
                SetActiveSprite(true);
            }
        }

        /// <summary>
        /// Sets the active sprite to the next or previous sprite in the sprite list.
        /// </summary>
        /// <param name="nextSprite">A boolean value that determines if we're moving to the next or previous sprite in the sprite list.</param>
        internal void SetActiveSprite(bool nextSprite)
        {
            // Make sure there's at least one sprite
            if (_sprites.Any())
            {
                // Keep the sprite number in the bounds of the active sprites on the screen (wrap if necessary)
                _curSpriteNumber += (nextSprite ? 1 : -1);
                if (_curSpriteNumber < 1)
                    _curSpriteNumber = _sprites.Count;
                else if (_curSpriteNumber > _sprites.Count)
                    _curSpriteNumber = 1;

                // Setup the sprite id
                var spriteId = "sprite" + _curSpriteNumber;

                // Reset the active indicator for the previous sprite if necessary
                if (_activeSprite != null)
                    _activeSprite.Scale = 1;

                // Make sure the sprite exists
                if (_sprites.ContainsKey(spriteId))
                {
                    // Make the selected sprite the action sprite
                    _activeSprite = _sprites[spriteId];

                    // Make the active sprite appear a little bigger
                    _activeSprite.Scale = 1.2f;
                }
            }
        }

        /// <summary>
        /// Clears all running actions for the active sprite or all sprites.
        /// </summary>
        /// <param name="allSprites"></param>
        internal void ClearAction(bool allSprites)
        {
            // See if we're clearing actions for all sprites
            if (allSprites)
                ActionManager.RemoveAllActions();

            // Make sure we have an action sprite
            else if (_activeSprite != null)
                _activeSprite.StopAllActions();
        }

        /// <summary>
        /// Moves the active sprite to a specific location on the screen.
        /// </summary>
        /// <param name="position">A CCPoint object that contains the location where the sprite should be moved.</param>
        internal void MoveTo(CCPoint position)
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
            {
                // Move the sprite to the position
                CCActionInterval action = new CCMoveTo(1, position);
                _activeSprite.RunAction(action);
            }
        }

        /// <summary>
        /// Rotates the active sprite.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        internal void Rotate(bool repeatForever)
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.RotateAction(repeatForever));
        }

        /// <summary>
        /// Scales the active sprite.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        internal void Scale(bool repeatForever)
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.ScaleAction(repeatForever));
        }

        internal void Skew(bool repeatForever)
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.SkewAction(repeatForever));
        }

        /// <summary>
        /// Makes the active sprite jump on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        internal void Jump(bool repeatForever)
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.JumpAction(repeatForever));
        }

        /// <summary>
        /// Moves the active sprite in a box on the screen.
        /// </summary>
        internal void BoxMove()
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.BoxAction(true));
        }

        /// <summary>
        /// Moves the active sprite in a triangle on the screen.
        /// </summary>
        internal void TriangleMove()
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.TriangleAction(true));
        }

        /// <summary>
        /// Moves the active sprite using all the shape actions on the screen.
        /// </summary>
        internal void AllShapesMove()
        {
            // Make sure we have a sprite to run the action
            if (_activeSprite != null)
                _activeSprite.RunAction(BasicActionsLayer.AllShapesAction(true));
        }

        /// <summary>
        /// Creates an action that rotates a sprite on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCFiniteTimeAction RotateAction(bool repeatForever)
        {
            // Create an action to rotate the sprite by 20 degrees
            CCActionInterval rotateAction = new CCRotateBy(0.2f, 20);

            // If necessary, repeat the scale actions above indefinitely
            if (repeatForever) rotateAction = new CCRepeatForever(rotateAction);

            return rotateAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to be scaled on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCFiniteTimeAction ScaleAction(bool repeatForever)
        {
            // Create an action to scale the sprite to 1.5 times it's size
            CCActionInterval scaleAction = new CCScaleBy(0.2f, 1.5f);

            // Setup an action sequence to perform the scale action, then reverse it
            scaleAction = new CCSequence(scaleAction, scaleAction.Reverse());

            // If necessary, repeat the scale actions above indefinitely
            if (repeatForever) scaleAction = new CCRepeatForever(scaleAction);

            return scaleAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to be skewed on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        private static CCActionInterval SkewAction(bool repeatForever)
        {
            // Create an action to skew the sprite
            CCActionInterval skewAction = new CCSkewBy(0.2f, 10, 5);

            // Setup an action sequence to perform the skew action, then reverse it
            skewAction = new CCSequence(skewAction, skewAction.Reverse());

            // If necessary, repeat the skew actions above indefinitely
            if (repeatForever) skewAction = new CCRepeatForever(skewAction);

            return skewAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to jump on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCFiniteTimeAction JumpAction(bool repeatForever)
        {
            // Create an action to jump the active sprite from it's current position
            CCActionInterval jumpAction = new CCJumpBy(0.3f, new CCPoint(150, 0), 100, 1);

            // Setup an action sequence to perform the jump action, then reverse it
            jumpAction = new CCSequence(jumpAction, jumpAction.Reverse());

            // Repeat the jump sequence three times, just for fun
            jumpAction = new CCRepeat(jumpAction, 3);

            // If necessary, repeat the jump actions above indefinitely
            if (repeatForever) jumpAction = new CCRepeatForever(jumpAction);

            return jumpAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to move in a box shape on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCFiniteTimeAction BoxAction(bool repeatForever)
        {
            // Create a new action sequence that will move the sprite in a box shape at fixed positions on the screen
            CCActionInterval boxAction = new CCSequence(
                new CCMoveTo(1, new CCPoint(100, 100)),
                new CCMoveTo(1, new CCPoint(100, 500)),
                new CCMoveTo(1, new CCPoint(500, 500)),
                new CCMoveTo(1, new CCPoint(500, 100)));

            // See if we're repeating the action indefinitely
            if (repeatForever) boxAction = new CCRepeatForever(boxAction);

            return boxAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to move in a triangle shape on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCFiniteTimeAction TriangleAction(bool repeatForever)
        {
            // Create a new action sequence that will move the sprite in a triangle shape at fixed positions on the screen
            CCActionInterval triangleAction = new CCSequence(
                new CCMoveTo(1, new CCPoint(100, 100)),
                new CCMoveTo(1, new CCPoint(300, 500)),
                new CCMoveTo(1, new CCPoint(500, 100)));

            // See if we're repeating the action indefinitely
            if (repeatForever) triangleAction = new CCRepeatForever(triangleAction);

            return triangleAction;
        }

        /// <summary>
        /// Creates a sequence of actions that causes a sprite to move using all the actions on the screen.
        /// </summary>
        /// <param name="repeatForever">A boolean value that determines if the action should be repeated indefinitely.</param>
        /// <returns></returns>
        internal static CCAction AllShapesAction(bool repeatForever)
        {
            // Create a new action sequence that will move the sprite using all the other actions
            CCActionInterval allAction = new CCSequence(RotateAction(false), ScaleAction(false), JumpAction(false), BoxAction(false), TriangleAction(false));

            // See if we're repeating the action indefinitely
            if (repeatForever) allAction = new CCRepeatForever(allAction);

            return allAction;
        }
    }
}

