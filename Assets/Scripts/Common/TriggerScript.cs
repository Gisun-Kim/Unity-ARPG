using UnityEngine;
using System;
using System.Collections;

public class AreaTrigger : MonoBehaviour
{
    public event Action<Collider> OnAreaEnter;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

    }
}
