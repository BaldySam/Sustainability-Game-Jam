using UnityEngine;

public class PlayerCarControl : MonoBehaviour
{
    [SerializeField] private Transform steeringWheel;
    CarControl carControl;
    public float hInput;
    public float vInput;
    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        carControl = GetComponent<CarControl>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelRotate();
        if(playerHealth.playerCurrentHealth <= 0 || playerHealth.playerCurrentCharge <= 0)
        {
            vInput = 0;
            hInput = 0;
        }
        else
        {
            vInput = Input.GetAxis("Vertical");
        }
        hInput = Input.GetAxis("Horizontal");
        carControl.vInput = vInput;
        carControl.hInput = hInput;
    }

    void WheelRotate()
    {
        steeringWheel.localEulerAngles = new Vector3(steeringWheel.localEulerAngles.x, Quaternion.Slerp(steeringWheel.localRotation, Quaternion.Euler(0, hInput * 90, 0), Time.deltaTime * 5).eulerAngles.y, steeringWheel.localEulerAngles.z);
    }
}
