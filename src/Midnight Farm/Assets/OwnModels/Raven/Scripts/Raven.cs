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

    private bool isDying = false;
    private float dyingSince = 0f;
    private float constTimeUntilDeath = 5f;

    public Vector3 wingLeftTorque = Vector3.zero;
    public Vector3 wingRightTorque = Vector3.zero;
    public Vector3 wingLeftTorque2 = Vector3.zero;
    public Vector3 wingRightTorque2 = Vector3.zero;

    public float aggressionTriggerDistance = 5f;
    public float aggressionActionSmaller = 0.5f;
    public bool isAggressive = false;

    private Transform player = null;

    public bool getIsDying()
    {
        return isDying;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Search for the player
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        if (objs.Length > 0)
        {
            player = objs[0].transform;
        }
    }

    private void Update()
    {
        if (isDying && Time.time > dyingSince + constTimeUntilDeath)
        {
            Destroy(gameObject);
        }

        if (!isDying && player != null)
        {
            Vector3 delta = player.transform.position - baseBoneRigidBody.position;
            if (!isAggressive && delta.magnitude < aggressionTriggerDistance * aggressionActionSmaller)
            {
                isAggressive = true;
            }
            
            if (isAggressive)
            {
                targetTransform.position = player.transform.position + Vector3.up * 1.7f;
            }
        }
    }

    private void FixedUpdate()
    {
        float currentWingJointStrength = wingJointStrength;
        float currentWingJointStrength2 = wingJointStrength2;
        if (isDying)
        {
            // Strength of bird is fading
            float factorTilDead = (Time.time - dyingSince) / constTimeUntilDeath;
            currentWingJointStrength = 0.4f * factorTilDead * wingJointStrength;
            currentWingJointStrength2 = 0.4f * factorTilDead * wingJointStrength2;
        }

        leftWingRigidBody.AddRelativeTorque(wingLeftTorque * currentWingJointStrength);
        rightWingRigidBody.AddRelativeTorque(wingRightTorque * currentWingJointStrength);
        leftWingRigidBody2.AddRelativeTorque(wingLeftTorque2 * currentWingJointStrength2);
        rightWingRigidBody2.AddRelativeTorque(wingRightTorque2 * currentWingJointStrength2);

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
        
        Vector3 leftPushback = -400 * leftWingDragRigidBody.velocity *leftPushbackStrength;
        Vector3 rightPushback = -400 * rightWingDragRigidBody.velocity * rightPushbackStrength;

        baseBoneRigidBody.AddForceAtPosition(leftPushback, leftWingRigidBody.position);
        baseBoneRigidBody.AddForceAtPosition(rightPushback, rightWingRigidBody.position);
        
        // baseBoneRigidBody.AddForce(0, 9.81f * baseBoneRigidBody.mass, 0); // Conteract gravity

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

    private void OnDrawGizmos()
    {
        if (!isDying)
        {
            Gizmos.color = isAggressive ? Color.red : Color.grey;
            Gizmos.DrawWireSphere(baseBoneRigidBody.position, aggressionTriggerDistance);
            Gizmos.DrawWireSphere(baseBoneRigidBody.position, aggressionTriggerDistance * aggressionActionSmaller);
        }
    }

    public bool die()
    {
        if (isDying)
        {
            return false;
        }

        isDying = true;
        dyingSince = Time.time;
        baseBoneRigidBody.useGravity = true;
        return true;
        
    }
}
