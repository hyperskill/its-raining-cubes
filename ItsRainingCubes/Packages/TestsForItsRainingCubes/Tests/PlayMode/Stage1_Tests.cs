using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("The cube is the sole working tool for creation"), Category("1")]
public class Stage1_Tests
{
    private GameObject Platform,
        LoseLeft,
        LoseRight,
        Floor,
        Wall,
        OkLeft,
        OkRight,
        MainCamera,
        GameManager;

    [UnityTest, Order(0)]
    public IEnumerator ObjectCheck()
    {
        yield return null;
        
        if (!Application.CanStreamedLevelBeLoaded("Game"))
        {
            Assert.Fail("\"Game\" scene is misspelled or was not added to build settings");
        }
        SceneManager.LoadScene("Game");
        
        yield return null;
        
        //Check objects Exist 
        Floor = GameObject.Find("Floor");
        if(!Floor)
            Assert.Fail("There should be a \"Floor\" object on scene");
        Wall = GameObject.Find("Wall");
        if(!Wall)
            Assert.Fail("There should be a \"Wall\" object on scene");
        Platform = GameObject.Find("Platform");
        if(!Platform)
            Assert.Fail("There should be a \"Platform\" object on scene");
        LoseLeft = GameObject.Find("LoseLeft");
        if(!LoseLeft)
            Assert.Fail("There should be a \"LoseLeft\" object on scene");
        OkLeft = GameObject.Find("OkLeft");
        if(!OkLeft)
            Assert.Fail("There should be a \"OkLeft\" object on scene");
        LoseRight = GameObject.Find("LoseRight");
        if(!LoseRight)
            Assert.Fail("There should be a \"LoseRight\" object on scene");
        OkRight = GameObject.Find("OkRight");
        if(!OkRight)
            Assert.Fail("There should be a \"OkRight\" object on scene");
        GameManager = GameObject.Find("GameManager");
        if(!GameManager)
            Assert.Fail("There should be a \"GameManager\" object on scene");
        MainCamera = GameObject.Find("Main Camera");
        if(!MainCamera)
            Assert.Fail("There should be a camera, named \"MainCamera\" object on scene");
        
        //Check child-parent hierarchy
        if(!PMHelper.Child(LoseLeft,Platform))
            Assert.Fail("\"LoseLeft\" object should be a child of \"Platform\" object");
        if(!PMHelper.Child(OkLeft,Platform))
            Assert.Fail("\"OkLeft\" object should be a child of \"Platform\" object");
        if(!PMHelper.Child(LoseRight,Platform))
            Assert.Fail("\"LoseRight\" object should be a child of \"Platform\" object");
        if(!PMHelper.Child(OkRight,Platform))
            Assert.Fail("\"OkRight\" object should be a child of \"Platform\" object");
        
        yield return null;
    }

