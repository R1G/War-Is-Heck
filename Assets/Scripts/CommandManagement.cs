using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManagement : MonoBehaviour {

	public static Vector3 pointSelected;
	public static GameObject target;
	public static List<TroopClass> troops = new List<TroopClass> ();
	public static Stack<TroopClass> selectedTroops = new Stack<TroopClass>();
	public static bool attacking = false;
	public static Weapon itemTarget;

	public static void SelectTroops() {
		ProgUtils.IterateAll (troops, SelectTroop, Camera.main.gameObject);
	}

	//Delegated helper function for SelectTroops
	static void SelectTroop(TroopClass troop, Vector3 corner1, Vector3 corner2) {
		if (troop!=null && ProgUtils.InSelectWindow (corner1, corner2, troop.transform.position)) {
			troop.MarkSelected ();
			selectedTroops.Push (troop);
		}
	}

	public static void SetItemTarget(Weapon target) {
		if(selectedTroops.Count==0) {
			return;
		}
		itemTarget = target;
		selectedTroops.Peek().SetPickupTarget(target.gameObject);
	}

	public static void AddTroop(TroopClass troop) {
		troop.MarkSelected ();
		selectedTroops.Push (troop);
	}

	public static void MoveTroops() {
		ProgUtils.IterateAll (selectedTroops, MoveTroop);
	}

	//Delegated helper function for MoveTroops
	static void MoveTroop(TroopClass troop) {
		if(target==null) {
			troop.ClearCombatTarget();
		}
		troop.Move (pointSelected);

	}

	public static void ClearSelection() {
		ProgUtils.IterateAll (selectedTroops, UnmarkTroop);
		selectedTroops.Clear ();
	}

	public static void UnmarkTroop(TroopClass troop) {
		troop.UnmarkSelected ();
	}

	public static void SetAttackTargets() {
		ProgUtils.IterateAll (selectedTroops, SetAttackTarget);
		attacking = false;
	}

	public static void SetAttackTarget(TroopClass troop) {
		if (target != null) {
			troop.SetCombatTarget (target);
		}
	}
}

