using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopClass : MonoBehaviour {

	public NavMeshAgent agent;
	public Combat combat;
	public bool pc;
	public bool hideHealthBar;
	bool hasMoveTarget;
	public SpriteRenderer healthBar;
	public Animator anim;
	public string attackTag;
	public bool isHidden = false;
	public List<Weapon> inventory = new List<Weapon>();
	public Transform holdTarget;
	private Weapon item;

	virtual protected void Start () {
		agent = gameObject.GetComponent<NavMeshAgent> ();
		combat = gameObject.GetComponent<Combat> ();
		anim.SetFloat ("vertical", 0f);
		if (gameObject.tag=="Friendly") 
			CommandManagement.blueTroops.Add (this);
		else if(gameObject.tag=="Enemy") {
			CommandManagement.redTroops.Add (this);
		}
		if(hideHealthBar) 
			UnmarkSelected ();
	}

	void FixedUpdate() {
		anim.SetFloat ("vertical", Mathf.Clamp(agent.velocity.sqrMagnitude,0f,1f));
		Loot();
	}

	public void Move(Vector3 moveDestination) {
		agent.SetDestination (moveDestination);
	}

	// More of a visual indicator that a troop is selected is that its healthbar is visible
	public void MarkSelected() {
		if (healthBar != null) {
			healthBar.enabled = true;
		}
	}

	public void UnmarkSelected() {
		if (healthBar != null) {
			healthBar.enabled = false;
		}
	}

	public void SetPickupTarget(GameObject target) {
		if(item!=null) {
			item.dropLooted();
			item=null;
			combat.SetWeapon(null);
			combat.SetAnimLayer();
		}
		Move(target.transform.position);
		item = target.GetComponent<Weapon>();
		Debug.Log(item.name);
	}

	// Checks if there are any hostiles within a 25f sphere radius. Then gets the closest one within line of sight.
	// Line of sight here means not obstructed by buildings/terrain/other units/ etc
	// Returns null if no such hostile is found. 
	// This is currently being called in several Update methods, need an optimization to drastically reduce calls
	public GameObject AcquireTarget() {
		Debug.Log("Acquiring target");
		Collider[] colls = Physics.OverlapSphere (transform.position, 15f);
		GameObject targetLocal = null;
		float distanceLocal = float.MaxValue;
		foreach (Collider coll in colls) {
			if (coll.transform.gameObject.tag == attackTag) {
				float newDistance = Vector3.Distance(coll.transform.position, this.gameObject.transform.position);
				if (targetLocal == null || newDistance < distanceLocal) {
					if(coll.gameObject==targetLocal) {
					}
					targetLocal = coll.gameObject;
					distanceLocal = newDistance;
				}
			}
		}
		if(targetLocal!=null && ProgUtils.InLineOfSight(gameObject,targetLocal) &&
		   targetLocal.GetComponent<TroopClass>().isHidden==false) {
			return targetLocal;
		}
		return null;
	}

	public bool IsStopped() {
		return agent.velocity.sqrMagnitude <= 0.1;
	}

	//These could be called either by CommandManagement or by the Health Class
	public void SetCombatTarget(GameObject target) {
		if(!isHidden) {
			Debug.Assert(target.GetComponent<Health>()!=null);
			combat.SetTarget(target);	
		}
	}

	public void ClearCombatTarget() {
		combat.ClearTarget();
	}

	// TODO: Implement the loot/inventory system
	public void Loot() {
		if(item==null) {
			return;
		}
		if(Vector3.Distance(item.transform.position, transform.position) <= agent.stoppingDistance+.2f &&
			!item.GetComponent<Weapon>().isLooted) {
			inventory.Add(item.GetComponent<Weapon>());
			item.GetComponent<Weapon>().SetLooted(holdTarget);
			combat.SetWeapon(item.GetComponent<Weapon>());
			combat.SetAnimLayer();
		}
	}

	// Used for the bushes that conceal characters that enter them
	public void Hide() {
		isHidden=true;
		Color col = gameObject.GetComponentInChildren<MeshRenderer>().material.color;
		col.a = .5f;
	}

	public void Reveal() {
		isHidden=false;
		Color col = gameObject.GetComponentInChildren<MeshRenderer>().material.color;
		col.a = 1f;
	}

}
