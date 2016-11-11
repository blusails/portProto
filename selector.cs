using UnityEngine;
using System.Collections;
// this behavior handles the selection of ship objects

public class selector : MonoBehaviour
{
    public bool selectorMode;  // toggles whether or not you can select
    public GameObject[] shipObjs;  // list of all ships
    public GameObject selectedShip;
    // Use this for initialization
    void Start()
    {
        shipObjs = GameObject.FindGameObjectsWithTag("ships");  // get current ships


    }

    // Update is called once per frame
    bool waitingUnclick = false;
    void Update()
    {
        if (Input.GetKeyDown("s")) selectorMode = true;  // toggle selection mode

        if (waitingUnclick && Input.GetButtonUp("Fire1")) toggleSetPath();



        if (selectorMode)
        {
            shipObjs = GameObject.FindGameObjectsWithTag("ships");
            if (Input.GetButtonDown("Fire1"))  // if a click is detected
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Returns a ray going from camera through a screen point.
                if (Physics.Raycast(ray, out hit)) // if that click hits a collider
                {

                    float prevDist = 10000000;   // inf
                    int selectedInt = -1;  // null case
                    for (int i = 0; i < shipObjs.Length; i++)  // step through ships and find nearest to selection
                    {
                        float currentDist = Vector3.Distance(hit.point, shipObjs[i].transform.position);

                        if (currentDist < prevDist)
                        {
                            selectedInt = i;
                        }
                        
                        prevDist = currentDist;
                    }

                    selectedShip = shipObjs[selectedInt];
                    shipController selectedController = shipObjs[selectedInt].GetComponent<shipController>();
                    deselectAll();
                    selectedController.onSelect();
                    selectorMode = false;
                    waitingUnclick = true;

                }
            }

        }
    }

    void deselectAll()
    {
        for (int i = 0; i < shipObjs.Length; i++)
        {
            shipController selectedController = shipObjs[i].GetComponent<shipController>();
            selectedController.onDeselect();
        }
    }

    void toggleSetPath()
    {
        GameObject pathTrail = GameObject.Find("pathTrail");
        swipe pathSwipe = pathTrail.GetComponent<swipe>();
        pathSwipe.setPath = true;
        waitingUnclick = false;
    }
        
}
