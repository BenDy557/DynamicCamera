﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CameraDualFocus : MonoBehaviour {

    private List<GameObject> m_GameplayVolumes;

    private GameObject m_GameplayVolumeParamount;
    private GameObject m_GameplayVolumePrimary;

    public GameObject m_ScreenSpaceViewer;

    private Camera m_ThisCamera;
    private Camera m_ScreenSpaceCamera;
    private RenderTexture m_RenderTexture;
    public Material m_GameplayVolumeRender;

    private Vector2 m_ScreenDimensions;
    private int m_ScreenPixelAmount;

    private float m_FOV;//horizontal
    private float m_FOVVertical;//vertical
    //direction vectors for third intersections//TODO
    
    private Vector3 m_BottomLeftAdjustment;
    private Vector3 m_TopLeftAdjustment;
    private Vector3 m_BottomRightAdjustment;
    private Vector3 m_TopRightAdjustment;

    private Vector3 m_TargetCameraPosition;
    private GameObject m_GameplayObjectFocus;

    GameObject m_TargetGraphic;

    //Size percentage for distance positioning
    public float m_DefaultSizeRatio = 0.3f;

	// Use this for initialization

    void Awake()
    {
        m_ScreenDimensions = new Vector2(Screen.width, Screen.height);
        m_ScreenPixelAmount = (int)(m_ScreenDimensions.x * m_ScreenDimensions.y);

       
        //direction vectors for third intersections
        //work out relative to field of view and aspect ratio
        m_BottomLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3));
        m_BottomRightAdjustment = new Vector3((m_ScreenDimensions.x / 3) * 2, (m_ScreenDimensions.y / 3));
        m_TopLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3) * 2);
        m_TopRightAdjustment = new Vector3((m_ScreenDimensions.x / 3) * 2, (m_ScreenDimensions.y / 3) * 2);

        m_GameplayObjectFocus = new GameObject("CameraParent");


        //m_TargetGraphic = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //m_TargetGraphic.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //Destroy(m_TargetGraphic.GetComponent<Collider>());
       // m_TargetGraphic.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Transparent");
    }

	void Start () 
    {
        m_GameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        m_ThisCamera = GetComponent<Camera>();
        
        m_FOV = m_ThisCamera.fieldOfView;
        m_FOVVertical = m_FOV / m_ThisCamera.aspect;
        Debug.Log("FOVH" + m_FOV);
        Debug.Log("FOVV" + m_FOVVertical);

        m_ScreenSpaceCamera = transform.GetChild(0).GetComponent<Camera>();
        if (!m_ScreenSpaceCamera)
        {
            Debug.Log("no camera found");
        }
        m_RenderTexture = m_ScreenSpaceCamera.targetTexture;
	}

   

	// Update is called once per frame
	void FixedUpdate () {


        //PRIORITY ASSIGNMENT///////////////////////////////////////////////
        ///////////////////////////////////////////////PRIORITY ASSIGNMENT//
        for (int i = 0; i < m_GameplayVolumes.Count; i++)
        {
            if (m_GameplayVolumes[i].GetComponent<GameplayVolume>().m_Active)
            {
                if (m_GameplayVolumes[i].GetComponent<GameplayVolume>().m_Priority == GameplayVolume.Priority.Paramount)
                {
                    m_GameplayVolumeParamount = m_GameplayVolumes[i];
                }
                else if (m_GameplayVolumes[i].GetComponent<GameplayVolume>().m_Priority == GameplayVolume.Priority.Primary)
                {
                    m_GameplayVolumePrimary = m_GameplayVolumes[i];
                }
            }
        }

        //Camera temporary Parent transform
        m_GameplayObjectFocus.transform.position = new Vector3(m_GameplayVolumeParamount.transform.position.x,m_GameplayVolumeParamount.transform.position.y,m_GameplayVolumeParamount.transform.position.z);
        
        //PRIORITY-PARAMOUNT/////////////////////////////////////////////
        if (m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_Active)
        {
       
            //SCREEN POSITIONING/////////////////////////////////////////////
            /////////////////////////////////////////////SCREEN POSITIONING//

            Vector3 targetScreenPosition;
            switch (m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_ScreenPosition)
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
                    targetScreenPosition = new Vector3(m_ScreenDimensions.x * m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_ScreenPositionCustom.x,
                                                       m_ScreenDimensions.y * m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_ScreenPositionCustom.y);
                    break;
                default:
                    targetScreenPosition = m_BottomLeftAdjustment;
                    break;
            }

            Vector3 screenDifference = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumeParamount.transform.position) - targetScreenPosition;
            //Debug.Log(screenDifference);
            transform.Rotate(-screenDifference.y / m_ScreenDimensions.y * m_FOVVertical, screenDifference.x / m_ScreenDimensions.x * m_FOV, -transform.rotation.eulerAngles.z);//screenDifference.y / m_ScreenDimensions.y * m_FOVVertical

        
            //SIZE ADJUSTING/////////////////////////////////////////////
            /////////////////////////////////////////////SIZE ADJUSTING//
        
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = m_ScreenSpaceCamera.targetTexture;
            m_ScreenSpaceCamera.Render();
            Texture2D image = new Texture2D(m_ScreenSpaceCamera.targetTexture.width, m_ScreenSpaceCamera.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, m_ScreenSpaceCamera.targetTexture.width, m_ScreenSpaceCamera.targetTexture.height), 0, 0);
            Color[] tempColors = image.GetPixels();
            RenderTexture.active = currentRT;


        
            int x = m_ScreenSpaceCamera.targetTexture.width;
            int y = m_ScreenSpaceCamera.targetTexture.height;

            //Counts green pixels
            int tempPixelCount = 0;
            for (int i = 0; i < (x * y); i++)
            {
                if (tempColors[i].g > 0.0f)
                {
                    tempPixelCount++;
                }
            }

            //finds ratio of pixels:blankpixels
            float pixelRatio = (float)tempPixelCount / (float)(x * y);

            //Direction vector between camera and gameplay volume
            Vector3 tempDirectionVector= m_GameplayVolumeParamount.transform.position - gameObject.transform.position;
            tempDirectionVector.Normalize();

            //Distance between camera and volume
            float tempCurrentDistance = Vector3.Distance(m_GameplayVolumeParamount.transform.position, gameObject.transform.position);


            float targetDistance = tempCurrentDistance * (Mathf.Sqrt(pixelRatio) / Mathf.Sqrt(m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_ScreenSize));

            m_TargetCameraPosition = transform.position + (tempDirectionVector * (tempCurrentDistance - targetDistance));

        }


        //PRIORITY-PRIMARY///////////////////////////////////////////////
        if (m_GameplayVolumePrimary)
        {
            if (m_GameplayVolumePrimary.GetComponent<GameplayVolume>().m_Active)
            {
                //POSITIONAL ALIGNMENT///////////////////////////////////////////
                ///////////////////////////////////////////POSITIONAL ALIGNMENT//

                Vector3 targetScreenPosition;
                switch (m_GameplayVolumePrimary.GetComponent<GameplayVolume>().m_ScreenPosition)
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
                        targetScreenPosition = new Vector3(m_ScreenDimensions.x * m_GameplayVolumePrimary.GetComponent<GameplayVolume>().m_ScreenPositionCustom.x,
                                                           m_ScreenDimensions.y * m_GameplayVolumePrimary.GetComponent<GameplayVolume>().m_ScreenPositionCustom.y);
                        break;
                    default:
                        targetScreenPosition = m_BottomLeftAdjustment;
                        break;
                }

                Vector3 tempScreenCoordinates = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumePrimary.transform.position);

                /*
                if (!(tempScreenCoordinates.x < 0 || tempScreenCoordinates.x > m_ScreenDimensions.x) && !(tempScreenCoordinates.y < 0 || tempScreenCoordinates.y > m_ScreenDimensions.y))
                {
                    Vector3 screenDifference = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumePrimary.transform.position) - targetScreenPosition;
                    m_TargetPosition = RotatePointAroundPivot(m_TargetPosition, m_GameplayVolumeParamount.transform.position, new Vector3(0.0f, (screenDifference.x / m_ScreenDimensions.x) * m_FOV, (-screenDifference.y / m_ScreenDimensions.y) * m_FOVVertical));
                }
                else
                {
                    Debug.Log("PRIMARY OFFSCREEN");
                }
                */

                Vector3 screenDifference = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumePrimary.transform.position) - targetScreenPosition;
                
                
                //Sets Parents rotation to cameras
                m_GameplayObjectFocus.transform.rotation = transform.rotation;
                //And then sets transform to parent of camera
                transform.parent = m_GameplayObjectFocus.transform;

                //m_GameplayObjectFocus.transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"),0.0f);
              
                m_GameplayObjectFocus.transform.Rotate((-screenDifference.y / m_ScreenDimensions.y) * m_FOVVertical, (screenDifference.x / m_ScreenDimensions.x) * m_FOV, 0.0f);

                transform.parent = null;
                               
            }
        }

        //PRIORITY-SECONDARY/////////////////////////////////////////////
        

        //CAMERA MOVEMENT////////////////////////////////////////////////
        ////////////////////////////////////////////////CAMERA MOVEMENT//
        transform.position += ((m_TargetCameraPosition - transform.position) / 5);

	}

    Vector3 RotatePointAroundPivot(Vector3 pointIn, Vector3 pivotIn, Vector3 angles)
    {
        Vector3 dir = pointIn - pivotIn; // get point direction relative to pivot

        
        dir = Quaternion.Euler(angles) * dir; // rotate it
        pointIn = dir + pivotIn; // calculate rotated point
        return pointIn; // return it
    }

   
}