    [UnityTest, Order(1)]
    public IEnumerator ComponentsCheck()
    {
        yield return null;
        //Check, that objects were added as Primitive Cube
        if(!PMHelper.Check3DPrimitivity(Floor, PrimitiveType.Cube))
            Assert.Fail("\"Floor\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.Check3DPrimitivity(Wall, PrimitiveType.Cube))
            Assert.Fail("\"Wall\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.Check3DPrimitivity(LoseLeft, PrimitiveType.Cube))
            Assert.Fail("\"LoseLeft\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.Check3DPrimitivity(LoseRight, PrimitiveType.Cube))
            Assert.Fail("\"LoseRight\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.Check3DPrimitivity(OkLeft, PrimitiveType.Cube))
            Assert.Fail("\"OkLeft\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.Check3DPrimitivity(OkRight, PrimitiveType.Cube))
            Assert.Fail("\"OkRight\" object was not created as primitive Cube object, it's mesh differences or components missing");
        
        //Check material difference
        if(!PMHelper.CheckMaterialDifference(Floor))
            Assert.Fail("The material of an object <Floor> should be changed!");
        if(!PMHelper.CheckMaterialDifference(Wall))
            Assert.Fail("The material of an object <Wall> should be changed!");
        if(!PMHelper.CheckMaterialDifference(LoseLeft))
            Assert.Fail("The material of an object <LoseLeft> should be changed!");
        if(!PMHelper.CheckMaterialDifference(LoseRight))
            Assert.Fail("The material of an object <LoseRight> should be changed!");
        if(!PMHelper.CheckMaterialDifference(OkLeft))
            Assert.Fail("The material of an object <OkLeft> should be changed!");
        if(!PMHelper.CheckMaterialDifference(OkRight))
            Assert.Fail("The material of an object <OkRight> should be changed!");
    }
    
    [UnityTest, Order(2)]
    public IEnumerator ObjectsCameraView()
    {
        Camera camera = PMHelper.Exist<Camera>(MainCamera);
        if(camera==null)
            Assert.Fail("There is no \"Camera\" component on \"Main Camera\" object");
        
        yield return null;
        
        //Check, that camera can see an object
        if(!PMHelper.CheckVisibility(camera, Floor.transform,3))
            Assert.Fail("\"Floor\" object is out of the camera view");
        if(!PMHelper.CheckVisibility(camera, Wall.transform,3))
            Assert.Fail("\"Wall\" object is out of the camera view");
        if(!PMHelper.CheckVisibility(camera, LoseLeft.transform,3))
            Assert.Fail("\"LoseLeft\" object is out of the camera view");
        if(!PMHelper.CheckVisibility(camera, LoseRight.transform,3))
            Assert.Fail("\"LoseRight\" object is out of the camera view");
        if(!PMHelper.CheckVisibility(camera, OkLeft.transform,3))
            Assert.Fail("\"OkLeft\" object is out of the camera view");
        if(!PMHelper.CheckVisibility(camera, OkRight.transform,3))
            Assert.Fail("\"OkRight\" object is out of the camera view");
    }
    
    //Check positioning
    [UnityTest, Order(3)]
    public IEnumerator PlatformCorrectCheck()
    {
        if (Platform.transform.position.x != 0)
        {
            Assert.Fail("\"Platform\"'s x-axis should be equal to 0");
        }
        Vector3 LoseLeftPosLocal = LoseLeft.transform.localPosition;
        Vector3 OkLeftPosLocal = OkLeft.transform.localPosition;
        Vector3 LoseRightPosLocal = LoseRight.transform.localPosition;
        Vector3 OkRightPosLocal = OkRight.transform.localPosition;
        
        Vector3 LoseLeftPos = LoseLeft.transform.position;
        Vector3 OkLeftPos = OkLeft.transform.position;
        Vector3 LoseRightPos = LoseRight.transform.position;
        Vector3 OkRightPos = OkRight.transform.position;

        yield return null;
        if (LoseLeftPosLocal.z != 0 || OkLeftPosLocal.z != 0 || LoseRightPosLocal.z != 0 || OkRightPosLocal.z != 0 ||
            LoseLeftPosLocal.y != 0 || OkLeftPosLocal.y != 0 || LoseRightPosLocal.y != 0 || OkRightPosLocal.y != 0)
        {
            Assert.Fail("\"LoseLeft\",\"LoseRight\",\"OkLeft\" and \"OkRight\" object's local y-axis and z-axis should" +
                        " be equal to 0. All position changes should be provided by their parent's \"Platform\" object");
        }

        if (LoseLeftPos.x + LoseLeft.transform.localScale.x / 2 != OkLeftPos.x - OkLeft.transform.localScale.x / 2)
        {
            Assert.Fail("Right face's x-axis of \"LoseLeft\" object should be equal to " +
                        "left face's x-axis of \"OkLeft\" object");
        }

        if (OkLeftPos.x >= OkRightPos.x)
        {
            Assert.Fail("Left part of platform should have less x-axis value, than the right one");
        }
        
        if (LoseLeftPos.x != -LoseRightPos.x ||
            LoseLeft.transform.localRotation != LoseRight.transform.localRotation ||
            LoseLeft.transform.localScale != LoseRight.transform.localScale)
        {
            Assert.Fail("\"Transform\" components of \"LoseLeft\" and \"LoseRight\" should be identical," +
                        "except for position's x-axis value - those two should be opposite");
        }
        
        if (OkLeftPos.x != -OkRightPos.x ||
            OkLeft.transform.localRotation != OkRight.transform.localRotation ||
            OkLeft.transform.localScale != OkRight.transform.localScale)
        {
            Assert.Fail("\"Transform\" components of \"OkLeft\" and \"OkRight\" should be identical," +
                        "except for position's x-axis value - those two should be opposite");
        }
        
        if (OkLeftPos.x + OkLeft.transform.localScale.x / 2 == OkRightPos.x - OkRight.transform.localScale.x / 2)
        {
            Assert.Fail("There should be a gap between two parts of platform");
        }
        
    }
    [UnityTest, Order(4)]
    public IEnumerator RelativeCheck()
    {
        yield return null;
        int floorWasLayer = Floor.layer,
            loseLeftWasLayer = LoseLeft.layer,
            loseRightWasLayer = LoseRight.layer,
            okLeftWasLayer = OkLeft.layer,
            okRightWasLayer = OkRight.layer,
            wallWasLayer = Wall.layer;
        
        Floor.layer = LayerMask.NameToLayer("Test");
        LoseLeft.layer = LayerMask.NameToLayer("Test");
        LoseRight.layer = LayerMask.NameToLayer("Test");
        OkLeft.layer = LayerMask.NameToLayer("Test");
        OkRight.layer = LayerMask.NameToLayer("Test");
        Wall.layer = LayerMask.NameToLayer("Test");
        
        yield return null;

        Vector3 direction= -(MainCamera.transform.position - LoseLeft.transform.position).normalized;;

        if(PMHelper.RaycastFront3D(MainCamera.transform.position,direction,1 << 16).collider.gameObject==Wall)
            Assert.Fail("\"Wall\" object should be behind the platform (greater z-axis value)");
        if(PMHelper.RaycastFront3D(MainCamera.transform.position,direction,1 << 16).collider.gameObject==Floor)
            Assert.Fail("\"Floor\" object should be under the platform (less y-axis value)");
        
        if (PMHelper.RaycastFront3D(MainCamera.transform.position, direction, 1 << 16).normal != Vector3.up)
        {
            Assert.Fail("Platform should be placed lower (less y-axis) or/and be wider (greater x-axis) and not be rotated, " +
                        "so that the camera would see much more of it's top face, not the border ones");
        }
        direction = -(MainCamera.transform.position - Floor.transform.position).normalized;
        if (direction.x != 0)
        {
            Assert.Fail("\"Floor\" object should be in front of \"Camera\" object (their x-axis value should be the same)");
        }
        
        if (PMHelper.RaycastFront3D(MainCamera.transform.position, direction, 1 << 16).normal != Vector3.up)
            Assert.Fail("\"Floor\" should be placed lower (less y-axis) or/and be wider (greater x-axis) and not be rotated, " +
                        "so that the camera would see much more of it's top face, not the border ones");
        
        LoseLeft.layer = loseLeftWasLayer;
        LoseRight.layer = loseRightWasLayer;
        OkLeft.layer = okLeftWasLayer;
        OkRight.layer = okRightWasLayer;
        
        RaycastHit hit = PMHelper.RaycastFront3D(LoseLeft.transform.position, Vector3.forward, 1 << 16);
        if(!(hit.collider!=null && hit.collider.gameObject==Wall))
            Assert.Fail("\"Wall\" object should be wider (greater x-axis) and behind the platform (not colliding with it)," +
                        " platform objects should be sliding by the wall");
        hit = PMHelper.RaycastFront3D(LoseRight.transform.position, Vector3.forward, 1 << 16);
        if (!(hit.collider != null && hit.collider.gameObject == Wall))
            Assert.Fail("\"Wall\" object should be wider (greater x-axis) and behind the platform (not colliding with it)," +
                        " platform objects should be sliding by the wall");
        hit = PMHelper.RaycastFront3D(LoseLeft.transform.position, Vector3.down, 1 << 16);
        if(!(hit.collider!=null && hit.collider.gameObject==Floor))
            Assert.Fail("\"Floor\" object should be wider (greater x-axis) and under the platform (not colliding with it)," +
                        " platform objects should be sliding over the floor");
        hit = PMHelper.RaycastFront3D(LoseLeft.transform.position, Vector3.down, 1 << 16);
        if(!(hit.collider!=null && hit.collider.gameObject==Floor))
            Assert.Fail("\"Floor\" object should be wider (greater x-axis) and under the platform (not colliding with it)," +
                        " platform objects should be sliding over the floor");
        
        Floor.layer = floorWasLayer;
        Wall.layer = wallWasLayer;
    }
}