using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionBehaviour : MonoBehaviour
{
    public Text score;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FallingCube")) {
            Destroy(collision.gameObject);
            int currentScore = int.Parse(score.text);
            currentScore += 1;
            score.text = currentScore.ToString();
            /* GameObject OkLeft = GameObject.Find("OkLeft");
            GameObject OkRight = GameObject.Find("OkRight");
            if (collision.gameObject.CompareTag("FallingCube") && (Vector3.Distance(OkLeft.transform.position, collision.gameObject.transform.position) > 0.30f) && (Vector3.Distance(OkRight.transform.position, collision.gameObject.transform.position) > 0.30f)) {
            } */
        }
    }
}
