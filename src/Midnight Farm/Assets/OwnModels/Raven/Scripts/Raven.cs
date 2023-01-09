using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raven : MonoBehaviour
{
    [Tooltip("Target point to which the bird should fly to")]
    [SerializeField] public Transform targetTransform;

    [SerializeField] public Rigidbody baseBoneRigidBody;
    [SerializeField] public Rigidbody leftWingRigidBody;
    [SerializeField] public Rigidbody rightWingRigidBody;
    [SerializeField] public Rigidbody leftWingRigidBody2;
    [SerializeField] public Rigidbody rightWingRigidBody2;
    [SerializeField] public Rigidbody leftWingDragRigidBody;
    [SerializeField] public Rigidbody rightWingDragRigidBody;

    [SerializeField] private float wingJointStrength = 30f;
    [SerializeField] private float wingJointStrength2 = 5f;

    public Vector3 wingLeftTorque = Vector3.zero;
    public Vector3 wingRightTorque = Vector3.zero;
    public Vector3 wingLeftTorque2 = Vector3.zero;
    public Vector3 wingRightTorque2 = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        leftWingRigidBody.AddRelativeTorque(wingLeftTorque * wingJointStrength);
        rightWingRigidBody.AddRelativeTorque(wingRightTorque * wingJointStrength);
        leftWingRigidBody2.AddRelativeTorque(wingLeftTorque2 * wingJointStrength2);
        rightWingRigidBody2.AddRelativeTorque(wingRightTorque2 * wingJointStrength2);

        // Update the flying
        float leftPushbackStrength = Vector3.Dot(leftWingDragRigidBody.velocity.normalized, -leftWingDragRigidBody.transform.forward);
        float rightPushbackStrength = Vector3.Dot(rightWingDragRigidBody.velocity.normalized, -rightWingDragRigidBody.transform.forward);
        if (rightPushbackStrength < 0)
        {
            rightPushbackStrength *= -0.01f;
        }
        if (leftPushbackStrength < 0)
        {
            leftPushbackStrength *= -0.01f;
        }
        leftWingDragRigidBody.drag = leftPushbackStrength * 20f;
        rightWingDragRigidBody.drag = rightPushbackStrength * 20f;
        
        Vector3 leftPushback = -350 * leftWingDragRigidBody.velocity *leftPushbackStrength;
        Vector3 rightPushback = -350 * rightWingDragRigidBody.velocity * rightPushbackStrength;

        baseBoneRigidBody.AddForceAtPosition(leftPushback, leftWingRigidBody.position);
        baseBoneRigidBody.AddForceAtPosition(rightPushback, rightWingRigidBody.position);

        // baseBoneRigidBody.AddRelativeTorque(0, -20 * leftWingDragRigidBody.velocity.magnitude * leftPushbackStrength, 0);
        // baseBoneRigidBody.AddRelativeTorque(0, 20 * rightWingDragRigidBody.velocity.magnitude * rightPushbackStrength, 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(baseBoneRigidBody.position, targetTransform.position);

        Gizmos.DrawRay(leftWingDragRigidBody.position, -leftWingDragRigidBody.transform.forward);
        Gizmos.DrawRay(rightWingDragRigidBody.position, -rightWingDragRigidBody.transform.forward);
    }
}
