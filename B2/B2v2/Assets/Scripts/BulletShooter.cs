using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletShooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text statsText;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip shootingSound; // Shooting sound clip
    [SerializeField] private AudioClip reloadSound; // Reload sound clip

    private float nextFireTime = 0f;
    private bool isReloading = false;

    void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        UpdateAmmoText();
        UpdateStatsText();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !isReloading)
        {
            FireBullet();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void FireBullet()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        Vector3 direction;

        if (nearestEnemy != null)
        {
            direction = (nearestEnemy.transform.position - transform.position).normalized;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                direction = (hit.point - transform.position).normalized;
            }
            else
            {
                direction = Camera.main.transform.forward;
            }
        }

        if (playerStats.ammo > 0)
        {
            nextFireTime = Time.time + (1f / playerStats.fireRate);
            playerStats.ammo--;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = direction * playerStats.bulletSpeed;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(playerStats.damage);
            }

            if (nearestEnemy != null)
            {
                Spawner spawner = nearestEnemy.GetComponent<Spawner>();
                if (spawner != null)
                {
                    spawner.TakeDamage(playerStats.damage);
                }
            }

            // Play the shooting sound effect
            if (audioSource != null && shootingSound != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }

            UpdateAmmoText();

            if (playerStats.ammo <= 0 && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] variants = GameObject.FindGameObjectsWithTag("Variant");

        GameObject nearestTarget = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in enemies)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        foreach (GameObject target in variants)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        if (ammoText != null)
            ammoText.text = "Reloading...";

        // Play the reload sound effect
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(playerStats.reloadTime);
        playerStats.ammo = playerStats.maxAmmo;
        isReloading = false;
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null && playerStats != null && !isReloading)
        {
            ammoText.text = "Ammo: " + playerStats.ammo;
        }
    }

    private void UpdateStatsText()
    {
        if (statsText != null && playerStats != null)
        {
            statsText.text = $"Fire Rate: {playerStats.fireRate:0} bullets / sec\nDamage: {playerStats.damage}";
        }
    }
}