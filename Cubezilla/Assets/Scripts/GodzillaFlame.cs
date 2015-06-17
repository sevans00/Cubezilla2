using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodzillaFlame : MonoBehaviour {

	public void OnTriggerEnter ( Collider other )
	{
		other.BroadcastMessage("OnFire", SendMessageOptions.DontRequireReceiver);
	}
}
