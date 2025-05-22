using UnityEngine;
using TMPro;

public class GunStatsDisplay : MonoBehaviour
{
    public TextMeshPro floatingText; // Reference to the TextMeshPro object
    public Bullet bullet; // Reference to the Bullet script
    public float fireRate; // Fire rate of the gun

    void Update()
    {
        if (floatingText != null && bullet != null)
        {
            // Update the floating text with fire rate and damage values
            floatingText.text = $"Fire Rate: {fireRate:F2}\nDamage: {bullet.GetDamage()}";
        }
    }
}