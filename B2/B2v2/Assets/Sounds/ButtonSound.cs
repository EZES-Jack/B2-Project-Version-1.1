using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Awake()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Add EventTrigger component if not already present
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();

        // Add hover event
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        hoverEntry.callback.AddListener((eventData) => PlayHoverSound());
        trigger.triggers.Add(hoverEntry);

        // Add click event
        Button button = gameObject.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}