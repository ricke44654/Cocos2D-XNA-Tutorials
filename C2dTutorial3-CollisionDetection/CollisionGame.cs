using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CollisionGame : Game
    {
        internal static InputHelper Input;
        internal static CollisionGrid Grid;
        internal static Random Rand;

        internal static bool ShowCollisionGrid = true;
        internal static bool ShowBoundingBoxes = false;
        internal static bool MoveEnemies = true;

        private readonly GraphicsDeviceManager graphics;

        public CollisionGame()
        {
            graphics = new GraphicsDeviceManager(this);

            //#if MONOMAC
            //            Content.RootDirectory = "AngryNinjas/Content";
            //#else
            Content.RootDirectory = "Content";
            //#endif
            //
            //#if XBOX || OUYA
            //            graphics.IsFullScreen = true;
            //#else
            graphics.IsFullScreen = false;
            //#endif

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333 / 2);

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            CCApplication application = new AppDelegate(this, graphics);
            Components.Add(application);
            //#if XBOX || OUYA
            //            CCDirector.SharedDirector.GamePadEnabled = true;
            //            application.GamePadButtonUpdate += new CCGamePadButtonDelegate(application_GamePadButtonUpdate);
            //#endif
        }

        //#if XBOX || OUYA
        //        private void application_GamePadButtonUpdate(CCGamePadButtonStatus backButton, CCGamePadButtonStatus startButton, CCGamePadButtonStatus systemButton, CCGamePadButtonStatus aButton, CCGamePadButtonStatus bButton, CCGamePadButtonStatus xButton, CCGamePadButtonStatus yButton, CCGamePadButtonStatus leftShoulder, CCGamePadButtonStatus rightShoulder, PlayerIndex player)
        //        {
        //            if (backButton == CCGamePadButtonStatus.Pressed)
        //            {
        //                ProcessBackClick();
        //            }
        //        }
        //#endif

        private void ProcessBackClick()
        {
            if (CCDirector.SharedDirector.CanPopScene)
            {
                CCDirector.SharedDirector.PopScene();
            }
            else
            {
                Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the state of the input helper
            Input.Update();

            var escapePressed = false;
            if (Input.IsNewPress(Keys.Escape)) escapePressed=true;

            // Allows the game to exit
            if (escapePressed || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ProcessBackClick();
            }

            // Toggle the visible state of the grid if necessary
            if (Input.IsNewPress(Keys.B))
                ShowBoundingBoxes = !ShowBoundingBoxes;
            if (Input.IsNewPress(Keys.E))
                MoveEnemies = !MoveEnemies;
            if (Input.IsNewPress(Keys.G))
                ShowCollisionGrid = !ShowCollisionGrid;

            base.Update(gameTime);
        }
    }
}