using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {

	public string side;
	GameObject cam;

	void  Start() {
		if(side=="BLUE") {
			cam = CommandManagement.bluePlayer;
		} else if (side=="RED") {
			cam = CommandManagement.bluePlayer;
		}
	}


	void Update () {
		if(Camera.main!=null) {
			transform.LookAt (cam.gameObject.transform);		
		}
	}
	
}
