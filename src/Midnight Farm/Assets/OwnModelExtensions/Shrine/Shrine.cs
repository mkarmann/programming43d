using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{

    [SerializeField] private GameObject toSpawn;

    private bool spawned = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !spawned)
        {
            Instantiate(toSpawn, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            spawned = true;
            Debug.Log("Pumpkin Spawned!");
        }
    }
}
