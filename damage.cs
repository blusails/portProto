using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour {

    public float shipHealth;
    private float maxHealth;
    private float actualHealth;
    ParticleSystem smoke;
	// Use this for initialization

    void Start()
    {
        updateGlobalVars();
        actualHealth = maxHealth * shipHealth;
        GameObject smokeObj = transform.Find("smoke").gameObject;
        smoke = smokeObj.GetComponent<ParticleSystem>();
        smoke.enableEmission = false;

    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObj = collision.gameObject;
        if (collidedObj.tag == "ships")
        {

            actualHealth -= (collision.relativeVelocity.sqrMagnitude);
            if (actualHealth<0)
            {
                kill();
            }
            
            if (actualHealth<0.5f*maxHealth*shipHealth)
            {
                indicateDamage();
            }

        }
    }

    void updateGlobalVars()
    {
        GameObject masterController = GameObject.Find("masterController");
        globalShipParams gSP = masterController.GetComponent<globalShipParams>();
        maxHealth = gSP.maxHealth;
        


    }


 void indicateDamage()
    {
        smoke.enableEmission = true;
    }
    
    void kill()
    {
        GetComponent<pathAnimator>().killPath();
        Destroy(gameObject);
    }
}
