using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    //Public variables
    public float itemSpeed = 10f;
    public GameObject banana;
    public GameObject plasticBottle;
    public GameObject crumpledPaper;
    public InputAction drop;
    
    //Private variables
    private Vector2 _itemSpawnPosition;
    private bool _startedGame = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _itemSpawnPosition = gameObject.transform.position;
        
        if (!banana)
        {
            Debug.LogError("Banana not found");
            Application.Quit();
        }
        
        if (!plasticBottle)
        {
            Debug.LogError("Plastic bottle not found");
            Application.Quit();
        }
        
        if (!crumpledPaper)
        {
            Debug.LogError("Crumpled paper not found");
            Application.Quit();
        }

        drop.performed += ctx => OnDrop();
        drop.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_startedGame)
        {
            return;
        }
    }
    
    public void OnDrop()
    {
        Debug.Log("Drop!");
    }
}
