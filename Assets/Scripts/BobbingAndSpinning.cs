using UnityEngine;

public class BobbingAndSpinning : MonoBehaviour
{
    public float bobbingSpeed = 0f;  // Speed at which the object bobs
    public float bobbingAmount = 0f; // Range of the bobbing
    public float spinningSpeed = 0f; // Speed at which the object spins

    private Vector3 initialPosition;
    private float bobbingTimer;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (bobbingSpeed != 0 && bobbingAmount != 0)
        {
            // Bobbing
            bobbingTimer += Time.deltaTime * bobbingSpeed;
            float newY = initialPosition.y + Mathf.Sin(bobbingTimer) * bobbingAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (spinningSpeed != 0)
        {
            // Spinning
            transform.Rotate(0f, Time.deltaTime * spinningSpeed, 0f);
        }
    }
}
