using UnityEngine;
using System.Collections;

public class MainMenuHandler : MonoBehaviour 
{
    public void GoToLobby()
    {
        Application.LoadLevel("Lobby");
    }

    public void GoToAvatar()
    {
        Application.LoadLevel("Customize");
    }
}
