using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace C2dTutorial2_BasicActions
{
    /// <summary>
    /// This is the main class of your game.  It initializes the graphics device, creates the Cocos2D-XNA application
    /// delegate, and provides the Update method for servicing input events for the game.
    /// </summary>
    public class BasicActionsGame : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private InputHelper _input;

        public BasicActionsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            
            // Set the title of the window
            this.Window.Title = "Cocos2D-XNA Tutorials: Basic Actions";

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
            BasicActionsLayer spriteLayer = null;
            if (runningScene != null) spriteLayer = (BasicActionsLayer)runningScene.GetChildByTag(BasicActionsScene.ActionLayerTag);

            // Check input to see if it's something that we care about
            // Key mappings:
            // =                       : Add a new sprite to the game
            // -                       : Delete the last sprite added from the game
            // ,                       : Makes the previous sprite on the screen active
            // .                       : Makes the next sprite on the screen active
            // C / Shift-C             : Clears all actions for the active (all) sprite(s)
            // R / Shift-R             : Rotates the active sprite (indefinitely)
            // S / Shift-S             : Scales the active sprite (indefinitely)
            // K / Shift-K             : Skews the active sprite (indefinitely)
            // J / Shift-J             : Jumps the active sprite (indefinitely)
            // B                       : Moves the active sprite in a box shape
            // T                       : Moves the active sprite in a triangle shape
            // A                       : Moves the active sprite using all the shape movements
            if (_input.IsNewPress(Keys.OemPlus))
                spriteLayer.CreateSprite();
            if (_input.IsNewPress(Keys.OemMinus))
                spriteLayer.DeleteSprite();
            if (_input.IsNewPress(Keys.OemComma))
                spriteLayer.SetActiveSprite(false);
            if (_input.IsNewPress(Keys.OemPeriod))
                spriteLayer.SetActiveSprite(true);
            if (_input.IsNewPress(Keys.C))
                spriteLayer.ClearAction(_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.R))
                spriteLayer.Rotate(_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.S))
                spriteLayer.Scale(_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.K))
                spriteLayer.Skew(_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.J))
                spriteLayer.Jump(_input.IsCurPress(Keys.LeftShift));
            if (_input.IsNewPress(Keys.B))
                spriteLayer.BoxMove();
            if (_input.IsNewPress(Keys.T))
                spriteLayer.TriangleMove();
            if (_input.IsNewPress(Keys.A))
                spriteLayer.AllShapesMove();
            if (_input.IsNewPress(MouseButtons.LeftButton))
            {
                // Get the dimensions of the game window
                var winSize = CCDirector.SharedDirector.WinSize;
                // Convert the mouse position into Cocos2D-XNA position
                var position = new CCPoint((_input.MousePosition.X * winSize.Width) / CCApplication.SharedApplication.GraphicsDevice.Viewport.Width, 
                                            winSize.Height - ((_input.MousePosition.Y * winSize.Height) / CCApplication.SharedApplication.GraphicsDevice.Viewport.Height));

                spriteLayer.MoveTo(position);
            }

            base.Update(gameTime);
        }
    }
}