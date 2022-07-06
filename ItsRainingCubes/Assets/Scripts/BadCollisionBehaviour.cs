using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BadCollisionBehaviour : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("UglyPart called");
        if (collision.gameObject.CompareTag("FallingCube")) {
            SceneManager.LoadScene(0);
        }
        
    }
}
