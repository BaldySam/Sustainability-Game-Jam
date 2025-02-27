using UnityEngine;

public class CanvasRotate : MonoBehaviour
{
    [SerializeField] Transform camTransform;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + camTransform.forward);
    }
}