using UnityEngine;
using System.Collections;

public class GameplayVolume : MonoBehaviour {
    
    //Ideas for variables
    public enum ScreenPosition { TopLeft, TopRight, BottomLeft, BottomRight, Custom };
    //public enum VolumeType { LookAt, FitInFrame };
    public enum Priority { Paramount, Primary, Secondary, Tertiary }

    public Priority m_Priority;

    public float m_ScreenSize;
    public float m_MinCameraDistance;
    public float m_MaxCameraDistance;
    public ScreenPosition m_ScreenPosition;
    public Vector2 m_ScreenPositionCustom;
    //public VolumeType m_VolumeType;
    
    //public float m_MaxDistance, m_MinDistance;//TODO//convert to screenspace size

    public bool m_Active;
    private bool m_ActivePrev;
    public bool m_Visible;
    private bool m_VisiblePrev;
    //Screen position rules of thirds
    //opposite of player
    //player side

    private GameplayVolumeManager m_GameplayVolumeManager;
    private MeshRenderer m_MeshRenderer;

    void Awake()
    {
        if (m_Priority == Priority.Paramount)
        {
            gameObject.layer = LayerMask.NameToLayer("GameplayVolumesParamount");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("GameplayVolumes");
        }
    }

	// Use this for initialization
	void Start () 
    {
       
        m_Active = true;
        m_ActivePrev = m_Active;

        m_Visible = true;
        m_VisiblePrev = m_Visible;

        m_MeshRenderer = GetComponent<MeshRenderer>();


        
        m_GameplayVolumeManager = FindObjectOfType<GameplayVolumeManager>().GetComponent<GameplayVolumeManager>();
        if (m_Active)
        {
            m_GameplayVolumeManager.gameplayVolumes.Add(gameObject);
            m_MeshRenderer.material = Resources.Load<Material>("Materials/GameplayVolumeActive");
        }
        else
        {
            m_MeshRenderer.material = Resources.Load<Material>("Materials/GameplayVolumeDormant");
        }


        m_MeshRenderer.enabled = m_Visible;

	}
	
	// Update is called once per frame
	void Update () 
    {

        if (m_Active && m_Active!=m_ActivePrev)
        {
            m_MeshRenderer.material = Resources.Load<Material>("Materials/GameplayVolumeActive");
        }
        else if(!m_Active && m_Active!=m_ActivePrev)
        {
            m_MeshRenderer.material = Resources.Load<Material>("Materials/GameplayVolumeDormant");
        }

        m_MeshRenderer.enabled = m_Visible;


        m_ActivePrev = m_Active;
        m_VisiblePrev = m_Visible;
	}

    void TurnOff()
    {
        m_Active = false;
    }

    void TurnOn()
    {
        m_Active = true;
    }
}
