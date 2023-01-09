using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyInAirAgent : Agent
{
    [SerializeField] private Raven raven;


    private float highestPos = 0f;
    private Vector3[] initPositions = new Vector3[100];
    private Quaternion[] initRotations = new Quaternion[100];

    public int CollectStateRecursive(Transform t, int objIdx = 0)
    {
        initPositions[objIdx] = t.position;
        initRotations[objIdx] = t.rotation;
        objIdx++;

        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            objIdx = CollectStateRecursive(child, objIdx);
        }
        
        return objIdx;
    }

    public int ResetStateRecursive(Transform t, int objIdx = 0)
    {
        t.position = initPositions[objIdx];
        t.rotation = initRotations[objIdx];
        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        objIdx++;

        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            objIdx = ResetStateRecursive(child, objIdx);
        }
        
        return objIdx;
    }

    private void Start()
    {
        CollectStateRecursive(raven.baseBoneRigidBody.transform);
    }


    public override void OnEpisodeBegin()
    {
        // Reset pose somehow
        highestPos = 0f;
        ResetStateRecursive(raven.baseBoneRigidBody.transform);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation to target (3 observations)
        // sensor.AddObservation((raven.targetTransform.position - raven.baseBoneRigidBody.position).normalized);

        // Observation of the whole bodies orientation (3 observations)
        sensor.AddObservation(Vector3.Dot(raven.baseBoneRigidBody.transform.right, Vector3.up));
        sensor.AddObservation(Vector3.Dot(raven.baseBoneRigidBody.transform.up, Vector3.up));
        sensor.AddObservation(Vector3.Dot(raven.baseBoneRigidBody.transform.forward, Vector3.up));

        // Observation of the birds wings (18 observations)
        sensor.AddObservation(raven.leftWingRigidBody.velocity.y);
        sensor.AddObservation(raven.rightWingRigidBody.velocity.y);
        sensor.AddObservation(raven.leftWingRigidBody.transform.localRotation);
        sensor.AddObservation(raven.rightWingRigidBody.transform.localRotation);
        sensor.AddObservation(raven.leftWingRigidBody2.transform.localRotation);
        sensor.AddObservation(raven.rightWingRigidBody2.transform.localRotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        raven.wingLeftTorque.x = actions.ContinuousActions[0];
        raven.wingLeftTorque.y = actions.ContinuousActions[1];
        raven.wingLeftTorque.z = actions.ContinuousActions[2];

        raven.wingRightTorque.x = actions.ContinuousActions[3];
        raven.wingRightTorque.y = actions.ContinuousActions[4];
        raven.wingRightTorque.z = actions.ContinuousActions[5];

        raven.wingLeftTorque2.x = actions.ContinuousActions[6];
        raven.wingLeftTorque2.y = actions.ContinuousActions[7];
        raven.wingLeftTorque2.z = actions.ContinuousActions[8];

        raven.wingRightTorque2.x = actions.ContinuousActions[9];
        raven.wingRightTorque2.y = actions.ContinuousActions[10];
        raven.wingRightTorque2.z = actions.ContinuousActions[11];
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[3] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        // Debug.Log(continuousActions[0]);
    }

    private void Update()
    {
        float currentReward = raven.baseBoneRigidBody.position.y;
        if (currentReward > highestPos)
        {
            highestPos = currentReward;
            SetReward(currentReward);
        }
        
        if (raven.baseBoneRigidBody.position.y < 1 || raven.baseBoneRigidBody.position.y > 100)
        {
            EndEpisode();
        }
    }
}
