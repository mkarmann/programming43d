using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinMonster : MonoBehaviour
{
    // Start is called before the first frame update

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
        }
    }
}
