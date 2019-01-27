using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PickupTrigger : MonoBehaviour
{
    public GameObject SpawnedFrom;
    public GameObject mPickupObject;

    public float RotationSpeed = 100.0f;
    public float ItemRotation = 26.0f;
    public float SineSpeed = 5.0f;
    public float SineAmplitude = 0.1f;

    private float radiants = 0.0f;

    void Awake()
    {
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
    }

    public void SetPickupObject(GameObject item)
    {
        mPickupObject = item;
        mPickupObject.GetComponent<Pickable>().OnDemoBegin();

        mPickupObject.transform.Rotate(Vector3.left, ItemRotation);
    }

    void Update()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        radiants += (Time.deltaTime * SineSpeed) % 1;
        mPickupObject.transform.localPosition = Vector3.up * SineAmplitude * Mathf.Sin(radiants);
    }
    
    private void OnDestroy()
    {
        Pickable pickable = mPickupObject.GetComponent<Pickable>();
        if (SpawnedFrom != null && pickable.Spawner != null)
        {
            pickable.Spawner.ItemPickedUp(SpawnedFrom);
        }
    }
}
