using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenCollisionHandler : MonoBehaviour
{

    [SerializeField] private Raven raven;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Pumpkin")
        {
            GameObject pumpkin = collision.collider.gameObject;
            if (raven.die())
            {
                Vector3 delta = (pumpkin.transform.position - raven.baseBoneRigidBody.position).normalized;
                Rigidbody rb = pumpkin.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.AddForce(delta * 200f);
                raven.baseBoneRigidBody.AddForce(delta * -5000);
            }
        }
    }
}
