
using Unity.VisualScripting;
using UnityEngine;

public class DimensionObject : MonoBehaviour
{
    [Header("Colliders")]
    public Collider collider_2D;
    public Collider collider_3D;

    
    private void SwitchTo2D()
    {
        collider_2D.enabled = true;
        collider_3D.enabled = false;
    }

    private void SwitchTo3D()
    {
        collider_3D.enabled = true;
        collider_2D.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var contact=collision.contacts[0];
        var hitNormal = -contact.normal;
        if (Vector3.Dot(hitNormal, Vector3.up) > 0.9f)
        {
            var rb = player.GetComponent<Rigidbody>();
            rb.position = new Vector3(PlayerPrefs.transform.position.x, PlayerPrefs.transform.position.y, transform.position.z);
        }
    }

    void Update()
    {
        
    }
}
