﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour {


    BallSync network;
    Rigidbody rigidbody;

    public bool bInPlay = false;

    float MaxPower = 50;

	public List<ActorSync> ActorTouches = new List<ActorSync>();

	// Use this for initialization
	void Start () 
    {
        network = GetComponent<BallSync>();
        rigidbody = GetComponent<Rigidbody>();

        bInPlay = true;
	}	
	
    void Update()
    {

    }

    public void Dribble(Vector3 direction)
    {
        rigidbody.AddForce(direction * 0.5f);

		GameSoundManager.Instance.PlaySound("Kick");
    }

    public void Shoot(Vector2 direction, Vector3 force, float deadDir)
    {
        Vector3 dir = new Vector3(direction.x, 0, direction.y);

        dir += force;
		dir = dir.normalized;

		if (dir.magnitude < 1) 
		{			
			dir = new Vector3(deadDir, 0, rigidbody.velocity.normalized.z/2);
		}

		float Power = Mathf.Clamp(force.magnitude, 12, 15);
		Power *= Random.Range (2.8f, 3.2f);

		dir *= Power;


        //dir *= Random.Range(0.8f, 1.5f);

		/*
        if (dir.magnitude < 1)
        {
            float x = (deadDir + rigidbody.velocity.x) * 1.8f;
            if(x < 6)
                x = deadDir * 6;

            dir = new Vector3(x, 0, rigidbody.velocity.z/10);
        }
        */

        dir.y = Random.Range(5f, 30f);

        //if (dir.magnitude > MaxPower)
            //dir = ClampPower(dir);

		DebugManager.Instance.OnShot(Power, dir, rigidbody.velocity);

		rigidbody.AddForce(dir);

		GameSoundManager.Instance.PlaySound("Kick");
    }

    public void Head(Vector2 direction, Vector3 force, float deadDir)
    {
        Vector3 dir = new Vector3(direction.x, 0, direction.y);
        
        dir += force/6;
        
        dir *= Random.Range(0.8f, 1.5f);
        
        if (dir.magnitude < 1)
        {
            float x = (deadDir + rigidbody.velocity.x) * 1.8f;
            if(x < 6)
                x = deadDir * 6;
            
            dir = new Vector3(x, 0, rigidbody.velocity.z/10);
        }
        
        dir *= 6;
        
        dir.y = Random.Range(-20f, 25f);
        
        if (dir.magnitude > MaxPower)
            dir = ClampPower(dir);

		DebugManager.Instance.OnShot(force.magnitude, dir, rigidbody.velocity);
        
        rigidbody.AddForce(dir);

		GameSoundManager.Instance.PlaySound("Kick");
    }

	public void Cross(float dis)
	{
		float min = 25;

		Vector3 dir = new Vector3(-rigidbody.velocity.x, Mathf.Abs(dis)/1.8f, dis + 5);

		if(Mathf.Abs(dis) < min)
		{			
			dir.z = dis < 0? -min : min;
		}

		dir.y = Mathf.Clamp(dir.y, 12f,16f);

		dir *= 1.5f;

		dir.y *= Random.Range(0.9f, 1.1f);
		dir.z *= Random.Range(0.9f, 1.1f);

		Debug.Log(dir);

		rigidbody.AddForce(dir);

		GameSoundManager.Instance.PlaySound("Kick");
	}

    Vector3 ClampPower(Vector3 dir)
    {
        return dir.normalized * MaxPower;
    }

    public void Slide(Vector3 direction)
    {
        rigidbody.AddForce(direction * 2);
    }

	public void AddTouch(ActorSync a)
	{
		if (a.tag == "Keeper")
			return;

		ActorTouches.Add (a);

		if (ActorTouches.Count > 5)  
		{
			ActorTouches.Remove (ActorTouches [0]);
		}
	}

	public string GetGoalScorer()
	{
		if (ActorTouches.Count == 0)
			return "OG";

		return ActorTouches [ActorTouches.Count - 1].GetName ();
	}
}
