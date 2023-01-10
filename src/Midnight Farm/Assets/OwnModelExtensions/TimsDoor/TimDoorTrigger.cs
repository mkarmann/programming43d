using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimDoorTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationName;
    [SerializeField] private float animationSpeed = 1f;

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            // play Bounce but start at a quarter of the way though
            animator.speed = animationSpeed;
            animator.Play(animationName);
            Destroy(gameObject);
        }
    }
}
