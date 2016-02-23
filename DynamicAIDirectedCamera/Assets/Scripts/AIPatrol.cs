using UnityEngine;
using System.Collections;
//using UnityStandardAssets.Characters.ThirdPerson;

public class AIPatrol : MonoBehaviour 
{
    private Vector3 targetPosition;
    private Transform targetTransform;
    public GameObject[] patrolPoints;
    private NavMeshAgent agent;

    private int currentPatrolNode = 0;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        

        targetTransform = patrolPoints[currentPatrolNode].transform;
        targetPosition = targetTransform.position;

       


        Vector3 tempVector = transform.position - patrolPoints[currentPatrolNode].transform.position;

        if (tempVector.magnitude < 1.0f)
        {
            currentPatrolNode++;
            if (currentPatrolNode == patrolPoints.Length)
            {
                currentPatrolNode = 0;
            }
        }


        //GetComponent<AICharacterControl>().target = targetTransform;
    }
}



