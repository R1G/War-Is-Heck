using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgUtils : MonoBehaviour {

	public delegate void TroopCommand(TroopClass troop);
	public delegate void SelectCommand(TroopClass troop, Vector3 v1, Vector3 v2);

	// OVERLOAD: Applies the command method to every element of the troopstack. ONLY for Stack<TroopClass>
	public static void IterateAll(Stack<TroopClass> troopStack, TroopCommand command) {
		Stack<TroopClass> temp = new Stack<TroopClass>();
		while (troopStack.Count > 0) {
			TroopClass troop = (TroopClass)troopStack.Pop ();
			command (troop);
			temp.Push (troop);
		}
		while (temp.Count > 0) {
			troopStack.Push (temp.Pop ());
		}
		return;
	}

	// OVERLOAD: Applies the command method to every element of the troopList. ONLY for List<TroopClass>
	public static void IterateAll(List<TroopClass> troopList, SelectCommand command, cameraScript cam) {
		if(cam==null) {return;}
		Vector3 corner1 = cam.selectedStartPoint;
		Vector3 corner2 = cam.selectedEndPoint;
		for (int i = 0; i < troopList.Count; i++) {
			command (troopList[i] , corner1, corner2);
		}
	}

	// Picture a rectangle with diagonal from corner1 to corner 2. Returns true if pos falls within the rectangle
	public static bool InSelectWindow(Vector3 corner1, Vector3 corner2, Vector3 pos) {
		float lowX = Mathf.Min (corner1.x, corner2.x);
		float hiX = Mathf.Max (corner1.x, corner2.x);
		float lowZ = Mathf.Min (corner1.z, corner2.z);
		float hiZ = Mathf.Max (corner1.z, corner2.z);

		if (lowX <= pos.x && pos.x <= hiX && lowZ <= pos.z && pos.z <= hiZ) {
			return true;
		}
		return false;
	}

	// Returns true if val is within the range (lo, hi) //////////////////////////////////////
	public static bool InRange(float lo, float hi, float val) {
		if (val >= lo && val <= hi) 
			return true;
		return false;
	}

	// Overload that checks if two gameobjects are within range of each other
	public static bool InRange(GameObject go1, GameObject go2, float attackRange) {
		if (go2 == null)
			return false;
		if (Vector3.Distance (go1.transform.position, go2.transform.position) <= attackRange)
			return true;
		return false;
	}

	// Overload that checks if GameObject middle is between GameObjects go1 and go2
	public static bool InRange(GameObject go1, GameObject go2, GameObject middle) {
		if (go2 == null || go1 == null || middle == null) {
			return false;
		}
		return InSelectWindow(go1.transform.position, go2.transform.position, middle.transform.position);
	}

	// If segment AB has the same slope as segment BC then by definition ABC are colinear
	public static bool IsColinear(Vector3 vec1, Vector3 vec2, Vector3 vec3) {
		return GetSlope(vec1,vec2)==GetSlope(vec2,vec3);
	}

	// For 3D lines, the slope is a Vector2, as in horizontal slope and vertical slope
	public static Vector2 GetSlope(Vector3 vec1, Vector3 vec2) {
		float x = (vec1.x-vec2.x)/(vec1.z-vec2.z);
		float y = (vec1.y-vec2.y)/(vec1.x-vec2.x);
		return new Vector2(x,y);
	}


	/////////////////////////////////////////////////////////////////////////////////////////

	// Returns the closest point to go1 within range of go2.
	public static Vector3 GetNearestWithinRange(GameObject go1, GameObject go2, float range) {
		//rx/d, ry/d
		float d = Vector3.Distance(go1.transform.position, go2.transform.position);
		Vector3 t = go2.transform.position - go1.transform.position;
		return go2.transform.position - new Vector3(t.x*range/d, t.y, t.z*range/d);
	}
 
	// Returns true if float is positive /////////////////////////////////////////////////////
	public static float GetSign(float val) {
		if (val < 0)
			return -1;
		return 1;
	}

	// Overload for int ^
		public static float GetSign(int val) {
		if (val < 0)
			return -1;
		return 1;
	}

	/////////////////////////////////////////////////////////////////////////////////////////

	// Main use is to get the vector3 point where the player clicked their mouse
	// But if the player clicks on an enemy, sets CommandManagement's target to this enemy
	public static Vector3 GetClickPoint(Camera cam, CommandManagement manager) {
		RaycastHit hit;
		if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity)) {
			
			manager.SetManagerTarget(hit.transform.gameObject);
			return hit.point;
		}

		Debug.LogError ("Map collider not found!");
		Application.Quit ();
		return hit.point;
	}

	// overload that won't implicitly assign attackTarget on commandManager
	public static Vector3 GetClickPoint(Camera cam) {
		RaycastHit hit;
		if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity)) {
			return hit.point;
		}

		Debug.LogError ("Map collider not found!");
		Application.Quit ();
		return hit.point;
	}

	//Deletes heirarchies of transform parent/children
	public static void RecursiveDestruction(GameObject GO) {
		foreach (Transform child in GO.transform) {
			RecursiveDestruction (child.gameObject);
		}
		Destroy (GO);
	}

	//Mainly used to see if a troop can see another troop
	public static bool InLineOfSight(GameObject Go1, GameObject Go2) {
		RaycastHit hit;
		int mask = 1<<8;
		if(Go1.GetComponent<TroopClass>()!=null) {
			TroopClass tc = Go1.GetComponent<TroopClass>();
			if(tc.attackTag=="Enemy") {
				mask = mask | (1<<9);
			} else if(tc.attackTag=="Friendly") {
				mask = mask | (1<<10);
			}
		}
		mask = ~mask;

		if(Physics.SphereCast(Go1.transform.position, .01f, Go2.transform.position-Go1.transform.position, out hit, Mathf.Infinity, mask)) {
			if(hit.collider.gameObject==Go2) 
				return true;
		}
		return false;
	}

	static string GetTagFromSide(string side) {
		if(side=="BLUE") {
			return "Enemy";
		} 
		return "Friendly";
	}

}

