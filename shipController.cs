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

    // 
    //Use this for initialization
    void Start () {
        indicatorLight = GetComponent<Light>();
        shipRB = GetComponent<Rigidbody>();
        shipTransform = GetComponent<Transform>();
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isSelected)
        {
            indicatorLight.intensity = Mathf.Abs(10.0f * Mathf.Sin(Time.realtimeSinceStartup*5.0f));
        }

        if (moving)
        {

            advancePosition();
        }
	
	}

    public void onSelect()
    {
        isSelected = true;
        indicatorLight.intensity = 30.0f;
        
    }

    public void onDeselect()
    {
        isSelected = false;
        indicatorLight.intensity = 0.0f;
    }

    public void move(Vector3[] path)
    {
        print("moving");
        movePath = conditionPath(path);
        print(movePath.Length);
        moving = true;

        advanceVec = movePath[pathIndex] - shipTransform.position;
        advanceVec.Normalize();
        
        tolAngle = 15;
    }


    public float moveStep;
    private float tolAngle;

    void advancePosition()
    {



        print(Vector3.Angle(advanceVec, movePath[pathIndex] - shipTransform.position));
        if (Vector3.Angle(advanceVec, movePath[pathIndex] - shipTransform.position) > tolAngle)
        {
            tolAngle = 15;
            pathIndex++;
            advanceVec = movePath[pathIndex] - shipTransform.position;
            advanceVec.Normalize();

        }
        if (pathIndex > movePath.Length-2)
        {
            moving = false;
        }
        

        shipTransform.position= shipTransform.position + advanceVec * moveStep;
        //ipRB.AddForce(advanceVec * moveStep);
        
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
        tempVec.Add(inArray[0]);
        for (int i = 1; i < inArray.Length; i++)
        {
            if (Vector3.Distance(inArray[i],inArray[i - 1])>moveStep*20.0f)
            {
                tempVec.Add(inArray[i]);
            }
        }

        return (Vector3[])tempVec.ToArray(typeof(Vector3));

    }

}
