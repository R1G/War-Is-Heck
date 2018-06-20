using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushClass : MonoBehaviour {

	void OnTriggerEnter(Collider coll) {
		TroopClass tc = coll.gameObject.GetComponent<TroopClass>();
		if(tc!=null) {
			Debug.Log("Entered a bush!");
			tc.Hide();
		}
	}

	void OnTriggerExit(Collider coll) {
		TroopClass tc = coll.gameObject.GetComponent<TroopClass>();
		if(tc!=null) {
			tc.Reveal();
		}
	}
}
