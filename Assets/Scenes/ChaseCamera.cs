using System.Numerics;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ChaseCamera : MonoBehaviour
{
    public GameObject VehicleBody; // Reference to the car's transform
    public float distance = 10.0f; // Distance behind the car
    public float height = 5.0f; // Height above the car
    public float rotationDamping = 3.0f; // Damping for smooth rotation
    public float positionDamping = 2.0f; // Damping for smooth position

    public float centerOffset = -1.0f;

    public float noseOffset = 1.0f;

    private Transform car; // Reference to the car's transform
    private RayCastWheel rayCastWheel; // Reference to the RayCastWheel script

    void Start()
    {
        rayCastWheel = VehicleBody.GetComponent<RayCastWheel>(); // Get the RayCastWheel script
        car = VehicleBody.transform; // Get the car's transform
    }

    void LateUpdate()
    {
        if (!car || !rayCastWheel)
            return;



        // // Set the camera's position and rotation
        Vector3 carPosition = car.position;

        transform.position = carPosition + (car.right * distance) + (car.forward * centerOffset) + (car.up * height); 

        transform.rotation = Quaternion.LookRotation(carPosition + (car.right * noseOffset) +(car.forward * centerOffset) - transform.position, car.up);


        // Calculate the current rotation angles
        // float wantedRotationAngle = car.eulerAngles.y - 85;
        // float wantedHeight = car.position.y + height;

        // float currentRotationAngle = transform.eulerAngles.y;
        // float currentHeight = transform.position.y;

        // // Damp the rotation around the y-axis
        // currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // // Damp the height
        // currentHeight = Mathf.Lerp(currentHeight, wantedHeight, positionDamping * Time.deltaTime);

        // // Convert the angle into a rotation
        // Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // // Set the position of the camera on the x-z plane to:
        // // distance meters behind the car
        // Vector3 position = car.position - currentRotation * Vector3.forward * distance;
        // position.y = currentHeight;
        // transform.position = position;

        // // Always look at the car
        // transform.LookAt(car);

        // Adjust the camera rotation based on the car's turning angle
        // float steeringAngle = rayCastWheel.CurrentSteeringAngle;
        // float smoothedSteeringAngle = Mathf.LerpAngle(0, steeringAngle, rotationDamping * Time.deltaTime);
        // transform.RotateAround(car.position, Vector3.up, smoothedSteeringAngle * 0.5f); // Adjust the multiplier as needed
    }
}