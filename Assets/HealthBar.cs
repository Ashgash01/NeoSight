using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private float maxHealth = 100;
    private float currentHealth;
    public static HealthBar Instance;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
<<<<<<< HEAD

    private void Awake()
=======
    [SerializeField] private float fillSpeed;
    void Start()
>>>>>>> 1d1d29b8325eb7a0c1a838ef7803e961952daa19
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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
