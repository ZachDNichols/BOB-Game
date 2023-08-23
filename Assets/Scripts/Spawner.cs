using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class Spawner : MonoBehaviour
{
    //Public variables
    public float itemSpeed = 10f;
    public GameObject banana;
    public GameObject plasticBottle;
    public GameObject crumpledPaper;
    public InputAction drop;
    public static Spawner Instance;
    
    //Private variables
    private Vector2 _itemSpawnPosition;
    private bool _startedGame = false;
    private Queue<GarbageItem> _items = new Queue<GarbageItem>();
    private GarbageItem currentGameObject;

    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void GameManagerOnOnGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.WaitingToStart:
                break;
            case GameManager.GameState.Playing:
                StartCoroutine(SpawningItems());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _itemSpawnPosition = gameObject.transform.position;
        
        if (!banana)
        {
            throw new Exception("Banana not found");
        }
        
        if (!plasticBottle)
        {
            throw new Exception("Plastic bottle not found");
        }
        
        if (!crumpledPaper)
        {
            throw new Exception("Crumpled paper not found");
        }

        drop.performed += OnDrop;
        drop.Enable();
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
        drop.performed -= OnDrop;
    }

    IEnumerator SpawningItems()
    {
        while (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            SpawnItem();
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnItem()
    {
        int itemSpawnIndex = UnityEngine.Random.Range(1, 3);
        GarbageItem spawnedItem;
        
        
        switch (itemSpawnIndex)
        {
            case 1:
                spawnedItem = Instantiate(banana, _itemSpawnPosition, Quaternion.identity).GetComponent<GarbageItem>();
                break;
            case 2:
                spawnedItem = Instantiate(plasticBottle, _itemSpawnPosition, Quaternion.identity).GetComponent<GarbageItem>();
                break;
            case 3:
                spawnedItem = Instantiate(crumpledPaper, _itemSpawnPosition, Quaternion.identity).GetComponent<GarbageItem>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemSpawnIndex), itemSpawnIndex, null);
        }
        
        _items.Enqueue(spawnedItem);
    }
    
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            if (currentGameObject == null)
            {
                currentGameObject = _items.Dequeue();
                currentGameObject.Drop();
                currentGameObject = _items.Any() ? _items.Dequeue() : null;
            }
            else
            {
                currentGameObject.Drop();
                currentGameObject = _items.Any() ? _items.Dequeue() : null;
            }
            
        }
    }
}
