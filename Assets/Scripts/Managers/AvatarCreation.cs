using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class AvatarCreation : MonoBehaviour 
{
    PlayerOptions PO;

    void Start()
    {
        PO = PlayerOptions.CurrentPlayerOptions;
        Load();
    }

    public PlayerAvatar avatar;

    public Text HairButtonText;
    public Text HairColourButtonText;
    public Text BrowButtonText;
    public Text SkinButtonText;

    public InputField PlayerName;

    public void SetHair(int i = -1)
    {
        int hair = avatar.ChangeHair(i);
        HairButtonText.text = ("Hair " + hair);
    }

    public void SetHairColour(int i = -1)
    {
        int hair = avatar.ChangeHairColour(i);
        HairColourButtonText.text = ("Hair Colour " + hair);
    }

    public void SetBrow(int i = -1)
    {
        int brow = avatar.ChangeBrow(i);
        BrowButtonText.text = ("Brow " + brow);
    }

    public void SetSkin(int i = -1)
    {
        int skin = avatar.ChangeSkin(i);
        SkinButtonText.text = ("Skin " + skin);
    }   

    public void Save()
    {
        PO.HairColor = avatar.CurrentHairColour;
        PO.HairStyle = avatar.CurrentHair;
        PO.BrowStyle = avatar.CurrentBrow;
        PO.SkinColor = avatar.CurrentSkinColour;

        PO.Username = PlayerName.text;
    }

    public void Load()
    {
        SetHairColour(PO.HairColor);
        SetBrow(PO.BrowStyle);
        SetHair(PO.HairStyle);
        SetSkin(PO.SkinColor);

        PlayerName.text = PO.Username;
    }

    public void Back()
    {
        Save();
        Application.LoadLevel("MainMenu");
    }
	
}
