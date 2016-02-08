using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class Team{

    public string name = "Home";

    public Color JerseyColour = Color.black;
    public Color SleeveColour = Color.black;
    public Color ShortsColor = Color.black;
    public Color KeeperColor = Color.black;

    List<string> Presets = new List<string>{"Home","Away"};

    List<Actor> Roster = new List<Actor>();

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

    public void SetKit(string s)
    {
        if (s == "Home")
        {
            SetValues(Color.red, Color.white, new Color(0.8f, 0.7f, 0.05f), Color.white);
        }
        else if (s == "Away")
        {
            SetValues(Color.blue, Color.blue, Color.grey, Color.black);
        }
    }

    public void SetValues(Color Jersey, Color Sleeve, Color Keeper, Color Shorts)
    {
        JerseyColour = Jersey;
        SleeveColour = Sleeve;
        KeeperColor = Keeper;
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
