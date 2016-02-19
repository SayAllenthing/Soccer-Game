using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

    float CloseDistance = 50;
    float FarDistance = 80;

    float BallDistance = 25;

    Transform MyPlayer;
    Transform MyBall;

	public void Init(Transform p)
    {
        MyPlayer = p;
        this.GetComponent<Camera>().enabled = true;
    }

    public void SetBall(Transform b)
    {
        MyBall = b;
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if(MyPlayer == null || MyBall == null)
            return;

        float dis = (MyBall.position -MyPlayer.position).magnitude;
        float wantPos = CloseDistance;

        if (dis > BallDistance)
        {
            dis = (dis - BallDistance) + CloseDistance;
            wantPos = Mathf.Clamp(dis, CloseDistance, FarDistance);
        }

        Vector3 MyPos = transform.localPosition;
        MyPos.z = -wantPos;

        transform.localPosition = Vector3.Lerp(transform.localPosition, MyPos, 10 * Time.deltaTime);
	}
}
