using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Mouse")]
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;

    [Header("Camera")]
    [SerializeField] private GameObject playerHead;
    [SerializeField] private Vector2 xClamp;
    [SerializeField] private Vector2 yClamp;

    [Header("Camera Sensitivity")]
    [SerializeField] private float sensitivity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCam();
    }

    void RotateCam()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, yClamp.x, yClamp.y);
        mouseX = Mathf.Clamp(mouseX, xClamp.x, xClamp.y);
        playerHead.transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
