using UnityEngine;

public class satnav : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float heightOffset = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + heightOffset, target.position.z);
        transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
    }
}
