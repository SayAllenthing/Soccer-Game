using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSync : ActorSync 
{
    public LobbyPlayer lobbyPlayer;

    GameObject ball;

    public string LobbyTeam = "Home";

   
    bool PingLock = false;
    float TimeStamp = 0;
    int PingCounter = 0;

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
        {
            HandleInput();
            HandlePing();
        }
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

    //Ping===========================================================================================
    [Command]
    void CmdSendPing()
    {
        if (isLocalPlayer)
            GetPing();
        else
            RpcGetPing();
    }

    [ClientRpc]
    void RpcGetPing()
    {
        if (isLocalPlayer)
            GetPing();
    }

    void GetPing()
    {
        if (isLocalPlayer)
        {
            float ping = TimeStamp*1000f;
            DebugManager.Instance.SetPing(ping);
            //Debug.Log("Ping Time = " + ping.ToString("0.0") + "ms");
            PingLock = false;
            TimeStamp = 0;
        }
    }

    void HandlePing()
    {
        if (!PingLock)
        {
            if(PingCounter > 500)
            {
                PingLock = true;
                TimeStamp += Time.deltaTime;
                CmdSendPing();
                PingCounter = 0;
            }
            else
            {
                PingCounter++;
            }           
        } 
        else
        {
           TimeStamp += Time.deltaTime;
        }
    }
}
