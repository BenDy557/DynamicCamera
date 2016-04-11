using UnityEngine;
using System.Collections;

public class LaserSpaceShooter : MonoBehaviour {

    public float mProjectileSpeed;
    private Rigidbody mRigidBody;

	// Use this for initialization
	void Start ()
    {
        mRigidBody = GetComponent<Rigidbody>();

        mRigidBody.velocity = transform.forward * mProjectileSpeed;
	}
	
	// Update is called once per frame
	void Update ()
    {

        //mRigidBody.velocity = transform.forward * mProjectileSpeed;
	}
}
