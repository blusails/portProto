using UnityEngine;
using System.Collections;

public class pathAnimator : MonoBehaviour {
    private float tubeDiameter;
    private GameObject tube;
    private GameObject pathIndicatorChain;
    private Mesh chainMesh;
    private bool pathActive=false;
    private ArrayList tubeList = new ArrayList();
    private float yOffset;
    private Vector3 upOffset;
    private Renderer chainRenderer;
    private Material chainMaterialS;
    private Material chainMaterialNS;

    void Start()
    {
        chainMaterialS = Resources.Load("pathMaterialS", typeof(Material)) as Material;
        chainMaterialNS = Resources.Load("pathMaterialNS", typeof(Material)) as Material;
        updateGlobalVars();
        upOffset = Vector3.up * yOffset;
        
    }

    void updateGlobalVars()
    {
        GameObject masterController = GameObject.Find("masterController");
        globalShipParams gSP = masterController.GetComponent<globalShipParams>();
        yOffset= gSP.pathRenderYOffset;
        tubeDiameter = gSP.pathRenderWidth;


    }

    public void renderPathOff()
    {
        if (pathActive) chainRenderer.enabled = false;
    }

    public void renderPathOn()
    {
        if (pathActive) chainRenderer.enabled = true;
        
    }

    public void renderPathSelected()
    {
        if (pathActive) chainRenderer.material = chainMaterialS;
    }

    public void renderPathDeSelected()
    {
        if (pathActive) chainRenderer.material = chainMaterialNS;
    }


    public void killPath()
    {
        //renderPathOff();
        Destroy(pathIndicatorChain);
        pathActive = false;
    }
    public void createPathIndicatorChain(Vector3 startPoint, Vector3 stopPoint)
    {
        pathActive = true;
        // create object, remove collider
        pathIndicatorChain = new GameObject();
        pathIndicatorChain.AddComponent<MeshFilter>();
        pathIndicatorChain.AddComponent<MeshRenderer>();
        // get mesh
        Mesh blankMesh = new Mesh();
        pathIndicatorChain.GetComponent<MeshFilter>().mesh = blankMesh;
        chainMesh = pathIndicatorChain.GetComponent<MeshFilter>().mesh;
        chainRenderer = pathIndicatorChain.GetComponent<Renderer>();
        chainRenderer.material = chainMaterialS;
        // chainMesh.Clear();
        // assign vertices
        Vector3 towardVec = stopPoint - startPoint;
        Vector3 left = Vector3.Normalize(Vector3.Cross(Vector3.up, towardVec));
        Vector3 right = Vector3.Normalize(Vector3.Cross(towardVec, Vector3.up));
        Vector3[] vertices = new Vector3[4];
        // all vertcies are ordered left to right then along the path 
        // so that new links have the highest indicies

        vertices[0] = startPoint + tubeDiameter * left+upOffset;
        vertices[2] = stopPoint + tubeDiameter * left + upOffset;
        vertices[3] = stopPoint + tubeDiameter * right + upOffset;
        vertices[1] = startPoint + tubeDiameter * right + upOffset;
        chainMesh.vertices = vertices;
        // assign triangles
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 3;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 1;
        chainMesh.triangles = triangles;

    }

    public void addLink(Vector3 prevPoint, Vector3 newPoint)
    {
        if (pathActive==false)
        {
            createPathIndicatorChain(prevPoint, newPoint);
            return;
        }

        // establish center line and left/right geometry
        Vector3 towardVec = prevPoint - newPoint;
        Vector3 left = Vector3.Normalize(Vector3.Cross(Vector3.up, towardVec));
        Vector3 right = Vector3.Normalize(Vector3.Cross(towardVec, Vector3.up));
        // get old verts and copy to new vector3 array
        Vector3[] oldVerticies = chainMesh.vertices;
        Vector3[] newVerticies = new Vector3[oldVerticies.Length + 2];
        for (int i=0;i<oldVerticies.Length;i++)
        {
            newVerticies[i] = oldVerticies[i];
        }
        // build new vertices
        newVerticies[oldVerticies.Length + 0] = newPoint + tubeDiameter * left + upOffset;
        newVerticies[oldVerticies.Length + 1] = newPoint + tubeDiameter * right + upOffset;

        // get old triangles and copy to new array
        int[] oldTriangles = chainMesh.triangles;
        int[] newTriangles = new int[oldTriangles.Length + 2*3];
        for (int i = 0; i < oldTriangles.Length; i++)
        {
            newTriangles[i] = oldTriangles[i];
        }

        //  build new triangle indicies based on new verticies
        newTriangles[oldTriangles.Length+0] = oldVerticies.Length - 2;
        newTriangles[oldTriangles.Length+1] = oldVerticies.Length + 0;
        newTriangles[oldTriangles.Length+2] = oldVerticies.Length + 1;
        newTriangles[oldTriangles.Length+3] = oldVerticies.Length - 2;
        newTriangles[oldTriangles.Length+4] = oldVerticies.Length + 1;
        newTriangles[oldTriangles.Length+5] = oldVerticies.Length - 1 ;

        // assign new verts and tris
        chainMesh.vertices = newVerticies;
        chainMesh.triangles = newTriangles;

    }

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
