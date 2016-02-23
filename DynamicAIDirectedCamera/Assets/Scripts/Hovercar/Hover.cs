using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {


    Rigidbody m_RigidBody;
    public float hoverDistance;

	// Use this for initialization
	void Start () {
        m_RigidBody = new Rigidbody();
        m_RigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        Ray hoverRay = new Ray(gameObject.transform.position,-Vector3.up * hoverDistance);

        Physics.Raycast(hoverRay, out hit);

        if (hit.distance < hoverDistance)
        {
            m_RigidBody.AddForce(new Vector3(0.0f, 10.0f, 0.0f));
        }

	}
}
