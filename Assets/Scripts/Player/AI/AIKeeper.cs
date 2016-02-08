using UnityEngine;
using System.Collections;

public class AIKeeper : Actor 
{
    enum PlayStyle
    {
        FOLLOW_BALL,
        STAY_MIDDLE
    }

    PlayStyle style;

    GameObject ball;

    Vector3 WantPosition;

    float aiTimer;
    float BaseX;

    bool bIsInitialized = false;

	// Use this for initialization
	void Start ()
    {
       
	}

    public void Init()
    {
        style = PlayStyle.FOLLOW_BALL;
        WantPosition = transform.position;
        MoveThreshold = 0.5f;
        aiTimer = 0;
        BaseX = transform.position.x;
        
        bAllowTurn = false;
        
        Speed = 11;
        SprintSpeed = 11;
        
        avatar.SetRandomAvatar();
        network.SendAvatar(avatar.CurrentHairColour, avatar.CurrentHair, avatar.CurrentBrow, avatar.CurrentSkinColour);

        bIsInitialized = true;
    }

    public void SetupAI(Game g)
    {
        //Send Avatar again, as it's likely some players connecting later don't have it.
        network.SendAvatar(avatar.CurrentHairColour, avatar.CurrentHair, avatar.CurrentBrow, avatar.CurrentSkinColour);
    }

    public override void OnGameReset()
    {
        ball = GameObject.Find("Ball") as GameObject;
    }
	
	// Update is called once per frame
	void Update () 
    {
        base.Update();

        if (!bIsInitialized)
            return;

        if (Time.time > aiTimer)
        {
            HandleAI();
        }
	}

    void HandleAI()
    {
        FindPosition();

        aiTimer = Time.time + 0.2f;
    }

    void FindPosition()
    {
        if (ball == null)
            return;

        Vector3 ballPos = ball.transform.position;
        ballPos.z = Mathf.Clamp(ballPos.z, -8, 8);

        WantPosition = (ballPos - transform.position);

        float xDist = ballPos.x - transform.position.x;

        WantPosition.y = 0;
        WantPosition.x = 0;

        if (Mathf.Abs(xDist) < 0.5f)
            xDist = 0;

        float ForwardDir = transform.localScale.x;

        if (xDist < 0)//Ball is to the left of keeper
        {
            if(ForwardDir == 1)//Keeper facing right
            {
                if(transform.position.x > BaseX - 1.5f)
                    WantPosition.x = -1;
            }
            else//Keeper facing left
            {
                if(transform.position.x > BaseX)//If keeper right of starting pos
                    WantPosition.x = -1;
                else if(transform.position.x < BaseX - 2f)
                    WantPosition.x = 1;
            }
        } 
        else if(xDist > 0)//Ball is to the right of keeper
        {
            if(ForwardDir == 1)//Keeper facing right
            {
                if(transform.position.x < BaseX)
                    WantPosition.x = 1;
                else if(transform.position.x > BaseX + 2f)
                    WantPosition.x = -1;
            }
            else//Keeper facing left
            {
                if(transform.position.x < BaseX + 1.5f)
                    WantPosition.x = 1;
            }
        }       

        if (WantPosition.magnitude < 1)
            WantPosition.z = 0;

        WantPosition = WantPosition.normalized;

        network.WantDir = new Vector2(WantPosition.x, WantPosition.z);
    }
}
