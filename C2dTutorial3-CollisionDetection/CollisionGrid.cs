using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocos2D;

namespace C2dTutorial3_CollisionDetection
{
    /// <summary>
    /// This class implements a grid to help eliminate unnecessary collision detections.
    /// </summary>
    /// <remarks>
    /// This is an implementation based on the excellent 2D collision detection methodology discussed here: http://www.director-online.com/buildArticle.php?id=1114
    /// 
    /// I chose this method based on the game type that I wanted to demonstrate, which is a space shooter style game where I needed pixel-perfect collision detection.
    /// However, even with 2013 technology, it is still quite expensive to check for collisions between all game objects (or even some objects) during each game
    /// update cycle using pixel-based detection (especially true with phones, tablets, etc.) The idea behind this method of detection is to eliminate areas of the 
    /// screen where you don't need to perform detection because one object is not in close proximity to another object. In order to achieve this, it works as follows:
    /// 
    /// * The screen is partitioned into a grid of squares, with each grid square sized roughly to the size of the sprites you're using in your game (NOTE: the  
    ///   implicit assumption with this method is that your sprites are sized similarly; if they are not, then you may need to modify this method or use a 
    ///   completely different method for collisions). You control the grid size with the GridSizeX / GridSizeY constants below. You're looking for a balance where
    ///   the grid square size is between too small (a single object ends up in multiple bins) and too big (multiple objects in one bin). In this implementation,
    ///   the final size of the grid square is tweaked to distribute the squares as evenly as possible across the screen to minimize the size of any partial grid
    ///   squares.
    /// * A list of "bins" is built for the grid, where each game object is put into the corresponding bin based on the game object's grid position. The bins are
    ///   cleared and rebuilt during each game update iteration in the GameObjectLayer.Update method.
    /// * Check the bins where a possible collision could exist (i.e. 2 or more objects exist in that bin). For these bins, perform a pixel-perfect check on those
    ///   objects to see if they collide. Cocos2d-XNA provides the CCMaskedSprite.CollidesWith method for pixel-perect checking, in which it performs a bounding
    ///   box check first and if needed, a pixel check between the sprite masks for an accurate collision. This is performed during each game update iteration in
    ///   the GameObjectLayer.Update method.
    /// </remarks>
    public class CollisionGrid
    {
        #region Constants

        /// <summary>
        /// Controls the horizontal size of a square in the grid.
        /// </summary>
        private const int GridSizeX = 100;

        /// <summary>
        /// Controls the vertical size of a square in the grid.
        /// </summary>
        private const int GridSizeY = 100;

        #endregion

        #region Variables

        private Dictionary<int, List<GameObject>> _gridBins;           // Represents the bins for the collision grid
        private Dictionary<GameObject, List<int>> _gameObjectBins;     // Cross-reference of bins that contain a game object for quick lookup

        private int _gridRows;                                         // The number of rows in the grid
        private int _gridColumns;                                      // The number of columns in the grid
        private int _gridSquareX;                                      // The actual horizontal size of a square in the grid
        private int _gridSquareY;                                      // The actual vertical size of a square in the grid

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the collision grid.
        /// </summary>
        public CollisionGrid()
        {
            // Create new instance of grid bins list
            _gridBins = new Dictionary<int, List<GameObject>>();
            _gameObjectBins = new Dictionary<GameObject, List<int>>();
            
            // Setup the grid dimensions
            SetGridDimensions(GridSizeX, GridSizeY);
        }

        #endregion

        /// <summary>
        /// Fires when a pixel-based collision has occurred between two game objects.
        /// </summary>
        public event EventHandler<CollisionEventArgs> Collision;

        #region Properties

        /// <summary>
        /// Gets the number of rows in the collision grid.
        /// </summary>
        public int GridRows
        {
            get { return _gridRows; }
        }

        /// <summary>
        /// Gets the number of columns in the collision grid.
        /// </summary>
        public int GridColumns
        {
            get { return _gridColumns; }
        }

        /// <summary>
        /// Gets the horizontal size of a square in the collision grid.
        /// </summary>
        public int GridSquareX
        {
            get { return _gridSquareX; }
        }

