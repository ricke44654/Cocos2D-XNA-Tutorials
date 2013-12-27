using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This game object represents a bullet that has been fired from the player's ship or an enemy ship.
    /// </summary>
    public class Bullet : GameObject
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of a player's bullet, using the Ship Bullet content and mask.
        /// </summary>
        public Bullet() : base(GameObjectType.ShipBullet, "Images/ShipBullet", "ShipBullet")
        {
        }

        /// <summary>
        /// Creates a new instance of a bullet of the specified type, using the Ship Bullet content and mask.
        /// </summary>
        /// <param name="type">The type of bullet to create.</param>
        public Bullet(GameObjectType type) : base(type, "Images/ShipBullet", "ShipBullet")
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when the bullet has collided with another game object that it should destroy.
        /// </summary>
        public event EventHandler BulletCollision;

        #endregion

        #region Methods

        /// <summary>
        /// Raises the BulletCollision event after a collision has occurred.
        /// </summary>
        public void FireBulletCollision()
        {
            // Let any subscribers know that this bullet has collided with another object
            if (BulletCollision != null)
                BulletCollision(this, EventArgs.Empty);
        }

        #endregion
    }
}
