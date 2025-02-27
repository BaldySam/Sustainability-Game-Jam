using UnityEngine;

public class CarEnemy : MonoBehaviour
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

    public GameObject player;
    public float distanceToPlayer;

    WheelControl[] wheels;
    Rigidbody rigidBody;
    bool hitObject;
    public float time;
    [SerializeField] private float teleportOffset;
    [SerializeField] private float playerDistanceOffset;
    public Terrain terrainCarIsIn;
    public Terrain[] terrains;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
        terrains = Terrain.activeTerrains;
    }

    // Update is called once per frame
    void Update()
    {
        hInput = (Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles.y - transform.rotation.eulerAngles.y) / 10;
        hInput = Mathf.Clamp(hInput, -1, 1);
        Debug.Log(hInput);
        distanceToPlayer = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
        if(forwardSpeed > distanceToPlayer)
        {
            if(forwardSpeed - distanceToPlayer > 5)
            {
                vInput = -1;
            }
            else
            {
                vInput = 0;
            }
        }
        else
        {
            vInput = 1;
        }

        if(distanceToPlayer > playerDistanceOffset)
        {
            TeleportToPlayer();
        }

        Debug.Log(hInput);
        ObstacleAvoidance();
        

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

    void TeleportToPlayer()
    {
        for(int i = 0; i < terrains.Length; i++)
        {
            Debug.Log(i);
            if(transform.position.x >= terrains[i].transform.position.x && transform.position.x <= terrains[i].transform.position.x + terrains[i].terrainData.size.x && transform.position.z >= terrains[i].transform.position.z && transform.position.z <= terrains[i].transform.position.z + terrains[i].terrainData.size.z)
            {
                terrainCarIsIn = terrains[i];
                break;
            }
        }

        transform.position = new Vector3(player.transform.position.x - player.transform.forward.x * teleportOffset, terrainCarIsIn.SampleHeight(transform.position) + terrainCarIsIn.transform.position.y + 2, player.transform.position.z - player.transform.forward.z * teleportOffset);
    }

    void ObstacleAvoidance()
    {
        if(forwardSpeed < 0.1f)
        {
            time += Time.deltaTime;
            if(time > 2)
            {
                vInput = -1;
                hInput = -hInput;
                if(forwardSpeed > -0.1f && time > 2.5f)
                {
                    time = 0;
                    vInput = 1;
                }
                if(time > 3)
                {
                    time = 0;
                    vInput = 1;
                }

            }
        }
        else
        {
            time = 0;
            vInput = 1;
        }
    }
}
