using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenPositioning : MonoBehaviour 
{

    private List<GameObject> m_GameplayVolumes;
    private Camera m_ThisCamera;


    public float smoothingDuration;


    private GameObject m_BottomLeft;
    private Vector3 m_BottomLeft_worldPositionToLookAt;
    private Vector3 m_BottomLeft_screenPositionPlacement;

    private GameObject m_TopRight;
    private Vector3 m_TopRight_worldPositionToLookAt;
    private Vector3 m_TopRight_screenPositionPlacement;

	// Use this for initialization
	void Start () {
        m_GameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        m_ThisCamera = GetComponent<Camera>();

        m_BottomLeft = new GameObject();
        m_TopRight = new GameObject();
	}
	
	// Update is called once per frame
	void Update () {

       float maxAngleDifference = 0.0f;

       for (int i = 0; i < m_GameplayVolumes.Count; i++)
        {
            float tempAngleDifference = 0.0f;

            tempAngleDifference = Vector3.Angle(transform.forward, m_GameplayVolumes[i].transform.position - transform.position);

            if (tempAngleDifference > maxAngleDifference)
            {
                maxAngleDifference = tempAngleDifference;
            }
        }




       if (m_GameplayVolumes.Count > 0)
        {

            //FOLLOW THEM
            for (int i = 0; i < m_GameplayVolumes.Count; i++)
            {

                if (m_GameplayVolumes[i].GetComponent<GameplayVolume>().m_ScreenPosition == GameplayVolume.ScreenPosition.BottomLeft)
                {
                    m_BottomLeft_worldPositionToLookAt = m_GameplayVolumes[i].transform.position;
                    m_BottomLeft_screenPositionPlacement = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumes[i].transform.position);
                    m_BottomLeft_screenPositionPlacement += new Vector3(Screen.width / 6, Screen.height / 6);
                }

                if (m_GameplayVolumes[i].GetComponent<GameplayVolume>().m_ScreenPosition == GameplayVolume.ScreenPosition.TopRight)
                {
                    m_TopRight_worldPositionToLookAt = m_GameplayVolumes[i].transform.position;
                    m_TopRight_screenPositionPlacement = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumes[i].transform.position);
                    m_TopRight_screenPositionPlacement += new Vector3(-Screen.width / 6, -Screen.height / 6);
                }
                
                
            }
        }



       gameObject.transform.LookAt(m_ThisCamera.ScreenToWorldPoint(m_BottomLeft_screenPositionPlacement));




        //if(distance from screen placements is more that 1/3 of Sqrt(b^2 + c^2))
        //move closer
        //else move farther

        //if (Vector3.Distance(m_BottomLeft_screenPositionPlacement, m_TopRight_screenPositionPlacement) > Mathf.Sqrt((Screen.width * Screen.width) + (Screen.height * Screen.height)) / 3.0f)
        Debug.Log(m_BottomLeft_screenPositionPlacement); 
        Debug.Log(m_TopRight_screenPositionPlacement);
        Debug.Log(Vector3.Distance(m_BottomLeft_screenPositionPlacement, m_TopRight_screenPositionPlacement));

        if(Vector3.Distance(m_BottomLeft_screenPositionPlacement, m_TopRight_screenPositionPlacement) < 10.0f)
        {
            //move away
            transform.position -= transform.forward * 0.03f;
        }
        else
        {
            transform.position += transform.forward * 0.03f;
        }
        
        //transform.position += (worldPositionToLookAt - transform.position) * (Time.deltaTime / smoothingDuration) * ((thisCamera.fieldOfView / 2) - maxAngleDifference);
        //



	}
}
