using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTo : MonoBehaviour 
{
    private List<GameObject> gameplayVolumes;
    private Camera thisCamera;

    private Vector3 averagePosition;

    public float smoothingDuration;
    public float targetFOV;

	// Use this for initialization
	void Start () 
    {
        Vector3 averagePosition;
        averagePosition = new Vector3(0.0f, 0.0f, 0.0f);
        

        gameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        thisCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 tempAveragePosition = new Vector3(averagePosition.x,averagePosition.y,averagePosition.z);
        averagePosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (gameplayVolumes.Count > 0)
        {
            
            //FOLLOW THEM
            for (int i = 0; i < gameplayVolumes.Count; i++)
            {
                averagePosition += gameplayVolumes[i].transform.position;  
            }

            averagePosition /= gameplayVolumes.Count;

           
        }


        transform.position += (averagePosition-transform.position)*(Time.deltaTime/smoothingDuration) * (targetFOV - thisCamera.fieldOfView);

	}
}
