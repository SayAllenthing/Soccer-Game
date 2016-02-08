using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class NetworkHandler : NetworkManager
{
    public GameObject GamePlayerPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
   	
}
