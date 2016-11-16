using UnityEngine;
using System.Collections;

public class pathAnimator : MonoBehaviour {
    public float tubeDiameter;
    private GameObject tube;
   
    private ArrayList tubeList = new ArrayList();


    
    public void drawTube(Vector3 startPoint, Vector3 stopPoint)
    {
        //create new cylinder
        tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        tube.tag = "pathTube";
        CapsuleCollider tubeCollider = tube.GetComponent<CapsuleCollider>();
        DestroyImmediate(tubeCollider);
        Vector3 center = (startPoint + stopPoint) / 2.0f;
        
        float distance = Vector3.Distance(stopPoint, startPoint);
        // scale length to distance
        tube.transform.localScale = new Vector3(tubeDiameter, distance/2.0f, tubeDiameter);
        // position bottom of cylinder at start Point
        tube.transform.position = center;
        // rotate to stop Point
        tube.transform.rotation = Quaternion.FromToRotation(Vector3.up, stopPoint-startPoint);
        tubeList.Add(tube);
    }

    public void drawTubePath(Vector3[] path)
    {
        for (int i = 0; i<path.Length-2; i++)
        {
            drawTube(path[i], path[i + 1]);
        }
    }

    public void clearTubes()
    {
        GameObject[] tubes = (GameObject[])tubeList.ToArray(typeof(GameObject));
        foreach(GameObject obj in tubes)
        {
           DestroyImmediate(obj);
        }
    }
}
