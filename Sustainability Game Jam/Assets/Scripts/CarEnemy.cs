using UnityEngine;

public class CarEnemy : MonoBehaviour
{
    [Header("Rubber Banding")]
    [SerializeField] private float teleportOffset;
    [SerializeField] private float playerDistanceOffset;
    public Terrain terrainCarIsIn;
    public Terrain[] terrains;

    [Header("Obstacle Avoidance")]
    [SerializeField] private Transform raycastPointLeft;
    [SerializeField] private Transform raycastPointRight;
    [SerializeField] private Transform raycastPointCentre;
    [SerializeField] private float obstacleDistance;
    
    [Header("Damage")]
    public float damageMultiplier;
    private float AngleToPlayer;
    private GameObject player;
    public float time;
    private bool hitObject;
    private UnityEngine.Vector3 VectorToPlayer;
    private CarControl carControl;
    private float hInput;
    private float vInput;
    private float distanceToPlayer;
    public bool leftHit;
    public bool rightHit;
    public bool centreHit;
    public Transform[] lastObjectHit = new Transform[3];

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        terrains = Terrain.activeTerrains;
        carControl = GetComponent<CarControl>();
        lastObjectHit[0] = new GameObject().transform;
        lastObjectHit[1] = new GameObject().transform;
        lastObjectHit[2] = new GameObject().transform;
    }

    // Update is called once per frame
    void Update()
    {
        SteerInDirectionOfPlayer();
        RubberBanding();
        ObstacleAvoidance();
        carControl.hInput = hInput;
        carControl.vInput = vInput;
    }

    void RubberBanding()
    {
        distanceToPlayer = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
        if(carControl.forwardSpeed > distanceToPlayer)
        {
            if(carControl.forwardSpeed - distanceToPlayer > 5)
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
    }

    void SteerInDirectionOfPlayer()
    {
        VectorToPlayer = player.transform.position - transform.position;

        AngleToPlayer = Mathf.DeltaAngle(transform.localRotation.eulerAngles.y, Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles.y) / 10;

        hInput = Mathf.Clamp(AngleToPlayer, -1, 1);
    }

    void TeleportToPlayer()
    {
        for(int i = 0; i < terrains.Length; i++)
        {
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
        float raycastDistanceMultiplier = Mathf.Abs(carControl.forwardSpeed) + 5f;
        Debug.DrawRay(raycastPointLeft.position, raycastPointLeft.forward * raycastDistanceMultiplier, Color.red);
        RaycastHit hitLeft;
        if(Physics.Raycast(raycastPointLeft.position, raycastPointLeft.forward, out hitLeft, raycastDistanceMultiplier))
        {
            if(hitLeft.collider.gameObject.tag != "IgnoreObstacleAvoidance")
            {
                leftHit = true;
                vInput = 0.1f;
                lastObjectHit[0] = hitLeft.transform;
            }
            else
            {
                leftHit = false;
            }
        }
        else
        {
            leftHit = false;
        }

        Debug.DrawRay(raycastPointRight.position, raycastPointRight.forward * raycastDistanceMultiplier, Color.blue);
        RaycastHit hitRight;
        if(Physics.Raycast(raycastPointRight.position, raycastPointRight.forward, out hitRight, raycastDistanceMultiplier))
        {
            if(hitRight.collider.gameObject.tag != "IgnoreObstacleAvoidance")
            {
                rightHit = true;
                vInput = 0.1f;
                lastObjectHit[1] = hitRight.transform;
            }
            else
            {
                rightHit = false;
            }
        }
        else
        {
            rightHit = false;
        }

        if(rightHit && !leftHit)
        {
            hInput = -1;
        }
        else if(leftHit && !rightHit)
        {
            hInput = 1;
        }
        else if(rightHit && leftHit)
        {
            if(Vector3.Distance(hitRight.point, raycastPointRight.position) < Vector3.Distance(hitLeft.point, raycastPointLeft.position))
            {
                hInput = -1;
            }
            else if(Vector3.Distance(hitRight.point, raycastPointRight.position) > Vector3.Distance(hitLeft.point, raycastPointLeft.position))
            {
                hInput = 1;
            }
        }

        Debug.DrawRay(raycastPointCentre.position, raycastPointCentre.forward * raycastDistanceMultiplier, Color.green);
        RaycastHit hitCentre;
        if(Physics.Raycast(raycastPointCentre.position, raycastPointCentre.forward, out hitCentre, raycastDistanceMultiplier))
        {
            if(hitCentre.collider.gameObject.tag != "IgnoreObstacleAvoidance")
            {
                centreHit = true;
                vInput = 0.1f;
                lastObjectHit[2] = hitCentre.transform;
            }
            else
            {
                centreHit = false;
            }
        }
        else
        {
            centreHit = false;
        }

        CheckIfCarIsStationary();
    }

    void CheckIfCarIsStationary()
    {
        if(carControl.forwardSpeed < 1)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }

        if(time > 1)
        {
            if(rightHit || leftHit || centreHit || Vector3.Distance(lastObjectHit[0].position, transform.position) < obstacleDistance || Vector3.Distance(lastObjectHit[1].position, transform.position) < obstacleDistance || Vector3.Distance(lastObjectHit[2].position, transform.position) < obstacleDistance)
            {
                vInput = -1;
                hInput = -hInput;
            }
        }

        if(vInput == -1 && time > 4)
        {
            hInput = -hInput;
            vInput = 1;
        }
    }
}
