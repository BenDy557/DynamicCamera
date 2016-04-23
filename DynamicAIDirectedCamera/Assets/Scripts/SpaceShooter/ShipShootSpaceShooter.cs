using UnityEngine;
using System.Collections;

public class ShipShootSpaceShooter : MonoBehaviour {

    private GameObject mBulletPrefab;
    public GameObject mAimingReticule;
    public GameplayVolume mIsoCameraVolume;
    public GameObject mPlayerGameplayVolume;
    public GameObject[] mTargets;
    public float mRateOfFire;

    bool mTargetLock;

	// Use this for initialization
	void Start ()
    {
        mTargetLock = false;


        mBulletPrefab = (GameObject)Resources.Load("SpaceShooter/Prefabs/Laser");

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Input.GetButtonDown("XboxPlayer1Target"))
        {
            mTargetLock = !mTargetLock;
        }


        Vector3 tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        mAimingReticule.transform.position = tempPos;


        if (mTargetLock)
        {
            mTargets[0].GetComponentInChildren<GameplayVolume>().m_Active = true;
            mIsoCameraVolume.m_Active = false;
            mPlayerGameplayVolume.GetComponent<GameplayVolume>().m_ScreenPosition = GameplayVolume.ScreenPosition.BottomLeft;
            mPlayerGameplayVolume.GetComponent<GameplayVolume>().m_ScreenSize = 0.15f;
            mAimingReticule.transform.LookAt(mTargets[0].transform);
            mAimingReticule.SetActive(true);
        }
        else
        {
            mTargets[0].GetComponentInChildren<GameplayVolume>().m_Active = false;
            mIsoCameraVolume.m_Active = true;
            

            //mAimingReticule.transform.Rotate(new Vector3(0.0f, 2.5f * Input.GetAxis("XboxPlayer1RightStickAxisX")));
            //mAimingReticule.transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            mAimingReticule.SetActive(false);

            mPlayerGameplayVolume.GetComponent<GameplayVolume>().m_ScreenPosition = GameplayVolume.ScreenPosition.Custom;
            mPlayerGameplayVolume.GetComponent<GameplayVolume>().m_ScreenSize = 0.002f;
        }
    
        
        


        if (Input.GetButtonDown("XboxPlayer1RightBumper"))
        {
            if (mTargetLock)
            {
                Instantiate(mBulletPrefab, transform.position, mAimingReticule.transform.rotation);
            }
            else
            {
                Instantiate(mBulletPrefab, transform.position, transform.rotation);
            }
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
