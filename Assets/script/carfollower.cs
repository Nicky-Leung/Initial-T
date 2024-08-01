using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class carfollower : MonoBehaviour
{
    public SplineContainer splinePath; // Reference to the premade spline
    
    public GameObject car; 
    public float speed = 0.5f; // Speed of the vehicle
    public float steeringSensitivity = 1f; // Sensitivity of the steering
    public float maxSteeringAngle = 30f; // Maximum steering angle

    private float t = 0f; // Parameter to track position along the spline

    public float Throttle { get; private set; } // Public property for throttle
    public float Steering { get; private set; } // Public property for steering
    private Rigidbody vehicleRigidbody;

    private Spline targetPath; 
 




    void Start()
    {
        vehicleRigidbody = car.GetComponent<Rigidbody>();
        targetPath = splinePath.Spline; 

        
    }

    Vector3 getTargetPosition(Vector3 position,Spline path )
    {

        foreach (var knot in path.Knots)
        {
           if (Vector3.Dot(position, knot.Position) > 0)
           {
               Debug.Log(knot.Position);
               return knot.Position;
           }

        }
        return Vector3.zero;


    }

    void FixedUpdate()
    {

        Vector3 targetPosition = getTargetPosition(vehicleRigidbody.position, targetPath );

        Vector3 targetDirection = targetPosition - vehicleRigidbody.position;

        Throttle = speed;

        Steering = Vector3.SignedAngle(vehicleRigidbody.transform.right, targetDirection, Vector3.up) / maxSteeringAngle;

    }
}