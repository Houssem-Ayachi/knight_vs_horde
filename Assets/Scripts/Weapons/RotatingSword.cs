using UnityEngine;

public class RotatingSword : MonoBehaviour
{
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 90f; // degrees per second

    public float currentAngle = 180f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Increment angle over time
        currentAngle += orbitSpeed * Time.deltaTime;

        // Calculate position relative to parent (the player)
        float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * orbitRadius;
        float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * orbitRadius;

        // Set local position (relative to player)
        transform.localPosition = new Vector3(x, y, 0f);

        // Optional: make the weapon always point outward from the player
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle); 
    }
}
