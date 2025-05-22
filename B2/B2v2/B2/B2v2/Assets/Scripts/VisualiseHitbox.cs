using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VisualizeCollider : MonoBehaviour
{
    // Reference to the player's collider
    private Collider playerCollider;
    // Material used for drawing the wireframe
    private Material lineMaterial;

    void Start()
    {
        // Get the Collider component attached to the player
        playerCollider = GetComponent<Collider>();
        // Create the material for drawing lines
        CreateLineMaterial();
    }

    // Create a material for drawing lines
    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Find the shader used for drawing lines
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            // Create a new material with the found shader
            lineMaterial = new Material(shader)
            {
                // Hide the material in the editor and don't save it
                hideFlags = HideFlags.HideAndDontSave
            };
            // Set material properties for transparency and rendering
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Render the collider as wireframe
    void OnRenderObject()
    {
        // Ensure the playerCollider is assigned
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider>();
        }

        if (playerCollider != null)
        {
            // Set the material for rendering
            lineMaterial.SetPass(0);
            // Save the current transformation matrix
            GL.PushMatrix();
            // Apply the local transformation of the GameObject
            GL.MultMatrix(transform.localToWorldMatrix);
            // Begin drawing lines
            GL.Begin(GL.LINES);
            // Set the color of the lines to red
            GL.Color(Color.red);

            // Draw the appropriate wireframe based on the collider type
            if (playerCollider is BoxCollider box)
            {
                // Draw a wireframe cube for BoxCollider
                DrawWireCube(box.center, box.size);
            }
            else if (playerCollider is SphereCollider sphere)
            {
                // Draw a wireframe sphere for SphereCollider
                DrawWireSphere(sphere.center, sphere.radius);
            }
            else if (playerCollider is CapsuleCollider capsule)
            {
                // Draw a wireframe sphere for CapsuleCollider (as a placeholder)
                DrawWireSphere(capsule.center, capsule.radius);
            }

            // End drawing lines
            GL.End();
            // Restore the previous transformation matrix
            GL.PopMatrix();
        }
    }

    // Draw a wireframe cube
    void DrawWireCube(Vector3 center, Vector3 size)
    {
        // Calculate the half size of the cube
        Vector3 halfSize = size * 0.5f;
        // Define the vertices of the cube
        Vector3[] vertices = {
            center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, halfSize.z)
        };

        // Define the lines connecting the vertices to form the edges of the cube
        int[] lines = {
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 4, 1, 5, 2, 6, 3, 7
        };

        // Draw lines between the vertices to form the wireframe cube
        for (int i = 0; i < lines.Length; i += 2)
        {
            GL.Vertex(vertices[lines[i]]);
            GL.Vertex(vertices[lines[i + 1]]);
        }
    }

    // Draw a wireframe sphere
    void DrawWireSphere(Vector3 center, float radius)
    {
        // Number of segments to approximate the sphere
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            // Calculate the angle for the current and next segment
            float theta = (float)i / segments * 2.0f * Mathf.PI;
            float nextTheta = (float)(i + 1) / segments * 2.0f * Mathf.PI;

            // Draw lines on the XZ plane to form the wireframe sphere
            GL.Vertex(center + new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * radius);
            GL.Vertex(center + new Vector3(Mathf.Cos(nextTheta), 0, Mathf.Sin(nextTheta)) * radius);
        }
    }
}