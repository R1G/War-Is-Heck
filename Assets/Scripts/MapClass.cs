using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapClass : MonoBehaviour {
	public float xBound;
	public float yBound;

	public static GameObject[] cover;

	//public GameObject[] PatrolPoints;

	void Start() {
		cover = GameObject.FindGameObjectsWithTag("Cover");
	}

	public float getXBound() {
		return xBound;
	}

	public float getYBound() {
		return yBound;
	}

	public static bool InCover(GameObject attacker, GameObject target) {
		foreach(GameObject cov in cover) {
			if(ProgUtils.InRange(attacker,target,cov)) {
				Vector3 vec1 = attacker.transform.position;
				Vector3 vec2 = target.transform.position;
				RaycastHit hit;
				if(Physics.Raycast(vec1, vec2-vec1, out hit, Vector3.Distance(vec1,vec2))) {
					if(hit.collider.gameObject.tag=="Cover") {
						float attackDistance = Mathf.Abs(Vector3.Distance(attacker.transform.position,cov.transform.position));
						float targetDistance = Mathf.Abs(Vector3.Distance(cov.transform.position,target.transform.position));
						if(attackDistance>targetDistance) {
							return true;
						}
					}
				}
			}
		}
		return false;
	}
}
