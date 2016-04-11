using UnityEngine;
using System.Collections;

public class ShipShootSpaceShooter : MonoBehaviour {

    private GameObject mBulletPrefab;
    public GameObject mAimingReticule;
    private GameplayVolume mReticuleCameraVolume;
    public GameObject[] mTargets;
    public float mRateOfFire;

    bool mTargetLock;

	// Use this for initialization
	void Start ()
    {
        mTargetLock = false;

        mReticuleCameraVolume = mAimingReticule.GetComponentInChildren<GameplayVolume>();

        mBulletPrefab = (GameObject)Resources.Load("SpaceShooter/Prefabs/Laser");

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetButtonDown("XboxPlayer1Target"))
        {
            mTargetLock = !mTargetLock;
        }


        if (mTargetLock)
        {
            mTargets[0].GetComponentInChildren<GameplayVolume>().m_Active = true;
            mReticuleCameraVolume.m_Active = false;
        }
        else
        {
            mTargets[0].GetComponentInChildren<GameplayVolume>().m_Active = false;
            mReticuleCameraVolume.m_Active = true;

            Vector3 tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            mAimingReticule.transform.position = tempPos;
            mAimingReticule.transform.Rotate(new Vector3(0.0f, 2.5f * Input.GetAxis("XboxPlayer1RightStickAxisX")));
        }

        if (Input.GetButtonDown("XboxPlayer1RightBumper"))
        {
            Instantiate(mBulletPrefab, transform.position, mAimingReticule.transform.rotation);
        }


	}

    Vector3 RotatePointAroundPivot(Vector3 pointIn, Vector3 pivotIn, Vector3 angles)
    {
        Vector3 dir = pointIn - pivotIn; // get point direction relative to pivot


        dir = Quaternion.Euler(angles) * dir; // rotate it
        pointIn = dir + pivotIn; // calculate rotated point
        return pointIn; // return it
    }
}
