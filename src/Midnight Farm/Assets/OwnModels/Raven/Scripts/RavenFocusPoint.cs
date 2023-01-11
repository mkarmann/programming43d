using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenFocusPoint : MonoBehaviour
{
    [SerializeField] private Transform focusPoint;
    [SerializeField] private Transform goalPoint;
    [SerializeField] private Raven raven;

    // Update is called once per frame
    void Update()
    {
        if (!raven.isAggressive)
        {
            Vector3 direction = (focusPoint.transform.position - raven.baseBoneRigidBody.transform.position).normalized;
            goalPoint.transform.position = focusPoint.transform.position + direction * 3f;
        }
    }
}
