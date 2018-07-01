using UnityEngine;
using UnityEngine.Networking;
public class NetManagerEXT : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
    	string troopTag;
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        if(GameObject.Find("Blue_Player") == null) {
        	player.GetComponent<CommandManagement>().objectName = "Blue_Player";
        	troopTag="Friendly";
        } else {
        	player.GetComponent<CommandManagement>().objectName = "Red_Player";
        	troopTag="Enemy";
        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        foreach(GameObject GO in GameObject.FindGameObjectsWithTag(troopTag)) {
        	GO.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        }
    }
}