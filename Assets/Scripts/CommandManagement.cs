using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CommandManagement : NetworkBehaviour {

	public GameObject player = null;
	public Vector3 pointSelected;
	public GameObject target;
	cameraScript camScript;

	public List<TroopClass> troops = new List<TroopClass> ();
	public Stack<TroopClass> selectedTroops = new Stack<TroopClass>();

	public bool attacking = false;

	[SyncVar]
	public string objectName;
	
	string attackSide;
	string allySide;

	void Start() {
		gameObject.name = objectName;
		camScript = GetComponent<cameraScript>();
		if(gameObject.name=="Blue_Player") {
			attackSide="Enemy";
			allySide="Friendly";
			camScript.cam.cullingMask |= 1<<9;
		} else {
			allySide="Enemy";
			attackSide="Friendly";
			camScript.cam.cullingMask |= 1<<10;
			transform.Rotate(0,180,0);
			transform.position = new Vector3(45, transform.position.y, 45);
		}

		foreach(GameObject go in GameObject.FindGameObjectsWithTag(allySide)) {
			troops.Add(go.GetComponent<TroopClass>());
		}
	}

	public void SelectTroops() {
		ProgUtils.IterateAll (troops, SelectTroop, camScript);
		
	}

	//Delegated helper function for SelectTroops
	void SelectTroop(TroopClass troop, Vector3 corner1, Vector3 corner2) {
		if (troop!=null && ProgUtils.InSelectWindow (corner1, corner2, troop.transform.position)) {
			troop.MarkSelected ();
			selectedTroops.Push(troop);
		}
	}

	public void AddTroop(TroopClass troop) {
		troop.MarkSelected ();
		selectedTroops.Push(troop);
	}

	public void MoveTroops() {
		ProgUtils.IterateAll(selectedTroops, MoveTroop);
	}

	//Delegated helper function for MoveTroops
	void MoveTroop(TroopClass troop) {
		troop.Move (pointSelected, true);

	}

	public void ClearSelection() {
		ProgUtils.IterateAll (selectedTroops, UnmarkTroop);
		selectedTroops.Clear ();
	}

	public void UnmarkTroop(TroopClass troop) {
		troop.UnmarkSelected ();
	}

	public void SetAttackTargets() {
		ProgUtils.IterateAll (selectedTroops, SetAttackTarget);
		attacking=false;
	}

	public void SetAttackTarget(TroopClass troop) {
		if (target != null) {
			troop.SetCombatTarget (target);
		}
	}

	public void SetManagerTarget(GameObject GO) {
		if(GO.tag==attackSide) {
			target=GO;
		} else {
			target=null;
		}
	}

	public string GetSide() {
		return allySide;
	}

	public string GetEnemySide() {
		return attackSide;
	}
}



