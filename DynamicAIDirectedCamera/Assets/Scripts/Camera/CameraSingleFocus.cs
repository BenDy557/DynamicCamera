using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSingleFocus : MonoBehaviour {

    private List<GameObject> m_GameplayVolumes;
    private Camera m_ThisCamera;

    private Vector2 m_ScreenDimensions;
    private int m_ScreenPixelAmount;

    private float m_FOV;//horizontal
    private float m_FOVVertical;//vertical
    //direction vectors for third intersections//TODO
    
    private Vector3 m_BottomLeftAdjustment;
    private Vector3 m_TopLeftAdjustment;
    private Vector3 m_BottomRightAdjustment;
    private Vector3 m_TopRightAdjustment;
    

	// Use this for initialization
	void Start () {
        m_GameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        m_ThisCamera = GetComponent<Camera>();

        m_ScreenDimensions = new Vector2(Screen.width, Screen.height);
        m_ScreenPixelAmount = (int)(m_ScreenDimensions.x * m_ScreenDimensions.y);
        m_FOV = m_ThisCamera.fieldOfView;

        m_FOVVertical = m_FOV / m_ThisCamera.aspect;
        Debug.Log("FOVH"+m_FOV);
        Debug.Log("FOVV"+m_FOVVertical);
        //direction vectors for third intersections
        //work out relative to field of view and aspect ratio

        m_BottomLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3));
        m_BottomRightAdjustment = new Vector3((m_ScreenDimensions.x / 3)*2, (m_ScreenDimensions.y / 3));
        m_TopLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3) * 2);
        m_TopRightAdjustment = new Vector3((m_ScreenDimensions.x / 3) * 2, (m_ScreenDimensions.y / 3)*2);


	}
	
	// Update is called once per frame
	void Update () {


        //HORIZONTAL ALIGNMENT/////////////////////////////////////////////
        /////////////////////////////////////////////HORIZONTAL ALIGNMENT//
        if (m_GameplayVolumes[0].GetComponent<GameplayVolume>().m_Active)
        {
            float currentAngleDifference = Vector3.Angle(transform.forward, m_GameplayVolumes[0].transform.position - transform.position);
            //transform.LookAt(m_GameplayVolumes[0].transform.position);

            Vector3 targetScreenPosition;
            switch (m_GameplayVolumes[0].GetComponent<GameplayVolume>().m_ScreenPosition)
            {
                case GameplayVolume.ScreenPosition.BottomLeft:
                    targetScreenPosition = m_BottomLeftAdjustment;
                    break;
                case GameplayVolume.ScreenPosition.BottomRight:
                    targetScreenPosition = m_BottomRightAdjustment;
                    break;
                case GameplayVolume.ScreenPosition.TopLeft:
                    targetScreenPosition = m_TopLeftAdjustment;
                    break;
                case GameplayVolume.ScreenPosition.TopRight:
                    targetScreenPosition = m_TopRightAdjustment;
                    break;
                case GameplayVolume.ScreenPosition.Custom:
                    targetScreenPosition = new Vector3(m_ScreenDimensions.x * m_GameplayVolumes[0].GetComponent<GameplayVolume>().m_ScreenPositionCustom.x,
                                                       m_ScreenDimensions.y * m_GameplayVolumes[0].GetComponent<GameplayVolume>().m_ScreenPositionCustom.y);
                    break;
                default:
                    targetScreenPosition = m_BottomLeftAdjustment;
                    break;
            }




            Vector3 screenDifference = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumes[0].transform.position) - targetScreenPosition;
            Debug.Log(screenDifference);

            transform.Rotate(-screenDifference.y / m_ScreenDimensions.y * m_FOVVertical, screenDifference.x / m_ScreenDimensions.x * m_FOV, -transform.rotation.eulerAngles.z);//screenDifference.y / m_ScreenDimensions.y * m_FOVVertical
        }

        //Debug.DrawLine(m_ThisCamera.ScreenToWorldPoint(new Vector3(m_ScreenDimensions.x/3,m_ScreenDimensions.y/3,0.0f)),m_ThisCamera.ScreenToWorldPoint(new Vector3(m_ScreenDimensions.x/3,m_ScreenDimensions.y,0.0f)));
        
            
            //transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), currentAngleDifference);


	}
}
