using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Godzilla : MonoBehaviour, IGameEntity {

	private List<Vector3> path = new List<Vector3>();

	public float speed = 3f;

	bool isJumping = false;

	public LineRenderer lineRenderer;

	public bool pathInput = false;

	//Quick references:
	private CityGrid cityGrid;

	private float cubeSize = 0.4f;

	public ParticleSystem flamePS;
	public Collider flameCollider;
	public GameObject flameGO;

	public void DoStart ()
	{ 
		cityGrid = GameManager.instance.cityGrid;
	}
	public void DoFixedUpdate ()
	{ }
	public void DoUpdate ()
	{
		if ( pathInput ) {
			if ( path.Count > 0 ) {
				//Follow the path
				followPath();
				//drawPath
			} else {
				//Face the mouse
				LookAtPoint( GameManager.instance.inputPoint );
			}
		} else {
			//Do Movement based update:
			float inputX = Input.GetAxis("Vertical");
			float inputZ = Input.GetAxis("Horizontal");

			//Vector3 inputVector = new Vector3(inputX, 0, inputZ); //Rotated 90 degrees CW
			Vector3 inputVector = new Vector3(inputZ, 0, inputX); //Rotated 90 degrees CW

			//Speed hack
			if ( Input.GetButton("Fire1") ) {
				speed = 6f;
			} else {
				speed = 3f;
			}

			//Flame hack
			CheckFlame(inputVector);

			//Collision avoidance:
			if ( true ) {
				inputVector *= speed*Time.deltaTime;

				float inputMagnitude = inputVector.magnitude;

				transform.LookAt (transform.position + inputVector);

				Vector3 globalFL = transform.TransformPoint(0.4f, 0, 0.4f);
				Vector3 globalFR = transform.TransformPoint(-0.4f, 0, 0.4f);
				Vector3 globalBL = transform.TransformPoint(-0.4f, 0, -0.4f);
				Vector3 globalBR = transform.TransformPoint(0.4f, 0, -0.4f);
				
				bool hitFL = !cityGrid.canMoveWorldPoint(globalFL);
				bool hitFR = !cityGrid.canMoveWorldPoint(globalFR);
				bool hitBL = !cityGrid.canMoveWorldPoint(globalBL);
				bool hitBR = !cityGrid.canMoveWorldPoint(globalBR);

				Vector3 globalF = transform.TransformPoint(0f, 0f, 0.4f);
				Vector3 globalB = transform.TransformPoint(0f, 0f, -0.4f);
				Vector3 globalR = transform.TransformPoint(0.4f, 0f, 0f);
				Vector3 globalL = transform.TransformPoint(-0.4f, 0f, 0f);
				
				bool hitF = !cityGrid.canMoveWorldPoint(globalF);
				bool hitB = !cityGrid.canMoveWorldPoint(globalB);
				bool hitR = !cityGrid.canMoveWorldPoint(globalR);
				bool hitL = !cityGrid.canMoveWorldPoint(globalL);



				bool moreMovingX = false;
				bool moreMovingZ = false;
				if ( Mathf.Abs(inputVector.x) > Mathf.Abs(inputVector.z) ) {
					moreMovingX = true;
				}
				if ( Mathf.Abs(inputVector.x) < Mathf.Abs(inputVector.z) ) {
					moreMovingZ = true;
				}


				
				Debug.DrawLine(globalF, globalF+inputVector);
				Debug.DrawLine(globalL, globalL+inputVector);


				//Hit both?  can't help you
				if ( hitFL && hitFR ) {
					inputVector = Vector3.zero;
				}
				else if ( hitFL || hitFR ) {
					Vector3 next_GridPoint = Vector3.zero;
					Vector2 current_GridPoint = cityGrid.worldPointToGridPoint(transform.position);
					if ( hitFL ) {
						next_GridPoint = cityGrid.worldPointToGridPoint(globalFL);
					}
					if ( hitFR ) {
						next_GridPoint = cityGrid.worldPointToGridPoint(globalFR);
					}
					Vector3 normal = new Vector3(next_GridPoint.x - current_GridPoint.x, 0, next_GridPoint.y - current_GridPoint.y);
					inputVector = normal;
				}


				moveTo(transform.position + inputVector);
				return;
			}
			//Now we see where we're going:
			moveTo(transform.position + inputVector);

		}
	}

	//Look at a point
	public void LookAtPoint(Vector3 point) {
		if ( path.Count == 0 && !isJumping ) {
			point.y = transform.position.y;
			transform.LookAt(point);
		}
	}

	//Sets up the path
	public void setPath(List<Vector3> newPath) {
		path = newPath;
	}



	//Flame once
	private float lastFlameTime = -1;
	private bool isFlaming = false;
	private float flameTime = 2f;
	public void CheckFlame(Vector3 inputVector) {
		if ( lastFlameTime + flameTime < Time.time ) {
			isFlaming = false;
		}
		if ( Input.GetButtonDown("Fire3") && lastFlameTime + flameTime < Time.time) {
			isFlaming = true;
			lastFlameTime = Time.time;
			Debug.Log("Flame!");
		}
		flameCollider.enabled = (isFlaming);
		if ( isFlaming ) {
			flamePS.startSpeed = 1f;//+inputVector.magnitude*speed;
			if ( flamePS.isStopped )
				flamePS.Play();
		} else {
			if ( flamePS.isPlaying ) {
				flamePS.Stop();
			}
		}
	}






	//Follow a path:
	private void followPath(){
		if (path.Count > 0) {
			Vector3 point = path[0];
			point.y = transform.position.y;
			Vector3 newPoint;
			newPoint = Vector3.MoveTowards(transform.position, path[0], speed*Time.deltaTime);
			if ( newPoint.Equals(point) ){
				path.RemoveAt(0);
			}

			float distance = Vector3.Distance (transform.position, point);
			Debug.DrawLine(transform.position, point, Color.white,1);
			Debug.Log("Distance:"+distance);
			if(distance <= 0.1f){
				Debug.Log("At point");
				path.RemoveAt(0);
			}else{
				Debug.Log("Move to point");
				moveTo(point);
			}
		}
	}

	//Move to a given point with Canmove detection:
	private void moveToCanMove ( Vector3 point )
	{
		float distance = Vector3.Distance (transform.position, point);
		//Debug.Log("Distance "+distance);
		if (distance > 0) {
			transform.LookAt (point);
			transform.position += transform.forward * Time.deltaTime * speed;
		} else {
			transform.position = point;
		}
	}

	//Move to a given point:
	private void moveTo ( Vector3 point )
	{
		float distance = Vector3.Distance (transform.position, point);
		//Debug.Log("Distance "+distance);
		if (distance > 0) {
			transform.LookAt (point);
			transform.position += transform.forward * Time.deltaTime * speed;
		} else {
			transform.position = point;
		}
	}




	
	//Draw a line along the path
	private void drawPath() {
		lineRenderer.SetVertexCount(path.Count);
		Vector3 point;
		for ( int ii = 0; ii < path.Count; ii++ ) {
			point = path[ii];
			point.y = 0.1f;
			lineRenderer.SetPosition(ii, point);
		}
	}

}
