using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spawner : MonoBehaviour
{
    //Public variables
    public GarbageItem banana;
    public GarbageItem plasticBottle;
    public GarbageItem crumpledPaper;
    public InputAction dropBanana;
    public InputAction dropFiber;
    public InputAction dropBottle;
    public InputAction startGame;
    
    //Private variables
    private Vector2 _itemSpawnPosition;
    private readonly Queue<GarbageItem> _fiberItems = new Queue<GarbageItem>();
    private readonly Queue<GarbageItem> _bananaItems = new Queue<GarbageItem>();
    private readonly Queue<GarbageItem> _plasticBottleItems = new Queue<GarbageItem>();

    private float _spawnRate = 2.5f;
    private GarbageItem _fiberItem;
    private GarbageItem _bananaItem;
    private GarbageItem _plasticBottleItem;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
        GameManager.OnItemSpeedChanged += IncreaseSpawnRate;
        _spawnRate = 2.5f;
    }
    
    void IncreaseSpawnRate(float newSpeed)
    {
        if (_spawnRate > .5f)
        {
            _spawnRate -= .25f;
        }
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
                _spawnRate = 2.5f;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void CleanUpSpawnedItems()
    {
        var otherItems = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);
        _fiberItems.Clear();
        _bananaItems.Clear();
        _plasticBottleItems.Clear();
        
        foreach (var item in otherItems ?? Enumerable.Empty<GarbageItem>())
        {
            if (item is not null)
            {
                Destroy(item.gameObject);
            }
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
        //Debug.Log("Start");
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
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    void SpawnItem()
    {
        int itemSpawnIndex = UnityEngine.Random.Range(1, 4);
        GarbageItem spawnedItem;
        
        
        switch (itemSpawnIndex)
        {
            case 1:
                spawnedItem = Instantiate(banana, _itemSpawnPosition, Quaternion.identity);
                _bananaItems.Enqueue(spawnedItem);
                break;
            case 2:
                spawnedItem = Instantiate(plasticBottle, _itemSpawnPosition, Quaternion.identity);
                _plasticBottleItems.Enqueue(spawnedItem);
                break;
            case 3:
                spawnedItem = Instantiate(crumpledPaper, _itemSpawnPosition, Quaternion.identity);
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
                bool itemAdded = false;
                
                while (!itemAdded)
                {
                    if (!_bananaItems.Any())
                    {
                        return;
                    }
                    
                    _bananaItem = _bananaItems.Dequeue();
                    
                    if (_bananaItem is not null)
                    {
                        itemAdded = true;
                    }
                }

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
                bool itemAdded = false;

                while (!itemAdded)
                {
                    if (!_fiberItems.Any())
                    {
                        return;
                    }
                    
                    _fiberItem = _fiberItems.Dequeue();
                    
                    if (_fiberItem is not null)
                    {
                        itemAdded = true;
                    }
                }
                
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
                bool itemAdded = false;
                
                while (!itemAdded)
                {
                    if (!_plasticBottleItems.Any())
                    {
                        return;
                    }
                    
                    _plasticBottleItem = _plasticBottleItems.Dequeue();
                    
                    if (_plasticBottleItem is not null)
                    {
                        itemAdded = true;
                    }
                }

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
