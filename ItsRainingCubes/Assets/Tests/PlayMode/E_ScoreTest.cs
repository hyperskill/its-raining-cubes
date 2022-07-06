using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
public class E_ScoreTest
{
    public GameObject score;
    public GameObject canvas;

    public GameObject LoseLeft;
    public GameObject Floor;
    private Text text;
    
    
    [UnityTest, Order(0)]
    public IEnumerator CheckCorrect()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 20;
        yield return null;
        
        //Check existence
        canvas = PMHelper.Exist("Canvas");
        if (!canvas)
            Assert.Fail("There is no canvas in scene named \"Canvas\"");
        score = PMHelper.Exist("Score");
        if(!score)
            Assert.Fail("There is no score text-field in scene named \"Score\"!");

        //Check components
        if(!PMHelper.Exist<RectTransform>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Rect Transform>");
        if(!PMHelper.Exist<Canvas>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Canvas>");
        if(!PMHelper.Exist<CanvasScaler>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Canvas Scaler>");
        if(!PMHelper.Exist<GraphicRaycaster>(canvas))
            Assert.Fail("\"Canvas\" object has no component <Graphic Raycaster>");

        RectTransform rect = PMHelper.Exist<RectTransform>(score);
        text = PMHelper.Exist<Text>(score);
        if(!rect)
            Assert.Fail("\"Score\" object has no component <Rect Transform>");
        if (PMHelper.CheckRectTransform(rect))
        {
            Assert.Fail("Anchors of \"Score\"'s <RectTransform> component are incorrect or it's offsets " +
                        "are not equal to zero, might be troubles with different resolutions");
        }
        if(!text)
            Assert.Fail("\"Score\" object has no component <Text>");
        if(!PMHelper.Exist<CanvasRenderer>(score))
            Assert.Fail("\"Score\" object has no component <Canvas Renderer>");

        //Check if score is child of canvas
        if (!PMHelper.Child(score, canvas))
        {
            Assert.Fail("\"Score\" object should be a child of \"Canvas\" object");
        }

        //Check if by default 0
        int tmp = -1;
        try
        {
            tmp = int.Parse(text.text);
        }
        catch (Exception)
        {
            Assert.Fail("\"Score\"'s text value should be initialized as \"0\" by default");
        }

        if (tmp != 0)
        {
            Assert.Fail("\"Score\"'s text value should be initialized as \"0\" by default");
        }
    }
    
    [UnityTest, Order(1)]
    public IEnumerator CheckScoreChanging()
    {
        //Check correct score increasing
        Floor = GameObject.Find("Floor");
        yield return null;
        Vector3 wasFloor = Floor.transform.position;
        GameObject cube;
        for (int i = 0; i < 5; i++)
        {
             cube = GameObject.FindWithTag("FallingCube");
            Floor.transform.position = cube.transform.position;
            yield return null;
            yield return null;
            Floor.transform.position = wasFloor;
            if (text.text!=(i+1).ToString())
            {
                Assert.Fail("Score text field not changing, or changing not properly");
            }

            yield return new WaitForSeconds(1);
        }
        //Check correct score decreasing with reloaded scene
        LoseLeft = GameObject.Find("LoseLeft");
        Scene curScene = SceneManager.GetActiveScene();
        cube = GameObject.FindWithTag("FallingCube");
        yield return null;
        LoseLeft.transform.position = cube.transform.position;
        yield return null;
        yield return null;
        if (curScene == SceneManager.GetActiveScene())
        {
            Assert.Fail("Scene is not being reloaded, when \"FallingCube\" object is colliding with \"Lose*\" objects");
        }
        
        score = PMHelper.Exist("Score");
        text = PMHelper.Exist<Text>(score);
        
        yield return null;
        if (!score || !text)
        {
            Assert.Fail();
        }
        if (text.text!="0")
        {
            Assert.Fail("Score text field not changing to \"0\" if player loses!");
        }
        
        //Check correct score increasing after scene is reloaded
        Floor = GameObject.Find("Floor");
        yield return null;
        wasFloor = Floor.transform.position;
        for (int i = 0; i < 5; i++)
        {
            cube = GameObject.FindWithTag("FallingCube");
            Floor.transform.position = cube.transform.position;
            yield return null;
            yield return null;
            Floor.transform.position = wasFloor;
            if (text.text!=(i+1).ToString())
            {
                Assert.Fail("Score text field not changing, or changing not properly after player loses");
            }

            yield return new WaitForSeconds(1);
        }
    }
}