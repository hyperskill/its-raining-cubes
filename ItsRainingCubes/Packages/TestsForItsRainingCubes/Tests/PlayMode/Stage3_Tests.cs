using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("Want a sugar cube?"), Category("3")]
public class Stage3_Tests
{
    Camera camera;
    private GameObject helper;
    
    [UnityTest, Order(0)]
    public IEnumerator CheckCorrectSpawn()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 40;

        yield return null;
       
        //Check if tag exist
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals("FallingCube")) { found = true; break; }
        }

        if (!found)
        {
            Assert.Fail("\"FallingCube\" tag was not added to project settings");
        }
        
        //Check borders
        helper = new GameObject("Helper");
        helper.AddComponent<StagesHelper>();
        
        float leftX, rightX;
        
        VInput.KeyDown(KeyCode.A);
        yield return new WaitForSeconds(3);
        VInput.KeyUp(KeyCode.A);
        yield return null;
        leftX = GameObject.Find("Platform").transform.position.x;
        
        VInput.KeyDown(KeyCode.D);
        yield return new WaitForSeconds(3);
        VInput.KeyUp(KeyCode.D);
        yield return null;
        rightX = GameObject.Find("Platform").transform.position.x;
        
        //Reload scene        
        SceneManager.LoadScene("Game");
        yield return null;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        //Existence and components check
        GameObject tmp = GameObject.FindWithTag("FallingCube");
        if (tmp == null)
        {
            Assert.Fail("Falling cubes not spawning instantly after opening scene, or prefab's tag is misspelled!");
        }
        if(!PMHelper.Check3DPrimitivity(tmp, PrimitiveType.Cube))
            Assert.Fail("\"FallingCube\" object was not created as primitive Cube object, it's mesh differences or components missing");
        if(!PMHelper.CheckMaterialDifference(tmp))
            Assert.Fail("The material of an \"FallingCube\" object should be changed!");
        Rigidbody rb = PMHelper.Exist<Rigidbody>(tmp);
        if(!rb)
            Assert.Fail("\"FallingCube\" should have assigned <Rigidbody> component");
        if(!rb.useGravity)
            Assert.Fail("\"FallingCube\"'s <Rigidbody> component should have checked <Use Gravity> parameter");
        if(rb.isKinematic)
            Assert.Fail("\"FallingCube\"'s <Rigidbody> component should have unchecked <Is Kinematic> parameter");

        //Check position
        GameObject Floor = GameObject.Find("Floor");
        int floorWasLayer = Floor.layer;
        Floor.layer = LayerMask.NameToLayer("Test");
        RaycastHit hit = PMHelper.RaycastFront3D(tmp.transform.position, Vector3.down, 1 << 16);
        if (!hit.collider)
        {
            Assert.Fail("\"FallingCube\"'s objects should be spawned above the \"Platform\" object");   
        }
        if (tmp.transform.position.z >= GameObject.Find("Wall").transform.position.z)
        {
            Assert.Fail("\"FallingCube\"'s objects should be spawned in front of the \"Wall\" object");
        }
        
        //Check if visible when spawns
        if(PMHelper.CheckVisibility(camera, tmp.transform,3))
            Assert.Fail("\"FallingCube\" object should not be in a camera view when it spawns");
        
        //Check falling
        Vector3 start = tmp.transform.position;
        yield return new WaitForSeconds(0.01f);
        Vector3 end = tmp.transform.position;
        if (start.x != end.x || start.z != end.z)
        {
            Assert.Fail("X-axis and z-axis of \"FallingCube\"'s object should not change");
        }
        if (!(start.y > end.y))
        {
            Assert.Fail("Y-axis of \"FallingCube\"'s object should decrease by the time (while falling)");
        }
        
        //Reload scene        
        SceneManager.LoadScene("Game");
        yield return null;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        tmp = GameObject.FindWithTag("FallingCube");
        rb = PMHelper.Exist<Rigidbody>(tmp);
        rb.isKinematic = true;
        
        //Check frequency
        Time.timeScale = 10;
        for (int j = 0; j < 5; j++)
        {
            yield return new WaitForSeconds(1);
            yield return null;
            GameObject[] tmp2 = GameObject.FindGameObjectsWithTag("FallingCube");
            foreach (GameObject cube in tmp2)
            {
                rb = PMHelper.Exist<Rigidbody>(cube);
                rb.isKinematic = true;
            }
            if (tmp2.Length != j + 2)
            {
                Assert.Fail("\"FallingCube\" object should be instantiated at the rate of 1 per second");
            }
        }
        //Check random and correct spawning
        List<float> positions = new List<float>();
        foreach (GameObject cube in GameObject.FindGameObjectsWithTag("FallingCube"))
        {
            if (Mathf.RoundToInt(cube.transform.position.z) != Mathf.RoundToInt(tmp.transform.position.z) ||
                Mathf.RoundToInt(cube.transform.position.y) != Mathf.RoundToInt(tmp.transform.position.y))
            {
                Assert.Fail("All of the \"FallingCube\" objects should be instantiated with the same z-axis and y-axis");
            }
            if (cube.transform.position.x < leftX || cube.transform.position.x > rightX)
            {
                Assert.Fail("All of the \"FallingCube\" objects should be \"catchable\" by platform");
            }
            if (positions.Contains(cube.transform.position.x))
            {
                Assert.Fail("All of the \"FallingCube\" objects should be instantiated with random x-axis. Some of them are identical");
            }
            positions.Add(cube.transform.position.x);
        }
    }
    
    [UnityTest, Order(1)]
    public IEnumerator CheckRotating()
    {
        SceneManager.LoadScene("Game");
        yield return null;
        
        GameObject tmp = GameObject.FindWithTag("FallingCube");
        Quaternion rotStart = tmp.transform.rotation;
        yield return null;
        Quaternion rotEnd = tmp.transform.rotation;

        if (rotStart == rotEnd)
        {
            Assert.Fail("\"FallingCube\" objects should be continuously rotating");
        }
    }
}