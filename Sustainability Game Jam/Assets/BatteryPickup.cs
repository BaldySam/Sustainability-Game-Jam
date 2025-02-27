using UnityEngine;

public class Collectable : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CarControl>().charge = other.gameObject.GetComponent<CarControl>().maxCharge;
            Destroy(gameObject);
        }
    }
}
