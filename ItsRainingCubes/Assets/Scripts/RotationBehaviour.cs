using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBehaviour : MonoBehaviour
{
    public float rotationRate = 100f;
    private bool rotateBody = true;
    void Update()
    {
        if (rotateBody) {
            transform.Rotate(Vector3.down, rotationRate * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision) {
        // GameObject OkLeft = GameObject.Find("OkLeft");
        // GameObject OkRight = GameObject.Find("OkRight");
        // Debug.Log(OkLeft.transform.position);
        // Debug.Log(OkRight.transform.position);
        // Debug.Log(collision.gameObject.transform.position);
        
        // Debug.Log(Vector3.Distance(OkLeft.transform.position, collision.gameObject.transform.position));
        // Debug.Log(Vector3.Distance(OkRight.transform.position, collision.gameObject.transform.position));
        
        // Debug.Log("Seperator");
        rotateBody = false;
    }
}
