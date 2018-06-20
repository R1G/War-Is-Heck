using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public string weaponType;
	public int attackDamage;
	public float attackRange;
	public float attackSpeed;
	public bool isLooted=false;

	public Transform holdTarget;

	// Helps TroopClass decide which animation layer to use
	public string GetWeaponType() {
		return weaponType;
	}

	public void SetLooted(Transform targetTransform) {
		gameObject.GetComponent<BoxCollider>().enabled = false;
		isLooted=true;
		transform.position = targetTransform.position;
		transform.SetParent(targetTransform);
	}

	public void dropLooted() {
		gameObject.GetComponent<BoxCollider>().enabled = true;
		transform.SetParent(null);
		isLooted=false;
	}
	
}
