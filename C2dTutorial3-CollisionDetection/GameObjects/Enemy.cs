using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    public class Enemy : GameObject
    {
        private const int NumberOfShots = 2;
        private const int ShotMovementFactor = 100;

        private CCSize _winSize;
        private List<GameObject> _shots;
        private int _speed;
        private float _nextShot;
        private float _shotTimer;

        public Enemy()
            : base(GameObjectType.Enemy, "Images/Enemy", "Enemy")
        {
            // Create a new instance of the shots list
            _shots = new List<GameObject>();

            // Get the dimensions of the window
            _winSize = CCDirector.SharedDirector.WinSize;

            // Set the speed of the enemy
            _speed = CollisionGame.Rand.Next(2, 7);

            // Get the next time that a shot will be fired
            _nextShot = CollisionGame.Rand.Next(2, 5);

            ScheduleUpdate();
        }

        public override void Update(float dt)
        {
            base.Update(dt);

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

            // Update the shot counter and see if it's time to shoot
            _shotTimer += dt;
            if (_shotTimer >= _nextShot)
            {
                // Fire a shot
                FireShot();

                // Reset the time to fire the next shot
                _nextShot = CollisionGame.Rand.Next(2, 5);
            }
        }

        private void FireShot()
        {
            // HACK: Quit for now, not ready for prime-time yet
            return;

            // Limit the number of shots that can be active at once
            if (_shots.Count >= NumberOfShots)
                return;

            // Get the window dimensions
            var winSize = CCDirector.SharedDirector.WinSize;

            // Create a new bullet at the ship's position
            var bullet = new Bullet();
            bullet.SetPosition(PositionX, PositionY + ContentSize.Height / 2);
            this.Parent.AddChild(bullet);

            // Create an action to move the bullet to the top of the screen
            var moveAction = new CCRepeatForever(new CCMoveBy(0.25f, new CCPoint(0, ShotMovementFactor)));
            bullet.RunAction(moveAction);

            // Add the bullet to the list of shots for the ship
            _shots.Add(bullet);
        }

    }
}
