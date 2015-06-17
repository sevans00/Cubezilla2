using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Civilian : MonoBehaviour, IGameEntity {

	float walkSpeed = 1f;
	float runSpeed = 3f;
	float viewRadius = 5f;
	float runRadius = 1f;
	float safeRadius = 2f;
	float onFireTime = 3.0f;
	float speedDeviation = 0.5f;
	float maxRandTurn = 5.0f;
	
	CityGrid cityGrid;
	Transform godzilla;
	//Game game;
	private List<Vector3> path = new List<Vector3>();
	GameObject safezone;
	List<GameObject> safezones;
	bool fleeing = false;
	float screamVolume;
	AudioClip screamSounds;
	
	ParticleSystem flames;

	public void DoStart () {
		cityGrid = GameObject.FindObjectOfType<CityGrid>();
		GameObject zilla = GameObject.Find("Godzilla");
		godzilla = zilla.transform;
		//game = GameObject.Find("Game").GetComponent(Game);
		flames = GetComponentInChildren<ParticleSystem>();
		
//		safezones = GameObject.FindGameObjectsWithTag("SafeZone");
//		findNearestSafeZone();
		//Debug.Log("safezone info: " + safezone.transform.position);

		Vector3 pos = transform.position;
		pos.y = 0.06f;
		transform.position = pos;

		speedDeviation = Mathf.Clamp01(speedDeviation);
		var deviation = Random.Range(1-speedDeviation, speedDeviation+1);
		walkSpeed *= deviation;
		runSpeed *= deviation;
		
		
	}
	
	public void DoUpdate () {

		/*
		float distToEnemy = Vector3.Distance(transform.position, godzilla.position);
		if (distToEnemy < runRadius) {
			fleeing = false;
			RunFromEnemy();
		} else if ((distToEnemy < safeRadius) || fleeing) {
			RunToSafeZone();
			fleeing = true;
		} else {
			fleeing = false;
			RandomWalk();
		}
		*/
	}
	public void DoFixedUpdate () {

	}


	/*
	function OnTriggerEnter(other : Collider) {
		if (other.name == "Godzilla") {
			Kill();
		} else if (other.name == "FireBreath") {
			OnFire();
		} else if (other.name == "SafeZone") {
			Escape();
		}
	}
	*/

	void Move( float speed ) {
		//Non physics method:
		Vector2 current_GridPoint = cityGrid.worldPointToGridPoint(transform.position);
		float distance = speed * Time.deltaTime * 2f + transform.localScale.z;
		Vector2 next_GridPoint = cityGrid.worldPointToGridPoint(transform.position + transform.forward * distance);
		if ( current_GridPoint != next_GridPoint && !cityGrid.isGridPointAvailable(next_GridPoint) ) {
			//Need to turn:
			Vector3 direction = transform.forward;
			Vector3 normal = new Vector3(next_GridPoint.x - current_GridPoint.x, 0, next_GridPoint.y - current_GridPoint.y);
			direction = Vector3.Reflect(direction, normal);
			direction.y = 0;
			direction.Normalize();
			Debug.DrawLine(transform.position, transform.position + direction, Color.red, 1);
			Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red, 1);
			RotateToDirection(direction);
		}

		transform.position += transform.forward * speed * Time.deltaTime;
	}
	
	void moveTo (Vector3 point) {
		point.y = transform.position.y;
		float distance = Vector3.Distance (transform.position, point);
		transform.LookAt (point);
		if (distance > 0.1f) {
			transform.position += transform.forward * Time.deltaTime * runSpeed;
		}
	}
	
	//Rotate to a direction:
	void RotateToDirection(Vector3 direction) {
		Vector2 dir_vec2 = new Vector2(direction.x, direction.z);
		Vector2 forward_vec2 = new Vector2(transform.forward.x, transform.forward.z);
		float angle = Vector2.Angle(forward_vec2, dir_vec2);
		
		float cross = Vector3.Cross(direction, transform.forward).y;
		int sign = cross < 0 ? 1 : -1;
		
		transform.Rotate(0, sign * angle, 0);
	}
	
	void RandomWalk() {
		var randTurn = Random.Range(-maxRandTurn, maxRandTurn);
		transform.Rotate(0, randTurn, 0);
		Move(walkSpeed);
	}
	
	void RunToPoint(Vector3 newPos) {
		RotateToDirection( newPos );
		Move(runSpeed);
	}
	
	void RunFromEnemy() {
		Vector3 dirToRun = transform.position - godzilla.position;
		dirToRun.y = 0;
		RunToPoint(dirToRun);
	}
	
	void RunToSafeZone() {
		if(safezone){
			//Debug.Log("safezone exists");
			startMovement(safezone.transform.position);
		}else{
			RunFromEnemy();
		}
	}
	
	void OnFire() {
		flames.Play();
		Camera.main.GetComponent<AudioSource>().PlayOneShot(screamSounds, screamVolume);
		//yield new WaitForSeconds(onFireTime);
		Kill();
	}
	
	void Kill() {
//		if (game != null)
//			game.incrementKillBy(1);
		Destroy(gameObject);
	}
	
	void Escape() {
//		if (game != null)
//			game.civilianEscaped(1);
		Destroy(gameObject);
	}
	
	void findNearestSafeZone(){
		float nearest = 100;
		Vector3 safezoneLoc;
		for(int i = 0; i < safezones.Count; ++i){
			float distance = Vector3.Distance(transform.position, safezones[i].transform.position);
			if( distance < nearest ){
				nearest = distance;
				safezone = safezones[i];
				safezoneLoc = safezones[i].transform.position;
			}
		}
		//var safezoneLoc : Vector3 = safezone.transform.position;
		//Debug.Log("safezone: " + safezoneLoc);
		//startMovement(safezoneLoc);
	}
	
	void startMovement(Vector3 toPosition){
		if(path.Count > 0){
			//Debug.Log("path exists");
			followPath(path);
		}else{
			//Debug.Log("path does not exist");
			path = cityGrid.getWorldPath(transform.position, toPosition);
			followPath(path);
		}
	}
	
	void followPath(List<Vector3> path){
		if (path.Count > 0){
			float distance = Vector3.Distance (transform.position, path[0]);
			if(distance < 0.2f){
				//Debug.Log("At point");
				path.RemoveAt(0);
			}else{
				//Debug.Log("Move to point");
				moveTo(path[0]);
			}
		}
	}
	
	public void SetColor(float num) {

		gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, num);
		
	}
	
	void panic(float num) {
		if (num > 1f)
			num = Mathf.Clamp01(num);
		SetColor(num);
		SetSpeed(num);
	}
	
	void SetSpeed( float num) {
		walkSpeed = Mathf.Lerp(0.15f, 0.8f, num);
		runSpeed = Mathf.Lerp(0.8f, 1.5f, num);
	}

}