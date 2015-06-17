using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	//GameManager
	public static GameManager instance;

	//CityGrid
	public CityGrid cityGrid;

	//Godzilla reference
	public Godzilla godzilla;

	//All game entities
	public List<IGameEntity> gameEntities = new List<IGameEntity>();

	//Input point for now - used by godzilla to face the mouse
	public Vector3 inputPoint;


	// Use this for initialization
	void Start () {
		GameManager.instance = this;
		cityGrid = GetComponent<CityGrid>();
		//gameEntities = GameObject.FindObjectsOfType<IGameEntity>();
		MonoBehaviour[] monoBehaviors = GameObject.FindObjectsOfType<MonoBehaviour>();
		foreach ( MonoBehaviour monoBehavior in monoBehaviors ) {
			if ( monoBehavior is IGameEntity ) {
				gameEntities.Add(monoBehavior as IGameEntity);
			}
		}
		godzilla = GameObject.FindObjectOfType<Godzilla>();
		gameEntities.Remove(godzilla ); //We want godzilla to update independantly
		godzilla.DoStart();
	}
	
	// Update is called once per frame
	void Update () {
		//Update input:
		DoInputUpdate();

		//Update Godzilla first, then update civilians
		godzilla.DoUpdate();
		for ( int ii = 0; ii < gameEntities.Count; ii++ ) {
			gameEntities[ii].DoUpdate();
		}

	}

	//Update input:
	void DoInputUpdate () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(Vector3.up, Vector3.zero);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance);
		inputPoint = point;
		if ( Input.GetMouseButtonDown(0) ) {
			//If we're too far outside the boundaries of the world, ignore the point:
			Vector3 gridPoint = GameManager.instance.cityGrid.worldPointToGridPoint(point);
			if ( GameManager.instance.cityGrid.isGridPointInGrid(gridPoint) ){
				godzilla.setPath(GameManager.instance.cityGrid.getWorldPath_forGodzilla(godzilla.transform.position, GameManager.instance.cityGrid.gridPointToWorldPoint(gridPoint)));
			}
		}
	}
}
