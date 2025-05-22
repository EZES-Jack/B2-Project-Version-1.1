using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BulletShooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 1.0f; // Time between shots
    [SerializeField] private int magazineSize = 10; // Number of shots before reloading
    [SerializeField] private float reloadTime = 2.0f; // Time to reload
    [SerializeField] private float bulletSpeed = 20f; // Speed of the bullet
    [SerializeField] private float damageAmount = 10f; // Damage amount
    [SerializeField] private Text ammoText; // Reference to the UI Text element

    private float nextFireTime = 0f;
    private int shotsFired = 0;
    private bool isReloading = false;

    void Start()
    {
        UpdateAmmoText();
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
        if (nearestEnemy != null)
        {
            nextFireTime = Time.time + fireRate;
            shotsFired++;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(damageAmount);
            }

            // Check if the nearest enemy is a spawner and apply damage
            Spawner spawner = nearestEnemy.GetComponent<Spawner>();
            if (spawner != null)
            {
                spawner.TakeDamage(damageAmount);
            }

            UpdateAmmoText();

            if (shotsFired >= magazineSize)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        shotsFired = 0;
        isReloading = false;
        UpdateAmmoText();
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + (magazineSize - shotsFired);
        }
    }
}