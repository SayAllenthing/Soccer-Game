using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Actor
{
    GameObject ball;
    
    Vector3 WantPosition;
    
    float aiTimer;
    
    bool bIsInitialized = false;

    bool bAiIsSetup = false;

    Game game;

    float myNetDir = 0;
    Vector3 myNet;

    Vector3 ballPos;
    Vector3 ballToMe;
    Vector3 ballToMyNet;
    Vector3 meToMyNet;
    Vector3 ballVel;

    float CloseDistance = 3;

    bool inPosition = false;

    enum AIBaseState
    {
        NONE,
        MOVETOPOSITION,
        DEFENSE,
        OFFENSE
    }

    enum AIDefenseState
    {
        NONE,
        DEFEND,
        GETCLEARINGPOSITION,
        CLEARBALL
    }

    AIBaseState baseState;
    AIDefenseState defenseState;

    // Use this for initialization
    void Start()
    {
	
    }

    public void Init()
    {
        WantPosition = transform.position;

        aiTimer = 0;
        
        avatar.SetRandomAvatar();
        network.SendAvatar(avatar.CurrentHairColour, avatar.CurrentHair, avatar.CurrentBrow, avatar.CurrentSkinColour);
        
        bIsInitialized = true;
    }

    public override void OnGameReset()
    {
          
    }

	public void OnBallSpawned(GameObject b)
	{
		if(b)			
			ball = b;
	}

    public void SetupAI(Game g)
    {
        game = g;

        Transform net = game.GetMyNet(this);
        myNet = net.position;
        myNet.y = 0;

        //Get the direction of your own net, so the AI knows which way to go
        myNetDir = net.localScale.x * -1;

        bAiIsSetup = true;

        network.Sprinting = true;
    }

    void HandleAI()
    {
        SetBaseState();
        GetPositions();

        if (baseState == AIBaseState.DEFENSE)
        {
            SetDefensiveState();
        } 
        else if (baseState == AIBaseState.MOVETOPOSITION)
        {
            if(inPosition)
                baseState = AIBaseState.NONE;
        }

        FindPosition();

        aiTimer += 0.25f;
    }

    void GetPositions()
    {
		if(!ball)
		{
			ball = GameObject.Find("Ball") as GameObject;    
			return;
		}

        //Find where the ball is discluding how high it is.
        ballPos = ball.transform.position;
        ballPos.y = 0;

        ballToMe = ballPos - transform.position;
        ballToMyNet = ballPos - myNet;

        meToMyNet = transform.position - myNet;

        ballVel = ball.GetComponent<Rigidbody>().velocity;
    }

    void SetBaseState()
    {
        //Check if we have somewhere to be
        if (baseState == AIBaseState.MOVETOPOSITION)
            return;

        //Defend for now
        baseState = AIBaseState.DEFENSE;

        //WantPosition = GetRealisticPosition(new Vector3(-85, 0, 0));
        //baseState = AIBaseState.MOVETOPOSITION;
    }

    void SetDefensiveState()
    {
        if (ball == null)
            return;

        if (!IsBehindBall(10, false))
            GetInDefensivePosition();
        else if(IsBallInMyHalf())
            GetInClearingPosition();
    }

    void GoToPosition(Vector3 pos)
    {
        inPosition = false;
        WantPosition = GetRealisticPosition(pos);
        baseState = AIBaseState.MOVETOPOSITION;
    }

    void FindPosition()
    {
        //Is this position too close to my net?
        //WantPosition = IsTooCloseToNet(WantPosition, 15);

        Vector3 pos = WantPosition - transform.position;
        pos.y = 0;

        if (pos.magnitude < CloseDistance)
        {

            OnInPosition();
            return;
        }

        inPosition = false;

        network.WantDir.x = pos.normalized.x;
        network.WantDir.y = pos.normalized.z;
    }  
	
    // Update is called once per frame
    void Update()
    {
        base.Update();
        
        if (!bIsInitialized)
            return;
        
        if (bAiIsSetup && Time.time > aiTimer)
        {
            HandleAI();
        }
    }

    //Callbacks
    void OnInPosition()
    {
        //At Position
        network.WantDir = Vector2.zero;
        Faceball();

        inPosition = true;
    }

    //Helper functions
    bool IsBehindBall(float padding, bool considerNetPosition = true)
    {
        float DisX = (ballToMe.x * myNetDir) + padding;

        if (!considerNetPosition)
        {
            return (ballToMe.x * myNetDir) + padding < 0;
        }

        return false;
    }

    bool IsBallInMyHalf()
    {
        return ballPos.x * myNetDir > 0;
    }

    Vector3 GetRealisticPosition(Vector3 pos)
    {
        //Don't go off the field
        pos.x = Mathf.Clamp(pos.x, -85, 85);
        pos.z = Mathf.Clamp(pos.z, -28, 28);

        //Stay 20 units away from your net
        Vector3 dis = pos - myNet;
        if (dis.magnitude < 20)
        {
            pos = myNet + dis.normalized * 20;
        }

        return pos;
    }

    //Actions
    void Faceball()
    {
        if (ball == null)
            return;

        if (ball.transform.position.x < transform.position.x)
            network.ChangeDirection(-1);
        else
            network.ChangeDirection(1);
    }

    void GetBehindBall()
    {
        Vector3 pos = myNet + ballToMyNet/1.2f;
        GoToPosition(pos);
    }

    void GetInDefensivePosition()
    {        
        //Debug.Log("Am I behind the ball? " + IsBehindBall(2, false));

        network.Shooting = false;

        GetBehindBall();

    }

    void GetInClearingPosition()
    {
        Vector3 pos = myNet + ballToMyNet/2f;
        GoToPosition(pos);

        /*
        Vector3 pos = ballPos;
        WantPosition = GetRealisticPosition(pos);

        network.Shooting = true;
        */
    }
}

