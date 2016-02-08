using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSync : ActorSync 
{
    public LobbyPlayer lobbyPlayer;

    GameObject ball;

    public string LobbyTeam = "Home";

    void Start()
    {
        if (!isLocalPlayer)
        {
            transform.eulerAngles = new Vector3(45, 0, 0);
        } 
        else
        {
            EnableLocalPlayer();
        }
    }


    public override void OnStartLocalPlayer()
    {
        anim.SetParameterAutoSend(0, true);
        anim.SetParameterAutoSend(1, true);

        EnableLocalPlayer();
        GetAvatarValues();
    }   

    void EnableLocalPlayer()
    {
        //Enable Audio Listener too
        (player as Player).Init();
    }

	// Update is called once per frame
	void Update () 
    {
        base.Update();

        if (isLocalPlayer)
            HandleInput();
      
	}

    public void SetLobbyPlayer(LobbyPlayer lp)
    {
        lobbyPlayer = lp;
        LobbyTeam = lp.MyTeam;
    }

    //Client to Server functions====================================================================
    [Command]
    void CmdSendInput(Vector2 wantDir, bool wantShoot, bool wantSprint)
    {
        WantDir = wantDir.normalized;
        Shooting = wantShoot;
        Sprinting = wantSprint;
    }

    [ClientCallback]
    void HandleInput()
    {
        Vector2 wantDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool wantShoot = Input.GetButton("Shoot");
        bool wantSprint = Input.GetButton("Sprint");
        CmdSendInput(wantDir, wantShoot, wantSprint);
    }
    //==============================================================================================

    [ClientCallback]
    void GetAvatarValues()
    {
        PlayerOptions PO = PlayerOptions.CurrentPlayerOptions;
        CmdUpdateAvatar(PO.HairColor, PO.HairStyle, PO.BrowStyle, PO.SkinColor);

        SendAvatar(PO.HairColor, PO.HairStyle, PO.BrowStyle, PO.SkinColor);
    }
}
