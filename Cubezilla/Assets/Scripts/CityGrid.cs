using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGrid : MonoBehaviour {

	/*
	//--- City variables ---//
	//City generation: min/max horizontal dimension of city. Size scales to city population.
	int GAME_CITYMINWIDTH = 33;
	int GAME_CITYMAXWIDTH = 45;

	//City generation: min/max vertical dimension of city. Size scales to city population.
	int GAME_CITYMINHEIGHT = 26;
	int GAME_CITYMAXHEIGHT = 30;

	//Building generation: min/max size a building can be.
	int MINBUILDINGSIZE = 2;
	int MAXBUILDINGSIZE = 5;
	
	//Building generation: min/max height a building can be.
	int MINBUILDINGHEIGHT = 10;
	int MAXBUILDINGHEIGHT = 100;

	//Buildings: min/max health. Health scales depending on their size.
	int GAME_BLD_MINHEALTH = 10;
	int GAME_BLD_MAXHEALTH = 70;

	//Min/max amount of parks in a city.
	int GAME_CITYPARKS_MIN = 1;
	int GAME_CITYPARKS_MAX = 5;

	//City populations. Scales according to its zombie outbreak levels.
	int CITYPOP_LVL1 = 125;
	int CITYPOP_LVL2 = 250;
	int CITYPOP_LVL3 = 350;
	*/

	
	/*
	//Not ready for this yet
	void GenerateCity ( int width, int height ) {
		cityTiles = new CityTile[width,height];


	}
	*/


	

	//TileMap reference:
	public tk2dTileMap tileMap;

	//City tile enum
	public enum CityTile {
		UNDEFINED = 0,
		ROAD = 1,
		BUILDING = 2,
		BUILDING_DESTROYED = 3,
		PARK = 4,
		WATER = 5
	};
	public CityTile[,] cityTiles;
	public bool[,] canMoveGrid; //Can move grid (not including water)
	public bool[,] canMoveGridWater; //Can-move grid including water

	public void Start () {
		Initialize();
	}

	// Use this for initialization
	public void Initialize () {
		InitializeFromTileMap( GameObject.FindObjectOfType<tk2dTileMap>() );
		debug_outputGrid();
	}
	//Initialize from a tilemap input
	int minTileX;
	int maxTileX;
	int minTileY;
	int maxTileY;
	int width;
	int height;
	public void InitializeFromTileMap ( tk2dTileMap tileMap ) {
		this.tileMap = tileMap;

		minTileX = tileMap.width;
		maxTileX = -1;
		minTileY = tileMap.height;
		maxTileY = -1;
		int tile;
		int ii;
		int jj;
		CityTile cityTile;

		//Determine the width and height of the tilemap's tiles and determine the tiles
		cityTiles = new CityTile[tileMap.width,tileMap.height];
		canMoveGrid = new bool[tileMap.width, tileMap.height]; //Auto-initializes to false
		canMoveGridWater = new bool[tileMap.width, tileMap.height]; //Auto-initializes to false
		for ( ii = 0; ii < tileMap.width; ii++ ) {
			for ( jj = 0; jj < tileMap.height; jj++ ) {
				tile = tileMap.GetTile(ii, jj, 0);
				if ( tile != -1 ) { //Tile exists
					if ( ii < minTileX ) {
						minTileX = ii;
					}
					if ( jj < minTileY ) {
						minTileY = jj;
					}
					if ( ii > maxTileX ) {
						maxTileX = ii;
					}
					if ( jj > maxTileY ) {
						maxTileY = jj;
					}
				}
				cityTile = get_CityTile_By_TileMapId( tile );
				cityTiles[ii,jj] = cityTile;
				canMoveGrid[ii,jj] = canMoveTile( cityTile );
				canMoveGridWater[ii,jj] = canMoveTileWater( cityTile );

			}
		}
		width  = maxTileX - minTileX + 1;
		height = maxTileY - minTileY + 1;

		Debug.Log("W:"+width+" H:"+height);
		Debug.Log("MinX:"+minTileX+" MaxX:"+maxTileX);
		Debug.Log("MinY:"+minTileY+" MaxY:"+maxTileY);

	}
	//Get the city tile by tilemap id
	CityTile get_CityTile_By_TileMapId ( int tile ) {
		switch ( tile ) {
		case 2: return CityTile.BUILDING_DESTROYED;
		case 6: return CityTile.BUILDING;
		case 10: return CityTile.PARK;
		case 14: return CityTile.ROAD;
		case 15: return CityTile.ROAD;
		case 18: return CityTile.ROAD;
		case 11: return CityTile.ROAD;
		case 1: return CityTile.WATER;
		}
		return CityTile.UNDEFINED;
	}
	//Can move:
	bool canMoveTile ( CityTile cityTile ) {
		switch ( cityTile ) {
		case CityTile.BUILDING_DESTROYED : return true;
		case CityTile.BUILDING  : return false;
		case CityTile.PARK  : return true;
		case CityTile.ROAD  : return true;
		case CityTile.WATER : return false;
		}
		return false;
	}
	//Can move (plus water):
	bool canMoveTileWater ( CityTile cityTile ) {
		if ( cityTile == CityTile.WATER ) {
			return false;
		}
		return canMoveTile( cityTile );
	}

	
	//Utility function to trace out the Grid:
	public void debug_outputGrid() {
		string gridString = "";
		int ii;
		int jj;
		CityTile cityTile;
		for ( jj = height-1; jj >= 0; jj-- ) {
			for ( ii = 0; ii < width; ii++ ) {
				cityTile = cityTiles[ii,jj];
				switch (cityTile) {
				case CityTile.BUILDING: gridString += "B";	break;
				case CityTile.PARK: gridString += "P";		break;
				case CityTile.ROAD: gridString += "+";		break;
				case CityTile.WATER: gridString += "w";		break;
				case CityTile.UNDEFINED: gridString += "?";	break;
				}
			}
			gridString += "\n";
		}
		Debug.Log(gridString);
	}









	/**
	 * Translate a World Point into a Grid Point
	 * @param	worldPoint
	 * @return
	 */
	//World Point to Grid Point:
	public Vector2 worldPointToGridPoint ( Vector3 worldPoint ) {
		int x, y;
		tileMap.GetTileAtPosition(worldPoint, out x, out y);
		return new Vector2(x, y);
	}
	/**
	 * Translate a Grid Point into a World Point
	 * @param	gridPoint
	 * @return
	 */
	//World Point to Grid Point:
	public Vector3 gridPointToWorldPoint ( Vector2 gridPoint ) {
		//Assume all world point coordinates' y values are zero:
		Vector3 worldPoint = tileMap.GetTilePosition((int)gridPoint.x, (int)gridPoint.y);
		//Offset by half of a tile's size
		float worldPointOffset = 0.5f;
		worldPoint.x += worldPointOffset;
		worldPoint.z -= worldPointOffset;
		return worldPoint;
	}














	/**
	 * Gets the World Path from one point to another point FOR GODZILLA!
	 * @param	from
	 * @param	to
	 * @return
	 */
	public List<Vector3> getWorldPath_forGodzilla(Vector3 from, Vector3 to) {
		Vector3 worldPointTo = to;
		//Use the closest Grid Point to the inputted point:
		Vector2 gridPointTo = worldPointToGridPoint(worldPointTo);
		if ( !isGridPointAvailable(gridPointTo) ) {
			gridPointTo = getClosestAvailableGridPointTo(worldPointTo);
		}
		//Get the closest real-world point to the to point:
		Vector3 directionToToPoint = to - gridPointToWorldPoint(gridPointTo);
		directionToToPoint.y = 0;
		//The tricky part, not clipping values if the direction we wanted to go in is 
		int dirToToPoint_X_inc = directionToToPoint.x < 0 ? -1 : 1;
		int dirToToPoint_Y_inc = directionToToPoint.z < 0 ? -1 : 1;
		Vector2 gridPoint_inXdir = gridPointTo + new Vector2(dirToToPoint_X_inc, 0f );
		Vector2 gridPoint_inYdir = gridPointTo + new Vector2(0f, dirToToPoint_Y_inc );
		bool isGridPointAvailable_InXdir = isGridPointAvailable(gridPoint_inXdir);
		bool isGridPointAvailable_InYdir = isGridPointAvailable(gridPoint_inYdir);
		
		//Now, set up the close to point to be 
		Vector3 closeToPoint = gridPointToWorldPoint(gridPointTo);
		closeToPoint.y = 0;
		//Godzilla's size is 0.5:
		float godzillaBuffer = 0.25f;
		if (directionToToPoint.x > godzillaBuffer && !isGridPointAvailable_InXdir ) {
			directionToToPoint.x = godzillaBuffer;
		}
		if (directionToToPoint.z > godzillaBuffer && !isGridPointAvailable_InYdir ) {
			directionToToPoint.z = godzillaBuffer;
		}
		if (directionToToPoint.x < -1 * godzillaBuffer && !isGridPointAvailable_InXdir ) {
			directionToToPoint.x = -1 * godzillaBuffer;
		}
		if (directionToToPoint.z < -1 * godzillaBuffer && !isGridPointAvailable_InYdir ) {
			directionToToPoint.z = -1 * godzillaBuffer;
		}
		if (directionToToPoint.x > godzillaBuffer*2 ) {
			directionToToPoint.x = godzillaBuffer*2;
		}
		if (directionToToPoint.z > godzillaBuffer*2 ) {
			directionToToPoint.z = godzillaBuffer * 2;
		}
		if (directionToToPoint.z < -1 * godzillaBuffer * 2 ) {
			directionToToPoint.z = -1 * godzillaBuffer * 2;
		}
		if (directionToToPoint.z < -1 * godzillaBuffer*2 ) {
			directionToToPoint.z = -1 * godzillaBuffer * 2;
		}
		
		closeToPoint += directionToToPoint;
		
		//Regular pathfinding:
		List<Vector2> gridPath = getGridPath( worldPointToGridPoint(from), gridPointTo );
		//gridPath = smoothGridPath(gridPath);
		if ( gridPath.Count == 0 ) {
			return new List<Vector3>();
		}
		//Copy into a world coordinate based path:
		List<Vector3> worldPath = new List<Vector3>(gridPath.Count);
		foreach ( Vector2 gridPoint in gridPath ) {
			worldPath.Add( gridPointToWorldPoint(gridPoint) );
		}
		
		//Add the close to point, and rejigger to get rid of additional points:
		//Smooth last part of path:
		if ( worldPath.Count > 1 ) {
			Vector3 worldPathPoint_last = worldPath[worldPath.Count - 1];
			Vector3 worldPathPoint_secondLast = worldPath[worldPath.Count - 2];
			
			float distanceTo_LastPathPoint = (worldPathPoint_secondLast - worldPathPoint_last).magnitude;
			float distanceTo_ClosePoint    = (worldPathPoint_secondLast - closeToPoint).magnitude;
			if (distanceTo_LastPathPoint > distanceTo_ClosePoint) {
				worldPath.RemoveAt(worldPath.Count-1);//Remove the last element because it's too far
			}
		}
		//Smooth first part of path:
		//Add our from point to the path:
		worldPath.Insert(0, from);
		if ( worldPath.Count > 2 ) { //BASICALLY DO WE NEED SECOND???????
			Vector3 worldPathPoint_first  = worldPath[0];
			Vector3 worldPathPoint_second = worldPath[1];
			Vector3 worldPathPoint_third = worldPath[2];
			//Distances to Third PathPoint:
			float distanceTo_first_PathPoint  = (worldPathPoint_third - worldPathPoint_first).magnitude;
			float distanceTo_second_PathPoint = (worldPathPoint_third - worldPathPoint_second).magnitude;
			if (distanceTo_first_PathPoint < distanceTo_second_PathPoint) {
				worldPath.RemoveAt(1);//Remove the middle element because it jogs
				//Debug.Log("Remove!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			}
		}
		if ( worldPath.Count == 1 ) {
			//Just add the closetopoint, because there's no point in adding any others
			worldPath.RemoveAt(worldPath.Count-1);
		}
		//Always add the closest point
		//worldPath.Add(closeToPoint);
		return worldPath;
	}
	/**
	 * Gets the World Path from one point to another point
	 * @param	from
	 * @param	to
	 * @return
	 */
	public List<Vector3> getWorldPath(Vector3 from, Vector3 to) {
		Vector3 worldPointTo = to;
		//Use the closest Grid Point to the inputted point:
		Vector2 gridPointTo = worldPointToGridPoint(worldPointTo);
		if ( !isGridPointAvailable(gridPointTo) ) {
			gridPointTo = getClosestAvailableGridPointTo(worldPointTo);
		}
		List<Vector2> gridPath = getGridPath( worldPointToGridPoint(from), gridPointTo );
		//var gridPath : Array = getGridPath( worldPointToGridPoint(from), worldPointToGridPoint(worldPointTo) );
		//Copy into a world coordinate based path:
		List<Vector3> worldPath = new List<Vector3>(gridPath.Count);
		for ( int ii = 0; ii < gridPath.Count; ii++ ) {
			worldPath[ii] = gridPointToWorldPoint(gridPath[ii]);
		}
		//Add the destination point
		return worldPath;
	}
	
	//Find the closest available grid point to a particular world point:
	Vector2 getClosestAvailableGridPointTo(Vector3 worldPoint) {
		float closestDistanceToWorldPoint = Mathf.Infinity;
		float distanceToWorldPoint;
		Vector3 otherWorldPoint;
		Vector2 otherGridPoint;
		Vector2 closestGridPoint = nullVector2();
		int ii;
		int jj;
		for ( jj = height-1; jj >= 0; jj-- ) {
			for ( ii = 0; ii < width; ii++ ) {
				if ( canMoveGrid[ii, jj] ) {
					otherGridPoint = new Vector2(ii, jj);
					otherWorldPoint = gridPointToWorldPoint(otherGridPoint);
					distanceToWorldPoint = (worldPoint - otherWorldPoint).magnitude;
					if ( distanceToWorldPoint < closestDistanceToWorldPoint ) {
						closestDistanceToWorldPoint = distanceToWorldPoint;
						closestGridPoint = otherGridPoint;
					}
				}
			}
		}
		return closestGridPoint;
	}























	// ===== getGridPath - A* Pathfinding =====
	
	/**
	 * Gets the Grid Path from one gridPoint to another gridPoint
	 * @param	from
	 * @param	to
	 * @return
	 */
	public List<Vector2> getGridPath(Vector2 from, Vector2 to) {
		//Grid variables: (we'll be using these a lot)
		int ii;
		int jj;
		
		//A* movement:
		List<Vector2> closedSet = new List<Vector2>();//The set of nodes already evaluated
		List<Vector2> openSet = new List<Vector2>();
		openSet.Add(from);					//initially containing the first node
		Vector2[,] cameFrom = new Vector2[width, height]; //The map of navigated nodes.
		
		float[,] g_score = new float[width, height]; //Cost from start along best known path
		float[,] f_score = new float[width, height]; //Estimated total cost from gridX to goal
		
		//Initialize grids:
		for ( ii = 0; ii < width; ii++ ) {
			for ( jj = 0; jj < height; jj++ ) {
				cameFrom[ii, jj] = nullVector2();
				g_score[ii, jj] = 0;
				f_score[ii, jj] = 0;
			}
		}
		
		f_score[(int)from.x, (int)from.y] = heuristic_cost_estimate(from, to); //Heuristic cost estimate
		
		//Main loop:
		Vector2 currentPoint;
		while ( openSet.Count > 0 )
		{
			currentPoint = get_node_w_lowest_f_score_in_openSet(openSet, f_score);
			if (currentPoint == to) {
				return reconstruct_gridPath(cameFrom, to);
			}
			
			openSet.RemoveAt( indexOfPointInArray(openSet,currentPoint) ); //Remove current from openset
			closedSet.Add(currentPoint); //Add current to closed set
			float tentative_g_score;
			foreach ( Vector2 neighbor in getAdjacentGridPointsTo(currentPoint) )
			{
				if ( isPointInArray( closedSet, neighbor) ) {
					continue;
				}
				tentative_g_score = g_score[(int)currentPoint.x,(int)currentPoint.y] + heuristic_cost_estimate(currentPoint, neighbor);
				
				//if neighbor not in openset or tentative_g_score <= g_score[neighbor] 
				if ( !isPointInArray(openSet, neighbor) || tentative_g_score <= g_score[(int)neighbor.x,(int)neighbor.y] ) {
					cameFrom[(int)neighbor.x,(int)neighbor.y] = currentPoint;
					g_score[(int)neighbor.x,(int)neighbor.y] = tentative_g_score;
					f_score[(int)neighbor.x,(int)neighbor.y] = g_score[(int)neighbor.x,(int)neighbor.y] + heuristic_cost_estimate(neighbor,to);
					if ( !isPointInArray(openSet,neighbor) ) {
						openSet.Add(neighbor);
					}
				}
			}//End foreach neighbor
		}//End while loop
		return new List<Vector2>(); //Could not find path
	}
	
	// ===== Utility functions used by getGridPath =====
	
	//Heuristic cost estimate between two points:
	float heuristic_cost_estimate( Vector2 estimatePoint, Vector2 toPoint ) {
		return (estimatePoint - toPoint).magnitude;
		//return getManhattenDistanceBetween(estimatePoint, toPoint);
	}
	
	//Substitute for a null Vector2 (since gridPoints will never be below 0)
	Vector2 nullVector2() {
		return new Vector2( -1, -1);
	}
	
	//Reconstruct path
	List<Vector2> reconstruct_gridPath(Vector2[,] cameFrom, Vector2 goal)
	{
		Vector2 cameFromPoint = cameFrom[(int)goal.x,(int)goal.y];
		List<Vector2> pathArray = new List<Vector2>();
		if ( cameFromPoint != nullVector2() ) {
			pathArray = reconstruct_gridPath(cameFrom, cameFromPoint);
			pathArray.Add( goal );
			return pathArray;
		} else {
			pathArray = new List<Vector2>();
			pathArray.Add( goal );
			return pathArray;
		}
		return new List<Vector2>(); //Reconstruct path
	}
	
	
	//Get manhatten distance between two points
	float getManhattenDistanceBetween(Vector2 from, Vector2 to)
	{
		return Mathf.Abs(from.x - to.x ) + Mathf.Abs(from.y - to.y );
	}
	
	//Utility function to get the node with the lowest f score
	Vector2 get_node_w_lowest_f_score_in_openSet(List<Vector2> openSet, float[,] f_score) {
		float current_f_score = Mathf.Infinity;
		Vector2 currentPoint = nullVector2();
		foreach ( Vector2 point in openSet ) {
			if ( f_score[(int)point.x,(int)point.y] < current_f_score ) {
				current_f_score = f_score[(int)point.x,(int)point.y];
				currentPoint = point;
			}
		}
		return currentPoint;
	}
	
	//Utility function to get a list of the adjacent points:
	List<Vector2> getAdjacentGridPointsTo(Vector2 centerPoint) {
		List<Vector2> adjacentPoints = new List<Vector2>();
		//Orthogonal directions
		Vector2 point_xm = new Vector2(centerPoint.x-1, centerPoint.y);
		Vector2 point_xp = new Vector2(centerPoint.x+1, centerPoint.y);
		Vector2 point_ym = new Vector2(centerPoint.x, centerPoint.y-1);
		Vector2 point_yp = new Vector2(centerPoint.x, centerPoint.y + 1);
		if ( isGridPointAvailable ( point_xm ) ) {
			adjacentPoints.Add(point_xm);
		}
		if ( isGridPointAvailable ( point_xp ) ) {
			adjacentPoints.Add(point_xp);
		}
		if ( isGridPointAvailable ( point_ym ) ) {
			adjacentPoints.Add(point_ym);
		}
		if ( isGridPointAvailable ( point_yp ) ) {
			adjacentPoints.Add(point_yp);
		}
		//Diagonal directions are only available if both points along the diagonal are also available
		Vector2 point_mm = new Vector2(centerPoint.x-1, centerPoint.y-1);
		Vector2 point_mp = new Vector2(centerPoint.x-1, centerPoint.y+1);
		Vector2 point_pm = new Vector2(centerPoint.x+1, centerPoint.y-1);
		Vector2 point_pp = new Vector2(centerPoint.x+1, centerPoint.y+1);
		if ( isGridPointAvailable ( point_mm ) && isGridPointAvailable ( point_xm ) && isGridPointAvailable ( point_ym ) ) {
			adjacentPoints.Add(point_mm);
		}
		if ( isGridPointAvailable ( point_mp ) && isGridPointAvailable ( point_xm ) && isGridPointAvailable ( point_yp ) ) {
			adjacentPoints.Add(point_mp);
		}
		if ( isGridPointAvailable ( point_pm ) && isGridPointAvailable ( point_xp ) && isGridPointAvailable ( point_ym ) ) {
			adjacentPoints.Add(point_pm);
		}
		if ( isGridPointAvailable ( point_pp ) && isGridPointAvailable ( point_xp ) && isGridPointAvailable ( point_yp ) ) {
			adjacentPoints.Add(point_pp);
		}
		return adjacentPoints;
	}
	
	//Utility function to get a point's index in an array
	int indexOfPointInArray( List<Vector2> arr, Vector2 searchPoint ) {
		Vector2 arrPoint;
		for (int index = 0; index < arr.Count; index++) {
			arrPoint = arr[index];
			if ( searchPoint == arrPoint ) {
				return index;
			}
		}
		return -1;
	}
	
	//Utility function to see if a point is in an array
	bool isPointInArray( List<Vector2> arr, Vector2 comparisonPoint ) {
		foreach (Vector2 point in arr) {
			if ( point == comparisonPoint) {
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Is GridPoint available?
	 * @return
	 */
	public bool isGridPointAvailable(Vector2 gridPoint) {

		if ( isGridPointInGrid(gridPoint) ) {
			return canMoveGrid[(int)gridPoint.x, (int)gridPoint.y];
		}
		return false;
	}
	
	/**
	 * Is GridPoint inside the grid?
	 * @param	gridPoint
	 */
	public bool isGridPointInGrid(Vector2 gridPoint) {
		if (gridPoint.x <= minTileX ||
		    gridPoint.y <= minTileY ||
		    gridPoint.x > maxTileX ||
		    gridPoint.y > maxTileY 
		    ) {
			return false;
		}
		return true;
	}


	//Test if a point can be moved using world point
	public bool canMoveWorldPoint(Vector3 point) {
		Vector2 gridPoint = worldPointToGridPoint(point);
		if ( !isGridPointInGrid(gridPoint) ) {
			return false;
		}
		return canMoveGrid[(int)gridPoint.x,(int)gridPoint.y];
	}















}




