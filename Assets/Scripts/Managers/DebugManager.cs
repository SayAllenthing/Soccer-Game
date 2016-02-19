using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugManager : MonoBehaviour {

    public static DebugManager Instance;

    public CanvasGroup DebugPanel;
    public Text GameSpeedText;

    float ping = 0;

    float fps = 0;


    int counter = 0;

	// Use this for initialization
	void Start () 
    {
        Instance = this;

        GetComponent<RectTransform>().anchoredPosition = new Vector3(450, -7, 0);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugPanel.alpha = DebugPanel.alpha == 1 ? 0 : 1;
        }

        if (counter > 20)
        {
            UpdateValues();
            UpdateText();
            counter = 0;
        }
        counter++;
	}

    void UpdateValues()
    {
        fps = 1.0f / Time.deltaTime;
    }

    void UpdateText()
    {
        GameSpeedText.text = "FPS: " + fps.ToString("0.0") + "\nPing: " + ping.ToString("0.0") + "ms";
    }

    public void OnLevelWasLoaded(int level)
    {
        if (level != 3)
        {
            //Destroy
            Instance = null;
        }
    }

    public void SetPing(float p)
    {
        ping = p;
    }
}
