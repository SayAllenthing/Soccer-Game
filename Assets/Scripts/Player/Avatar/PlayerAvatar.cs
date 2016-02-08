using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAvatar : MonoBehaviour {

    public SpriteRenderer EyeBrowL;
    public SpriteRenderer EyeBrowR;

    public List<SpriteRenderer> Skin;

    public List<Sprite> BrowOptions;
    public int CurrentBrow = 0;

    public List<Sprite> HairOptions;
    public SpriteRenderer Hair;
    public int CurrentHair = 0;

    Vector3[] HairOffsets = new Vector3[]
    {
        new Vector3(0f, -0.23f, 0f),//Hair 1
        new Vector3(0f, -0.23f, 0f),//Hair 2
        new Vector3(0f, -0.23f, 0f),//Hair 3
        new Vector3(0f, -0.23f, 0f),//Hair 4
        new Vector3(0.1f, -0.23f, 0f),//Hair 5
        new Vector3(0f, -0.23f, 0f),//Hair 6
        new Vector3(0f, -0.23f, 0f),//Hair 7
        new Vector3(0f, -0.1f, 0f),//Hair 8
        new Vector3(0f, -0.23f, 0f),//Bald
    };

    public List<Color> HairColours;
    public int CurrentHairColour = 0;

    public List<Color> SkinColours;
    public int CurrentSkinColour = 0;

    public int ChangeHair(int i = -1)
    {
        if (i < 0)
        {
            CurrentHair++;
            if (CurrentHair > HairOptions.Count)
            {
                CurrentHair = 0;
            }
        } 
        else
        {
            CurrentHair = i;
        }
       

        SetHair();

        return CurrentHair + 1;
    }

    public int ChangeHairColour(int i = -1)
    {
        if (i < 0)
        {
            CurrentHairColour++;
            if (CurrentHairColour >= HairColours.Count)
            {
                CurrentHairColour = 0;
            }
        }
        else
        {
            CurrentHairColour = i;
        }

        SetHair();
        
        return CurrentHairColour + 1;
    }

    public void SetHair()
    {
        if (CurrentHair == HairOptions.Count)
        {
            Hair.sprite = null;
        } 
        else
        {
            Hair.sprite = HairOptions [CurrentHair];
        }       

        EyeBrowL.color = HairColours [CurrentHairColour];
        EyeBrowR.color = HairColours [CurrentHairColour];

        Hair.color = HairColours [CurrentHairColour];
        Hair.transform.localPosition = HairOffsets [CurrentHair];
    }

    public int ChangeBrow(int i = -1)
    {
        if (i < 0)
        {
            CurrentBrow++;
            if (CurrentBrow >= BrowOptions.Count)
                CurrentBrow = 0;
        }
        else
        {
            CurrentBrow = i;
        }

        SetBrow();

        return CurrentBrow + 1;
    }

    public void SetBrow()
    {
        EyeBrowL.sprite = BrowOptions [CurrentBrow];
        EyeBrowR.sprite = BrowOptions [CurrentBrow];
    }

    public int ChangeSkin(int i = -1)
    {
        if (i < 0)
        {
            CurrentSkinColour++;
            if (CurrentSkinColour >= SkinColours.Count)
            {
                CurrentSkinColour = 0;
            }
        }
        else
        {
            CurrentSkinColour = i;
        }
        
        SetSkin();
        
        return CurrentSkinColour + 1;
    }

    public void SetSkin()
    {
        for(int i = 0; i < Skin.Count; i++)
        {
            Skin[i].color = SkinColours[CurrentSkinColour];
        }
    }

    public void SetRandomAvatar()
    {
        ChangeSkin(Random.Range(0, HairColours.Count));

        if (CurrentSkinColour > 2)
            ChangeHairColour(0);
        else
            ChangeHairColour(Random.Range(0, HairColours.Count));
        ChangeBrow(Random.Range(0, BrowOptions.Count));

        ChangeHair(Random.Range(0, HairOptions.Count + 1));
    }
}
