using UnityEngine;
using System.Collections;

public class Player : Actor
{
    public Camera camera;

	// Use this for initialization
	void Start () 
	{		
        PlayerOptions PO = PlayerOptions.CurrentPlayerOptions;
        SetAvatar(PO.HairColor, PO.HairStyle, PO.BrowStyle, PO.SkinColor);
	}

    public void Init()
    {
        transform.eulerAngles = new Vector3(45, 0, 0);
        camera.enabled = true;
    }

	// Update is called once per frame
	void Update () 
	{
        base.Update();

        if (network.isLocalPlayer)
        {
            HandleInput();
        }
    }

	void HandleInput()
	{
        if (Input.GetButtonDown("Slide"))
        {
            network.CmdAction("Sliding");
        }
	}
}
