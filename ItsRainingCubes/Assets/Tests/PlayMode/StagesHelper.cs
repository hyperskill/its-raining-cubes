using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StagesHelper : MonoBehaviour
{
    private bool TagExist = false;
    private bool ready = true;
    private void Start()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals("FallingCube")) { TagExist = true; break; }
        }
    }

    private void Update()
    {
        if (ready)
        {
            ready = false;
            StartCoroutine(CubesRemove());
        }
    }

    IEnumerator CubesRemove()
    {
        if (TagExist)
        {
            GameObject[] cubes = GameObject.FindGameObjectsWithTag("FallingCube");
            foreach (GameObject cube in cubes)
            {
                Destroy(cube);
            }
        }

        yield return new WaitForSeconds(0.1f);
        ready = true;
    }
}
