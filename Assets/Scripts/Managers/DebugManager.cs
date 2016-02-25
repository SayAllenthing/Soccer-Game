using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugManager : MonoBehaviour {

    public static DebugManager Instance;

    public CanvasGroup DebugPanel;
    public Text GameSpeedText;
	public Text ShotText;

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
		if (Input.GetKeyDown(KeyCode.P))
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

	public void OnShot(float power, Vector3 direction, Vector3 BallVel)
	{
		float height = direction.y;
		direction.y = 0;

		ShotText.text = "Last Shot:\n   Direction: " + direction + "\nBall Vel: " + BallVel + "\n   Power: " + power.ToString("0") + ", Height: " + height.ToString("0.0");
	}
}
