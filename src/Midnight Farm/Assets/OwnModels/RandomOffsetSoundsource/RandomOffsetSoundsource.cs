using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomOffsetSoundsource : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float delayPercent = 1.0f;
    [SerializeField] private float maxDelay = 9999;
    [SerializeField] private float overwriteSpatialBlend = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        float delayTime = Random.Range(0, Mathf.Min(maxDelay, audioSource.clip.length * delayPercent));
        audioSource.PlayDelayed(delayTime);
        audioSource.spatialBlend = overwriteSpatialBlend;
    }
}
