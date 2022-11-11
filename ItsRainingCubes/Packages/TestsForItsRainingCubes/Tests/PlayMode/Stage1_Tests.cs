using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("The cube is the sole working tool for creation"), Category("1")]
public class Stage1_Tests
{
    private GameObject Platform, LoseLeft, LoseRight, Floor, Wall, OkLeft, OkRight, MainCamera, GameManager;
    private LayerMask testLayer;

    [UnityTest, Order(0)]
    public IEnumerator ObjectCheck()
    {
        if (!Application.CanStreamedLevelBeLoaded("Game"))
        {
            Assert.Fail("\"Game\" scene is misspelled or was not added to build settings");
        }

        PMHelper.TurnCollisions(false);
        SceneManager.LoadScene("Game");

        float start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }

        if (!PMHelper.CheckLayerExistance("Test"))
        {
            Assert.Fail("Please, do not remove \"Test\" layer, it's existence necessary for tests");
        }

        testLayer = LayerMask.NameToLayer("Test");

        //Check objects Exist 
        Floor = GameObject.Find("Floor");
        if (!Floor)
            Assert.Fail("There should be a \"Floor\" object on scene");
        Wall = GameObject.Find("Wall");
        if (!Wall)
            Assert.Fail("There should be a \"Wall\" object on scene");
        Platform = GameObject.Find("Platform");
        if (!Platform)
            Assert.Fail("There should be a \"Platform\" object on scene");
        LoseLeft = GameObject.Find("LoseLeft");
        if (!LoseLeft)
            Assert.Fail("There should be a \"LoseLeft\" object on scene");
        OkLeft = GameObject.Find("OkLeft");
        if (!OkLeft)
            Assert.Fail("There should be a \"OkLeft\" object on scene");
        LoseRight = GameObject.Find("LoseRight");
        if (!LoseRight)
            Assert.Fail("There should be a \"LoseRight\" object on scene");
        OkRight = GameObject.Find("OkRight");
        if (!OkRight)
            Assert.Fail("There should be a \"OkRight\" object on scene");
        GameManager = GameObject.Find("GameManager");
        if (!GameManager)
            Assert.Fail("There should be a \"GameManager\" object on scene");
        MainCamera = GameObject.Find("Main Camera");
        if (!MainCamera)
            Assert.Fail("There should be a camera, named \"MainCamera\" object on scene");

        //Check child-parent hierarchy
        (GameObject, string)[] platformPieces =
        {
            (LoseLeft, "LoseLeft"), (OkLeft, "OkLeft"), (LoseRight, "LoseRight"), (OkRight, "OkRight")
        };
        foreach ((GameObject, string) p in platformPieces)
        {
            if (!PMHelper.Child(p.Item1, Platform))
                Assert.Fail("\"" + p.Item2 + "\" object should be a child of \"Platform\" object");
        }

        //Check, that objects were added as Primitive Cube
        (GameObject, string)[] objects =
        {
            (Floor, "Floor"), (Wall, "Wall"), (LoseLeft, "LoseLeft"), (OkLeft, "OkLeft"), (LoseRight, "LoseRight"),
            (OkRight, "OkRight")
        };
        foreach ((GameObject, string) p in objects)
        {
            if (!PMHelper.Check3DPrimitivity(p.Item1, PrimitiveType.Cube))
                Assert.Fail("\"" + p.Item2 +
                            "\" object was not created as primitive Cube object, it's mesh differs or components missing");
        }

        //Check material difference
        foreach ((GameObject, string) p in objects)
        {
            if (!PMHelper.CheckMaterialDifference(p.Item1))
                Assert.Fail("The material of \"" + p.Item2 + "\" object should be changed!");
        }

        //Check, that camera can see an object
        Camera camera = PMHelper.Exist<Camera>(MainCamera);
        if (!camera)
            Assert.Fail("There is no \"Camera\" component on \"Main Camera\" object");

        foreach ((GameObject, string) p in objects)
        {
            if (!PMHelper.CheckVisibility(camera, p.Item1.transform, 3))
                Assert.Fail("\"" + p.Item2 + "\" object is out of the camera view");
        }

       
        //Check positioning
        if (Platform.transform.position.x != 0)
        {
            Assert.Fail("\"Platform\"'s x-axis should be equal to 0");
        }

