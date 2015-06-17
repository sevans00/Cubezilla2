using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject followTarget;

	public float zDistance = 7f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = followTarget.transform.position + Vector3.up*zDistance;
	}
}
