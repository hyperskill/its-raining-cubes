using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class D_PlayLogicTest
{
    private GameObject LoseLeft, LoseRight, OkLeft, OkRight, Floor;

    [UnityTest, Order(0)]
    public IEnumerator CheckDestroying()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 0.5f;
        yield return null;

        GameObject cube = GameObject.FindWithTag("FallingCube");
        
        OkLeft = GameObject.Find("OkLeft");
        OkRight = GameObject.Find("OkRight");
        Floor = GameObject.Find("Floor");

        //Check if gap is too narrow
        if (Mathf.Abs(
            (OkLeft.transform.position.x + OkLeft.transform.localScale.x / 2)
            - (OkRight.transform.position.x - OkRight.transform.localScale.x / 2)
        )<=cube.transform.localScale.x*2)
        {
            Assert.Fail("Gap is too narrow to pass the cube, make it at least twice as wide as FallingCube's objects");
        }
        
        //Check if cubes are being destroyed
        Scene curScene = SceneManager.GetActiveScene();
        Vector3 okLeftWas = OkLeft.transform.position;
        Vector3 okRightWas = OkRight.transform.position;
        Vector3 floorWas = Floor.transform.position;
        
        OkLeft.transform.position = cube.transform.position;
        yield return null;
        yield return null;
        if (curScene!=SceneManager.GetActiveScene())
            Assert.Fail("Scene should not be reloaded, when \"FallingCube\"'s objects collide with \"Ok*\" objects");
        if (!OkLeft)
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Ok*\", \"Ok*\" should not be destroyed");
        if (!cube)
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Ok*\", they should not be destroyed");
        OkLeft.transform.position = okLeftWas;
        
        OkRight.transform.position = cube.transform.position;
        yield return null;
        yield return null;
        if (curScene!=SceneManager.GetActiveScene())
            Assert.Fail("Scene should not be reloaded, when \"FallingCube\"'s objects collide with \"Ok*\" objects");
        if (!OkRight)
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Ok*\", \"Ok*\" should not be destroyed");
        if (!cube)
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Ok*\", they should not be destroyed");
        OkRight.transform.position = okRightWas;
        
        Floor.transform.position = cube.transform.position;
        // yield return null;
        // yield return null;
        yield return new WaitUntil(() => cube==null);
        if (curScene!=SceneManager.GetActiveScene())
            Assert.Fail("Scene should not be reloaded, when \"FallingCube\"'s objects collide with the \"Floor\"");
        if (!Floor)
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Floor\", \"Floor\" should not be destroyed");
        if (cube) 
            Assert.Fail("When \"FallingCube\"'s objects collide with the \"Floor\", they should be destroyed");
        Floor.transform.position = floorWas;
        
        SceneManager.LoadScene("Game");
        yield return null;
        cube = GameObject.FindWithTag("FallingCube");
        yield return null;
        curScene = SceneManager.GetActiveScene();
        
        LoseLeft = GameObject.Find("LoseRight");
        yield return null;
        
        LoseLeft.transform.position = cube.transform.position;
        // yield return null;
        // yield return null;
        yield return new WaitUntil(() => curScene!=SceneManager.GetActiveScene());
        if (curScene==SceneManager.GetActiveScene())
            Assert.Fail("Scene should be reloaded, when \"FallingCube\"'s objects collide with \"Lose*\" objects");

        SceneManager.LoadScene("Game");
        yield return null;
        cube = GameObject.FindWithTag("FallingCube");
        yield return null;
        curScene = SceneManager.GetActiveScene();
        
        LoseRight = GameObject.Find("LoseRight");
        // yield return null;
        yield return new WaitUntil(() => curScene!=SceneManager.GetActiveScene());
        
        LoseRight.transform.position = cube.transform.position;
        yield return null;
        yield return null;
        if (curScene==SceneManager.GetActiveScene())
            Assert.Fail("Scene should be reloaded, when \"FallingCube\"'s objects collide with \"Lose*\" objects");
        
        //Check timing
        SceneManager.LoadScene("Game");
        yield return null;
        cube = GameObject.FindWithTag("FallingCube");
        yield return null;
        
        GameObject.Destroy(GameObject.Find("Platform"));
        GameObject.Destroy(GameObject.Find("Floor"));
        yield return null;

        Time.timeScale = 40;

        yield return new WaitForSeconds(4);

        if (cube.transform.position.y > floorWas.y)
        {
            Assert.Fail("\"FallingCube\"'s objects are falling too slow, they should be able to reach the \"Floor\" object in less than 4 seconds");
        }
    }
}