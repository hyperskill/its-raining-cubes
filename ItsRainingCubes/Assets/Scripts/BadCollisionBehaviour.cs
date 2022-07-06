using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BadCollisionBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("UglyPart called");
        GameObject OkLeft = GameObject.Find("OkLeft");
        GameObject OkRight = GameObject.Find("OkRight");
        if (collision.gameObject.CompareTag("FallingCube") && (Vector3.Distance(OkLeft.transform.position, collision.gameObject.transform.position) > 0.36f) && (Vector3.Distance(OkRight.transform.position, collision.gameObject.transform.position) > 0.36f)) {
            SceneManager.LoadScene(0);
        }
        
    }
}