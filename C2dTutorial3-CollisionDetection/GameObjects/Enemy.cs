using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This game object represents an enemy ship on the screen.  It moves horizontally across the screen, firing bullets at
    /// the player's ship.
    /// </summary>
    public class Enemy : GameObject
    {
        #region Constants

        /// <summary>
        /// The maximum number of shots that the enemy can have active at a given time.
        /// </summary>
        private const int MaximumShots = 2;

        /// <summary>
        /// The speed of the enemy's shots on the screen.
        /// </summary>
        private const int ShotSpeed = 100;

        #endregion

        #region Variables

        private CCSize _winSize;               // Contains the dimensions of the game window
        private List<GameObject> _shots;       // The list of active shots for the enemy
        private int _speed;                    // The speed of the enemy ship
        private float _nextShot;               // Contains the number of seconds to wait until firing a shot
        private float _nextShotTimer;          // Keeps an increment of game time used to determine when the next shot should be fired

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of an enemy using the Enemy content and mask.
        /// </summary>
        public Enemy() : base(GameObjectType.Enemy, "Images/Enemy", "Enemy")
        {
            // Create a new instance of the shots list
            _shots = new List<GameObject>();

            // Get the dimensions of the window
            _winSize = CCDirector.SharedDirector.WinSize;

            // Set the speed of the enemy
            _speed = CollisionGame.Rand.Next(2, 7);

            // Get the next time that a shot will be fired
            _nextShot = CollisionGame.Rand.Next(2, 5);

            // Tell Cocos2d-XNA to schedule a call to this sprite's Update method
            ScheduleUpdate();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of active shots for the enemy.
        /// </summary>
        public List<GameObject> Shots
        {
            get { return _shots; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override method to updates the enemy's state for each game loop iteration.
        /// </summary>
        /// <param name="dt">The amount of time that has passed since the last call to the Update method.</param>
        public override void Update(float dt)
        {
            // Let the base object do it's thing
            base.Update(dt);

            // Make sure enemy movement is enabled
            if (CollisionGame.MoveEnemies)
            {
                // Move the enemy ship horizontally at the specified speed
                PositionX += _speed;

                // Keep the enemy on the screen and reverse it's direction if necessary
                if (PositionX < TextureRect.MidX)
                {
                    PositionX = TextureRect.MidX;
                    _speed *= -1;
                }
                else if (PositionX > _winSize.Width - TextureRect.MidX)
                {
                    PositionX = _winSize.Width - TextureRect.MidX;
                    _speed *= -1;
                }
            }

            // Remove any shots from the screen and shot list that have moved past the bottom of the screen
            for (int i = 0; i < _shots.Count; i++)
            {
                var shot = _shots[i];
                if (shot.PositionY < 0)
                {
                    // Detach the collision event for this shot
                    ((Bullet)shot).BulletCollision -= bullet_BulletCollision;

                    shot.RemoveFromParent();
                    _shots.Remove(shot);
                }
            }

            // Update the shot timer and see if it's time to shoot
            _nextShotTimer += dt;
            if (_nextShotTimer >= _nextShot)
            {
                // Fire a shot
                FireShot();

                // Reset the time to fire the next shot
                _nextShot = CollisionGame.Rand.Next(2, 5);

                // Reset the shot timer
                _nextShotTimer = 0;
            }
        }

        /// <summary>
        /// Fires a shot for the enemy.
        /// </summary>
        private void FireShot()
        {
            // Limit the number of shots that can be active at once
            if (_shots.Count >= MaximumShots) return;

            // Don't fire when enemy shots aren't moving on the screen
            if (!CollisionGame.MoveBullets) return;

            // Create a new bullet at the enemy's position
            var bullet = new Bullet(GameObjectType.EnemyBullet);
            bullet.SetPosition(PositionX, PositionY - TextureRect.MidY);

            // Hook up to the collision event for the bullet so we can remove it later
            bullet.BulletCollision += bullet_BulletCollision;
            
            // Add the shot to the enemy's parent container (i.e. the GameObjectLayer). This is a handy bit of Cocos2d-XNA architecture
            // so you don't have to maintain references in your code to get game objects on the screen... booyah.
            Parent.AddChild(bullet);

            // Create an action to move the bullet to the bottom of the screen. Basically, we're moving the shot at a fixed speed
            // over a quarter second of time forever (code in the Update method will remove the shot once it's off the screen).
            var moveAction = new CCRepeatForever(new CCMoveBy(0.25f, new CCPoint(0, -ShotSpeed)));
            bullet.RunAction(moveAction);

            // Add the bullet to the list of shots for the ship
            _shots.Add(bullet);
        }

        /// <summary>
        /// An event method that handles removing the bullet from the enemy's shot list.
        /// </summary>
        /// <param name="sender">The bullet instance that had the collision.</param>
        /// <param name="e">Any parameters needed for the event.</param>
        private void bullet_BulletCollision(object sender, EventArgs e)
        {
            // Get the bullet instance that had the collision
            var bullet = (Bullet) sender;

            // Detach the event listener
            bullet.BulletCollision -= bullet_BulletCollision;

            // Remove the bullet from the shots list
            _shots.Remove(bullet);
        }

        #endregion
    }
}
