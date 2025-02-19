using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private float maxHealth = 100;
    private float currentHealth;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float fillSpeed;
    void Start()
    {
        currentHealth = maxHealth;
        healthText.text = "Health : " + currentHealth;
    } 

    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthText.text = "Health : " + currentHealth;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
    }
}
