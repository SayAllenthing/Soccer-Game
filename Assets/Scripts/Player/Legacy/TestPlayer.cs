using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class TestPlayer : NetworkBehaviour {

    int i = 0;

    public override void OnStartLocalPlayer()
    {
        string mine = "Mine";
        if (!isLocalPlayer)
            mine = "Not Mine";
    }

    public override void OnStartServer()
    {
        string mine = "Mine";
        if (!isLocalPlayer)
            mine = "Not Mine";

        Debug.Log("OnStartServer " + mine);
    }

    public override void OnStartClient()
    {
        string mine = "Mine";
        if (!isLocalPlayer)
            mine = "Not Mine";

        Debug.Log("OnStartClient " + mine);
    }

    public override void PreStartClient()
    {
        string mine = "Mine";
        if (!isLocalPlayer)
            mine = "Not Mine";

        Debug.Log("PreStartClient " + mine);

        i++;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CmdTestFunction();
                ClientTest();
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Server - " + NetworkServer.active);
            Debug.Log("Client - " + NetworkClient.active);
        }

    }

    [ClientRpc]
    void RpcTestFunction()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("RPC Thing Called - " + isLocalPlayer);
    }

    [Command]
    void CmdTestFunction()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("Command Thing Called - " + isLocalPlayer);

        RpcTestFunction();
    }

    [Server]
    void ServerTest()
    {
        Debug.Log("Server Test");
    }

    [Client]
    void ClientTest()
    {
        Debug.Log("Client Test");
    }
    
}
