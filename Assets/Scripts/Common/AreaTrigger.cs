using UnityEngine;
using System;
using System.Collections;

public class AreaTrigger : MonoBehaviour
{
    public event Action<Collider> OnAreaEnter;

    void OnTriggerEnter(Collider other)
    {
        if (OnAreaEnter != null) OnAreaEnter(other);
    }
}
