using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;  // Assign your player object in the inspector

    [SerializeField]
    private Scrollbar healthSlider, staminaSlider, boostSlider;

    private void Update()
    {
        healthSlider.value = player.CurrentHealth;  // Assuming you have public getter for CurrentHealth in Player script
        staminaSlider.value = player.CurrentStamina;  // Similarly for CurrentStamina
        boostSlider.value = player.CurrentBoost;  // And for CurrentBoost
    }
}