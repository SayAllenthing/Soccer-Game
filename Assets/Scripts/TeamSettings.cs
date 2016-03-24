using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamSettings : NetworkBehaviour
{
	Color[] Colors = new Color[] {Color.white, Color.black, 
		Color.blue, Color.red, Color.gray,
		new Color(63f/255f, 129f/255f, 63f/255f),
		new Color(116f/255f, 195f/255f, 221f/255f),
		new Color(210f/255f, 216f/255f, 77f/255f),
		};

	public string TeamName = "Home";

	[SyncVar(hook = "OnShirtColor")]
	public int ShirtColor = 0;

	[SyncVar(hook = "OnSleevesColor")]
	public int SleevesColor = 0;

	[SyncVar(hook = "OnShortsColor")]
	public int ShortsColor = 0;

	public Image ImageShirtColour;
	public Image ImageSleevesColour;
	public Image ImageShortsColour;

	public Button BtnShirt;
	public Button BtnSleeves;
	public Button BtnShorts;

	public override void PreStartClient()
	{
		//setup the player data on UI. The value are SyncVar so the player
		//will be created with the right value currently on server
		OnShirtColor(ShirtColor);
		OnSleevesColor(SleevesColor);
		OnShortsColor(ShortsColor);
	}

	//Events
	public void OnShirtPressed()
	{
		if (!isServer) 
		{
			LobbyPlayer.MyPlayer.AskToChangeTeamSettings ("Shirt", TeamName);
			return;
		}

		int s = ShirtColor;
		s++;
		if (s == Colors.Length)
			s = 0;

		CmdKitChanged (s, SleevesColor, ShortsColor);
	}

	public void OnSleevesPressed()
	{
		if (!isServer) 
		{
			LobbyPlayer.MyPlayer.AskToChangeTeamSettings ("Sleeves", TeamName);
			return;
		}

		int s = SleevesColor;
		s++;
		if (s == Colors.Length)
			s = 0;

		CmdKitChanged (ShirtColor, s, ShortsColor);
	}

	public void OnShortsPressed()
	{
		if (!isServer) 
		{
			LobbyPlayer.MyPlayer.AskToChangeTeamSettings ("Shorts", TeamName);
			return;
		}

		int s = ShortsColor;
		s++;
		if (s == Colors.Length)
			s = 0;

		CmdKitChanged (ShirtColor, SleevesColor, s);
	}

	//Hooks
	void OnShirtColor(int c)
	{
		ShirtColor = c;
		ImageShirtColour.color = Colors[c];
	}

	void OnSleevesColor(int c)
	{
		SleevesColor = c;
		ImageSleevesColour.color = Colors[c];
	}

	void OnShortsColor(int c)
	{
		ShortsColor = c;
		ImageShortsColour.color = Colors[c];
	}

	//Server Catch
	[Command]
	public void CmdKitChanged(int shirt, int sleeves, int shorts)
	{
		ShirtColor = shirt;
		SleevesColor = sleeves;
		ShortsColor = shorts;
	}

	//Getters
	public Color GetShirtColor()
	{
		return ImageShirtColour.color;
	}

	public Color GetSleevesColor()
	{
		return ImageSleevesColour.color;
	}

	public Color GetShortsColor()
	{
		return ImageShortsColour.color;
	}
}
