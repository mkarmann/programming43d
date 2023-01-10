using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pumpkin : MonoBehaviour
{
    private bool inHand = false;
    private float inHandSince = 0f;
    private bool pullbackActive = false;
    private GameObject posessing = null;
    private Transform grabberTransform = null;
    private GameObject objectInGrabRange = null;

    [SerializeField] private Collider physicsCollider = null;
    [SerializeField] private float retrivalSpeed = 40f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (posessing == null && objectInGrabRange != null)
            {
                grab(objectInGrabRange);
            }
            else if (posessing != null)
            {
                if (inHand)
                {
                    throwHead();
                }
                else
                {
                    pullback();
                }
            }
        }

        if (pullbackActive)
        {
            Vector3 delta = grabberTransform.position - transform.position;
            transform.LookAt(grabberTransform.position);
            if (delta.magnitude < Time.deltaTime * retrivalSpeed)
            {
                hold();
            }
            else
            {
                transform.position += (delta).normalized * Time.deltaTime * retrivalSpeed;
            }
        }
    }

    private void grab(GameObject grabber)
    {
        posessing = grabber;
        grabberTransform = grabber.transform.Find("PlayerCamera");
        hold();
    }

    private void hold()
    {
        inHand = true;
        pullbackActive = false;
        inHandSince = Time.time;
        physicsCollider.enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.parent = grabberTransform;
        transform.position = grabberTransform.position + grabberTransform.forward * 1f + grabberTransform.right * 0.5f - grabberTransform.up * 0.3f;
        transform.LookAt(grabberTransform.position);
    }

    private void throwHead()
    {
        inHand = false;
        physicsCollider.enabled = true;
        transform.parent = null;
        transform.position = grabberTransform.position + grabberTransform.forward * 1.5f;
        Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(grabberTransform.forward * 400f + Vector3.up * 100f);
    }

    private void pullback()
    {
        physicsCollider.enabled = false;
        Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        pullbackActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && posessing == null)
        {
            Debug.Log("In grab range");
            objectInGrabRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Out of grab range");
            objectInGrabRange = null;
        }
    }
}
