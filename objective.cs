using UnityEngine;
using System.Collections;

public class objective : MonoBehaviour {

	// Use this for initialization
	void OnCollisionEnter(Collision collision)
    {
        GameObject collidedShip = collision.gameObject;
        collidedShip.GetComponent<pathAnimator>().killPath();
        print("collisionDetected");
        Destroy(collidedShip);

    }
	
}
