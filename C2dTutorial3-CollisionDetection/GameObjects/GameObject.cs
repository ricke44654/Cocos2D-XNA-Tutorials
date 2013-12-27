using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// Specifies the type of game object.
    /// </summary>
    public enum GameObjectType
    {
        Ship,
        Enemy,
        ShipBullet,
        EnemyBullet
    }

    /// <summary>
    /// This represents the base class for all objects in the game, which uses a masked sprite as it's foundation. It provides support for
    /// rendering the sprite's bounding box if necessary and loading the sprite mask from embedded resources in the assembly.
    /// </summary>
    public class GameObject : CCMaskedSprite
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of a game object, defaulting to the player's ship.
        /// </summary>
        public GameObject()
        {
            // Default to ship type
            Type = GameObjectType.Ship;
        }

        /// <summary>
        /// Creates an instance of a game object using the specified type and content.
        /// </summary>
        /// <param name="type">The type of game object to create.</param>
        /// <param name="spriteContent">The image content to use for the game object's sprite.</param>
        public GameObject(GameObjectType type, string spriteContent) : this()
        {
            // Set the type of game object
            Type = type;

            // Initialize the sprite with the specified content
            InitWithFile(spriteContent);
        }

        /// <summary>
        /// Creates an instance of a game object using the specified type, content, and mask.
        /// </summary>
        /// <param name="type">The type of game object to create.</param>
        /// <param name="spriteContent">The image content to use for the game object's sprite.</param>
        /// <param name="maskContent">The collision mask to use for the game object's sprite.</param>
        public GameObject(GameObjectType type, string spriteContent, string maskContent) : this(type, spriteContent)
        {
            // Load the collision mask
            CollisionMask = GetCollisionMask(maskContent);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of game object.
        /// </summary>
        public GameObjectType Type { get; set; }

        #endregion

        /// <summary>
        /// Override method to draw the game object on the screen.
        /// </summary>
        public override void Draw()
        {
            // Let the base object do it's thing
            base.Draw();

            // Show the bounding box if necessary
            if (CollisionGame.ShowBoundingBoxes)
            {
                // Start the process of drawing primitives
                CCDrawingPrimitives.Begin();

                // Translate the bounding box coordinates to values based on the position of the sprite
                var trans = ParentToNodeTransform();
                var boundBox = trans.Transform(BoundingBox);

                // Use the Cocos2d-XNA drawing primitives to draw the bounding box around the sprite
                CCDrawingPrimitives.DrawRect(boundBox, new CCColor4B(Microsoft.Xna.Framework.Color.Red));
                
                // We're finished drawing primitives
                CCDrawingPrimitives.End();
            }
        }

        #region Collision Mask Methods

        /// <summary>
        /// Gets a stream containing the data for an embedded resource in the assembly.
        /// </summary>
        /// <param name="name">The name of the embedded resource to load.</param>
        /// <returns></returns>
        private Stream GetEmbeddedResource(string name)
        {
#if !WINRT && !NETFX_CORE
            // Try to get the embedded resource from the root of the assembly first
            Assembly assem = Assembly.GetExecutingAssembly();
            Stream stream = assem.GetManifestResourceStream(name);

            // If we didn't find it at the root, get it from the Masks folder
            if (stream == null) stream = assem.GetManifestResourceStream("C2dTutorial3_CollisionDetection.Content.Masks." + name);

            return stream;
#else
            return null;
#endif
        }

        /// <summary>
        /// Gets a byte array containing the collision mask for the game object's sprite. The collision mask is a text file
        /// containing 0/1 values, with 0 representing transparent areas of the sprite that shouldn't cause a collision.
        /// </summary>
        /// <param name="maskContent">The name of the mask content to retrieve.</param>
        /// <returns></returns>
        private byte[] GetCollisionMask(string maskContent)
        {
            byte[] mask = null;

            // Get a stream containing the collision mask
            Stream stream = GetEmbeddedResource(maskContent + ".mask");

            // Make sure our mask was loaded
            if (stream != null)
            {
                MemoryStream ms = new MemoryStream();
                using (stream)
                {
                    // Setup a stream reader to parse the mask content
                    StreamReader sr = new StreamReader(stream);
                    while (true)
                    {
                        // Read a line from the mask stream
                        string s = sr.ReadLine();

                        // We're finished if we didn't retrieve any more content
                        if (s == null) break;

                        // Ignore empty lines in the content
                        if (s.Length == 0 || s[0] == '#') continue;

                        // Add the appropriate byte value based on the value from the mask
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

            // No collision mask could be loaded
            return null;
        }

        #endregion
    }
}
