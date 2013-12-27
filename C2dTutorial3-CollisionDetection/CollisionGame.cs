using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This is the main class of your game.  It initializes the graphics device, creates the Cocos2D-XNA application
    /// delegate, and provides the Update method for servicing input events for the game.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CollisionGame : Game
    {
        #region Variables

        public static InputHelper Input;                 // Manages the input for the game
        public static CollisionGrid Grid;                // Detects general collisions in the game
        public static Random Rand;                       // Generates any random numbers for the game

        public static bool ShowCollisionGrid = true;     // Determines if the collision grid is displayed on the screen
        public static bool ShowBoundingBoxes = false;    // Determines if the bounding boxes for the game objects are displayed on the screen
        public static bool MoveEnemies = true;           // Determines if the enemies are moved on the screen
        public static bool MoveBullets = true;           // Determines if enemy bullets are moved on the screen

        private readonly GraphicsDeviceManager graphics;

        #endregion

        public CollisionGame()
        {
            // Set the title of the window
            Window.Title = "Cocos2D-XNA Tutorials: Collision Detection";

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
        /// Override method of the MonoGame update for servicing input in the game. Input can also be handled in various Cocos2d-XNA objects by
        /// overriding the Update method and checking there as well. I update the InputHelper state once here and then use it in the game objects.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            // Update the state of the input helper
            Input.Update();

            // Allows the game to exit
            if (Input.IsNewPress(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                ProcessBackClick();

            // Toggle the visible state of the bounding boxes
            if (Input.IsNewPress(Keys.B))
                ShowBoundingBoxes = !ShowBoundingBoxes;

            // Toggle the visible state of the collision grid
            if (Input.IsNewPress(Keys.G))
                ShowCollisionGrid = !ShowCollisionGrid;

            // Toggle the moving of enemies on the screen
            if (Input.IsNewPress(Keys.E))
                MoveEnemies = !MoveEnemies;

            // Toggle the moving of enemy shots on the screen
            if (Input.IsNewPress(Keys.S))
                MoveBullets = !MoveBullets;

            // Let the base object do it's thing
            base.Update(gameTime);
        }
    }
}