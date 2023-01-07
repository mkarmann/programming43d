using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FlyInAirAgent : Agent
{
    [Tooltip("Target point to which the bird should fly to")]
    [SerializeField] private Transform targetTransform;

    [SerializeField] private Transform baseBoneTransform;

    // privet members for optimization

    // Start is called before the first frame update
    public override void Initialize()
    {
        base.Initialize();
        Debug.Log("here");
        baseBoneTransform = transform.Find("Armature").Find("Bone");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observation to find the target
        sensor.AddObservation((targetTransform.position - baseBoneTransform.position).normalized);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // float moveZ = actions.ContinuousActions[0];
    }

    private void OnDrawGizmos()
    {
        Transform bone = baseBoneTransform = transform.Find("Armature").Find("Bone");
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bone.position, targetTransform.position);
    }
}
