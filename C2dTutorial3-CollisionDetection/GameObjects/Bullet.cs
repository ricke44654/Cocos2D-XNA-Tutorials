using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2dTutorial3_CollisionDetection
{
    public class Bullet : GameObject
    {
        public Bullet()
            : base(GameObjectType.ShipBullet, "Images/ShipBullet", "ShipBullet")
        {

        }

        public Bullet(GameObjectType type, string spriteContent, string maskContent)
            : base(type, spriteContent, maskContent)
        {

        }
    }
}
