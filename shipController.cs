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
        minPathSpacing = gSP.minPathSpacing;  // number of moveSteps adjacent movePath values must be apart
        tolAngle = gSP.tolAngle;
        maxMoveStep = gSP.maxMoveStep;
        minMoveStep = gSP.minMoveStep;

    }
    public void move(Vector3[] path)
    {
        //print("moving");
        moveStep = speed * maxMoveStep;
        if (moveStep < minMoveStep) moveStep = minMoveStep;
        if (moveStep > maxMoveStep) moveStep = maxMoveStep;

        movePath = conditionPath(path);  // remove redundant points and ensure minimum point spacing of 20.0*moveStep
       // print(movePath.Length);
        moving = true;

        //advanceVec = movePath[pathIndex] - shipTransform.position; // get first advanceVec
        // advanceVec.Normalize();

        
        
    }


    


    void advancePosition()
    {



       // print(Vector3.Angle(advanceVec, movePath[pathIndex] - shipTransform.position));
        //if (Vector3.Angle(advanceVec, movePath[pathIndex] - shipTransform.position) > tolAngle)
        //{
            
        //    pathIndex++;


   
        if (pathIndex > movePath.Length-1)
        {
            moving = false;
            pathIndex = 0;
            return;
        }

        advanceVec = movePath[pathIndex] - shipTransform.position;
        advanceVec.Normalize();

        shipTransform.position= shipTransform.position + advanceVec * moveStep;
        
        pathIndex++;

        //shipRB.AddForce(advanceVec * moveStep);
        
    }

    private Vector3[] unique(Vector3[] inArray)  // assumes adjacency
    {
        ArrayList tempVec = new ArrayList();
        tempVec.Add(inArray[0]);
        for (int i = 1; i<inArray.Length; i++)
        {
            if (inArray[i]!=inArray[i-1])
            {
                tempVec.Add(inArray[i]);
            }
        }
        
        return (Vector3[])tempVec.ToArray(typeof(Vector3));
            
    }

    private Vector3[] conditionPath(Vector3[] inArray)  // assumes adjacency
    {
        ArrayList tempVec = new ArrayList();
        Vector3 prevPoint = shipTransform.position;
        tempVec.Add(shipTransform.position);
        for (int i = 1; i +1< inArray.Length; i++)
        {
            if (Vector3.Distance(inArray[i],prevPoint)>moveStep)  // interp between 
            {
                prevPoint = Vector3.Lerp(prevPoint, inArray[i], moveStep/Vector3.Distance(prevPoint, inArray[i]));
                tempVec.Add(prevPoint);
                i--;
            }
            else
            {
                prevPoint = Vector3.Lerp(inArray[i], inArray[i + 1], moveStep / Vector3.Distance(inArray[i], inArray[i + 1]));
                tempVec.Add(prevPoint);
            }
        }

        return (Vector3[])tempVec.ToArray(typeof(Vector3));

    }

}
