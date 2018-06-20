using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboard : MonoBehaviour {

	void Update () {
		transform.LookAt (Camera.main.gameObject.transform);
	}
}
