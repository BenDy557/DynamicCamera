using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAt : MonoBehaviour {

    private List<GameObject> gameplayVolumes;
    private Camera thisCamera;
	// Use this for initialization
	void Start () {
        gameplayVolumes = FindObjectOfType<GameplayVolumeManager>().gameplayVolumes;

        thisCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

        if (gameplayVolumes.Count > 0)
        {
            Vector3 averagePosition;
            averagePosition = new Vector3(0.0f,0.0f,0.0f);
            //FOLLOW THEM
            for (int i = 0; i < gameplayVolumes.Count; i++)
            {
                averagePosition += gameplayVolumes[i].transform.position;
            }

            averagePosition /= gameplayVolumes.Count;

            gameObject.transform.LookAt(averagePosition);


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

            thisCamera.fieldOfView = maxAngleDifference * 2;
        }
	}
}
