using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private SphereCollider collider;
    [SerializeField] private GameObject objectToSpawn;

    private void OnDrawGizmos()
    {
        float scale = collider.radius * (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, scale);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, spawnpoint.position);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            // play Bounce but start at a quarter of the way though
            GameObject obj = Instantiate(objectToSpawn, spawnpoint);
            obj.GetComponent<Raven>().aggressionTriggerDistance = 999;  // Let it trigger imideately
            obj.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
