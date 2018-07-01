using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {

	public string side;
	CommandManagement commandManager;
	public GameObject cam;

	void Update () {
		if(cam!=null) {
			transform.LookAt (cam.gameObject.transform);		
		}
	}
	
}
