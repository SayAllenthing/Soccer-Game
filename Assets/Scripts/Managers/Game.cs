using UnityEngine;
using System.Collections;

using UnityEngine.Networking;

public class Game : NetworkBehaviour 
{
    //This stores all the data for the game, and is accessible by all players

    [SyncVar]
    public int ScoreHome = 0;
    [SyncVar]
    public int ScoreAway = 0;

    Transform HomeNet;
    Transform AwayNet;

    bool bIsInitialized = false;

    //Start for all players
    void Start()
    {
        HomeNet = GameObject.Find("GoalLeft").transform;
        AwayNet = GameObject.Find("GoalRight").transform;
    }

    //Start for server
    void OnStartServer()
    {

    }

    [Server]
    public void OnGoal(string net)
    {
        if (net == HomeNet.name)
        {
            ScoreAway++;
        } 
        else
        {
            ScoreHome++;
        }
    }

    public Transform GetMyNet(Actor a)
    {
        if (a.Team == "Home")
            return HomeNet;

        return AwayNet;
    }
}
