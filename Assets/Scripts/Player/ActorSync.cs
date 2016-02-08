using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ActorSync : NetworkBehaviour {

    [SerializeField] protected NetworkAnimator anim;

    [SerializeField] Transform myTransform;
    [SerializeField] float lerpRate = 15;
    [SerializeField] Rigidbody myRigidbody;

    [SyncVar]
    public Vector3 SyncVel = Vector3.zero;
    [SyncVar]
    protected Vector3 SyncPos;
    [SyncVar]
    protected float SyncDir = 1;

    //Avatar
    public PlayerAvatar avatar;
    protected bool AvatarSent = false;

    [SyncVar]
    int SyncHairColour = -1;
    [SyncVar]
    int SyncHair = -1;
    [SyncVar]
    int SyncBrow = -1;
    [SyncVar]
    int SyncSkin = -1;

    //Player State Variables
    public Vector2 WantDir;
    public bool Shooting;
    public bool Sprinting;

    protected Actor player;

    float KickLock = -1;

    public bool bIsServer = false;

	// Use this for initialization
    public override void PreStartClient()
    {
        player = GetComponent<Actor>();
        
        anim.SetParameterAutoSend(0, true);
        anim.SetParameterAutoSend(1, true);
    }

    public override void OnStartServer()
    {
        player = GetComponent<Actor>();

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;

        GameObject.Find("GameManager").GetComponent<GameManager>().AddActor(player);

        if (gameObject.tag == "Keeper")
        {
            (player as AIKeeper).Init();
        }
        else if (gameObject.tag == "AI")
        {
            (player as AIPlayer).Init();
        }
    }

	void Start () 
    {
        if (!isLocalPlayer)
        {
            transform.eulerAngles = new Vector3(45, 0, 0);
        }
	}
	
	// Update is called once per frame
	protected void Update () 
    {
        if (NetworkServer.active)        
            ServerUpdate();

        LerpPosition();

        if (tag != "Keeper")
            SetAvatar();
	}

    protected void ServerUpdate()
    {
        SendPosition();
        
        if (KickLock > 0 && Time.time > KickLock)
            KickLock = -1;
    }

    //Position and Direction syncing================================================================
    void LerpPosition()
    {
        //Only lerp other characters
        if (!NetworkServer.active)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, SyncPos, Time.deltaTime * lerpRate);

            if(player.bAllowTurn)
                ChangeDirection(SyncDir);
        }
    }
    
    public void ChangeDirection(float dir)
    {
        if (dir > 0)
            dir = 1;
        else if (dir < 0)
            dir = -1;
        else
            return;
        
        Vector3 scale = transform.localScale;
        scale.x = dir;
        transform.localScale = scale;
    }
    
    [Server]
    void UpdatePosition(Vector3 pos, float dir)
    {
        SyncPos = pos;
        SyncDir = dir;
    }
    
    [ServerCallback]
    void SendPosition()
    {
        float wantSpeed = player.GetSpeed(Sprinting);
        
        SyncVel = new Vector3(WantDir.x, 0, WantDir.y) * wantSpeed;
        
        myRigidbody.velocity = SyncVel;

        if(player.bAllowTurn)
            ChangeDirection(WantDir.x);
        UpdatePosition(myTransform.position, myTransform.localScale.x);
    }
    //==============================================================================================


    //ANIMATION TRIGGERS============================================================================
    [ClientCallback]
    public void SetAnimTrigger(string trigger)
    {       
        CmdSetAnimTrigger(trigger);
    }
    
    [ClientRpc]
    protected void RpcSetAnimTrigger(string trigger)
    {      
        //localAnim.SetTrigger(trigger);
        if (player.GetCurrentAnimationName() != trigger)
        {
            player.anim.SetTrigger(trigger);
            player.ColiderAnim.SetTrigger(trigger);
        }
    }   
    
    [Command]
    protected void CmdSetAnimTrigger(string trigger)
    {
        RpcSetAnimTrigger(trigger);
        player.ColiderAnim.SetTrigger(trigger);
    }
    //==============================================================================================

    //Events=============================================================================
    [Server]
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ball")
        {
            if(KickLock > 0)
                return;

            if(gameObject.tag == "Keeper")
            {
                OnKeeperSave(col.gameObject.GetComponent<Ball>());                
                return;
            }
            
            Ball theBall = col.gameObject.GetComponent<Ball>();
            
            if(player.GetCurrentAnimationName() == "Sliding")
            {
                theBall.Slide(SyncVel);
            }
            else if(Shooting)
            {
                theBall.Shoot(WantDir.normalized, SyncVel, transform.localScale.x);
                RpcSetAnimTrigger("Shooting");
                KickLock = Time.time + 0.8f;
                return;
            }
            else
            {
                theBall.Dribble(SyncVel);
            }
            
            KickLock = Time.time + 0.5f;
            //Do shoot/dribble code here
            //I can now pass it off to the ball itself
        }
    }

    [Server]
    void OnKeeperSave(Ball ball)
    {
        Rigidbody rigBall = ball.GetComponent<Rigidbody>();

        bool deflect = true;

        if (ball.transform.position.y > 1.5f)
        {
            RpcSetAnimTrigger("StandSave");
        } 
        else if (rigBall.velocity.magnitude > 12)
        {
            RpcSetAnimTrigger("Sliding");
        } 
        else
        {
            RpcSetAnimTrigger("Shooting");
            deflect = false;
        }

        if (deflect)
            rigBall.AddForce(transform.localScale.x * Random.Range(4, 10), Random.Range(2, 5), Random.Range(-10, 10));
        else
            rigBall.AddForce(transform.localScale.x * Random.Range(50, 60), Random.Range(15, 25), Random.Range(-2, 2));

        KickLock = Time.time + 0.2f;
    }

    [Command]
    public void CmdAction(string action)
    {
        if(player.GetCurrentAnimationName() != action)
            RpcSetAnimTrigger(action);
    }

    [Command]
    protected void CmdUpdateAvatar(int colour, int hair, int brow, int skin)
    {
        SyncHairColour = colour;
        SyncHair = hair;
        SyncBrow = brow;
        SyncSkin = skin;

        //Keepers start in the scene so they need to be done differently
        if (tag == "Keeper")
            RpcSetAvatar(colour, hair, brow, skin);
        //Debug.Log("Getting Avatar - " + colour + " " + hair + " " + brow);
    }

    public void SendAvatar(int colour, int hair, int brow, int skin)
    {
        CmdUpdateAvatar(colour, hair, brow, skin);
        
        AvatarSent = true;
    }

    [ClientRpc]
    protected void RpcSetAvatar(int colour, int hair, int brow, int skin)
    {
        avatar.ChangeHairColour(SyncHairColour);
        avatar.ChangeHair(SyncHair);
        avatar.ChangeBrow(SyncBrow);
        avatar.ChangeSkin(SyncSkin);

        AvatarSent = true;
    }
    
    protected void SetAvatar()
    {
        if (tag == "Keeper")
        {
            Debug.Log("Trying to get Keeper");
        }

        if (AvatarSent || !avatarReady())
            return;
        
        if (!isLocalPlayer)
        {
            avatar.ChangeHairColour(SyncHairColour);
            avatar.ChangeHair(SyncHair);
            avatar.ChangeBrow(SyncBrow);
            avatar.ChangeSkin(SyncSkin);
        }
        
        AvatarSent = true;
    }

    bool avatarReady()
    {
        if (SyncHair < 0 || SyncHair > avatar.HairOptions.Count)
            return false;

        if (SyncHairColour < 0 || SyncHairColour > avatar.HairColours.Count)
            return false;

        if (SyncBrow < 0 || SyncBrow > avatar.BrowOptions.Count)
            return false;

        if (SyncSkin < 0 || SyncSkin > avatar.SkinColours.Count)
            return false;

        return true;
    }

    public void AddToTeam(Team t)
    {
        RpcAddToTeam(t.SleeveColour, t.JerseyColour, t.KeeperColor, t.ShortsColor);
    }

    [ClientRpc]
    void RpcAddToTeam(Color s, Color j, Color k, Color sh)
    {
        Team t = new Team();
        t.SleeveColour = s;
        t.JerseyColour = j;
        t.KeeperColor = k;
        t.ShortsColor = sh;

        player.SetKit(t);
    }
}
