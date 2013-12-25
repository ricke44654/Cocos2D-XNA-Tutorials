using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial3_CollisionDetection
{
    internal class Ship : GameObject
    {
        private const float ShipMovementFactor = 1;
        private const float ShotMovementFactor = 100;
        private const int NumberOfShots = 4;

        private List<GameObject> _shots;

        public Ship()
            : base(GameObjectType.Ship, "Images/Ship", "Ship")
        {
            // Create list of shots fired from the ship
            _shots = new List<GameObject>();

            // Tell C2D to schedule a call to this sprite's Update method
            ScheduleUpdate();
        }

        public List<GameObject> Shots
        {
            get { return _shots; }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Get the window dimensions
            var winSize = CCDirector.SharedDirector.WinSize;

            // Get a reference to the input helper
            var input = CollisionGame.Input;

            if (input.IsCurPress(Keys.Right))
                PositionX += ShipMovementFactor;
            if (input.IsCurPress(Keys.Left))
                PositionX -= ShipMovementFactor;
            if (input.IsCurPress(Keys.Up))
                PositionY += ShipMovementFactor;
            if (input.IsCurPress(Keys.Down))
                PositionY -= ShipMovementFactor;
            if (input.IsNewPress(Keys.LeftControl))
                FireShot();

            // Keep the ship on the screen
            if (PositionX < TextureRect.MidX) PositionX = TextureRect.MidX;
            if (PositionX > winSize.Width - TextureRect.MidX) PositionX = winSize.Width - TextureRect.MidX;
            if (PositionY < TextureRect.MidY) PositionY = TextureRect.MidY;
            if (PositionY > winSize.Height - TextureRect.MidY) PositionY = winSize.Height - TextureRect.MidY;

            for (int i = 0; i < _shots.Count; i++)
            {
                var shot = _shots[i];
                if (shot.PositionY > CCDirector.SharedDirector.WinSize.Height)
                {
                    _shots.Remove(shot);
                    shot.RemoveFromParent();
                }
            }
        }

        private void FireShot()
        {
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
