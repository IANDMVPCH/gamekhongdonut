using UnityEngine;

public class SpinImage : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}