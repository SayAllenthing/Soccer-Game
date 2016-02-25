using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameSoundManager : NetworkBehaviour
{
	public static GameSoundManager Instance;

	public AudioSource GoalMusic;
	public AudioSource SFX;
	public AudioSource Crowd;

	public List<AudioClip> Clips;



	// Use this for initialization
	void Start () 
	{
		Instance = this;
	}
	
	public void PlaySound(string sound)
	{
		RpcPlaySound(sound);
	}

	[ClientRpc]
	void RpcPlaySound(string sound)
	{
		if(sound == "Kick")
		{
			SFX.clip = GetClip(sound);
			SFX.Play();
		}

		if(sound == "CrowdGoal" || sound == "Crowd")
		{
			if(Crowd.clip.name == sound)
				return;
			
			Crowd.clip = GetClip(sound);
			Crowd.Play();
		}

		if(sound == "GoalMusic")
		{
			GoalMusic.Play();
		}
	}

	AudioClip GetClip(string s)
	{
		for(int i = 0; i < Clips.Count; i++)
		{
			if(Clips[i].name == s)
				return Clips[i];
		}

		return null;
	}
}
