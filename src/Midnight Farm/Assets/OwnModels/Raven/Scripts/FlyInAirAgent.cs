using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyInAirAgent : Agent
{
    [SerializeField] private Raven raven;

    // For training
    private Vector3[] initPositions = new Vector3[100];
    private Quaternion[] initRotations = new Quaternion[100];
    [SerializeField] private float currentGoalClosestDistance = 0f;
    [SerializeField] private float nextPointAtTime = 0f;
    private float stayAtPointTime = 5f;
    private float nextPointSpawnDistance = 5f;

    private float maxRewardForStayAtPoint = 2f;
    private float maxRewardForTravelToPoint = 1f;

    [SerializeField] private float accumulatedDistanceReward = 0f;
    [SerializeField] private float accumulatedStayReward = 0f;
    [SerializeField] private float accumulatedReward = 0f;
    

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
        ResetStateRecursive(raven.baseBoneRigidBody.transform);
        raven.targetTransform.position = raven.baseBoneRigidBody.transform.position + Vector3.up * nextPointSpawnDistance;
        currentGoalClosestDistance = nextPointSpawnDistance;
        nextPointAtTime = Time.time + 9999;
        accumulatedDistanceReward = 0;
        accumulatedStayReward = 0;
        accumulatedReward = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation to target (3 observations)
        Vector3 goalRelative = raven.baseBoneRigidBody.transform.InverseTransformPoint(raven.targetTransform.position);
        float closenessMul = Mathf.Min(1f, goalRelative.magnitude);
        sensor.AddObservation(goalRelative.normalized * closenessMul);

        // Observation of the whole bodies tilting (3 observations)
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
        // ******************
        // Reward Caclulation
        // ******************
        Vector3 posToTarget = raven.targetTransform.position - raven.baseBoneRigidBody.transform.position;
        float distanceToTarget = posToTarget.magnitude;

        // Reward based on traveled distance towards goal
        float distanceReward = 0f;
        if (distanceToTarget < currentGoalClosestDistance)
        {
            float deltaDistance = currentGoalClosestDistance - distanceToTarget;
            Vector2 posToTarget2d = new Vector2(posToTarget.x, posToTarget.z);
            float distance2d = posToTarget2d.magnitude;
            Vector2 birdForward2d = new Vector2(raven.baseBoneRigidBody.transform.up.x, raven.baseBoneRigidBody.transform.up.z);
            float scaleDirectional = Mathf.Min(1, distance2d);
            float orientationBasedScale = scaleDirectional * Mathf.Max(0f, Vector2.Dot(birdForward2d, posToTarget2d.normalized)) + (1 - scaleDirectional);
            distanceReward = maxRewardForTravelToPoint * deltaDistance * orientationBasedScale / nextPointSpawnDistance;
        }

        // Additional reward when staying close to the center
        float stayReward = 0;
        if (nextPointAtTime - stayAtPointTime < Time.time)
        {
            stayReward = maxRewardForStayAtPoint * Mathf.Max(0, 1 - distanceToTarget) * Time.deltaTime / stayAtPointTime;
        }

        accumulatedDistanceReward += distanceReward;
        accumulatedStayReward += stayReward;
        accumulatedReward += distanceReward + stayReward;
        SetReward(distanceReward + stayReward);

        // ***************************************
        // Updating goal and episode end condition
        // ***************************************
        if (distanceToTarget < currentGoalClosestDistance)
        {
            currentGoalClosestDistance = distanceToTarget;
        }

        // Start stay at point timer
        if (distanceToTarget < 1 && nextPointAtTime > Time.time + stayAtPointTime)
        {
            nextPointAtTime = Time.time + stayAtPointTime;
        }

        // Move goal to new point
        if (nextPointAtTime < Time.time)
        {
            nextPointAtTime = Time.time + 9999;
            currentGoalClosestDistance = nextPointSpawnDistance;
            do
            {
                Vector3 newPoint = raven.targetTransform.position + Random.onUnitSphere * nextPointSpawnDistance;
                if (newPoint.y < 2)
                {
                    continue;
                }
                raven.targetTransform.position = newPoint;
            } while (false);
            Debug.Log("New point!");
        }

        // Kill bird if it falls to low
        if (raven.baseBoneRigidBody.transform.position.y < 1)
        {
            EndEpisode();
            Debug.Log("Collected reward: " + GetCumulativeReward());
        }
    }
}
