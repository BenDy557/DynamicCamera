using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrototypeCameraControl : MonoBehaviour 
{

    private List<GameObject> gameplayVolumes;
    private Camera thisCamera;

    private Vector3 averagePosition;
    private Vector3 averageScreenPosition;

    public float smoothingDuration;
    public float targetFOV;

    //private GameObject averagePositionObject;
    //private GameObject averageScreenPositionObject;


	// Use this for initialization
	void Start () {
        
        
        
        averagePosition = new Vector3(0.0f, 0.0f, 0.0f);
        averageScreenPosition = new Vector3(0.0f, 0.0f, 0.0f);


        gameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;
        thisCamera = GetComponent<Camera>();


        //averagePositionObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //averagePositionObject.transform.position = averagePosition;

        //averageScreenPositionObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //averageScreenPositionObject.transform.position = averageScreenPosition;

	}
	
	// Update is called once per frame
    void LateUpdate()
    {


        float maxAngleDifference = 0.0f;

        for (int i = 0; i < gameplayVolumes.Count; i++)
        {
            float tempAngleDifference = 0.0f;

            tempAngleDifference = Vector3.Angle(transform.forward, gameplayVolumes[i].transform.position - transform.position);

            if (tempAngleDifference > maxAngleDifference)
            {
                maxAngleDifference = tempAngleDifference;
            }
        }


        //Debug.Log("maxAngleDifference: "+maxAngleDifference);

        //thisCamera.fieldOfView = maxAngleDifference * 2;

        //angles outside of view (maxAngleDifference - thisCamera.fieldOfView/2)






        //Vector3 tempAveragePosition = new Vector3(averagePosition.x, averagePosition.y, averagePosition.z);
        averagePosition = new Vector3(0.0f, 0.0f, 0.0f);
        averageScreenPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (gameplayVolumes.Count > 0)
        {

            //FOLLOW THEM
            for (int i = 0; i < gameplayVolumes.Count; i++)
            {
                averagePosition += gameplayVolumes[i].transform.position;
                averageScreenPosition += thisCamera.WorldToScreenPoint(gameplayVolumes[i].transform.position);
            }

            averagePosition /= gameplayVolumes.Count;
            averageScreenPosition /= gameplayVolumes.Count;  

        }

        gameObject.transform.LookAt(thisCamera.ScreenToWorldPoint(averageScreenPosition));
        transform.position += (averagePosition - transform.position) * (Time.deltaTime / smoothingDuration) * ((thisCamera.fieldOfView/2) - maxAngleDifference);




        //averagePositionObject.transform.position = averagePosition;
        //averageScreenPositionObject.transform.position = averageScreenPosition;
	}
}
