using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour 
{
    public ActorSync network;
    public PlayerAvatar avatar;

    public Animator anim;
    public Animator ColiderAnim;

    protected float MoveThreshold = 0.1f;
    protected float SprintThreshold = 12f;

    protected bool Sliding;

    protected float Speed = 11;
    protected float SprintSpeed = 15;

    [HideInInspector]
    public bool bAllowTurn = true;

    [SerializeField] Kit kit;

    public string Team = "Home";

	// Use this for initialization
	void Start () 
    {

	}

    public virtual void OnGameReset()
    {

    }
	
	// Update is called once per frame
	public void Update () 
    {
        HandleAnimation();
	}

    protected void HandleAnimation()
    {
        if (!anim)
            return;
        
        bool moving = network.SyncVel.magnitude > MoveThreshold;
        bool sprinting = network.SyncVel.magnitude > SprintThreshold;
        
        anim.SetBool("Moving", moving);
        anim.SetBool("Sprinting", sprinting);
        
        ColiderAnim.SetBool("Moving", moving);
        ColiderAnim.SetBool("Sprinting", sprinting);
    }

    public void SetAvatar(int colour, int hair, int brow, int skin)
    {
        avatar.ChangeHairColour(colour);
        avatar.ChangeBrow(brow);
        avatar.ChangeHair(hair);
        avatar.ChangeSkin(skin);
    }

    public string GetCurrentAnimationName()
    {       
        AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        
        if (currentBaseState.IsName("Player_Shoot"))
        {
            return "Shooting";
        }
        else if (currentBaseState.IsName("Player_Slide"))
        {
            return "Sliding";
        }
        
        return "";
    }

    public void SetTeamKit(Team t)
    {
        network.AddToTeam(t);
        SetKit(t);
    }

    public void SetKit(Team t)
    {
        kit.SetKit(t);
    }

    public float GetSpeed(bool sprint = false)
    {
        if (sprint)
            return SprintSpeed;

        return Speed;
    }
}
