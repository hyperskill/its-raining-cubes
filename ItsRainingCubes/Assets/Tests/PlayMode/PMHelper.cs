using System;
using UnityEngine;
using System.Collections.Generic;

public static class PMHelper
{
    public static GameObject Exist(String s)
    {
        return GameObject.Find(s);
    }
    public static T Exist<T>(GameObject g)
    {
        return g.GetComponent<T>();
    }
    public static bool Child(GameObject child, GameObject parent)
    {
        return child.transform.IsChildOf(parent.transform);
    }

    public static bool Check3DPrimitivity(GameObject gameObject, PrimitiveType type)
    {
        MeshFilter filter;
        MeshRenderer renderer;
        Collider collider;
        
        GameObject primitive = GameObject.CreatePrimitive(type);
        
        MeshFilter primitiveMeshFilter=Exist<MeshFilter>(primitive);

        switch (type)
        {
            case PrimitiveType.Cube:
                filter = Exist<MeshFilter>(gameObject);
                renderer = Exist<MeshRenderer>(gameObject);
                collider = Exist<BoxCollider>(gameObject);
                if (filter==null || renderer==null || collider==null)
                {
                    return false;
                }

                if (filter.sharedMesh != primitiveMeshFilter.sharedMesh)
                {
                    return false;
                }

                break;
        }
        
        GameObject.Destroy(primitive);
        return true;
    }
    
    public static bool CheckMaterialDifference(GameObject gameObject)
    {
        MeshRenderer renderer = Exist<MeshRenderer>(gameObject);
        
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        MeshRenderer primitiveMeshRenderer=Exist<MeshRenderer>(primitive);

        GameObject.Destroy(primitive);
        
        
        if (renderer.sharedMaterial == primitiveMeshRenderer.sharedMaterial)
        {
            return false;
        }
        return true;
    }

    public static bool CheckVisibility(Camera camera, Transform gameObject, int dimension)
    {
        List<Vector3> points = new List<Vector3>();
        if (dimension == 2)
        {
            float halfSX = gameObject.localScale.x / 2;
            float halfSY = gameObject.localScale.y / 2;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    points.Add(gameObject.position + new Vector3(i*halfSX, j*halfSY, 0));                
                }
            }
        }

        if (dimension == 3)
        {
            float halfSX = gameObject.localScale.x / 2;
            float halfSY = gameObject.localScale.y / 2;
            float halfSZ = gameObject.localScale.z / 2;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        points.Add(gameObject.position + new Vector3(i*halfSX, j*halfSY, k*halfSZ));                
                    }                
                }
            }
        }
        Vector3 point;
        for (int i = 0; i < points.Count; i++)
        {
            point = camera.WorldToViewportPoint(points[i]);
            if (point.x < 1 && point.x > 0 && point.y < 1 && point.y > 0 && point.z > 0)
            {
                return true;
            }
        }
        return false;
    }

    public static RaycastHit RaycastFront(Vector3 start, Vector3 direction, LayerMask mask)
    {
        RaycastHit hit;
        Physics.Raycast(start, direction, out hit, Mathf.Infinity, mask) ;
        return hit;
    }
    
    public static bool CheckRectTransform(RectTransform rect)
    {
        if (rect.anchorMax.x >= 1 || rect.anchorMax.x <= 0 ||
            rect.anchorMax.y >= 1 || rect.anchorMax.y <= 0 ||
            rect.anchorMin.x >= 1 || rect.anchorMin.x <= 0 ||
            rect.anchorMin.y >= 1 || rect.anchorMin.y <= 0)
        {
            return false;//Incorrect anchors
        }

        if (rect.anchorMin.x > rect.anchorMax.x ||
            rect.anchorMin.y > rect.anchorMax.y)
        {
            return false;//Incorrect anchors
        }

        if (rect.offsetMin != Vector2.zero || rect.offsetMax != Vector2.zero)
        {
            return false;//Might be troubles with changing resolution
        }
        
        return true;
    }

    public static void TurnCollisions(bool on)
    {
        List<LayerMask> ids = new List<LayerMask>();
        for (int i = 0; i < 32; i++)
        {
            if (LayerMask.LayerToName(i) != "")
            {
                ids.Add(i);
            }
        }

        foreach (int id1 in ids)
        {
            foreach (int id2 in ids)
            {
                Physics2D.IgnoreLayerCollision(id1,id2,!on);
                Physics.IgnoreLayerCollision(id1,id2,!on);
            }
        }
    }
}