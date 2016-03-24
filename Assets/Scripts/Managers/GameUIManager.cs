using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using UnityEngine.UI;

public class GameUIManager :  NetworkBehaviour
{
    public Text Score;
    public Text Goal;

    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector3(7, -7, 0);
    }

    [ClientRpc]
    public void RpcSetScore(int left, int right)
    {

        Score.text = "Home " + left + " - " + right + " Away";
    }

    public void SetScore(int left, int right)
    {

        //Debug.Log(GetComponent<RectTransform>().anchoredPosition 

        RpcSetScore(left, right);
    }

    [ClientRpc]
	public void RpcShowGoal(bool show, string scorer)
    {
		if (Goal) 
		{
			Goal.text = "Goal!\n" + scorer;
			Goal.gameObject.SetActive (show);
		}
    }
}
