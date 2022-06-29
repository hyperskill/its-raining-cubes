using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBehaviour : MonoBehaviour
{
    public GameObject cubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreatePrefab", 1.0f, 100.0f);
    }

    //Update is called once per frame
    void Update()
    {
        
    }

    void CreatePrefab()
    {
        Instantiate(cubePrefab, new Vector3(Random.Range(-3.0f, 3.0f), 10, 3), Random.rotation);
    }
}
