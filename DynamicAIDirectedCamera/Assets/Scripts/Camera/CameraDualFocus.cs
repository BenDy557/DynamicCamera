
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
    

    //Size percentage for distance positioning
    public float m_SizeRatioDefault = 0.3f;

	// Use this for initialization
	void Start () 
    {
        m_ScreenDimensions = new Vector2(Screen.width, Screen.height);
        m_ScreenPixelAmount = (int)(m_ScreenDimensions.x * m_ScreenDimensions.y);
        m_FOV = m_ThisCamera.fieldOfView;

        m_FOVVertical = m_FOV / m_ThisCamera.aspect;
        Debug.Log("FOVH" + m_FOV);
        Debug.Log("FOVV" + m_FOVVertical);
        //direction vectors for third intersections
        //work out relative to field of view and aspect ratio
        m_BottomLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3));
        m_BottomRightAdjustment = new Vector3((m_ScreenDimensions.x / 3) * 2, (m_ScreenDimensions.y / 3));
        m_TopLeftAdjustment = new Vector3((m_ScreenDimensions.x / 3), (m_ScreenDimensions.y / 3) * 2);
        m_TopRightAdjustment = new Vector3((m_ScreenDimensions.x / 3) * 2, (m_ScreenDimensions.y / 3) * 2);
	}

    void Awake()
    {
        m_GameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        m_ThisCamera = GetComponent<Camera>();
        m_ScreenSpaceCamera = transform.GetChild(0).GetComponent<Camera>();
        if (!m_ScreenSpaceCamera)
        {
            Debug.Log("no camera found");
        }
        m_RenderTexture = m_ScreenSpaceCamera.targetTexture; 

    }


	// Update is called once per frame
	void Update () {


        //PRIORITY ASSIGNMENT///////////////////////////////////////////////
        ///////////////////////////////////////////////PRIORITY ASSIGNMENT//
        for (int i = 0; i < m_GameplayVolumes.Count; i++)
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
        
        
        //PRIORITY-PARAMOUNT/////////////////////////////////////////////

        //SCREEN POSITIONING/////////////////////////////////////////////
        /////////////////////////////////////////////SCREEN POSITIONING//
        if (m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_Active)
        {
            //float currentAngleDifference = Vector3.Angle(transform.forward, m_GameplayVolumeParamount.transform.position - transform.position);
            //transform.LookAt(m_GameplayVolumes[0].transform.position);

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

        }
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

        int tempPixelCount = 0;
        for (int i = 0; i < (x * y); i++)
        {
            if (tempColors[i].g > 0.0f)
            {
                tempPixelCount++;
            }
        }

        float pixelRatio = (float)tempPixelCount / (float)(x * y);


        Vector3 tempCurrentDifference = m_GameplayVolumeParamount.transform.position - gameObject.transform.position;
        tempCurrentDifference.Normalize();

        float tempCurrentDistance = Vector3.Distance(m_GameplayVolumeParamount.transform.position, gameObject.transform.position);
        float targetDistance = tempCurrentDistance * (pixelRatio / m_GameplayVolumeParamount.GetComponent<GameplayVolume>().m_ScreenSize);

        transform.position = transform.position + (tempCurrentDifference * (tempCurrentDistance - targetDistance) * 0.5f);


        if (tempCurrentDistance > targetDistance)
        {
            //move towards
            //transform.Translate(0.0f, 0.0f, 0.1f);
        }
        else
        {
            //move away
            //transform.Translate(0.0f, 0.0f, -0.1f);
        }

        //image.SetPixel(30, 30, Color.white);
        //image.Apply();
        

        

        //File.WriteAllBytes(Application.dataPath + "/img.png", image.EncodeToPNG());

       


        //PRIORITY-PRIMARY///////////////////////////////////////////////

        //POSITIONAL ALIGNMENT///////////////////////////////////////////
        ///////////////////////////////////////////POSITIONAL ALIGNMENT//
        if (m_GameplayVolumePrimary)
        {
            if (m_GameplayVolumePrimary.GetComponent<GameplayVolume>().m_Active)
            {


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

                Vector3 screenDifference = m_ThisCamera.WorldToScreenPoint(m_GameplayVolumePrimary.transform.position) - targetScreenPosition;


                //transform.Rotate(-screenDifference.y / m_ScreenDimensions.y * m_FOVVertical
                //                , screenDifference.x / m_ScreenDimensions.x * m_FOV
                //                , -transform.rotation.eulerAngles.z);

                Debug.Log(screenDifference.x);


                //Moving towards to align with vertical thirds
                if (screenDifference.x > 0.0f)
                {
                    transform.Translate(0.0f, 0.0f, -0.1f);
                }
                else if (screenDifference.x < 0.0f)
                {
                    transform.Translate(0.0f, 0.0f, 0.1f);
                }


                if (screenDifference.x > 0.0f)
                {
                    transform.Translate(0.1f, 0.0f, 0.0f);
                }
                else if (screenDifference.x < 0.0f)
                {
                    transform.Translate(-0.1f, 0.0f, 0.0f);
                }


                //if (screenDifference.y > 0.0f)
                //{
                //    transform.Translate(0.0f, -0.1f, 0.0f);
                //}
                //else if (screenDifference.y < 0.0f)
                //{
                //    transform.Translate(0.0f, 0.1f, 0.0f);
                //}

            }
        }


        //PRIORITY-SECONDARY/////////////////////////////////////////////
        

	}
}
