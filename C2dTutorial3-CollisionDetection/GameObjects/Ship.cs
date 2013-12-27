using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This game object represents the player's ship on the screen. The player uses the arrow keys to move the ship horizontally or
    /// vertically on the screen and the Left Control key to fire at enemy ships.
    /// </summary>
    public class Ship : GameObject
    {
        #region Constants

        /// <summary>
        /// The speed of the player's ship on the screen.
        /// </summary>
        private const float ShipSpeed = 7;

        /// <summary>
        /// The speed of the player's shots on the screen.
        /// </summary>
        private const float ShotSpeed = 100;

        /// <summary>
        /// The maximum number of shots that the player can have active at a given time.
        /// </summary>
        private const int MaximumShots = 4;

        #endregion

        #region Variables

        private List<GameObject> _shots;       // The list of active shots for the player

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the player's ship using the Ship content and mask.
        /// </summary>
        public Ship() : base(GameObjectType.Ship, "Images/Ship", "Ship")
        {
            // Create list of shots fired from the ship
            _shots = new List<GameObject>();

            // Tell Cocos2d-XNA to schedule a call to this sprite's Update method
            ScheduleUpdate();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of active shots for the player's ship.
        /// </summary>
        public List<GameObject> Shots
        {
            get { return _shots; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override method to update the player's ship state for each game loop iteration.
        /// </summary>
        /// <param name="dt">The amount of time that has passed since the last call to the Update method.</param>
        public override void Update(float dt)
        {
            // Let the base object do it's thing
            base.Update(dt);

            // Get the window dimensions
            var winSize = CCDirector.SharedDirector.WinSize;

            // Get a reference to the input helper
            var input = CollisionGame.Input;

            // Move the ship horizontally / vertically based on the arrow keys pressed
            if (input.IsCurPress(Keys.Right))
                PositionX += ShipSpeed;
            if (input.IsCurPress(Keys.Left))
                PositionX -= ShipSpeed;
            if (input.IsCurPress(Keys.Up))
                PositionY += ShipSpeed;
            if (input.IsCurPress(Keys.Down))
                PositionY -= ShipSpeed;

            // Fire a shot if the Left Control key is pressed
            if (input.IsNewPress(Keys.LeftControl))
                FireShot();

            // Keep the ship on the screen
            if (PositionX < TextureRect.MidX) PositionX = TextureRect.MidX;
            if (PositionX > winSize.Width - TextureRect.MidX) PositionX = winSize.Width - TextureRect.MidX;
            if (PositionY < TextureRect.MidY) PositionY = TextureRect.MidY;
            if (PositionY > winSize.Height - TextureRect.MidY) PositionY = winSize.Height - TextureRect.MidY;

            // Remove any shots from the screen and shot list that have moved past the top of the screen
            for (int i = 0; i < _shots.Count; i++)
            {
                var shot = _shots[i];
                if (shot.PositionY > CCDirector.SharedDirector.WinSize.Height)
                {
                    shot.RemoveFromParent();
                    _shots.Remove(shot);
                }
            }
        }

        /// <summary>
        /// Fires a shot for the player's ship.
        /// </summary>
        private void FireShot()
        {
            // Limit the number of shots that can be active at once
            if (_shots.Count >= MaximumShots) return;

            // Create a new bullet at the ship's position
            var bullet = new Bullet();
            bullet.SetPosition(PositionX, PositionY + TextureRect.MidY);

            // Add the shot to the ship's parent container (i.e. the GameObjectLayer). This is a handy bit of Cocos2d-XNA architecture
            // so you don't have to maintain references in your code to get game objects on the screen... booyah.
            Parent.AddChild(bullet);

            // Create an action to move the bullet to the top of the screen. Basically, we're moving the shot at a fixed speed
            // over a quarter second of time forever (code in the Update method will remove the shot once it's off the screen).
            var moveAction = new CCRepeatForever(new CCMoveBy(0.25f, new CCPoint(0, ShotSpeed)));
            bullet.RunAction(moveAction);

            // Add the bullet to the list of shots for the ship
            _shots.Add(bullet);
        }

        #endregion
    }
}
