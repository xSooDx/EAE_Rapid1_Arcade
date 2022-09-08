using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffZoneMovementScript : MonoBehaviour
{
    public Transform orbitTarget;

    public float orbitAngularSpeed;
    public float orbitDistance;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(orbitTarget.position, Vector3.forward, orbitAngularSpeed * Time.deltaTime);
    }
}
