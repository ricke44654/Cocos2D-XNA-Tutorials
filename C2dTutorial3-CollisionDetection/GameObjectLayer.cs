using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial3_CollisionDetection
{
    internal class GameObjectLayer : CCLayer
    {
        private CollisionGrid _grid;
        private Ship _ship;
        private List<GameObject> _enemies;
        private InputHelper _input;
        private Random _rand;
        private float _enemyCountdown = 0;

        public GameObjectLayer()
        {
            // Create a new input helper
            _input = new InputHelper();

            // Create a new instance of random number generator
            _rand = new Random();

            // Get a reference to the collision grid and listen for collision events
            _grid = CollisionGame.Grid;
            _grid.Collision += CollisionOccurred;

            // Get the window dimensions
            var winSize = CCDirector.SharedDirector.WinSize;

            // Create the ship and an enemy
            _ship = new Ship();
            _ship.SetPosition(winSize.Width / 2, _ship.Texture.ContentSize.Height / 2);

            // Generate some enemies
            _enemies = new List<GameObject>();
            GenerateEnemies(5);

            // Add them to the sprite layer
            AddChild(_ship);

            // Tell C2D to schedule a call to this layer's Update method
            ScheduleUpdate();
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            // Generate more enemies after they're all gone for a few seconds
            if (!_enemies.Any())
            {
                _enemyCountdown += dt;
                if (_enemyCountdown >= 5)
                {
                    GenerateEnemies(5);
                    _enemyCountdown = 0;
                }
            }

            // Reset the collision grid
            _grid.Initialize();

            // Update the collision grid
            _grid.UpdateLocation(_ship);
            _grid.UpdateLocation(_enemies);
            if (_ship != null) _grid.UpdateLocation(_ship.Shots);

            // Check the collision grid for any collisions
            _grid.CheckCollision(_ship);
            if (_ship != null) _grid.CheckCollision(_ship.Shots);
        }

        /// <summary>
        /// Generate the specified number of enemies in random locations on the screen.
        /// </summary>
        /// <param name="numEnemies">The number of enemies to generate.</param>
        private void GenerateEnemies(int numEnemies)
        {
            for (int i = 1; i <= numEnemies; i++)
            {
                var enemy = new Enemy();
                enemy.SetPosition(_rand.Next(50, 975), _rand.Next(200, 725));
                _enemies.Add(enemy);
                AddChild(enemy);
            }
        }

        private void CollisionOccurred(object sender, CollisionEventArgs e)
        {
            // Handle the collision of each game object
            HandleCollision(e.SourceObject);
            HandleCollision(e.HitObject);
        }

        private void HandleCollision(GameObject gameObject)
        {
            switch (gameObject.Type)
            {
                case GameObjectType.Ship:
                    _ship = null;
                    break;

                case GameObjectType.Enemy:
                    // Remove the enemy from the enemies list
                    _enemies.Remove(gameObject);
                    break;

                case GameObjectType.ShipBullet:
                    // Remove the bullet from the ship's active shot list (if the ship is still active)
                    if (_ship != null) _ship.Shots.Remove(gameObject);
                    break;

                case GameObjectType.EnemyBullet:
                    break;
            }

            // Remove the game object from the game
            gameObject.RemoveFromParent();
        }
    }
}
