using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[Description("The Cube is an imitation of life itself"), Category("4")]
public class Stage4_Tests
{
    private GameObject LoseLeft, LoseRight, OkLeft, OkRight, Floor, Wall;
    private LayerMask testLayer;

    [UnityTest, Order(0)]
    public IEnumerator CheckDestroying()
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

        Scene curScene = SceneManager.GetActiveScene();

        GameObject cube = GameObject.FindWithTag("FallingCube");

        OkLeft = GameObject.Find("OkLeft");
        OkRight = GameObject.Find("OkRight");
        LoseLeft = GameObject.Find("LoseLeft");
        LoseRight = GameObject.Find("LoseRight");
        Floor = GameObject.Find("Floor");
        Wall = GameObject.Find("Wall");
        
        Vector3 floorWas = Floor.transform.position;

        //Check if gap is too narrow
        if (Mathf.Abs(
            (OkLeft.transform.position.x + OkLeft.transform.localScale.x / 2)
            - (OkRight.transform.position.x - OkRight.transform.localScale.x / 2)
        ) <= cube.transform.localScale.x * 2)
        {
            Assert.Fail("Gap is too narrow to pass the cube, make it at least twice as wide as FallingCube's objects");
        }

        //Check if cubes are being destroyed
        (GameObject, GameObject, bool, string)[] testCases =
        {
            (OkLeft, cube, false, "OkLeft"),
            (OkRight, cube, false, "OkRight"),
            (Wall, cube, false, "Wall"),
            (LoseLeft, cube, true, "LoseLeft"),
            (LoseRight, cube, true, "LoseRight"),
            (Floor, cube, true, "Floor"),
            (cube, cube, false, "FallingCube")
        };

        Physics.IgnoreLayerCollision(testLayer, testLayer, false);
        foreach (var testCase in testCases)
        {
            GameObject collidingObject = cube;
            if (testCase.Item3 || testCase.Item1.Equals(cube))
            {
                collidingObject = GameObject.Instantiate(cube);
            }

            LayerMask beforeLayer = testCase.Item1.layer;
            testCase.Item1.layer = testLayer;
            collidingObject.layer = testLayer;
            collidingObject.transform.position = testCase.Item1.transform.position;

            start = Time.unscaledTime;
            yield return new WaitUntil(() =>
                !collidingObject || (Time.unscaledTime - start) * Time.timeScale > 1);

            if (curScene != SceneManager.GetActiveScene())
                Assert.Fail("Scene should not be reloaded, when any objects collide");
            if (!testCase.Item1)
                Assert.Fail("When \"FallingCube\" object collide with \"" + testCase.Item4 + "\" object, \"" +
                            testCase.Item4 + "\" object should not be destroyed");
            if (!collidingObject && !testCase.Item3 || collidingObject && testCase.Item3)
                Assert.Fail("When \"FallingCube\" object collide with \"" + testCase.Item4 +
                            "\" object, \"FallingCube\" object should " + (testCase.Item3 ? "" : "not") +
                            " be destroyed");

            if (collidingObject)
                collidingObject.layer = LayerMask.NameToLayer("Default");
            testCase.Item1.layer = beforeLayer;
        }
        
        //Check timing
        PMHelper.TurnCollisions(false);

        SceneManager.LoadScene("Game");

        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            SceneManager.GetActiveScene().name == "Game" || (Time.unscaledTime - start) * Time.timeScale > 1);
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Assert.Fail("\"Game\" scene can't be loaded");
        }
        
        cube = GameObject.FindWithTag("FallingCube");
        
        start = Time.unscaledTime;
        yield return new WaitUntil(() =>
            cube.transform.position.y <= floorWas.y || (Time.unscaledTime - start) * Time.timeScale > 5);

        if (cube.transform.position.y > floorWas.y)
        {
            Assert.Fail("\"FallingCube\" objects are falling too slow, they should be able to reach the \"Floor\" object in less than 4 seconds");
        }
    }
}