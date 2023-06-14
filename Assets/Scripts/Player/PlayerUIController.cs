using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private Scrollbar healthBar;
    [SerializeField]
    private Scrollbar staminaBar;
    [SerializeField]
    private Scrollbar boostBar;
    [SerializeField]
    private Image healthHandleImage;
    [SerializeField]
    private Image staminaHandleImage;
    [SerializeField]
    private Image boostHandleImage;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        float healthRatio = currentHealth / maxHealth;
        healthBar.size = healthRatio;
        healthHandleImage.fillAmount = healthRatio;
    }

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        float staminaRatio = currentStamina / maxStamina;
        staminaBar.size = staminaRatio;
        staminaHandleImage.fillAmount = staminaRatio;
    }

    public void UpdateBoostBar(float currentBoost, float maxBoost)
    {
        float boostRatio = currentBoost / maxBoost;
        boostBar.size = boostRatio;
        boostHandleImage.fillAmount = boostRatio;
    }

    public void SetStaminaHandleAlpha(float alpha)
    {
        staminaHandleImage.color = new Color(staminaHandleImage.color.r, staminaHandleImage.color.g, staminaHandleImage.color.b, alpha);
    }

    public void SetBoostHandleAlpha(float alpha)
    {
        boostHandleImage.color = new Color(boostHandleImage.color.r, boostHandleImage.color.g, boostHandleImage.color.b, alpha);
    }

    public void LerpBoostBarSize(float targetSize, float speed)
    {
        boostBar.size = Mathf.Lerp(boostBar.size, targetSize, speed);
    }

    public void SetHealthHandleAlpha(float alpha)
    {
        healthHandleImage.color = new Color(healthHandleImage.color.r, healthHandleImage.color.g, healthHandleImage.color.b, alpha);
    }
}
