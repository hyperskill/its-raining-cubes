using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("Want a sugar cube?"), Category("3")]
public class Stage3_Tests
{
    Camera camera;
    private LayerMask testLayer;

    [UnityTest, Order(0)]
    public IEnumerator CheckCorrectSpawn()
    {
        PMHelper.TurnCollisions(false);
        Time.timeScale = 10;
        testLayer = LayerMask.NameToLayer("Test");

        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        //Check if tag exist
        if (!PMHelper.CheckTagExistance("FallingCube"))
        {
            Assert.Fail("\"FallingCube\" tag was not added to project settings");
        }

        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        //Existence and components check
        GameObject tmp = GameObject.FindWithTag("FallingCube");
        if (!tmp)
        {
            Assert.Fail(
                "\"FallingCube\" object not spawning instantly after opening scene, or prefab's tag is misspelled");
        }

        Transform tmpT = tmp.transform;
        if (!PMHelper.Check3DPrimitivity(tmp, PrimitiveType.Cube))
            Assert.Fail(
                "\"FallingCube\" object was not created as primitive Cube object, it's mesh differs or components missing");
        if (!PMHelper.CheckMaterialDifference(tmp))
            Assert.Fail("The material of \"FallingCube\" object should be changed");
        Rigidbody rb = PMHelper.Exist<Rigidbody>(tmp);
        if (!rb)
            Assert.Fail("\"FallingCube\" should have assigned <Rigidbody> component");
        if (!rb.useGravity)
            Assert.Fail("\"FallingCube\"'s <Rigidbody> component should have checked <Use Gravity> parameter");
        if (rb.isKinematic)
            Assert.Fail("\"FallingCube\"'s <Rigidbody> component should have unchecked <Is Kinematic> parameter");

        //Check if visible when spawns
        if (PMHelper.CheckVisibility(camera, tmpT, 3))
            Assert.Fail("\"FallingCube\" object should not be in a camera view when it spawns");

        //Check position
        GameObject Floor = GameObject.Find("Floor"),
            Wall = GameObject.Find("Wall");
        Floor.layer = testLayer;
        Wall.layer = testLayer;

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            Floor.layer == testLayer && Wall.layer == testLayer || (Time.unscaledTime - start) * Time.timeScale > 1);

        RaycastHit hit1 = PMHelper.RaycastFront3D(tmpT.position, Vector3.forward, 1 << testLayer);
        if (!(hit1.collider != null && hit1.collider.gameObject == Wall))
            Assert.Fail("\"FallingCube\"'s objects should be spawned in front of the \"Wall\" object");
        RaycastHit hit2 = PMHelper.RaycastFront3D(tmpT.position, Vector3.down, 1 << testLayer);
        if (!(hit2.collider != null && hit2.collider.gameObject == Floor))
            Assert.Fail("\"FallingCube\"'s objects should be spawned above the \"Platform\" object");

        //Check falling
        Vector3 spawnPos = tmpT.position;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            tmpT.position != spawnPos || (Time.unscaledTime - start) * Time.timeScale > 1);

        if (!(spawnPos.y > tmpT.position.y))
        {
            Assert.Fail("Y-axis of \"FallingCube\"'s object should decrease by the time (while falling)");
        }

        if (Math.Abs(spawnPos.x - tmpT.position.x) > 1 || Math.Abs(spawnPos.z - tmpT.position.z) > 1)
        {
            Assert.Fail("X-axis and z-axis of \"FallingCube\"'s object should not change");
        }
        
        //Check rotation
        Quaternion rotStart = tmpT.rotation;
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            rotStart != tmpT.rotation || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (rotStart == tmpT.rotation)
        {
            Assert.Fail("\"FallingCube\" objects should be continuously rotating");
        }
        
        //Destroy existing
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("FallingCube"))
        {
            GameObject.Destroy(obj);
        }

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            !GameObject.FindWithTag("FallingCube") || (Time.unscaledTime - start) * Time.timeScale > 1);
        
        //Check frequency
        List<Vector3> positions = new List<Vector3>();
        for (int j = 0; j < 5; j++)
        {
            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                GameObject.FindWithTag("FallingCube") || (Time.unscaledTime - start) * Time.timeScale > 2);
            GameObject cube = GameObject.FindWithTag("FallingCube");
            if (!cube || GameObject.FindGameObjectsWithTag("FallingCube").Length != 1)
            {
                Assert.Fail("\"FallingCube\" object should be instantiated at the rate of 1 per second");
            }
            positions.Add(cube.transform.position);
            GameObject.Destroy(cube);
            yield return new WaitUntil(() =>
                !GameObject.FindWithTag("FallingCube") || (Time.unscaledTime - start) * Time.timeScale > 1);
        }
        
        //Check random and correct spawning
        for (int i = 0; i < positions.Count-1; i++)
        {
            for (int j = i+1; j < positions.Count; j++)
            {
                if (Math.Abs(positions[i].z - positions[j].z) > 1 ||
                    Math.Abs(positions[i].y - positions[j].y) > 1)
                {
                    Assert.Fail(
                        "All of the \"FallingCube\" objects should be instantiated with the same z-axis and y-axis");
                }

                if (positions[i].x.Equals(positions[j].x))
                {
                    Assert.Fail(
                        "All of the \"FallingCube\" objects should be instantiated with random x-axis. Some of them are identical");
                }

                if (positions[i].x < PlayerPrefs.GetFloat("ForTestingOnly_Left") ||
                    positions[i].x > PlayerPrefs.GetFloat("ForTestingOnly_Right") ||
                    positions[j].x < PlayerPrefs.GetFloat("ForTestingOnly_Left") ||
                    positions[j].x > PlayerPrefs.GetFloat("ForTestingOnly_Right"))
                {
                    Assert.Fail("All of the \"FallingCube\" objects should be \"catchable\" by platform");
                }
            }
        }
    }
}