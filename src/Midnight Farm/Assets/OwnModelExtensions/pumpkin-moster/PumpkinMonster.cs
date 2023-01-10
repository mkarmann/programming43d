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

    // Update is called once per frame
    void Update()
    {
        
    }
}
