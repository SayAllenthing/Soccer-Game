using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class Team
{

    public string name = "Home";

    public Color JerseyColour = Color.black;
    public Color SleeveColour = Color.black;
    public Color ShortsColor = Color.black;
    public Color KeeperColor = Color.black;

    List<string> Presets = new List<string>{"Home","Away"};

    List<Actor> Roster = new List<Actor>();

	public Team(string s)
	{
		name = s;
	}

	// Use this for initialization
	void Start () 
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SetKitRandom()
    {

    }

    public void SetKitDefault(string s)
    {
        if (s == "Home")
        {
            SetValues(Color.red, Color.white, Color.white);
        }
        else if (s == "Away")
        {
            SetValues(Color.blue, Color.blue, Color.black);
        }

		SetKeeperKit (s);
    }

	public void SetKeeperKit(string s)
	{
		if (s == "Home") 
		{
			KeeperColor = new Color (0.8f, 0.7f, 0.05f);
		} 
		else 
		{
			KeeperColor = Color.grey;
		}
	}

	public void SetKitColors(Color shirt, Color sleeves, Color shorts)
	{		
		SetValues(shirt, sleeves, shorts);
	}

    public void SetValues(Color Jersey, Color Sleeve, Color Shorts)
    {
        JerseyColour = Jersey;
        SleeveColour = Sleeve;
        ShortsColor = Shorts;
    }

    public void AddPlayer(Actor a)
    {
        Roster.Add(a);
    }

    public List<Actor> GetPlayers()
    {
        return Roster;
    }
}
