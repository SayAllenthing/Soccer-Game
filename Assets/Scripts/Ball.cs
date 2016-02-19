using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {


    BallSync network;
    Rigidbody rigidbody;

    public bool bInPlay = false;

    float MaxPower = 45;

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
    }

    public void Shoot(Vector2 direction, Vector3 force, float deadDir)
    {
        Vector3 dir = new Vector3(direction.x, 0, direction.y);

        dir += force/5;

        dir *= Random.Range(0.8f, 1.5f);

        if (dir.magnitude < 1)
        {
            float x = (deadDir + rigidbody.velocity.x) * 1.8f;
            if(x < 6)
                x = deadDir * 6;

            dir = new Vector3(x, 0, rigidbody.velocity.z/10);
        }

        dir *= 10;

        dir.y = Random.Range(5, 30);

        if (dir.magnitude > MaxPower)
            dir = ClampPower(dir);

        rigidbody.AddForce(dir);
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
        
        dir.y = Random.Range(-20, 30);
        
        if (dir.magnitude > MaxPower)
            dir = ClampPower(dir);
        
        rigidbody.AddForce(dir);
    }

    Vector3 ClampPower(Vector3 dir)
    {
        return dir.normalized * MaxPower;
    }

    public void Slide(Vector3 direction)
    {
        rigidbody.AddForce(direction * 2);
    }
}
