using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

public class RayCastWheel : MonoBehaviour
{


    private float currentSteeringAngle = 0;

    public GameObject VehicleBody;
    public GameObject FrontRightWheel;
    public GameObject FrontLeftWheel;
    public GameObject RearRightWheel;
    public GameObject RearLeftWheel;
    private List<GameObject> Wheels = new List<GameObject>(); // List of all wheels
    public Rigidbody VechicleRigidBody;
    public float MaxSuspension = 0.5f;
    private int frameCounter = 0;
    public float maxSteerAngle = 50f;
    public float lateralFrictionMultiplier = 1;
    public AnimationCurve powerCurve;
    public float maxTorque = 100;
    public float maxSpeed = 100;
    public float maxSteeringForce = 100f;

    public bool isAICar = false;


    public float CurrentSteeringAngle
    {
        get { return currentSteeringAngle; }
    }

    public AnimationCurve FrontTireGripCurve;
    public AnimationCurve RearTireGripCurve;

    public float dragCoefficient = 0.2f;




    /** 

     * @brief Start is called before the first frame update
     * 
     * Start is called before the first frame update
     * @post Rigidbody of the vehicle is assigned to VechicleRigidBody
     * @post All wheels are added to the list of wheels
     * @post A forward marker is created for each wheel

    **/
    void Start()
    {
        VechicleRigidBody = VehicleBody.GetComponent<Rigidbody>();
        Wheels.Add(FrontLeftWheel);
        Wheels.Add(FrontRightWheel);
        Wheels.Add(RearLeftWheel);
        Wheels.Add(RearRightWheel);

    }

    /**
    * @brief Check if the wheel is in the air
    *
    * Check if the wheel is in the air
    * @param wheel The wheel to check
    * @return True if the wheel is in the air, false otherwise
    */
    bool suspension(GameObject wheel)
    {
        Vector3 origin = wheel.transform.position;
        Vector3 direction = Vector3.down;
        RaycastHit hit;
        bool rayhit = Physics.Raycast(origin, direction, out hit, MaxSuspension);
        if (Physics.Raycast(origin, direction, out hit, MaxSuspension))
        {
            // Debug.Log("Wheel hit the ground");
            Debug.DrawLine(origin, hit.point, Color.green, 1f);
        }
        else
        {
            // Debug.Log("Wheel is in the air");
            Debug.DrawLine(origin, hit.point, Color.red, 1f);
        }
        return rayhit;
    }

    /**
    * @brief Apply steering to the wheel
    *
    * Apply steering to the wheel
    * @param wheel The wheel to steer
    * @param throttle The throttle value
    * @param currentSteeringAngle The current steering angle
    * @param rayhit True if the wheel is in contact with the ground, false otherwise
    */
    void steering(GameObject wheel, float throttle, float currentSteeringAngle, bool rayhit, float speedRatio)
    {

        if (wheel.name == "FL" || wheel.name == "FR")
        {
            wheel.transform.localEulerAngles = new Vector3(currentSteeringAngle, 0, 0);

        }
        Vector3 wheeldirection = wheel.transform.forward;
        Debug.DrawLine(wheel.transform.position, wheel.transform.position + wheeldirection, Color.red, 1f);
        Vector3 tireVelocity = VechicleRigidBody.GetPointVelocity(wheel.transform.position);
        float steeringVelocity = Vector3.Dot(tireVelocity, wheeldirection); // The velocity of the wheel in the direction it is facing
        float frontWheelGrip = FrontTireGripCurve.Evaluate(speedRatio);
        float rearWheelGrip = RearTireGripCurve.Evaluate(speedRatio);
        float steeringChange;
        if (wheel.name == "FL" || wheel.name == "FR")
        {
            steeringChange = -steeringVelocity * frontWheelGrip; // The change in velocity required to correct for lateral slip
        }
        else
        {
            steeringChange = -steeringVelocity * rearWheelGrip; // The change in velocity required to correct for lateral slip
        }

        float acceleration = steeringChange / Time.fixedDeltaTime; // The acceleration required to correct for lateral slip


        if (rayhit)
        {
            Vector3 force = acceleration * wheeldirection;
            // Clamp the force to prevent excessive steering forces
            // Adjust this value as needed
            force = Vector3.ClampMagnitude(force, maxSteeringForce);
            Vector3 position = wheel.transform.position;
            VechicleRigidBody.AddForceAtPosition(force, position);



            if (frameCounter == 60)
            {
                // Debug.Log("Steering Force Applied: " + force + " at Position: " + position);
                // Debug.Log(steeringVelocity);
                // Debug.Log("currentspeed" + VechicleRigidBody.velocity.magnitude);  
                // Debug.Log("speedRatio" + speedRatio);
                // frameCounter = 0;
            }
            Debug.DrawLine(position, position + force, Color.green, 1f);
        }
    }

