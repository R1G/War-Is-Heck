using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {

	void Update () {
		if(Camera.main!=null) {
			transform.LookAt (Camera.main.gameObject.transform);		
		}
	}
	
}
