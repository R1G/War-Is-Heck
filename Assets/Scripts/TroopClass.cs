using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class TroopClass : NetworkBehaviour {

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
	public bool visible = false;
	private Weapon item;
	CommandManagement commandManager;

	void Awake() {
		agent = gameObject.GetComponent<NavMeshAgent> ();
	}

	virtual protected void Start () {
		combat = gameObject.GetComponent<Combat> ();
		anim.SetFloat ("vertical", 0f);
		if(hideHealthBar) 
			UnmarkSelected ();
	}

	void FixedUpdate() {
		if(agent!=null) {
			anim.SetFloat ("vertical", Mathf.Clamp(agent.velocity.sqrMagnitude,0f,1f));
		}
		if(visible) {
			gameObject.layer = 0;
			ChangeLayerOfChildren(transform);
		}
		Loot();
	}

	void ChangeLayerOfChildren(Transform T) {
		T.gameObject.layer = 0;
		foreach(Transform t in T) {
			ChangeLayerOfChildren(t);

		}
	}

	[Command]
	public void CmdMove(Vector3 moveDestination) {
	Debug.Log("CmdMove");
		if(agent!=null) {
			agent.SetDestination (moveDestination);
		}
	}

	public void Move(Vector3 moveDestination) {
	Debug.Log("CmdMove");
		if(agent!=null) {
			agent.SetDestination (moveDestination);
		}
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
		CmdMove(target.transform.position);
		item = target.GetComponent<Weapon>();
		Debug.Log(item.name);
	}

	// Checks if there are any hostiles within a 25f sphere radius. Then gets the closest one within line of sight.
	// Line of sight here means not obstructed by buildings/terrain/other units/ etc
	// Returns null if no such hostile is found. 
	// This is currently being called in several Update methods, need an optimization to drastically reduce calls
	// Do not make this a command, there will be too much traffic and its needless
	public GameObject AcquireTarget() {
		Collider[] colls = Physics.OverlapSphere (transform.position, 15f);
		GameObject targetLocal = null;
		float distanceLocal = float.MaxValue;
		foreach (Collider coll in colls) {
			if (coll.transform.gameObject.tag == attackTag) {
				TroopClass enemyTC = coll.transform.gameObject.GetComponent<TroopClass>();
				if(enemyTC!=null) {
					enemyTC.visible = true;
				}
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
		return agent.velocity.sqrMagnitude <= 0.1f;
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
