using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BallSync : NetworkBehaviour 
{
    GameManager Game;

    [SyncVar]
    Vector3 SyncPos;

    [SyncVar]
    Quaternion SyncQuat;

    [SyncVar]
    public Vector3 BallVel;

    [SerializeField] float lerpRate = 15;

    [SerializeField] SphereCollider HostCollider;
    [SerializeField] SphereCollider ClientCollider;

    public bool bIsHost = false;

    Ball ball;

    public void Start()
    {
        ball = GetComponent<Ball>();
    }

    public override void PreStartClient()
    {
       
    }

    void Update()
    {
        if (NetworkServer.active)
        {
            ServerUpdate();
        } 
        else
        {
            LerpPosition();
        }
    }

    void LerpPosition()
    {
        //Only lerp other characters
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, SyncPos, Time.deltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, SyncQuat, Time.deltaTime * lerpRate);
        }
    }
    
    [ClientRpc]
    void RpcUpdatePosition(Vector3 pos, Quaternion rot)
    {
        SyncPos = pos;
        SyncQuat = rot;
    }
    
    [ServerCallback]
    void SendPosition()
    {       
        RpcUpdatePosition(transform.position, transform.rotation);
    }

    [Server]
    void OnTriggerEnter(Collider c)
    {        
        if (c.tag == "Goal")
        {
            if(Game == null)
                Game = GameObject.Find("GameManager").GetComponent<GameManager>();
            
            Game.OnGoal(c.name);
            ball.bInPlay = false;
        }
    }

    [Server]
    void ServerUpdate()
    {
        SendPosition();

        if (transform.position.y < -5)
        {
            if(Game == null)
                Game = GameObject.Find("GameManager").GetComponent<GameManager>();
            
            Game.Reset();
        }
    }

}
