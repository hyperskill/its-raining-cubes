using System;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Description("I did not plan to make the Cube"), Category("2")]
public class Stage2_Tests
{
    Transform checkLeft, checkRight, checkMid;
    float tmpObjSize = 0.1f;
    GameObject platform;
    Camera camera;

    [UnityTest, Order(1)]
    public IEnumerator CheckMovement()
    {
        PMHelper.TurnCollisions(false);
        Time.timeScale = 10;
        //Check, that movement is working properly
        Vector3 wasMid;
        (KeyCode, string, bool)[] testCases =
        {
            (KeyCode.A, "A", true), (KeyCode.D, "D", false), (KeyCode.LeftArrow, "Left Arrow", true),
            (KeyCode.RightArrow, "Right Arrow", false)
        };
        foreach ((KeyCode, string, bool) t in testCases)
        {
            SceneManager.LoadScene("Game");

            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
            if (SceneManager.GetActiveScene().name != "Game")
            {
                Assert.Fail("\"Game\" scene can't be loaded");
            }

            platform = GameObject.Find("Platform");
            
            //Creating detect tmp object
            GameObject tmpMid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            checkMid = tmpMid.transform;
            checkMid.SetParent(platform.transform);
            checkMid.localPosition = Vector3.zero;
            checkMid.localScale = Vector3.one * tmpObjSize;

            start = Time.unscaledTime;
            yield return new WaitUntil(() => tmpMid || (Time.unscaledTime - start) * Time.timeScale > 1);

            wasMid = checkMid.position;
            VInput.KeyPress(t.Item1);

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                wasMid.x != checkMid.position.x || (Time.unscaledTime - start) * Time.timeScale > 2);

            if (wasMid.x <= checkMid.position.x && t.Item3 || wasMid.x >= checkMid.position.x && !t.Item3)
                Assert.Fail("Platform is not moving to the " + (t.Item3 ? "left" : "right") + " by pressing \"" +
                            t.Item2 + "\"-key");
            if (checkMid.position.y != wasMid.y || checkMid.position.z != wasMid.z)
                Assert.Fail("Y-axis and z-axis of platform should not be changed");
        }
    }

    [UnityTest, Order(2)]
    public IEnumerator CheckLimit()
    {
        Vector3[] lr = new Vector3[2];
        foreach (KeyCode k in new [] {KeyCode.A, KeyCode.D})
        {
            SceneManager.LoadScene("Game");

            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
            if (SceneManager.GetActiveScene().name != "Game")
            {
                Assert.Fail("\"Game\" scene can't be loaded");
            }

            platform = GameObject.Find("Platform");
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            //Creating 3 detect tmp objects
            Transform LoseLeft = GameObject.Find("LoseLeft").transform;
            Transform LoseRight = GameObject.Find("LoseRight").transform;
            Vector3 leftPlace = LoseLeft.localPosition - LoseLeft.localScale / 2,
                rightPlace = LoseRight.localPosition + LoseRight.localScale / 2,
                midPlace = Vector3.zero;

            GameObject tmpLeft = GameObject.CreatePrimitive(PrimitiveType.Cube),
                tmpRight = GameObject.CreatePrimitive(PrimitiveType.Cube),
                tmpMid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            checkLeft = tmpLeft.transform;
            checkRight = tmpRight.transform;
            checkMid = tmpMid.transform;
            
            (Transform, Vector3)[] tmps = {(checkLeft, leftPlace), (checkRight, rightPlace), (checkMid, midPlace)};
            foreach ((Transform, Vector3) t in tmps)
            {
                t.Item1.SetParent(platform.transform);
                t.Item1.localPosition = t.Item2;
                t.Item1.localScale = Vector3.one * tmpObjSize;
            }

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                tmpLeft && tmpRight && tmpMid || (Time.unscaledTime - start) * Time.timeScale > 1);

            //Check,that left part of LoseLeft and right part of LoseRight are out of camera view
            if (PMHelper.CheckVisibility(camera, checkLeft, 3) || PMHelper.CheckVisibility(camera, checkRight, 3))
                Assert.Fail(
                    "Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
            if (!PMHelper.CheckVisibility(camera, checkMid, 3))
                Assert.Fail("Gap between two parts should be always easily visible by camera");

            //Moving platform to the bound and check
            VInput.KeyDown(k);
            start = Time.unscaledTime;
            yield return new WaitUntil(() => (Time.unscaledTime - start) * Time.timeScale > 4);

            if (PMHelper.CheckVisibility(camera, checkLeft, 3) || PMHelper.CheckVisibility(camera, checkRight, 3))
                Assert.Fail(
                    "Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
            if (!PMHelper.CheckVisibility(camera, checkMid, 3))
                Assert.Fail("Gap between two parts should be always easily visible by camera");

            Vector3 pos = checkMid.position;
            yield return new WaitUntil(() =>
                checkMid.position != pos || (Time.unscaledTime - start) * Time.timeScale > 1);
            if (checkMid.position != pos)
            {
                Assert.Fail("Platform should be able to reach any limit zone by 3 seconds");
            }

            if (k == KeyCode.A) lr[0] = pos;
            else lr[1] = pos;
            
            VInput.KeyUp(k);
        }


        PlayerPrefs.SetFloat("ForTestingOnly_Left", lr[0].x);
        PlayerPrefs.SetFloat("ForTestingOnly_Right", lr[1].x);
        if (Math.Abs(Mathf.Abs(lr[0].x) - Mathf.Abs(lr[1].x)) > 0.01f)
        {
            Assert.Fail("Limit's absolute value for both sides should be the same");
        }
    }
}