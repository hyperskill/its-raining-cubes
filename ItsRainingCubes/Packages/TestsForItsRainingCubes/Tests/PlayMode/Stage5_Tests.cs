using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[Description("Platonic solid regular hexahedron"), Category("5")]
public class Stage5_Tests
{
    public GameObject score;
    public GameObject canvas;

    public GameObject LoseLeft;
    public GameObject Floor;
    private Text text;
    private LayerMask testLayer;


    [UnityTest, Order(0)]
    public IEnumerator CheckCorrect()
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

        //Check existence
        canvas = GameObject.Find("Canvas");
        if (!canvas)
            Assert.Fail("There is no canvas in scene named \"Canvas\"");
        score = GameObject.Find("Score");
        if (!score)
            Assert.Fail("There is no score text-field in scene named \"Score\"!");

        //Check components
        if (!PMHelper.Exist<RectTransform>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Rect Transform>");
        Canvas canvasC = PMHelper.Exist<Canvas>(canvas);
        if (!canvasC)
            Assert.Fail("\"Canvas\" object has no component <Canvas>");
        if (canvasC.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Assert.Fail(
                "\"Canvas\"' <Canvas> component should have it's Render Mode property set to \"Screen Space - Overlay\"," +
                " that should have it render in front of everything");
        }

        if (!PMHelper.Exist<CanvasScaler>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Canvas Scaler>");
        if (!PMHelper.Exist<GraphicRaycaster>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Graphic Raycaster>");

        RectTransform rect = PMHelper.Exist<RectTransform>(score);
        if (!rect)
            Assert.Fail("\"Score\" object has no component <Rect Transform>");
        if (!PMHelper.CheckRectTransform(rect))
        {
            Assert.Fail("Anchors of \"Score\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }

        text = PMHelper.Exist<Text>(score);
        if (!text)
            Assert.Fail("\"Score\" object has no component <Text>");
        if (!PMHelper.Exist<CanvasRenderer>(score))
            Assert.Fail("\"Score\" object has no component <Canvas Renderer>");

        //Check if score is child of canvas
        if (!PMHelper.Child(score, canvas))
        {
            Assert.Fail("\"Score\" object should be a child of \"Canvas\" object");
        }

        //Check if 0 by default
        if (text.text != "0")
        {
            Assert.Fail("\"Score\"'s text value should be initialized as \"0\" by default");
        }
    }

    [UnityTest, Order(1)]
    public IEnumerator CheckScoreChanging()
    {
        //Check correct score increasing
        Floor = GameObject.Find("Floor");
        LoseLeft = GameObject.Find("LoseLeft");

        Physics.IgnoreLayerCollision(testLayer, testLayer, false);

        bool[] testCases = {false, true, false, true, true, false, false, true, true, true, true, false};
        int score = 0;
        GameObject cubeTemplate = GameObject.FindWithTag("FallingCube");
        foreach (bool b in testCases)
        {
            GameObject colliding = b ? Floor : LoseLeft;
            GameObject cube = GameObject.Instantiate(cubeTemplate);
            float start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                cube || (Time.unscaledTime - start) * Time.timeScale > 1);

            LayerMask beforeLayer = colliding.layer;
            colliding.layer = testLayer;
            cube.layer = testLayer;
            cube.transform.position = colliding.transform.position;

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !cube || (Time.unscaledTime - start) * Time.timeScale > 1);

            score = b ? score + 1 : 0;
            if (text.text != score.ToString())
            {
                if (b)
                    Assert.Fail("Score text field not changing, or changing not properly");
                else
                    Assert.Fail("Score text field should change to \"0\" if player loses");
            }

            colliding.layer = beforeLayer;
        }
    }
}