    void ApplyThrottle(GameObject wheel, float throttle, Vector3 forward, bool rayhit, float speedRatio)
    {
        if (rayhit)

        {
            if (throttle > 0)
            {

                float avalibleTorque = maxTorque * powerCurve.Evaluate(speedRatio) * throttle; // Calculate the torque available to the wheel based on powercurve, throttle, and speed
                VechicleRigidBody.AddForceAtPosition(forward * avalibleTorque, wheel.transform.position);
                Debug.DrawLine(wheel.transform.position, wheel.transform.position + forward * 2, Color.blue, 1f);
            }
            else
            {

                float brakeForce = throttle - 0.1f * 10;
                VechicleRigidBody.AddForceAtPosition(forward * brakeForce, wheel.transform.position);

            }
        }
    }
    void ApplyFriction(Vector3 forward)
    {

        Vector3 velocity = VechicleRigidBody.velocity;
        float speed = velocity.magnitude;
        Vector3 dragForce = -velocity.normalized * speed * speed * dragCoefficient;

        // Apply the drag force to the vehicle's rigidbody
        VechicleRigidBody.AddForce(dragForce);
    }

    void VisualiseWheel(GameObject wheel, Vector3 forward)
    {
        if (Vector3.Dot(forward, VechicleRigidBody.velocity) > 0)
            wheel.transform.Rotate(Vector3.back * VechicleRigidBody.GetPointVelocity(wheel.transform.position).magnitude);

        else
        {
            wheel.transform.Rotate(-Vector3.back * VechicleRigidBody.GetPointVelocity(wheel.transform.position).magnitude);
        }
    }

    (float,float) getInput()
    {
        float throttle;
        float steer;
        if (isAICar)
        {

             throttle = GetComponent<carfollower>().Throttle;

             steer = GetComponent<carfollower>().Steering;
        }
        else
        {
             throttle = Input.GetAxis("Vertical");
             steer = Input.GetAxis("Horizontal");
        }

        return (throttle, steer);


    }




    void WheelPhysics(List<GameObject> Wheels)
    {
        (float throttle, float steer) = getInput();
        

        Vector3 forward = -VechicleRigidBody.transform.right;

        // calculate the current steering angle
        currentSteeringAngle = maxSteerAngle * steer;
        ApplyFriction(forward);
        foreach (GameObject wheel in Wheels)
        {
            float currentSpeed = VechicleRigidBody.velocity.magnitude;
            float speedRatio = currentSpeed / maxSpeed;

            bool rayhit = suspension(wheel);
            steering(wheel, throttle, currentSteeringAngle, rayhit, speedRatio);
            if (wheel.name == "RL" || wheel.name == "RR")
            {
                ApplyThrottle(wheel, throttle, forward, rayhit, speedRatio);
            }
            VisualiseWheel(wheel, forward);
        }



    }
    void FixedUpdate()
    {

        WheelPhysics(Wheels);

        frameCounter++;


    }
}
