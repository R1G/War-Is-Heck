using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManagement : MonoBehaviour {

	public static GameObject bluePlayer = null;
	public static GameObject redPlayer = null;

	public static Vector3 bluePointSelected;
	public static Vector3 redPointSelected;

	public static GameObject blueTarget;
	public static GameObject redTarget;

	public static List<TroopClass> blueTroops = new List<TroopClass> ();
	public static Stack<TroopClass> blueSelectedTroops = new Stack<TroopClass>();

	public static List<TroopClass> redTroops = new List<TroopClass> ();
	public static Stack<TroopClass> redSelectedTroops = new Stack<TroopClass>();

	public static bool blueAttacking = false;
	public static bool redAttacking = false;

	public static Weapon blueItemTarget;
	public static Weapon redItemTarget;

	public static void SelectTroops(string side) {
		List<TroopClass> troops = new List<TroopClass>();
		troops = blueTroops;
		GetSideSpecificObject<List<TroopClass>>(side, ref troops);
		Debug.Log(side);
		Debug.Log(troops.Count);
		if(side=="BLUE") {
			ProgUtils.IterateAll (troops, SelectTroop, bluePlayer, side);
		} else if(side=="RED") {
			ProgUtils.IterateAll (troops, SelectTroop, redPlayer, side);
		}
		
	}

	//Delegated helper function for SelectTroops
	static void SelectTroop(TroopClass troop, Vector3 corner1, Vector3 corner2, string side) {
		if (troop!=null && ProgUtils.InSelectWindow (corner1, corner2, troop.transform.position)) {
			Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
			GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
			troop.MarkSelected ();
			selectedTroops.Push(troop);
		}
	}

	public static void SetItemTarget(Weapon target, string side) {
		Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
		GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
		if(target==null || selectedTroops.Count==0) {
			return;
		}
		selectedTroops.Peek().SetPickupTarget(target.gameObject);
	}

	public static void AddTroop(TroopClass troop, string side) {
		troop.MarkSelected ();
		Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
		GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
		selectedTroops.Push(troop);
	}

	public static void MoveTroops(string side) {
		Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
		GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
		ProgUtils.IterateAll(selectedTroops, MoveTroop, side);
	}

	//Delegated helper function for MoveTroops
	static void MoveTroop(TroopClass troop, string side) {
		GameObject target = new GameObject();
		Vector3 pointSelected = new Vector3(0,0,0);
		GetSideSpecificObject<GameObject>(side, ref target);
		GetSideSpecificObject<Vector3>(side, ref pointSelected);
		if(target==null) {
			troop.ClearCombatTarget();
		}
		troop.Move (pointSelected);

	}

	public static void ClearSelection(string side) {
		Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
		GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
		ProgUtils.IterateAll (selectedTroops, UnmarkTroop);
		selectedTroops.Clear ();
	}

	public static void UnmarkTroop(TroopClass troop) {
		troop.UnmarkSelected ();
	}

	public static void SetAttackTargets(string side) {
		Stack<TroopClass> selectedTroops = new Stack<TroopClass>(); 
		GetSideSpecificObject<Stack<TroopClass>>(side, ref selectedTroops);
		ProgUtils.IterateAll (selectedTroops, SetAttackTarget, side);
		if(side=="BLUE") {
			blueAttacking=false;
		} else if(side=="RED") {
			redAttacking=false;
		}
	}

	public static void SetAttackTarget(TroopClass troop, string side) {
		GameObject target = new GameObject();
		GetSideSpecificObject<GameObject>(side, ref target);
		if (target != null) {
			troop.SetCombatTarget (target);
		}
	}

	public static void SetManagerTarget(GameObject target, string side) {
		if(side=="BLUE") {
			blueTarget = target;
		} else if(side=="RED") {
			redTarget = target;
		}
	}



	// Instead of having seperate yet identical pieces of logic for the blue team and red team
	// This method helps the coder set values to local variable depending on which side (blue/red)
	// is responsible for a call to any of the above methods. An object o will automatically get
	// assigned the corresponding value from the team that is making the call to the method. 
	static void GetSideSpecificObject<T>(string side, ref T obj) {
		if(side=="BLUE") {
			if(obj is Vector3) {
				obj = (T)(object)bluePointSelected;
			} else if(obj is GameObject) {
				obj = (T)(object)blueTarget;
			} else if(obj is List<TroopClass>) {
				obj = (T)(object)blueTroops;
			} else if(obj is Stack<TroopClass>) {
				obj = (T)(object)blueSelectedTroops;
			} else if(obj is Weapon) {
				obj = (T)(object)blueItemTarget;
			} else if(obj is bool) {
				obj = (T)(object)blueAttacking;
			}
		} else if(side=="RED") {
			if(obj is Vector3) {
				obj = (T)(object)redPointSelected;
			} else if(obj is GameObject) {
				obj = (T)(object)redTarget;
			} else if(obj is List<TroopClass>) {
				obj = (T)(object)redTroops;
			} else if(obj is Stack<TroopClass>) {
				obj = (T)(object)redSelectedTroops;
			} else if(obj is Weapon) {
				obj = (T)(object)redItemTarget;
			} else if(obj is bool) {
				obj = (T)(object)redAttacking;
			}
		}
	}

	static void printList(List<TroopClass> l) {
		foreach(TroopClass o in l) {
			Debug.Log(o);
		}
	}



}



