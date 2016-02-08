using UnityEngine;
using System.Collections;

public class Kit : MonoBehaviour {

    [SerializeField] SpriteRenderer ShortsL;
    [SerializeField] SpriteRenderer ShortsR;
    [SerializeField] SpriteRenderer ShortsM;
    
    [SerializeField] SpriteRenderer SleeveL;
    [SerializeField] SpriteRenderer SleeveR;
    [SerializeField] SpriteRenderer Jersey;

	public void SetKit(Team t)
    {
        if (gameObject.tag == "Keeper")
        {
            SleeveL.color = t.KeeperColor;
            SleeveR.color = t.KeeperColor;
            Jersey.color = t.KeeperColor;
        } 
        else
        {
            SleeveL.color = t.SleeveColour;
            SleeveR.color = t.SleeveColour;
            Jersey.color = t.JerseyColour;
        }

        ShortsL.color = t.ShortsColor;
        ShortsR.color = t.ShortsColor;
        ShortsM.color = t.ShortsColor;
    }
}
