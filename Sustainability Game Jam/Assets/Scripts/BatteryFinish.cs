using UnityEngine;
using UnityEngine.SceneManagement;

public class BatteryFinish : MonoBehaviour
{
    public float batteryCount;
    private GameObject[] batteries;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        batteries = GameObject.FindGameObjectsWithTag("Battery");
        batteryCount = batteries.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(batteryCount == 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