        /// <summary>
        /// Gets the vertical size of a square in the collision grid.
        /// </summary>
        public int GridSquareY
        {
            get { return _gridSquareY; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the dimensions of the squares and calculates the number of rows and columns in the collision grid.
        /// </summary>
        /// <param name="gridSquareX">The horizontal size of a square in the grid.</param>
        /// <param name="gridSquareY">The vertical size of a square in the grid.</param>
        private void SetGridDimensions(int gridSquareX, int gridSquareY)
        {
            // Get the dimensions of the game window
            var winSize = CCDirector.SharedDirector.WinSize;

            // Use the specified size of the grid squares as a starting point
            _gridSquareX = gridSquareX;
            _gridSquareY = gridSquareY;

            // Determine the number of rows and columns in the collision grid
            _gridColumns = (int)(winSize.Width / _gridSquareX);
            _gridRows = (int)(winSize.Height / _gridSquareY);

            // Distribute any remainder among the square width and height
            var remainX = ((int)winSize.Width % GridSizeX);
            _gridSquareX += (remainX / _gridColumns);
            var remainY = ((int)winSize.Height % GridSizeY);
            _gridSquareY += (remainY / _gridRows);

            // If the grid isn't aligned perfectly to the screen (which is likely), we need to add an extra column/row to account for
            // all the space on the screen (i.e. the space in part of a grid square)
            if (remainX > 0) _gridColumns += 1;
            if (remainY > 0) _gridRows += 1;
        }

        /// <summary>
        /// Initializes the collision grid.
        /// </summary>
        public void Initialize()
        {
            // Clear the list of grid bins
            _gridBins.Clear();

            // Clear the game object bins list
            _gameObjectBins.Clear();
        }

        /// <summary>
        /// Updates the grid position of the specified game object.
        /// </summary>
        /// <param name="gameObject">The game object to update.</param>
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

            // Add the list of positions to the game object bins list
            if (_gameObjectBins.ContainsKey(gameObject)) _gameObjectBins.Remove(gameObject);
            _gameObjectBins.Add(gameObject, gridPositions);
        }

        /// <summary>
        /// Updates the grid positions for the specified list of game objects.
        /// </summary>
        /// <param name="gameObjectList">The list of game objects to update.</param>
        public void UpdateLocation(List<GameObject> gameObjectList)
        {
            // Add each game object in the list to the appropriate grid position
            foreach (var gameObject in gameObjectList)
                UpdateLocation(gameObject);
        }

        /// <summary>
        /// Checks for a collision for the specified game object.
        /// </summary>
        /// <param name="sourceObject">The game object to check.</param>
        public void CheckCollision(GameObject sourceObject)
        {
            // We're finished if there's no game object to check (could happen for an object that has had a collision)
            if (sourceObject == null) return;

            // Get the grid positions for the specified game object
            var gridPositions = _gameObjectBins[sourceObject];

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

                        // Same type objects don't collide with each other (e.g. Enemy with enemy)
                        if (go.Type == sourceObject.Type) continue;

                        // Ship and ship bullets can't collide with each other
                        if ((go.Type == GameObjectType.Ship && sourceObject.Type == GameObjectType.ShipBullet) || (go.Type == GameObjectType.ShipBullet && sourceObject.Type == GameObjectType.Ship))
                            continue;

                        // Enemy ships and enemy bullets can't collide with each other
                        if ((go.Type == GameObjectType.Enemy && sourceObject.Type == GameObjectType.EnemyBullet) || (go.Type == GameObjectType.EnemyBullet && sourceObject.Type == GameObjectType.Enemy))
                            continue;

                        // All other collisions are valid, so check for a real collision (Cocos2d-XNA will give you the point of collision, but I don't
                        // care about it here)
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

        /// <summary>
        /// Checks for collisions for the specified list of game objects.
        /// </summary>
        /// <param name="sourceObjectList">The list of game objects to check.</param>
        public void CheckCollision(List<GameObject> sourceObjectList)
        {
            // Check each game object in the list for a collision. We use a for loop here instead of a foreach as game objects
            // will be removed from the list in the CheckCollision method and you'll get errors thrown in a foreach loop.
            for (int i = 0; i < sourceObjectList.Count; i++)
                CheckCollision(sourceObjectList[i]);
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
            // Bottom left
            int gridPos = GetGridPosition(boundBox.MinX, boundBox.MinY);
            positions.Add(gridPos);

            // Top left
            gridPos = GetGridPosition(boundBox.MinX, boundBox.MaxY);
            if (!positions.Contains(gridPos)) positions.Add(gridPos);

            // Top right
            gridPos = GetGridPosition(boundBox.MaxX, boundBox.MaxY);
            if (!positions.Contains(gridPos)) positions.Add(gridPos);

            // Bottom right
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
            // Convert the positions into grid units
            int xPos = (int)(x / _gridSquareX);
            int yPos = (int)(y / _gridSquareY);

            // The grid position returned starts at the bottom left of the screen (position 0) and continues right and up from there 
            // (e.g. in a 10 x 10 grid, position 10 would be the first column on the second row from the bottom of the screen).
            return (yPos * _gridColumns) + xPos;
        }

        #endregion
    }

    /// <summary>
    /// This class contains information needed by the Collision event.
    /// </summary>
    public class CollisionEventArgs : EventArgs
    {
        #region Constructors

        public CollisionEventArgs()
        {
        }

        public CollisionEventArgs(GameObject sourceObject, GameObject hitObject)
        {
            SourceObject = sourceObject;
            HitObject = hitObject;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the game object that was the source of the collision.
        /// </summary>
        public GameObject SourceObject { get; set; }

        /// <summary>
        /// Gets or sets the game object that was hit by the source object of the collision.
        /// </summary>
        public GameObject HitObject { get; set; }

        #endregion
    }
}