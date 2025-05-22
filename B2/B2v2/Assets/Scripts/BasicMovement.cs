using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Collider))]
public class BasicMovement2 : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private PlayerStats playerStats;

    public CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * playerStats.moveSpeed * Time.deltaTime;
        characterController.Move(movement);

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Vector3 playerPosition = transform.position;
            playerPosition.y = hit.point.y;
            transform.position = playerPosition;
            Debug.Log("Collision with Enemy detected!");
        }
    }
}