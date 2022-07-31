using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBehaviour : MonoBehaviour
{
    public GameObject cubePrefab;

    void Start()
    {
        InvokeRepeating("CreatePrefab", 0f, 1.0f);
    }

    void CreatePrefab()
    {
        Instantiate(cubePrefab, new Vector3(Random.Range(-3.0f, 3.0f), 10, 3), Random.rotation);
    }
}
