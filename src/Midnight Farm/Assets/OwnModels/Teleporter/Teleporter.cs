using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Teleporter : MonoBehaviour
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private SphereCollider collider;

    private FirstPersonController controller;
    private float teleportStartTime = 0;
    private float waitUntilMoveAgain = 0.1f;


    private void Update()
    {
        if (controller != null && teleportStartTime + waitUntilMoveAgain < Time.time)
        {
            controller.enabled = true;
            controller = null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            controller = other.gameObject.transform.GetComponent<FirstPersonController>();
            controller.enabled = false;
            other.gameObject.transform.position = targetTransform.position;
            other.gameObject.transform.rotation = targetTransform.rotation;
            teleportStartTime = Time.time;
            Debug.Log("Teleported player!");
        }
    }

    private void OnDrawGizmos()
    {
        float scale = collider.radius * (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, scale);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, targetTransform.position);
    }
}
