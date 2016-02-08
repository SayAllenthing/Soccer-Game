using UnityEngine;
using System.Collections;

public class BallRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        float z = transform.parent.rotation.z; 
        transform.localRotation = Quaternion.Euler(45, 0, 0);
	}
}
