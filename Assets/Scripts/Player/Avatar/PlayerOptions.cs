using UnityEngine;
using System.Collections;

public class PlayerOptions : MonoBehaviour 
{
    public int HairStyle = 0;
    public int HairColor = 0;
    public int BrowStyle = 0;
    public int SkinColor = 0;

    public string Username = "Player";

    public static PlayerOptions CurrentPlayerOptions = null;

	// Use this for initialization
	void Start () 
    {
        if (CurrentPlayerOptions != null)
        {
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(this);
        CurrentPlayerOptions = this;
	}
	
}
