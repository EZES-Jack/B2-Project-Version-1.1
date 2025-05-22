using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f; // Degrees per second

    private CharacterController characterController;

    void Start()
    {
        // Get the CharacterController component attached to the GameObject
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * moveSpeed * Time.deltaTime;

        // Move the character using the CharacterController
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
            // Ensure the player's vertical position remains unchanged
            Vector3 playerPosition = transform.position;
            playerPosition.y = hit.point.y;
            transform.position = playerPosition;
        }
    }
}