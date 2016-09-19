using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int _startHP = 100;
    [SerializeField]
    private int _attackPower;

    private int _curruntHP;
    private bool _attacking = false;
    private bool _died = false;

    private Transform lastAttackTarget;

    // Use this for initialization
    void Start()
    {
        _curruntHP = _startHP;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
