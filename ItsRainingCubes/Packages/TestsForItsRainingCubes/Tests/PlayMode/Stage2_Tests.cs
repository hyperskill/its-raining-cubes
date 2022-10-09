using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

[Description("I did not plan to make the Cube"), Category("2")]
public class Stage2_Tests
{
    Transform checkLeft, checkRight, checkMid;
    float tmpObjSize = 0.1f;
    GameObject platform;
    Camera camera;
    private GameObject helper;

    [UnityTest, Order(0)]
    public IEnumerator CheckMovementLeft()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 40;
        yield return null;
        
        helper = new GameObject("Helper");
        helper.AddComponent<StagesHelper>();
        
        platform = GameObject.Find("Platform");
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //Creating 3 detect tmp objects
        Transform LoseLeft = GameObject.Find("LoseLeft").transform;
        Transform LoseRight = GameObject.Find("LoseRight").transform;
        Vector3 leftPlace=LoseLeft.localPosition
                          +new Vector3(-(LoseLeft.localScale.x/2),LoseLeft.localScale.y/2,LoseLeft.localScale.z/2),
            rightPlace=LoseRight.localPosition
                       +new Vector3(LoseRight.localScale.x/2,LoseLeft.localScale.y/2,LoseLeft.localScale.z/2),
            midPlace=new Vector3(0,0,0);
        
        GameObject tmpLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tmpLeft.GetComponent<MeshRenderer>().enabled = false;
        checkLeft = tmpLeft.transform;
        checkLeft.SetParent(platform.transform);
        checkLeft.localPosition = leftPlace;
        checkLeft.localScale = new Vector3(tmpObjSize, tmpObjSize, tmpObjSize);
        
        GameObject tmpRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tmpRight.GetComponent<MeshRenderer>().enabled = false;
        checkRight = tmpRight.transform;
        checkRight.SetParent(platform.transform);
        checkRight.localPosition = rightPlace;
        checkRight.localScale = new Vector3(tmpObjSize, tmpObjSize, tmpObjSize);

        GameObject tmpMid = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tmpMid.GetComponent<MeshRenderer>().enabled = false;
        checkMid = tmpMid.transform;
        checkMid.SetParent(platform.transform);
        checkMid.localPosition = midPlace;
        checkMid.localScale = new Vector3(tmpObjSize, tmpObjSize, tmpObjSize);
        
        //Check,that left part of LoseLeft and right part of LoseRight are out of camera view
        if (PMHelper.CheckVisibility(camera, checkLeft,3) || PMHelper.CheckVisibility(camera, checkRight,3))
            Assert.Fail("Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
        if (!PMHelper.CheckVisibility(camera, checkMid,3))
            Assert.Fail("Gap between two parts should be always visible by camera");
        
        //Check, that movement is working properly
        
        Vector3 wasMid = checkMid.position;
        VInput.KeyPress(KeyCode.A);
        yield return null;
        if (wasMid.x <= checkMid.position.x)
            Assert.Fail("Platform is not moving to the left by pressing \"A\"-key");
        if(checkMid.position.y!=wasMid.y || checkMid.position.z!=wasMid.z)
            Assert.Fail("Y-axis and z-axis of platform should not be changed");
        
        wasMid = checkMid.position;
        VInput.KeyPress(KeyCode.D);
        yield return null;
        if (wasMid.x >= checkMid.position.x)
            Assert.Fail("Platform is not moving to the right by pressing \"D\"-key");
        if(checkMid.position.y!=wasMid.y || checkMid.position.z!=wasMid.z)
            Assert.Fail("Y-axis and z-axis of platform should not be changed");
        
        wasMid = checkMid.position;
        VInput.KeyPress(KeyCode.LeftArrow);
        yield return null;
        if (wasMid.x <= checkMid.position.x)
            Assert.Fail("Platform is not moving to the left by pressing \"Left Arrow\"-key");
        if(checkMid.position.y!=wasMid.y || checkMid.position.z!=wasMid.z)
            Assert.Fail("Y-axis and z-axis of platform should not be changed");

        wasMid = checkMid.position;
        VInput.KeyPress(KeyCode.RightArrow);
        yield return null;
        if (wasMid.x >= checkMid.position.x)
            Assert.Fail("Platform is not moving to the right by pressing \"Right Arrow\"-key");
        if(checkMid.position.y!=wasMid.y || checkMid.position.z!=wasMid.z)
            Assert.Fail("Y-axis and z-axis of platform should not be changed");
    }
    [UnityTest, Order(1)]
    
    public IEnumerator CheckLimit()
    {
        //Moving platform to the left bound
        yield return null;
        VInput.KeyDown(KeyCode.A);
        yield return null;
        for (int i = 0; i < 30; i++)
        {
            if (PMHelper.CheckVisibility(camera, checkLeft,3) || PMHelper.CheckVisibility(camera, checkRight,3))
                Assert.Fail("Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
            if (!PMHelper.CheckVisibility(camera, checkMid,3))
                Assert.Fail("Gap between two parts should be always visible by camera");
            yield return new WaitForSeconds(0.1f);
        }
        VInput.KeyUp(KeyCode.A);
        yield return null;
        
        //Checking right limit
        
        VInput.KeyDown(KeyCode.D);
        yield return null;
        for (int i = 0; i < 30; i++)
        {
            if (PMHelper.CheckVisibility(camera, checkLeft,3) || PMHelper.CheckVisibility(camera, checkRight,3))
                Assert.Fail("Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
            if (!PMHelper.CheckVisibility(camera, checkMid,3))
                Assert.Fail("Gap between two parts should be always visible by camera");
            yield return new WaitForSeconds(0.1f);
        }
        VInput.KeyUp(KeyCode.D);
        yield return null;
        Vector3 wasMid1 = checkMid.position;
        VInput.KeyPress(KeyCode.D);
        yield return null;

        if (wasMid1.x < checkMid.position.x)
        {
            Assert.Fail("Platform should be able to reach any limit zone by 3 seconds");
        }
        
        //Checking left limit
        
        VInput.KeyDown(KeyCode.A);
        yield return null;
        for (int i = 0; i < 30; i++)
        {
            if (PMHelper.CheckVisibility(camera, checkLeft,3) || PMHelper.CheckVisibility(camera, checkRight,3))
                Assert.Fail("Left part of \"LoseLeft\" object and right part of \"LoseRight\" object should always be out of camera view");
            if (!PMHelper.CheckVisibility(camera, checkMid,3))
                Assert.Fail("Gap between two parts should be always visible by camera");
            yield return new WaitForSeconds(0.1f);
        }
        VInput.KeyUp(KeyCode.A);
        yield return null;
        Vector3 wasMid2 = checkMid.position;
        VInput.KeyPress(KeyCode.A);
        yield return null;

        if (wasMid2.x > checkMid.position.x)
        {
            Assert.Fail("Platform should be able to reach any limit zone by 3 seconds");
        }

        if (Mathf.Abs(wasMid1.x) != Mathf.Abs(wasMid2.x))
        {
            Assert.Fail("Limit absolute value for both sides should be the same");
        }
    }
}