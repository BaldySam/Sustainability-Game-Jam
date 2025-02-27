using UnityEngine;
using UnityEngine.SceneManagement;

public class CarControl : MonoBehaviour
{
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    public float vInput;
    public float hInput;

    public float forwardSpeed;
    public float speedFactor;
    public float currentMotorTorque;
    public float currentSteerRange;
    public bool isAccelerating;

    public float decelerationMultiplier;

    WheelControl[] wheels;
    Rigidbody rigidBody;

    public float charge = 20f;
    public float maxCharge = 20f;
    [SerializeField] private float chargeLossRate = 1f;
    [SerializeField] private CustomSlider chargeSlider;
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private float recoverOffset = 3f;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
        chargeSlider.maxValue = maxCharge;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LevelReset();
        }

        if(Input.GetKeyDown(KeyCode.R) && time == 0)
        {
            Recover();
        }

        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = 0;
        }

        // steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, Mathf.Clamp(Mathf.SmoothStep(steeringWheel.localEulerAngles.z, -hInput * 90, Time.deltaTime * 5) , -90, 90));
        steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, Quaternion.Slerp(steeringWheel.localRotation, Quaternion.Euler(0, 0, -hInput * 90), Time.deltaTime * 5).eulerAngles.z);
        chargeSlider.currentValue = charge;

        if(charge > 0)
            vInput = Input.GetAxisRaw("Vertical");
        else
        {
            charge = 0;
            vInput = 0;
        }

        if(charge > 0 && vInput != 0)
        {
            charge -= Time.deltaTime * chargeLossRate;
        }

        hInput = Input.GetAxisRaw("Horizontal");

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);

        // Calculate how close the car is to top speed
        // as a number from zero to one
        speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction 
        // as the car's velocity
        isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }
            
            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    void LevelReset()
    {
        UnityEngine.SceneManagement.Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    void Recover()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + recoverOffset, transform.position.z);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        time = 3;
    }
}