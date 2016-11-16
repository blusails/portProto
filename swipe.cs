using UnityEngine;
using System.Collections;

public class swipe : MonoBehaviour
{
    public Vector3[] path;
    private ArrayList tempPath = new ArrayList();
    public bool setPath;
    public selector masterSelector;
    // Use this for initialization
    void Start()
    {
        GameObject masterController = GameObject.Find("masterController");
        masterSelector = masterController.GetComponent<selector>();
    }
    
    // Update is called once per frame
    bool readingPath = false;
    public Vector3 previousPoint;
    public pathAnimator currentPathAnimator;
    private float minTubeLength = .5f;
    void Update()
    {
        
        if (setPath)
        {
            
            RaycastHit hit;
            if (Input.GetButton("Fire1"))
            {
                readingPath = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Returns a ray going from camera through a screen point.
                if (Physics.Raycast(ray, out hit))
                {

                    if (Vector3.Distance(previousPoint, hit.point) > minTubeLength)
                    {
                        currentPathAnimator.drawTube(previousPoint, hit.point);
                        previousPoint = hit.point;
                    }

                    transform.position = hit.point;
                    tempPath.Add(hit.point);
                    
                }
            }

            if (Input.GetButtonUp("Fire1") && readingPath)
            {
                setPath = false;
                readingPath = false;
                masterSelector.deselectAll();
                path = (Vector3[])tempPath.ToArray(typeof(Vector3));
                shipController currentController = masterSelector.selectedShip.GetComponent<shipController>();
                currentController.move(path);
                tempPath.Clear();
            }
        }

    }
}
