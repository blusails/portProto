using UnityEngine;
using System.Collections;

public class shipController : MonoBehaviour {
    private Light indicatorLight;
    public bool isSelected;
    public Vector3[] movePath;
    private bool moving = false;
    private int pathIndex=0;
    private Rigidbody shipRB;
    private Transform shipTransform;
    public Vector3 advanceVec;
    public Vector3 previousAdvanceVec;
    private float indicatorLightFrequency = 5.0f;
    private float minPathSpacing = 5.0f;  // number of moveSteps adjacent movePath values must be apart
    private float tolAngle=30.0f;
    private float moveStep;
    private float maxMoveStep;
    private float minMoveStep;

    public float speed;  // boat speed, value between 0 and 1;
    // 
    //Use this for initialization
    void Start () {
        indicatorLight = GetComponent<Light>();
        shipRB = GetComponent<Rigidbody>();
        shipTransform = GetComponent<Transform>();
        updateGlobalVars();
        moveStep = speed * maxMoveStep; // calculation moveStep and ensure it's within global params
        if (moveStep < minMoveStep) moveStep = minMoveStep;
        if (moveStep > maxMoveStep) moveStep = maxMoveStep;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isSelected)
        {
            indicatorLight.intensity = Mathf.Abs(10.0f * Mathf.Sin(Time.realtimeSinceStartup* indicatorLightFrequency)); // oscillate light frequency to indication selection
        }

        if (moving)
        {

            advancePosition();
        }
	
	}

    public void onSelect()   
    {
        isSelected = true;
        indicatorLight.intensity = 30.0f;  // turn on indicatorLight
        
    }

    public void onDeselect()
    {
        isSelected = false;
        indicatorLight.intensity = 0.0f;  // turn off indicatorLight
    }

    void updateGlobalVars()
    {
        GameObject masterController = GameObject.Find("masterController");
        globalShipParams gSP = masterController.GetComponent<globalShipParams>();
        indicatorLightFrequency = gSP.indicatorLightFrequency;
        maxMoveStep = gSP.maxMoveStep;
        minMoveStep = gSP.minMoveStep;

    }

    public void move(Vector3[] path)
    {
        movePath = conditionPath(path);  
        moving = true;
             
    }
    
    void advancePosition()  // perform next move operation
    {
   
        if (pathIndex > movePath.Length-1) // test for end of path
        {
            moving = false; // shut it down
            pathIndex = 0;
            return;
        }

        advanceVec = movePath[pathIndex] - shipTransform.position; // get vector that points from position to next point
        advanceVec.Normalize(); // by normalizing and then multiplying on moveStep we ensure we are moving moveStep on fixedupdate
        //  the more straightforeward method of setting position to the newstep or not normalizing and multiplying provides less smooth movement

        shipTransform.position= shipTransform.position + advanceVec * moveStep; // actual move
        pathIndex++;

        //shipRB.AddForce(advanceVec * moveStep);
        
    }


    // this method takes any arbitrary list of Vector3's and outputs an interpolated path with a static step of moveStep length
    private Vector3[] conditionPath(Vector3[] inArray)  
    {
        ArrayList tempVec = new ArrayList(); // create temporary arraylist - allows for dynamic length
        Vector3 prevPoint = shipTransform.position; // set first point in path to current position
        tempVec.Add(shipTransform.position);  //                                   ''
        for (int i = 1; i +1< inArray.Length; i++) // iterate over entire inputted path
        {
            if (Vector3.Distance(inArray[i],prevPoint)>moveStep)  // if distance between last step and next point in array, interp between them 
            {
                prevPoint = Vector3.Lerp(prevPoint, inArray[i], moveStep/Vector3.Distance(prevPoint, inArray[i])); // linear interp of length movestep
                tempVec.Add(prevPoint);
                i--; // deincrement to remain on this index next iteration
            }
            else // otherwise, use next point for fractional interpolation
            {
                prevPoint = Vector3.Lerp(prevPoint, inArray[i + 1], moveStep / Vector3.Distance(prevPoint, inArray[i + 1]));
                tempVec.Add(prevPoint);
            }
        }

        return (Vector3[])tempVec.ToArray(typeof(Vector3)); // return Vector3[] version of tempVec
        

    }

    //unused methods
    private Vector3[] unique(Vector3[] inArray)  // **outdated - unused** assumes adjacency
    {
        ArrayList tempVec = new ArrayList();
        tempVec.Add(inArray[0]);
        for (int i = 1; i < inArray.Length; i++)
        {
            if (inArray[i] != inArray[i - 1])
            {
                tempVec.Add(inArray[i]);
            }
        }

        return (Vector3[])tempVec.ToArray(typeof(Vector3));

    }

}
