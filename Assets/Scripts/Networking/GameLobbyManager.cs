﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLobbyManager : NetworkLobbyManager 
{
    public GameObject MenuPanel;
    public GameObject PlayerPanel;
    public GameObject TopText;

	public InputField IPField;

    public Button PlayerPanelBackButton;

    public Transform HomePlayerListTransform;
    public Transform AwayPlayerListTransform;

    public static GameLobbyManager instance;

    public Dictionary<int, LobbyPlayer> LobbyPlayers = new Dictionary<int, LobbyPlayer>();

	public TeamSettings HomeSettings;
	public TeamSettings AwaySettings;

    void Start()
    {		
		connectionConfig.MaxSentMessageQueueSize = 256;

        if (instance != null)
            DestroyImmediate(instance.gameObject);
        
        instance = this;
    }

    public virtual void OnClientEnterLobby()
    {
        Debug.Log("Client Entered");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public void OnHostClicked()
    {
        this.StartHost();
    }

    public void OnJoinClicked()
    {
        SetIPAddress();
        SetPort();
        this.StartClient();

        MenuPanel.SetActive(false);
        PlayerPanel.SetActive(true);

        PlayerPanelBackButton.onClick.RemoveAllListeners();
        PlayerPanelBackButton.onClick.AddListener(OnClientBackPressed);
    }   

    public override void OnStartHost()
    {
        base.OnStartHost();

        Debug.Log("Host started");

        SetPort();
        //NetworkManager.singleton.StartHost();

        MenuPanel.SetActive(false);
        PlayerPanel.SetActive(true);

        PlayerPanelBackButton.onClick.RemoveAllListeners();
        PlayerPanelBackButton.onClick.AddListener(OnHostBackPressed);
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level == 0) //Main Menu
        {
            DestroyImmediate(this.gameObject);
        } 
        else if (level == 1)//Lobby
        {
            //Do nothing
        } 
        else if (level == 3)//Game
        {
            //Hide the lobby Menus
            MenuPanel.SetActive(false);
            PlayerPanel.SetActive(false);
            TopText.SetActive(false);
        }
    }

	public override void OnLobbyClientSceneChanged(NetworkConnection conn)
	{
		base.OnLobbyClientSceneChanged(conn);

		MenuPanel.SetActive(false);
		PlayerPanel.SetActive(false);
		TopText.SetActive(false);
	}

    public void OnMainMenuBackPressed()
    {
		SceneManager.LoadScene("MainMenu");

    }

	public override void OnLobbyServerPlayersReady()
	{
		Debug.Log("All players are ready");

		bool ready = true;
		foreach (LobbyPlayer p in lobbySlots)
		{
			if(!p)
			{				
				continue;
			}

			if(!p.readyToBegin)
				ready = false;			
		}

		if(ready)
		{
			GameManager.instance.SetHomeKit (HomeSettings);
			GameManager.instance.SetAwayKit (AwaySettings);

			ServerChangeScene(playScene);
		}
	}

	//public override void ServerChangeScene(string scene)
	//{		
	//	SceneManager.LoadScene(scene);
	//}

    public void OnHostBackPressed()
    {
        //Apparently you have to stop the client if you're the host
        StopHost();

        MenuPanel.SetActive(true);
        PlayerPanel.SetActive(false);
    }

    public void OnClientBackPressed()
    {
        //Apparently you have to stop the host if you're the client
        StopClient();
        
        MenuPanel.SetActive(true);
        PlayerPanel.SetActive(false);
    }

    void SetIPAddress()
    {
        //string ip = "10.138.8.64";
		string ip = IPField.text;
		if(ip == "")
			ip = "localhost";
        NetworkManager.singleton.networkAddress = ip;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    public void AddPlayer(LobbyPlayer player)
    {
        //if(HomePlayerListTransform.childCount > AwayPlayerListTransform.childCount)
          //  player.transform.SetParent(AwayPlayerListTransform, false);
        //else
		player.UI.SetParent(HomePlayerListTransform, false);
    }

    public void SetTeam(Transform t, string team)
    {		
        if(team == "Home")
            t.SetParent(HomePlayerListTransform, false);
        else
            t.SetParent(AwayPlayerListTransform, false);
    }

    //Server Callbacks
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		base.OnServerAddPlayer (conn, playerControllerId);
		Debug.Log ("OnServerAddPlayer");
	}

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

        Debug.Log("Adding Player - Connection: " + conn.connectionId);
        LobbyPlayers.Add(conn.connectionId, obj.GetComponent<LobbyPlayer>());

        return obj;
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
		GameObject player = (GameObject) Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);

		int HairColor = LobbyPlayers [conn.connectionId].HairColor;

		Debug.Log (LobbyPlayers [conn.connectionId].playerName + " " + conn.connectionId);

		player.GetComponent<Player> ().SetAvatar (HairColor, 0, 0, 0);

		NetworkServer.DestroyPlayersForConnection (conn);
		NetworkServer.AddPlayerForConnection(conn, player, 0);

		Debug.Log ("HairColor " + HairColor);

        return player; 
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        gamePlayer.GetComponent<PlayerSync>().SetLobbyPlayer(lobbyPlayer.GetComponent<LobbyPlayer>());

        return true;
    }
   
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Removing Player - Connection: " + conn.connectionId);
        LobbyPlayers.Remove(conn.connectionId);
    }

	public void ChangeTeamSetting(string setting, string team)
	{
		if (team == "Home") 
		{
			if (setting == "Shirt")
				HomeSettings.OnShirtPressed ();
			else if (setting == "Sleeves")
				HomeSettings.OnSleevesPressed ();
			else if (setting == "Shorts")
				HomeSettings.OnShortsPressed ();
		} 
		else if (team == "Away") 
		{
			if (setting == "Shirt")
				AwaySettings.OnShirtPressed ();
			else if (setting == "Sleeves")
				AwaySettings.OnSleevesPressed ();
			else if (setting == "Shorts")
				AwaySettings.OnShortsPressed ();
		}
	}
}
