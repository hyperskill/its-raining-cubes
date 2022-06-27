using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + Time.deltaTime * Input.GetAxisRaw("Horizontal"), -5.0f, 5.0f), transform.position.y, transform.position.z);
    }
}
