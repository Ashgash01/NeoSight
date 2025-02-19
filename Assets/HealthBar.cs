using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private float maxHealth = 100;
    private float currentHealth;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    void Start()
    {
        currentHealth = maxHealth;
        healthText.text = "Health : " + currentHealth;
    } 

    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        healthText.text = "Health : " + currentHealth;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        float targetFillAmount = currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
    }
}
