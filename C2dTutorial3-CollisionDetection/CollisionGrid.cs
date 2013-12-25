using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    internal class CollisionGrid
    {
        #region Constants

        private const int DefaultSizeX = 100;
        private const int DefaultSizeY = 100;

        #endregion

        #region Variables

        private Dictionary<int, List<GameObject>> _gridBins;
        private Dictionary<GameObject, List<int>> _gameObjectPos;

        private int _gridRows;
        private int _gridColumns;
        private int _gridCellX;
        private int _gridCellY;

        #endregion

        #region Constructors

        public CollisionGrid()
        {
            // Create new instance of grid bins list
            _gridBins = new Dictionary<int, List<GameObject>>();
            _gameObjectPos = new Dictionary<GameObject, List<int>>();

            GetGridCellDimensions(DefaultSizeX, DefaultSizeY);
        }

        public CollisionGrid(int gridCellX, int gridCellY)
        {
            // Create new instance of grid bins list
            _gridBins = new Dictionary<int, List<GameObject>>();
            _gameObjectPos = new Dictionary<GameObject, List<int>>();

            GetGridCellDimensions(gridCellX, gridCellY);
        }

        #endregion

        public event EventHandler<CollisionEventArgs> Collision;

        #region Properties

        public int GridRows
        {
            get { return _gridRows; }
        }

        public int GridColumns
        {
            get { return _gridColumns; }
        }

        public int GridCellX
        {
            get { return _gridCellX; }
        }

        public int GridCellY
        {
            get { return _gridCellY; }
        }

        #endregion

        #region Methods

        private void GetGridCellDimensions(int gridCellX, int gridCellY)
        {
            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Use the specified size of the grid cells as a starting point
            _gridCellX = gridCellX;
            _gridCellY = gridCellY;

            // Determine the number of rows and columns in the collision grid
            _gridColumns = (int)(winSize.Width / _gridCellX);
            _gridRows = (int)(winSize.Height / _gridCellY);

            // Distribute any remainder among the cell width and height
            var remainX = ((int)winSize.Width % DefaultSizeX);
            _gridCellX += (remainX / _gridColumns);
            var remainY = ((int)winSize.Height % DefaultSizeY);
            _gridCellY += (remainY / _gridRows);

            // If the grid isn't aligned perfectly to the screen (which is likely), we need to add an extra column/row to account for
            // all the space on the screen (i.e. the space in part of a grid square)
            if (remainX > 0) _gridColumns += 1;
            if (remainY > 0) _gridRows += 1;
        }

        public void Initialize()
        {
            // Clear the grid bins list
            _gridBins.Clear();
            _gameObjectPos.Clear();
        }

        public void UpdateLocation(GameObject gameObject)
        {
            // We're finished if there's no game object to update (could happen for an object that has had a collision)
            if (gameObject == null) return;

            // Get the grid positions that the game object occupies
            var gridPositions = GetGridPositions(gameObject);

            // Add the game object to each grid position list that it occupies
            foreach (var gridPos in gridPositions)
            {
                // Initialize the list of game objects for the grid position if necessary
                if (!_gridBins.ContainsKey(gridPos)) _gridBins.Add(gridPos, new List<GameObject>());

                // Get the list of game objects associated with the grid position
                var gameObjectList = _gridBins[gridPos];

                // Add the game object to this position's game object list if it's not already in there
                if (!gameObjectList.Contains(gameObject)) gameObjectList.Add(gameObject);
            }

            // Add the list of positions to the game object list
            if (_gameObjectPos.ContainsKey(gameObject)) _gameObjectPos.Remove(gameObject);
            _gameObjectPos.Add(gameObject, gridPositions);
        }

        public void UpdateLocation(List<GameObject> gameObjectList)
        {
            // Add each game object in the list to the appropriate grid position
            foreach (var gameObject in gameObjectList)
                UpdateLocation(gameObject);
        }

        public void CheckCollision(GameObject sourceObject)
        {
            // We're finished if there's no game object to check (could happen for an object that has had a collision)
            if (sourceObject == null) return;

            // Get the grid positions for the specified game object
            var gridPositions = _gameObjectPos[sourceObject];

            // Check each game object list for the grid positions and see if there are any possible collisions
            foreach (var position in gridPositions)
            {
                // Get the list of game objects at this position
                var gameObjectList = _gridBins[position];

                // Only check if there's more than one game object in a grid position
                if (gameObjectList.Count > 1)
                {
                    foreach (var go in gameObjectList)
                    {
                        // Obviously, checking collisions against the same object is right out
                        if (go == sourceObject) continue;

                        // Same type objects don't collide with each other
                        if (go.Type == sourceObject.Type) continue;

                        // Ship and ship bullets can't collide with each other
                        if ((go.Type == GameObjectType.Ship && sourceObject.Type == GameObjectType.ShipBullet) || (go.Type == GameObjectType.ShipBullet && sourceObject.Type == GameObjectType.Ship))
                            continue;

                        // Enemy ships and enemy bullets can't collide with each other
                        if ((go.Type == GameObjectType.Enemy && sourceObject.Type == GameObjectType.EnemyBullet) || (go.Type == GameObjectType.EnemyBullet && sourceObject.Type == GameObjectType.Enemy))
                            continue;

                        // All other collisions are valid, so check for a real collision
                        CCPoint collisionPoint;
                        if (sourceObject.CollidesWith(go, out collisionPoint))
                        {
                            // Let any subscribers know that a collision has occurred
                            if (Collision != null)
                                Collision(this, new CollisionEventArgs(sourceObject, go));
                            return;
                        }
                    }
                }
            }
        }

        public void CheckCollision(List<GameObject> sourceObjectList)
        {
            try
            {
                // Check each game object in the list for a collision
                foreach (var gameObject in sourceObjectList)
                    CheckCollision(gameObject);
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Gets the list of grid positions that a game object occupies (either 1, 2, or 4 different positions).
        /// </summary>
        /// <param name="gameObject">The game object to check.</param>
        /// <returns></returns>
        private List<int> GetGridPositions(GameObject gameObject)
        {
            // Create new list of grid positions
            var positions = new List<int>();

            // Get the bounding rectangle of the game object
            var boundBox = gameObject.BoundingBox;

            // Get the grid position for each corner of the game object's bounding box
            int gridPos = GetGridPosition(boundBox.MinX, boundBox.MinY);
            positions.Add(gridPos);

            gridPos = GetGridPosition(boundBox.MinX, boundBox.MaxY);
            if (!positions.Contains(gridPos)) positions.Add(gridPos);

            gridPos = GetGridPosition(boundBox.MaxX, boundBox.MaxY);
            if (!positions.Contains(gridPos)) positions.Add(gridPos);

            gridPos = GetGridPosition(boundBox.MaxX, boundBox.MinY);
            if (!positions.Contains(gridPos)) positions.Add(gridPos);

            // Return the list of positions
            return positions;
        }

        /// <summary>
        /// Calculates the grid position for the specifid location.
        /// </summary>
        /// <param name="x">The horizontal position on the screen.</param>
        /// <param name="y">The vertical position on the screen.</param>
        /// <returns></returns>
        private int GetGridPosition(float x, float y)
        {
            int xPos = (int)(x / _gridCellX);
            int yPos = (int)(y / _gridCellY);
            return (yPos * _gridColumns) + xPos;
        }

        #endregion
    }

    internal class CollisionEventArgs : EventArgs
    {
        public CollisionEventArgs()
        {
        }

        public CollisionEventArgs(GameObject sourceObject, GameObject hitObject)
        {
            SourceObject = sourceObject;
            HitObject = hitObject;
        }

        public GameObject SourceObject { get; set; }
        public GameObject HitObject { get; set; }
    }
}
