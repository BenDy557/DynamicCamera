using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PredictVolumePosition : MonoBehaviour {

    public enum PredictionType { Immediate, Average };
    public PredictionType m_PredictionType;

    public float m_MinDistance;

    public GameObject m_PredictedVolumePlacement;
    private List<Vector3> m_PreviousVelocities = new List<Vector3>();
    public int m_PreviousVelocitiesAmount = 30;
    private Vector3 m_CurrentVelocity;

    private Vector3 m_PrevPosition;

    private Vector3 m_AverageVelocity = Vector3.zero;
    private Vector3 m_PrevAverageVelocity;
    private Vector3 m_PrevCurrentVelocity;

    public float m_TimePrediction;

    public float m_AdditionDelay = 0.3f;
    private float m_AdditionTimer = 0.0f;

	// Use this for initialization
	void Start () {
        m_PrevPosition = transform.position;
        for (int i = 0; i < m_PreviousVelocitiesAmount; i++)
        {
            m_PreviousVelocities.Add(Vector3.zero);
        }

        m_PrevAverageVelocity = transform.forward * m_MinDistance;
        m_PrevCurrentVelocity = transform.forward * m_MinDistance;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    
        m_AdditionTimer += Time.deltaTime;


        m_CurrentVelocity = (transform.position - m_PrevPosition) / Time.deltaTime;
        m_PrevPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (m_AdditionTimer > m_AdditionDelay)
        {
            m_AdditionTimer -= m_AdditionDelay;


            m_PreviousVelocities.Add(m_CurrentVelocity);
            m_PreviousVelocities.RemoveAt(0);

            Vector3 tempTotal= Vector3.zero;
            for (int i = 0; i < m_PreviousVelocitiesAmount; i++)
            {
                tempTotal += m_PreviousVelocities[i];
            }

            m_AverageVelocity = tempTotal / m_PreviousVelocitiesAmount;
            
        }

        
        if( m_PredictionType == PredictionType.Average)
        {
            if ((m_AverageVelocity.magnitude * m_TimePrediction) > m_MinDistance)
            {
                m_PredictedVolumePlacement.transform.position = transform.position + (m_AverageVelocity * m_TimePrediction);
                m_PrevAverageVelocity = new Vector3(m_AverageVelocity.x,m_AverageVelocity.y,m_AverageVelocity.z);
            }
            else
            {
                m_PredictedVolumePlacement.transform.position = transform.position + (m_PrevAverageVelocity * m_TimePrediction);
            }


        }
        else if(m_PredictionType == PredictionType.Immediate)
        {
            if ((m_CurrentVelocity.magnitude * m_TimePrediction) > m_MinDistance)
            {
                m_PredictedVolumePlacement.transform.position = transform.position + (m_CurrentVelocity * m_TimePrediction);
                m_PrevCurrentVelocity = new Vector3(m_CurrentVelocity.x, m_CurrentVelocity.y, m_CurrentVelocity.z);
            }
            else
            {
                m_PredictedVolumePlacement.transform.position = transform.position + (m_PrevCurrentVelocity * m_TimePrediction);
            }
        }
            
        
	}
}
