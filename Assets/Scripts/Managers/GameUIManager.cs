using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using UnityEngine.UI;

public class GameUIManager :  NetworkBehaviour
{
    public Text Score;
    public Text Goal;

    [ClientRpc]
    public void RpcSetScore(int left, int right)
    {
        Score.text = left + " - " + right;
    }

    public void SetScore(int left, int right)
    {
        RpcSetScore(left, right);
    }

    [ClientRpc]
    public void RpcShowGoal(bool show)
    {
        Goal.gameObject.SetActive(show);
    }

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
