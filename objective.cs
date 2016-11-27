using UnityEngine;
using System.Collections;

public class objective : MonoBehaviour {

	// Use this for initialization
	void OnCollisionEnter(Collision collision)
    {
        GameObject collidedShip = collision.gameObject;
        print("collisionDetected");
        Destroy(collidedShip);

    }
	
}
