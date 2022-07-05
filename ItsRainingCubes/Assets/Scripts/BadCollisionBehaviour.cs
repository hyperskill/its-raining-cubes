using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BadCollisionBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("UglyPart called");
        if (collision != null) {
            SceneManager.LoadScene(0);
        }
        
    }

    // void OnCollisionStay(Collision collision)
    // {

    //     SceneManager.LoadScene(0);
    // }
}
