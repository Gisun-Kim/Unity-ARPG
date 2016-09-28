using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class AreaTrigger : MonoBehaviour
{
    public event Action<Collider> OnAreaEnter;
    public event Action<Collider> OnAreaExit;

    private SphereCollider _collider;

    void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        if (_collider == null)
        {
            Debug.LogError("No SphereCollider assigned. AreaTrigger will now be disabled.");
            this.enabled = false;
            return;
        }
        _collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (OnAreaEnter != null) OnAreaEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (OnAreaEnter != null) OnAreaEnter(other);
    }
}
