using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider WheelCollider;

    // Create properties for the CarControl script
    // (You should enable/disable these via the 
    // Editor Inspector window)
    public bool steerable;
    public bool motorized;
    public bool rear;

    public bool rwd;
    public bool awd;
    public bool fwd;

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    private void Start()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        WheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;

        if(rwd)
        {
            if(rear)
            {
                motorized = true;
            }
            else if(!rear)
            {
                motorized = false;
            }
        }
        else if(awd)
        {
            motorized = true;
        }
        else if(fwd)
        {
            if(!rear)
            {
                motorized = true;
            }
            if(rear)
            {
                motorized = false;
            }
        }
    }
}