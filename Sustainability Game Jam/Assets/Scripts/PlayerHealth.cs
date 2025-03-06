using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private CustomSlider healthBar;
    [SerializeField] private CustomSlider chargeBar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image chargeBarFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Gradient chargeGradient;
    private float enemySpeedOnCollision;
    public float playerMaxHealth = 100;
    public float playerCurrentHealth;
    private PlayerCarControl playerCarControl;
    public float playerMaxCharge = 100;
    public float playerCurrentCharge;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.maxValue = playerMaxHealth;
        chargeBar.maxValue = playerMaxCharge;
        playerCarControl = GetComponent<PlayerCarControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCurrentCharge > 0 && playerCarControl.vInput != 0)
        {
            playerCurrentCharge -= Time.deltaTime;
        }
        else if(playerCurrentCharge <= 0)
        {
            playerCurrentCharge = 0;
        }

        healthBar.currentValue = playerCurrentHealth;
        chargeBar.currentValue = playerCurrentCharge;
        healthBarFill.color = ColorFromGradient(healthBar.currentValue / healthBar.maxValue, healthGradient);
        chargeBarFill.color = ColorFromGradient(chargeBar.currentValue / chargeBar.maxValue, chargeGradient);
    }

    Color ColorFromGradient (float value, Gradient gradient)  // float between 0-1
    {
        return gradient.Evaluate(value);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            enemySpeedOnCollision = collision.gameObject.GetComponent<CarControl>().forwardSpeed;
            playerCurrentHealth -= Mathf.Abs(enemySpeedOnCollision * collision.gameObject.GetComponent<CarEnemy>().damageMultiplier);
        }
    }
}
