using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public Vector3 rotationAmount;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(rotationAmount);
	}
}
