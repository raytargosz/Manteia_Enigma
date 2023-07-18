using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    public float checkInterval = 0.5f; // Interval between checks
    public float maxDistance = 50f; // Max distance to check for objects
    private Camera cam;

    private Collider[] colliders;
    private float nextCheckTime;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Time.time > nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckVisibility();
        }
    }

    private void CheckVisibility()
    {
        // Get all objects within max distance
        colliders = Physics.OverlapSphere(transform.position, maxDistance);

        foreach (Collider col in colliders)
        {
            GameObject go = col.gameObject;

            // Disable game object if not visible
            if (IsVisible(go) == false)
            {
                go.SetActive(false);
            }
            // Enable game object if visible
            else
            {
                go.SetActive(true);
            }
        }
    }

    private bool IsVisible(GameObject go)
    {
        // Checks if the game object's bounds are within the camera's view frustum
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), go.GetComponent<Collider>().bounds);
    }
}