        foreach ((GameObject, string) p in platformPieces)
        {
            if (p.Item1.transform.localPosition.z != 0 || p.Item1.transform.localPosition.y != 0)
            {
                Assert.Fail(
                    "\"LoseLeft\",\"LoseRight\",\"OkLeft\" and \"OkRight\" object's local y-axis and z-axis should" +
                    " be equal to 0. All position changes should be provided by their parent's \"Platform\" object");
            }
        }

        Transform LoseLeftT = LoseLeft.transform,
            OkLeftT = OkLeft.transform,
            LoseRightT = LoseRight.transform,
            OkRightT = OkRight.transform;

        if (LoseLeftT.position.x + LoseLeftT.localScale.x / 2 != OkLeftT.position.x - OkLeftT.localScale.x / 2)
        {
            Assert.Fail("Right face's x-axis of \"LoseLeft\" object should be equal to " +
                        "left face's x-axis of \"OkLeft\" object");
        }

        if (OkLeftT.position.x >= OkRightT.position.x)
        {
            Assert.Fail("Left part of platform should have less x-axis value, than the right one");
        }
       
        if (LoseLeftT.position.x != -LoseRightT.position.x ||
            LoseLeftT.localRotation != LoseRightT.localRotation ||
            LoseLeftT.localScale != LoseRightT.localScale)
        {
            Assert.Fail("\"Transform\" components of \"LoseLeft\" and \"LoseRight\" should be identical, " +
                        "except for position's x-axis value - those two should be opposite");
        }

        if (OkLeftT.position.x != -OkRightT.position.x ||
            OkLeftT.localRotation != OkRightT.localRotation ||
            OkLeftT.localScale != OkRightT.localScale)
        {
            Assert.Fail("\"Transform\" components of \"OkLeft\" and \"OkRight\" should be identical, " +
                        "except for position's x-axis value - those two should be opposite");
        }

        if (Math.Abs(OkLeftT.position.x + OkLeftT.localScale.x / 2 -
                     (OkRightT.position.x - OkRightT.localScale.x / 2)) <= 0.01f)
        {
            Assert.Fail("There should be a gap between two parts of platform");
        }

        //Check relative
        foreach ((GameObject, string) p in objects)
        {
            p.Item1.layer = testLayer;
        }
        
        Vector3 direction;
        foreach ((GameObject, string) p in platformPieces)
        {
            direction = -(MainCamera.transform.position - p.Item1.transform.position).normalized;

            RaycastHit hit = PMHelper.RaycastFront3D(MainCamera.transform.position, direction, 1<<testLayer);
            if (hit.collider.gameObject == Wall)
                Assert.Fail("\"Wall\" object should be behind the platform (greater z-axis value)");
            if (hit.collider.gameObject == Floor)
                Assert.Fail("\"Floor\" object should be under the platform (less y-axis value)");
            if (hit.normal != Vector3.up)
            {
                Assert.Fail(
                    "Platform should not be rotated and it's top face should easily be visible by camera");
            }
        }

        direction = -(MainCamera.transform.position - Floor.transform.position).normalized;
        if (direction.x != 0)
        {
            Assert.Fail(
                "\"Floor\" object should be right in front of \"Camera\" object and their x-axis value should be the same");
        }

        if (PMHelper.RaycastFront3D(MainCamera.transform.position, direction, 1<<testLayer).normal != Vector3.up)
            Assert.Fail(
                "\"Floor\" object should not be rotated and it's top face should easily be visible by camera");
       
        //Remove Test layer from platform pieces
        foreach ((GameObject, string) p in platformPieces)
        {
            p.Item1.layer = LayerMask.NameToLayer("Default");
        }

        foreach ((GameObject, string) p in platformPieces)
        {
            RaycastHit hit1 = PMHelper.RaycastFront3D(p.Item1.transform.position, Vector3.forward, 1<<testLayer);
            if (!(hit1.collider != null && hit1.collider.gameObject == Wall))
                Assert.Fail(
                    "\"Wall\" object should be wider (greater x-axis) and behind the platform, so that platform objects should be sliding by it");
            RaycastHit hit2 = PMHelper.RaycastFront3D(p.Item1.transform.position, Vector3.down, 1<<testLayer);
            if (!(hit2.collider != null && hit2.collider.gameObject == Floor))
                Assert.Fail(
                    "\"Floor\" object should be wider (greater x-axis) and under the platform, platform objects should be sliding over it");
        }
    }
}