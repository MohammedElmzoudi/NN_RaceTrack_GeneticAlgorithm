using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class organism_manager : MonoBehaviour
{
    private dna dna_obj;
    private population_manager popM;
    private Rigidbody rb;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        dna_obj = GetComponent<dna>();
        popM = GameObject.FindGameObjectWithTag("Population Manager").GetComponent<population_manager>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
     
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(popM.isRunning())
        {
            float dir = dna_obj.predict();

            if(!float.IsNaN(dir) && dna_obj.isAlive())
            {
                rb.MovePosition(transform.position + (transform.forward * Time.fixedDeltaTime * 70 ));
                transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * dir* 100f * Time.fixedDeltaTime);
            }
        }
    }
    
    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Organism")
        {
            Physics.IgnoreCollision(other.collider,col);
        }
        
        if(other.collider.tag == "Obstacle")
        {
            dna_obj.setAlive(false);
        }
    }
}
