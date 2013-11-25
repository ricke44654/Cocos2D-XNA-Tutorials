using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace C2dTutorial1_BasicSprites
{
    /// <summary>
    /// This is the main class of your game.  It initializes the graphics device, creates the Cocos2D-XNA application
    /// delegate, and provides the Update method for servicing input events for the game.
    /// </summary>
    public class BasicSpritesGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private InputHelper _input;
        public BasicSpritesGame()
        {
            graphics = new GraphicsDeviceManager(this);

            // Create new instance of input helper
            _input = new InputHelper();

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333 / 2);

            // Create the application delegate and add it as a game component
            CCApplication application = new AppDelegate(this, graphics);
            Components.Add(application);
        }

        /// <summary>
        /// Provides back button support for moving to the previous scene or exiting the game.
        /// </summary>
        private void ProcessBackClick()
        {
            if (CCDirector.SharedDirector.CanPopScene)
                CCDirector.SharedDirector.PopScene();
            else
                Exit();
        }

        /// <summary>
        /// Main MonoGame update method, mostly used to handle game input.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            // Check the input state of the game
            _input.Update();

            // See if the escape key has been pressed
            bool escapeKey = false;
#if (!XBOX)
            escapeKey = _input.IsNewPress(Keys.Escape);
#endif
            // Allows the game to exit or move back a screen
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || escapeKey)
                ProcessBackClick();

            // Get the current scene and sprite layer
            var runningScene = CCDirector.SharedDirector.RunningScene;
            BasicSpritesLayer spriteLayer = null;
            if (runningScene != null) spriteLayer = (BasicSpritesLayer)runningScene.GetChildByTag(BasicSpritesScene.SpriteLayerTag);

            // Check input to see if it's something that we care about
            // Key mappings:
            // =+                       : Add a new sprite to the game
            //  -                       : Delete the last sprite added from the game
            // Space                    : Toggles the update of the sprites layer
            // Up / Down / Left / Right : Moves the sprites manually on the screen
            // V                        : Toggles the visibility of the sprites
            // Z                        : Changes the zorder of the sprites
            // O / Shift-O              : Changes the opacity of the sprites
            // R / Shift-R              : Rotates the sprites clockwise / counter-clockwise
            // S / Shift-S              : Scales the sprites to a bigger / smaller size
            // F / Shift-F              : Flips the sprites horizontally / vertically
            // X / Shift-X              : Skews the sprites forward / backward
            if (_input.IsNewPress(Keys.OemPlus))
                spriteLayer.CreateSprite();
            if (_input.IsNewPress(Keys.OemMinus))
                spriteLayer.DeleteSprite();
            if (_input.IsNewPress(Keys.Space))
                spriteLayer.ToggleUpdateSchedule();
            if (_input.IsCurPress(Keys.Up))
                spriteLayer.SetPosition(Keys.Up);
            if (_input.IsCurPress(Keys.Down))
                spriteLayer.SetPosition(Keys.Down);
            if (_input.IsCurPress(Keys.Left))
                spriteLayer.SetPosition(Keys.Left);
            if (_input.IsCurPress(Keys.Right))
                spriteLayer.SetPosition(Keys.Right);
            if (_input.IsNewPress(Keys.V))
                spriteLayer.ToggleVisible();
            if (_input.IsNewPress(Keys.Z))
                spriteLayer.ChangeZOrder();
            if (_input.IsCurPress(Keys.O))
                spriteLayer.Opacity(!_input.IsCurPress(Keys.LeftShift));
            if (_input.IsCurPress(Keys.R))
                spriteLayer.Rotate(!_input.IsCurPress(Keys.LeftShift));
            if (_input.IsCurPress(Keys.S))
                spriteLayer.Scale(!_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.F))
                spriteLayer.Flip(!_input.IsCurPress(Keys.LeftShift));
            if (_input.IsCurPress(Keys.X))
                spriteLayer.Skew(!_input.IsCurPress(Keys.LeftShift));

            base.Update(gameTime);
        }
    }
}