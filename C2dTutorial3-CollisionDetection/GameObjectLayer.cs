using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;
using Microsoft.Xna.Framework.Input;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// A Cocos2D-XNA layer that manages all the game objects in the game.
    /// </summary>
    public class GameObjectLayer : CCLayer
    {
        #region Variables

        private CollisionGrid _grid;           // Contains a reference to the collision grid
        private Ship _ship;                    // The instance of the player's ship
        private List<GameObject> _enemies;     // The list of enemies on the screen
        private float _shipSpawnTimer;         // Keeps an increment of game time that determines when to spawn a new ship for the player if it was destroyed
        private float _enemySpawnTimer;        // Keeps an increment of game time that determines when to create new enemies if they're all destroyed
        private bool _prevMoveBullets = true;  // Keeps track of the previous state of the move bullet flag

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the game object layer.
        /// </summary>
        public GameObjectLayer()
        {
            // Get a reference to the collision grid and listen for collision events
            _grid = CollisionGame.Grid;
            _grid.Collision += CollisionOccurred;

            // Spawn a new instance of the player's ship
            SpawnShip();

            // Create the enemies list and generate some new enemies
            _enemies = new List<GameObject>();
            SpawnEnemies(5);

            // Tell Cocos2d-XNA to schedule a call to this layer's Update method
            ScheduleUpdate();
        }

        #endregion

        /// <summary>
        /// Override method that updates the state of the collision grid and spawns the player's ship and enemies if necessary.
        /// </summary>
        /// <param name="dt">The amount of time that has passed since the last call to the Update method.</param>
        public override void Update(float dt)
        {
            // Let the base object do it's thing
            base.Update(dt);

            // See if the player's ship has been destroyed
            if (_ship == null)
            {
                // Generate a new ship after it's gone for a few seconds
                _shipSpawnTimer += dt;
                if (_shipSpawnTimer >= 3)
                {
                    SpawnShip();
                    _shipSpawnTimer = 0;
                }
            }

            // See if all the enemies have been destroyed
            if (!_enemies.Any())
            {
                // Generate more enemies after they're all gone for a few seconds
                _enemySpawnTimer += dt;
                if (_enemySpawnTimer >= 3)
                {
                    SpawnEnemies(5);
                    _enemySpawnTimer = 0;
                }
            }

            // See if the enemy move bullets state has changed
            if (CollisionGame.MoveBullets != _prevMoveBullets)
            {
                // Reset the previous move bullets state
                _prevMoveBullets = CollisionGame.MoveBullets;

                // See if bullets should be moving on the screen
                if (CollisionGame.MoveBullets)
                {
                    // Loop through all enemies and resume their bullet movement actions
                    foreach (var gameObject in _enemies)
                    {
                        var enemy = (Enemy)gameObject;
                        foreach (var shot in enemy.Shots)
                            ActionManager.ResumeTarget(shot);
                    }
                }
                else
                {
                    // Loop throught all enemies and pause their bullet movement actions
                    foreach (var gameObject in _enemies)
                    {
                        var enemy = (Enemy)gameObject;
                        foreach (var shot in enemy.Shots)
                            ActionManager.PauseTarget(shot);
                    }
                }
            }

            // Initialize the collision grid
            _grid.Initialize();

            // Update the collision grid for the player's ship and shots
            _grid.UpdateLocation(_ship);
            if (_ship != null) _grid.UpdateLocation(_ship.Shots);

            // Update the collision grid for the enemies and their shots
            _grid.UpdateLocation(_enemies);
            foreach (var enemy in _enemies)
                _grid.UpdateLocation(((Enemy) enemy).Shots);

            // Check the collision grid for any collisions. We only need to check for collisions on the player's ship and shots;
            // everything else would be redundant or worthless (e.g. enemy with enemy, enemy with enemy bullet) checks.
            _grid.CheckCollision(_ship);
            if (_ship != null) _grid.CheckCollision(_ship.Shots);
        }

        /// <summary>
        /// Spawns a new instance of the player's ship.
        /// </summary>
        private void SpawnShip()
        {
            // Get the window dimensions
            var winSize = CCDirector.SharedDirector.WinSize;

            // Create the ship and place it in the bottom middle of the screen
            _ship = new Ship();
            _ship.SetPosition(winSize.Width / 2, _ship.TextureRect.MidY);

            // Add the ship to the game object layer
            AddChild(_ship);
        }

        /// <summary>
        /// Spawns the specified number of enemies in random locations on the screen.
        /// </summary>
        /// <param name="numEnemies">The number of enemies to spawn.</param>
        private void SpawnEnemies(int numEnemies)
        {
            // Create the specified number of enemies
            for (int i = 1; i <= numEnemies; i++)
            {
                // Create the enemy and place it at a random location on the screen
                var enemy = new Enemy();
                enemy.SetPosition(CollisionGame.Rand.Next(50, 975), CollisionGame.Rand.Next(200, 725));

                // Add the enemy to the enemies list
                _enemies.Add(enemy);

                // Add the enemy to the game object layer
                AddChild(enemy);
            }
        }

        /// <summary>
        /// An event method that is fired when the collision grid has detected a valid pixel-based collision.
        /// </summary>
        /// <param name="sender">The collision grid object.</param>
        /// <param name="e">The event arguments for the collision.</param>
        private void CollisionOccurred(object sender, CollisionEventArgs e)
        {
            // Handle the collision of each game object involved in the collision
            HandleCollision(e.SourceObject);
            HandleCollision(e.HitObject);
        }

        /// <summary>
        /// Performs the appropriate action for a collision based on the type of game object.
        /// </summary>
        /// <param name="gameObject">The game object involved in a collision.</param>
        private void HandleCollision(GameObject gameObject)
        {
            // Take action based on the type of game object
            switch (gameObject.Type)
            {
                case GameObjectType.Ship:
                    // Destroy the player's ship, a new instance will be created in the Update method
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
                    // Remove the bullet from the enemy's active shot list. We do this with an event since we don't know which
                    // enemy fired this bullet and we don't want to look it up.
                    var bullet = (Bullet)gameObject;
                    bullet.FireBulletCollision();
                    break;
            }

            // Remove the game object from the game object layer
            gameObject.RemoveFromParent();
        }
    }
}
