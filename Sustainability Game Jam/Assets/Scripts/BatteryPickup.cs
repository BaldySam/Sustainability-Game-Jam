using UnityEngine;

public class Collectable : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealth>().playerCurrentCharge = other.gameObject.GetComponent<PlayerHealth>().playerMaxCharge;
            GameObject.FindGameObjectWithTag("BatteryFinish").GetComponent<BatteryFinish>().batteryCount--;
            Destroy(gameObject);
        }
    }
}
