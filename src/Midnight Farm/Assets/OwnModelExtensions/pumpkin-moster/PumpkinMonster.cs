using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinMonster : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] private Light light;
    [SerializeField] private AudioSource backgroundAmbient = null;
    [SerializeField] private AudioSource onFinishSound = null;
    private float timeSinceReconstructed = 999999;

    private Transform head;
    void Start()
    {
        head = transform.Find("pumpkin").Find("head");
        head.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pumpkin")
        {
            head.gameObject.SetActive(true);
            Destroy(other.gameObject);

            // Kill all the birds
            Raven[] ravens = (Raven[])FindObjectsOfType(typeof(Raven));
            foreach (Raven r in ravens)
            {
                r.die();
            }

            light.enabled = true;
            if (backgroundAmbient != null)
            {
                backgroundAmbient.Stop();
            }
            
            onFinishSound.Play();
        }
    }
}
