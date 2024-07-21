using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class RayCastWheel : MonoBehaviour
{


    public GameObject VehicleBody;
    public GameObject FrontRightWheel;
    public GameObject FrontLeftWheel;
    public GameObject RearRightWheel;
    public GameObject RearLeftWheel;
    private List<GameObject> Wheels = new List<GameObject>(); // List of all wheels
    public Rigidbody VechicleRigidBody;
    public float MaxSuspension = 0.5f;
    public float engineForce = 10000;
    private int frameCounter = 0;
    public float maxSteerAngle = 30f;



    /**
        * @brief Create a forward marker for each wheel
        * 
        * Create a forward marker for each wheel
        * @param Wheels List of all wheels
        * @post A forward marker is created for each wheel
        */
    void forwardmarker (List<GameObject> Wheels){
         foreach (GameObject wheel in Wheels)
        {
            // Create a new GameObject to indicate the forward direction of the wheel
            GameObject forwardMarker = new GameObject(wheel.name + "ForwardMarker");

        // Set the newly created GameObject as a child of the wheel
             forwardMarker.transform.SetParent(wheel.transform.parent, false);

        // Position the marker in front of the wheel to indicate the forward direction
        // Adjust the position as needed. Here, it's set 1 unit in front of the wheel.
            forwardMarker.transform.localPosition = new Vector3(0, 0, 1);
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        VechicleRigidBody = VehicleBody.GetComponent<Rigidbody>();
        Wheels.Add(FrontLeftWheel);
        Wheels.Add(FrontRightWheel);
        Wheels.Add(RearLeftWheel);
        Wheels.Add(RearRightWheel);
        // create a forward marker for each wheel
        forwardmarker(Wheels);
       
    }
        

    // Update is called once per frame
 // Assign this in the inspector or find it dynamically in Start()

    void Update()
    {
        float throttle = Input.GetAxis("Vertical");

        float steer = Input.GetAxis("Horizontal");
    
        // calculate the current steering angle
        float currentSteeringAngle = maxSteerAngle * steer;
         
    frameCounter++;

    // Check if the current frame is the 5th frame
    // apply suspension force to the wheels
    foreach (GameObject wheel in Wheels)
    {
        Vector3 origin = wheel.transform.position;
        Vector3 direction = Vector3.down;
        Vector3 forward = -wheel.transform.parent.Find(wheel.name + "ForwardMarker").transform.up;

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

        
        

        if (wheel.name == "FL" || wheel.name == "FR")
        {

            if(wheel.name == "FR"){
            wheel.transform.localEulerAngles = new Vector3(-currentSteeringAngle, wheel.transform.localEulerAngles.y,  wheel.transform.localEulerAngles.z);
            }
            else{
                wheel.transform.localEulerAngles = new Vector3(currentSteeringAngle, wheel.transform.localEulerAngles.y,  wheel.transform.localEulerAngles.z);
            }
            if (throttle != 0)
            {
                wheel.transform.Rotate(Vector3.back * throttle);

                if(rayhit){
                VechicleRigidBody.AddForceAtPosition(currentSteeringAngle * Vector3.right, wheel.transform.position); 
                }      
            }
               
        }

        if (wheel.name == "RL" || wheel.name == "RR")
        {
            
            Debug.Log (forward);
        
            FrontRightWheel.transform.Rotate(Vector3.back * throttle);
            FrontLeftWheel.transform.Rotate(Vector3.back * throttle);
            RearRightWheel.transform.Rotate(Vector3.back * throttle);
            RearLeftWheel.transform.Rotate(Vector3.back * throttle);            

        if (rayhit){
            VechicleRigidBody.AddForceAtPosition(engineForce * throttle * forward, wheel.transform.position);
             Debug.DrawLine(wheel.transform.position, wheel.transform.position + forward * 2, Color.blue, 1f);
        }
        }
    

       
    }

    

    if (frameCounter % 120 == 0)
    {
        Debug.Log("Tire Forward position" + FrontRightWheel.transform.forward);
        frameCounter = 0; // Reset the counter after logging
    }
        
        Debug.Log("Throttle value: " + throttle);



    }}
