using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTargetGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.07f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
