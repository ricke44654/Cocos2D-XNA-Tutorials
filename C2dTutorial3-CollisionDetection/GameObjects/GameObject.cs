using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    public enum GameObjectType
    {
        Ship,
        Enemy,
        ShipBullet,
        EnemyBullet
    }

    public class GameObject : CCMaskedSprite
    {
        private CollisionGrid _grid;

        #region Constructors

        public GameObject()
        {
            // Default to ship type
            Type = GameObjectType.Ship;

            // Keep a reference to the collision grid
            _grid = CollisionGame.Grid;
        }

        public GameObject(GameObjectType type, string spriteContent)
            : this()
        {
            // Set the type of game object
            Type = type;

            // Initialize the sprite with the specified content
            InitWithFile(spriteContent);
        }

        public GameObject(GameObjectType type, string spriteContent, string maskContent)
            : this(type, spriteContent)
        {
            // Load the collision mask
            CollisionMask = GetCollisionMask(maskContent);
        }

        #endregion

        #region Properties

        public GameObjectType Type { get; set; }

        #endregion

        public override void Draw()
        {
            base.Draw();

            // Show the bounding box if necessary
            if (CollisionGame.ShowBoundingBoxes)
            {
                // Draw a box around the sprite's bounding box
                CCDrawingPrimitives.Begin();
                var trans = ParentToNodeTransform();
                var bb = trans.Transform(BoundingBox);
                CCDrawingPrimitives.DrawRect(bb, new CCColor4B(Microsoft.Xna.Framework.Color.Red));
                CCDrawingPrimitives.End();
            }
        }

        #region Collision Mask Methods

        private Stream GetEmbeddedResource(string name)
        {
#if !WINRT && !NETFX_CORE
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream(name);
            if (stream == null)
                stream = assem.GetManifestResourceStream("C2dTutorial3_CollisionDetection.Content.Masks." + name);

            return stream;
#else
            return null;
#endif
        }

        private byte[] GetCollisionMask(string maskContent)
        {
            byte[] mask = null;
            Stream stream = GetEmbeddedResource(maskContent + ".mask");

            if (stream != null)
            {
                MemoryStream ms = new MemoryStream();
                using (stream)
                {
                    StreamReader sr = new StreamReader(stream);
                    while (true)
                    {
                        string s = sr.ReadLine();
                        if (s == null)
                            break;

                        if (s.Length == 0 || s[0] == '#')
                            continue;

                        for (int i = 0; i < s.Length; i++)
                        {
                            if (s[i] == '1')
                                ms.WriteByte(1);
                            else
                                ms.WriteByte(0);
                        }
                    }
                }

                return ms.ToArray();
            }

            return null;
        }

        #endregion
    }
}
