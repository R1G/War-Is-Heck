using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cameraScript : NetworkBehaviour {

	public float mouseSensitivity = 0.5f;
	public string side;
	Camera cam;

	float xBound = 0;
	float yBound = 0;
	MapClass map;

	public Vector3 selectedStartPoint;
	public Vector3 selectedEndPoint;

	Vector2 dragOrigin;

	void Start() {
		cam = gameObject.GetComponentInChildren<Camera>();
		if(!isLocalPlayer) {
			cam.enabled=false;
		}	
		SetMap ();
		if(CommandManagement.bluePlayer==null) {
			CommandManagement.bluePlayer=gameObject;
			side="BLUE";
		} else if(CommandManagement.redPlayer==null) {
			CommandManagement.redPlayer=gameObject;
			side="RED";
			transform.GetChild(0).Rotate(0,180,0);
		}
	}

	// Responsible for the drag-select UI display. Its for display only and won't affect gameplay
	void OnGUI() {
		if(!isLocalPlayer) {return;}
		if(Input.GetMouseButton(0)) {
			Rect drag = Rect.MinMaxRect(dragOrigin.x, dragOrigin.y, Input.mousePosition.x, Screen.height-Input.mousePosition.y);
			GUI.Box (drag,"");
		}

	}

	void Update () {
		if(!isLocalPlayer) {return;}
		if(side=="RED" && CommandManagement.redPlayer==null) {
			Debug.Log("Red player not found!");
		}
		CameraMovement ();

		// Left clicks are used for selection while...
		if(Input.GetMouseButtonDown(0)) {
			BeginSelect();
			CommandManagement.ClearSelection (side);
		}

		if (Input.GetMouseButtonUp (0)) {
			EndSelect ();
		}

		// ...right clicks are used for targeting and movement
		if (Input.GetMouseButtonDown (1)) {
			SetTarget ();
		}
	}

	// Like Warcraft/AoE/DotA the camera pans if the mouse moves to an edge
	void CameraMovement() {
		float horizontal;
		float vertical;

		if(ProgUtils.InRange(Screen.width*.05f,Screen.width*.95f,Input.mousePosition.x)) {
			horizontal = 0f;
		} else {
			horizontal = mouseSensitivity * ProgUtils.GetSign(Input.mousePosition.x - Screen.width/2);
		}

		if(ProgUtils.InRange(Screen.height*.05f,Screen.height*.95f,Input.mousePosition.y)) {
			vertical = 0f;
		} else {
			vertical = mouseSensitivity * ProgUtils.GetSign(Input.mousePosition.y - Screen.height/2);
		}

		Vector3 camPos = gameObject.transform.position;
		if (camPos.x >= xBound)
			Mathf.Clamp (horizontal, -1f, 0f);
		if (camPos.x <= 0)
			Mathf.Clamp (horizontal, 0f, 1f);
		if (camPos.y >= yBound)
			Mathf.Clamp (vertical, -1f, 0f);
		if (camPos.y <= 0)
			Mathf.Clamp (vertical, 0f, 1f);
		transform.Translate (horizontal, 0, vertical, Space.World);
	}
	
	// All instances of Map class have bounds so camera fits them accordingly	
	void SetMap() {
		map = GameObject.FindGameObjectWithTag ("MAP").GetComponent<MapClass>();
		xBound = map.getXBound ();
		yBound = map.getYBound ();
	}

	// Selection works by creating two Vector3's on the surface of the map. Any selectable in between
	// these Vector3 corners are considered selected and added to the selection stack.
	void BeginSelect() {
		selectedStartPoint = ProgUtils.GetClickPoint (cam, side);
		dragOrigin = new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
	}

	void EndSelect() {
		selectedEndPoint = ProgUtils.GetClickPoint (cam, side);
		RaycastHit hit;
		Vector3 pos = gameObject.transform.position;
		// This raycast handles selecting single troops
		if(Physics.Raycast(pos, selectedEndPoint-pos, out hit, Mathf.Infinity)) {
			if(hit.collider.gameObject.tag == "Friendly") {
				CommandManagement.AddTroop(hit.collider.gameObject.GetComponent<TroopClass>(), side);
			}
		}
		CommandManagement.SelectTroops (side);
	}

	void SetTarget() {
		GameObject newTarget = null;
		// Sets this player's target to null. GetClickPoint implicitly sets a new target if one is found
		if(side=="BLUE") {
			CommandManagement.blueTarget = null;
			CommandManagement.bluePointSelected = ProgUtils.GetClickPoint (cam, side);
			newTarget = CommandManagement.blueTarget;
		} else if(side=="RED") {
			CommandManagement.redTarget = null;
			CommandManagement.redPointSelected = ProgUtils.GetClickPoint (cam, side);
			newTarget = CommandManagement.redTarget;
		}

		if (newTarget != null) {
			//if the target is not null, it means GetClickPoint found an enemy so attack it
			//the Combat class handles movement towards enemies so it isn't done here
			CommandManagement.SetAttackTargets (side);
		} else {
			//There was no enemy so just move to the pointSelected as usual
			CommandManagement.MoveTroops (side);
		}
	}
}
