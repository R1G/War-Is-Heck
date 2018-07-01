using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cameraScript : NetworkBehaviour {

	public float mouseSensitivity = 0.5f;
	public Camera cam;

	float xBound = 0;
	float yBound = 0;
	MapClass map;

	public Vector3 selectedStartPoint;
	public Vector3 selectedEndPoint;

	Vector2 dragOrigin;
	CommandManagement commandManager;

	void Start() {
		SetMap();
		commandManager = GetComponent<CommandManagement>();
		cam = gameObject.GetComponentInChildren<Camera>();
		if(!isLocalPlayer) {
			cam.enabled=false;
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
		CameraMovement ();

		// Left clicks are used for selection while...
		if(Input.GetMouseButtonDown(0)) {
			BeginSelect();
			commandManager.ClearSelection ();
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
	// Not a command because only this client needs to be aware of its camera position
	void CameraMovement() {
		float horizontal, vertical;

		float screenLow=.05f;
		float screenHigh=.95f;
		float invertControls = 1f;
		if(gameObject.name=="Red_Player") {
			invertControls=-1f;
		}

		if(ProgUtils.InRange(Screen.width*screenLow,Screen.width*screenHigh,Input.mousePosition.x)) {
			horizontal = 0f;
		} else {
			horizontal = mouseSensitivity * ProgUtils.GetSign(Input.mousePosition.x - Screen.width/2) * invertControls;
		}

		if(ProgUtils.InRange(Screen.height*screenLow,Screen.height*screenHigh,Input.mousePosition.y)) {
			vertical = 0f;
		} else {
			vertical = mouseSensitivity * ProgUtils.GetSign(Input.mousePosition.y - Screen.height/2) * invertControls;
		}

		Vector3 camPos = gameObject.transform.position;
		if (camPos.x >= xBound)
			horizontal = Mathf.Clamp (horizontal, -1f, 0f);
		if (camPos.x <= -15)
			horizontal = Mathf.Clamp (horizontal, 0f, 1f);
		if (camPos.z >= yBound)
			vertical = Mathf.Clamp (vertical, -1f, 0f);
		if (camPos.z <= -15)
			vertical = Mathf.Clamp (vertical, 0f, 1f);
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
	// Also not a command because only determines where this client began selecting
	void BeginSelect() {
		selectedStartPoint = ProgUtils.GetClickPoint (cam);
		dragOrigin = new Vector2(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
	}

	void EndSelect() {
		selectedEndPoint = ProgUtils.GetClickPoint (cam, commandManager);
		RaycastHit hit;
		Vector3 pos = gameObject.transform.position;
		// This raycast handles selecting single troops
		if(Physics.Raycast(pos, selectedEndPoint-pos, out hit, Mathf.Infinity)) {
			if(hit.collider.gameObject.tag == "Friendly") {
				commandManager.AddTroop(hit.collider.gameObject.GetComponent<TroopClass>());
			}
		}
		commandManager.SelectTroops ();
	}

	void SetTarget() {
		GameObject newTarget = null;
		// Sets this player's target to null. GetClickPoint implicitly sets a new target if one is found
		commandManager.target = null;
		commandManager.pointSelected = ProgUtils.GetClickPoint (cam);
		newTarget = commandManager.target;

		if (newTarget != null && newTarget.GetComponent<TroopClass>()) {
			//if the target is not null, it means GetClickPoint found an enemy so attack it
			//the Combat class handles movement towards enemies so it isn't done here
			commandManager.SetAttackTargets ();
		} else {
			//There was no enemy so just move to the pointSelected as usual
			commandManager.MoveTroops ();
		}
	}
}
