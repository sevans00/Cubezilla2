using UnityEngine;
using System.Collections;

//This is the new base class for any sort of monobehavior
public interface IGameEntity {

	// Use this for initialization
	void DoStart ();
	
	// Update is called once per frame
	void DoUpdate ();

	// Fixed update is called once per physics step
	void DoFixedUpdate ();
}
