using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour
{

    void Start()
    {
        var v = new Vector3(0.1f, 0.1f, 0.1f);
        Debug.Log(v);
        Debug.Log(v.normalized);
    }

    void Update()
    {

    }
}
