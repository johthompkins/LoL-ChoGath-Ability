using System.Collections;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    // The prefab for the circle effect
    public GameObject circlePrefab;

    // The radius of the circle
    public float circleRadius = 20f;

    // The force with which to knock up colliders in the circle
    public float knockupForce = 200f;

    //The cooldown on when we can use the ability again
    public float cooldownTime = 3f;

    //this var tells us the time we can use the next ability at
    private float nextUse = 0f;
    // The layer mask for colliders that can be affected by the ability
    public LayerMask affectedLayers;
    public LayerMask ground;

    // The transform of the crosshair object
    public Transform crosshairTransform;

    private void start()
    {
        nextUse = Time.time;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isReady())
        {
            AbilityUsed();
            StartCoroutine(Ability());
        }
        
    }

    private bool isReady()
    {
        if (nextUse <= Time.time)
        {
            Debug.Log("Ability Ready!");
            return true;
        }
        else
        {
            Debug.Log("Ability Not Ready!");
            return false;
        }
    }

    private void AbilityUsed()
    {
        nextUse = Time.time + cooldownTime;
    }

    private IEnumerator Ability()
    {
        // Get the position of the crosshair
        Vector3 crosshairPos = crosshairTransform.position;

        // Create a ray from the camera to the crosshair position
        Ray ray = new Ray(Camera.main.transform.position, crosshairPos - Camera.main.transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, ground))
        {

            // Instantiate the circle prefab at the hit point and set its scale to the circle radius
            GameObject circle = Instantiate(circlePrefab, new Vector3(hit.point.x, hit.point.y - 0.06f, hit.point.z), Quaternion.identity);
            circle.transform.localScale = new Vector3(circleRadius * 2f, 0.1f, circleRadius * 2f);

            // Get all colliders within the circle
            Collider[] colliders = Physics.OverlapSphere(hit.point, circleRadius, affectedLayers);

            yield return new WaitForSeconds(1.5f);
            // Apply knockup force to all colliders within the circle
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * knockupForce, ForceMode.Impulse);
                }
            }
            yield return new WaitForSeconds(0.5f);
            Destroy(circle);
        }
        
    }
}
