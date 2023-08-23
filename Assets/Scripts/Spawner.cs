using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Object = System.Object;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class Spawner : MonoBehaviour
{
    //Public variables
    public float itemSpeed = -3f;
    public GameObject banana;
    public GameObject plasticBottle;
    public GameObject crumpledPaper;
    public InputAction dropBanana;
    public InputAction dropFiber;
    public InputAction dropBottle;
    public InputAction startGame;
    public static Spawner Instance;
    
    //Private variables
    private Vector2 _itemSpawnPosition;
    private readonly Queue<GarbageItem> _fiberItems = new Queue<GarbageItem>();
    private readonly Queue<GarbageItem> _bananaItems = new Queue<GarbageItem>();
    private readonly Queue<GarbageItem> _plasticBottleItems = new Queue<GarbageItem>();
    
    
    private GarbageItem _fiberItem;
    private GarbageItem _bananaItem;
    private GarbageItem _plasticBottleItem;

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
                CleanUpSpawnedItems();
                dropBottle.Disable();
                dropFiber.Disable();
                dropBanana.Disable();
                startGame.Enable();
                break;
            case GameManager.GameState.Playing:
                StartCoroutine(SpawningItems());
                dropBottle.Enable();
                dropFiber.Enable();
                dropBanana.Enable();
                startGame.Disable();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void CleanUpSpawnedItems()
    {
        var otherItems = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);

        foreach (var otherItem in otherItems)
        {
            _fiberItems.Enqueue(otherItem);
            _bananaItems.Enqueue(otherItem);
            _plasticBottleItems.Enqueue(otherItem);
        }
        
        foreach (var item in _fiberItems)
        {
            GameObject itemGameObject = item.gameObject;
            Destroy(itemGameObject);
        }

        foreach (var bananaItem in _bananaItems)
        {
            GameObject bananaGameObject = bananaItem.gameObject;
            Destroy(bananaGameObject);
        }
        
        foreach (var plasticBottleItem in _plasticBottleItems)
        {
            GameObject plasticBottleGameObject = plasticBottleItem.gameObject;
            Destroy(plasticBottleGameObject);
        }
        
        _bananaItems.Clear();
        _plasticBottleItems.Clear();
        _fiberItems.Clear();
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

        dropBanana.performed += OnDropBananaBanana;
        dropFiber.performed += OnDropFiber;
        dropBottle.performed += OnDropBottle;
        startGame.performed += StartGame;
        
        dropBottle.Disable();
        dropFiber.Disable();
        dropBanana.Disable();
        startGame.Enable();
    }

    void StartGame(InputAction.CallbackContext context)
    {
        Debug.Log("Start");
        if (GameManager.Instance.state == GameManager.GameState.WaitingToStart)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Playing);
        }
    }
    

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
        dropBanana.performed -= OnDropBananaBanana;
        dropFiber.performed -= OnDropFiber;
        dropBottle.performed -= OnDropBottle;
        dropBottle.performed -= StartGame;
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
                _bananaItems.Enqueue(spawnedItem);
                break;
            case 2:
                spawnedItem = Instantiate(plasticBottle, _itemSpawnPosition, Quaternion.identity).GetComponent<GarbageItem>();
                _plasticBottleItems.Enqueue(spawnedItem);
                break;
            case 3:
                spawnedItem = Instantiate(crumpledPaper, _itemSpawnPosition, Quaternion.identity).GetComponent<GarbageItem>();
                _fiberItems.Enqueue(spawnedItem);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemSpawnIndex), itemSpawnIndex, null);
        }
    }
    
    public void OnDropBananaBanana(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            if (_bananaItem == null)
            {
                if (!_bananaItems.Any())
                {
                    return;
                }
                
                _bananaItem = _bananaItems.Dequeue();
                _bananaItem.Drop();
                _bananaItem = _bananaItems.Any() ? _bananaItems.Dequeue() : null;
            }
            else
            {
                _bananaItem.Drop();
                _bananaItem = _bananaItems.Any() ? _bananaItems.Dequeue() : null;
            }
        }
    }
    
    public void OnDropFiber(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            if (_fiberItem == null)
            {
                if (!_fiberItems.Any())
                {
                    return;
                }
                
                _fiberItem = _fiberItems.Dequeue();
                _fiberItem.Drop();
                _fiberItem = _fiberItems.Any() ? _fiberItems.Dequeue() : null;
            }
            else
            {
                _fiberItem.Drop();
                _fiberItem = _fiberItems.Any() ? _fiberItems.Dequeue() : null;
            }
        }
    }
    
    public void OnDropBottle(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.state == GameManager.GameState.Playing)
        {
            if (_plasticBottleItem == null)
            {
                if (!_plasticBottleItems.Any())
                {
                    return;
                }
                
                _plasticBottleItem = _plasticBottleItems.Dequeue();
                _plasticBottleItem.Drop();
                _plasticBottleItem = _plasticBottleItems.Any() ? _plasticBottleItems.Dequeue() : null;
            }
            else
            {
                _plasticBottleItem.Drop();
                _plasticBottleItem = _plasticBottleItems.Any() ? _plasticBottleItems.Dequeue() : null;
            }
        }
    }
}
