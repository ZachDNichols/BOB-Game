using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class GarbageItem : MonoBehaviour
{
    public enum GarbageType
    {
        Banana,
        PlasticBottle,
        CrumpledPaper
    }
    
    public GarbageType garbageType;
    private Rigidbody2D _rigidbody2D;
    private float _itemSpeed;
    
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _itemSpeed = Spawner.Instance.itemSpeed;
        _rigidbody2D.isKinematic = true;
        _rigidbody2D.velocityX = _itemSpeed;
    }

    public void Drop()
    {
        _rigidbody2D.isKinematic = false;
        _rigidbody2D.velocityX = 0;
    }
    
}
