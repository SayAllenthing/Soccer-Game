using UnityEngine;
using System.Collections;

using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";
    [SyncVar(hook = "OnMyColor")]
    public Color playerColor = Color.red;
    [SyncVar(hook = "OnSetTeam")]
    public string MyTeam = "Home";

	public Transform UI;

    public Text NameDisplay;

    public Button ReadyButton;
    public Button ChangeTeamButton;

    public Image BGColor;

	public static LobbyPlayer MyPlayer = null;

	public int HairColor = -1;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

    public override void OnStartClient()
    {
        //All networkbehaviour base function don't do anything
        //but NetworkLobbyPlayer redefine OnStartClient, so we need to call it here
        base.OnStartClient();
        
        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        OnMyName(playerName);
        OnMyColor(playerColor);
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        Debug.Log("OnClientEnterLobby");

        GameObject.Find("NetworkManager").GetComponent<GameLobbyManager>().AddPlayer(this);
        
        //LobbyPlayerList._instance.AddPlayer(this);
        //LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);
        
        //if we return from a game, color of text can still be the one for "Ready"
        //readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        
        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }
        
        //setup the player data on UI. The value are SyncVar so the player
        //will be created with the right value currently on server
        OnMyName(playerName);
        OnMyColor(playerColor);
        OnSetTeam(MyTeam);
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");

        SetupLocalPlayer();
		SendAvatarInfo ();

		MyPlayer = this;
    }

    void SetupLocalPlayer()
    {
        CmdNameChanged(PlayerOptions.CurrentPlayerOptions.Username);

        ReadyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        ReadyButton.onClick.RemoveAllListeners();
        ReadyButton.onClick.AddListener(OnReadyClicked);       

        ChangeTeamButton = GameObject.Find("ChangeTeamButton").GetComponent<Button>();
        ChangeTeamButton.onClick.RemoveAllListeners();
        ChangeTeamButton.onClick.AddListener(OnTeamChanged);

		SendNotReadyToBeginMessage();
    }

	public override void OnClientReady(bool readyState)
	{
		Debug.Log("Am I ready State? " + readyState);
	}

    void SetupOtherPlayer()
    {
		
    }

    public void OnReadyClicked()
    {
		if(!readyToBegin)
		{
			SendReadyToBeginMessage();
        	CmdReadyChanged(Color.green);
		}
		else
		{
			SendNotReadyToBeginMessage();
			CmdReadyChanged(Color.red);
		}
    }

    //SyncVar Callbacks
    public void OnMyName(string newName)
    {
        playerName = newName;
        NameDisplay.text = playerName;
        //nameInput.text = playerName;
    }
    
    public void OnMyColor(Color newColor)
    {
        playerColor = newColor;
        BGColor.color = newColor;
        //colorButton.GetComponent<Image>().color = newColor;
    }

    public void OnTeamChanged()
    {
        string t = "Home";

        if (MyTeam == "Home")
            t = "Away";

        CmdSetTeam(t);
    }

    //Local player change name
    public void OnNameChanged(string str)
    {
        CmdNameChanged(str);
    }

    //Local player change team
    public void OnSetTeam(string team)
    {
        MyTeam = team;
        GameLobbyManager.instance.SetTeam(UI, team);
    }

	//public void OnLevelWasLoaded(int level)
	//{
	//	Debug.Log("Butts");
	//}

    //Server Catch
    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdReadyChanged(Color c)
    {
        playerColor = c;
    }

    [Command]
    public void CmdSetTeam(string team)
    {
        MyTeam = team;
    }

	public void AskToChangeTeamSettings(string s, string t)
	{
		Debug.Log ("Asking to change " + s);
		CmdChangeTeamSetting (s, t);
	}

	[Command]
	public void CmdChangeTeamSetting(string s, string t)
	{		
		GameLobbyManager.instance.ChangeTeamSetting (s, t);
	}

	void SendAvatarInfo()
	{
		CmdSetAvatar (PlayerOptions.CurrentPlayerOptions.HairColor);
	}

	[Command]
	void CmdSetAvatar(int hc)
	{
		HairColor = hc;
	}
}
