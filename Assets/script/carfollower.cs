using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

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
    private Vector3 splinePosition;
    private Vector3 targetPosition = Vector3.zero;

    private float currentSteeringAngle = 0;

    public float steeringSmoothness = 1f;





    void Start()
    {
        vehicleRigidbody = car.GetComponent<Rigidbody>();
        targetPath = splinePath.Spline;
        splinePosition = splinePath.transform.position;
        
    }

    /**
        * @brief Get the target position of the vehicle based on the path it is following
        * 
        * Get the target position of the vehicle
        * @param position The position of the vehicle
        * @param path The path the vehicle is following
        * @return The target position of the vehicle
        **/
Vector3 getTargetPosition(Vector3 position, Spline path)
{
   

    for (int i = 0; i < path.Count; i++)
    {
        BezierKnot knot = path[i];
        // Get the position of the knot
        Vector3 knotPosition = new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);
        // Get the position of the knot in world space
        Vector3 directionToKnot = knotPosition + splinePosition - position;

        if (Vector3.Dot(-vehicleRigidbody.transform.right, directionToKnot) > 0)
        {
            path.RemoveAt(i);
            return splinePosition + knotPosition;
        }
    }



    return Vector3.zero;
}

    void FixedUpdate()
    {
        
        // Get the target position only if the target position is zero or the vehicle has passed the target position
        if (targetPosition == Vector3.zero || Vector3.Dot(-vehicleRigidbody.transform.right, targetPosition - vehicleRigidbody.position) < 0)
        {
            targetPosition = getTargetPosition(vehicleRigidbody.position, targetPath);
         
        }
        
        Debug.Log("Vehcile poition " + vehicleRigidbody.position);
        Debug.Log("Target position " + targetPosition);

        // Get the direction to the target position
        Vector3 targetDirection = targetPosition - vehicleRigidbody.position;
        Debug.Log("Target direction" + targetDirection);
        Debug.Log(-vehicleRigidbody.transform.right);



        // Debug.Log(Vector3.SignedAngle(vehicleRigidbody.transform.right, targetDirection, Vector3.right));

        // Draw a line representing the vehicle's left direction
        Debug.DrawLine(vehicleRigidbody.position, vehicleRigidbody.position - vehicleRigidbody.transform.right * 5, Color.red, 1f);

        // Draw a line representing the target direction
        Debug.DrawLine(vehicleRigidbody.position, vehicleRigidbody.position + targetDirection, Color.magenta, 1f);

        // Calculate the steering angle based on the angle between the vehicle's forward direction and the target direction
        float targetSteeringAngle = Vector3.SignedAngle(-vehicleRigidbody.transform.right, targetDirection, Vector3.up);
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, Time.fixedDeltaTime * steeringSmoothness);
        Debug.Log("Steering angle " + currentSteeringAngle);

        // Apply the throttle and steering
        Throttle = speed;
        // Clamp the steering angle to the maximum steering angle and divide by the maximum steering angle to get a value between -1 and 1
        Steering = Mathf.Clamp(currentSteeringAngle, -maxSteeringAngle, maxSteeringAngle) / maxSteeringAngle;


    }
}