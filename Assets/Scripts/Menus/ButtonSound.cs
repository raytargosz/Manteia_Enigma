using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioSource audioSource;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button.interactable && selectSound != null)
        {
            audioSource.PlayOneShot(selectSound);
        }
    }
}
