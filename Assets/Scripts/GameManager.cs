using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;
    
    public TMP_Text scoreText;
    public GameObject startText;
    private const float defaultItemSpeed = -2f;
    private float itemSpeed = defaultItemSpeed;
    
    public float CurrentSpeed => itemSpeed;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<float> OnItemSpeedChanged;

    public enum GameState
    {
        WaitingToStart,
        Playing,
    }

    private const int maxFailuresInRow = 5;
    private const int maxSuccessesInRow = 5;

    private int _score;
    public int Score => _score;

    private int _successesInRow;
    private int _failuresInRow;

    private void Start()
    {
        Instance = this;
        scoreText = (TMP_Text)GameObject.Find("Score").GetComponent(typeof(TMP_Text));
        startText = GameObject.Find("StartText");
        UpdateGameState(GameState.WaitingToStart);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.WaitingToStart:
                startText.SetActive(true);
                itemSpeed = defaultItemSpeed;
                break;
            case GameState.Playing:
                startText.SetActive(false);
                _score = 0;
                scoreText.text = _score.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        OnGameStateChanged?.Invoke(newState);
    }

    public void UpdateScore()
    {
        _score++;
        scoreText.text = _score.ToString();
        _successesInRow++;
        if (_successesInRow >= maxSuccessesInRow)
        {
            _successesInRow = 0;
            UpdateGameSpeed();
        }
    }

    public void Failure()
    {
        _failuresInRow++;
        Debug.Log("Failures: " + _failuresInRow.ToString());
        if (_failuresInRow >= maxFailuresInRow)
        {
            _failuresInRow = 0;
            UpdateGameState(GameState.WaitingToStart);
        }
    }

    public void UpdateGameSpeed()
    {
        itemSpeed -= 0.5f;
        
        OnItemSpeedChanged?.Invoke(itemSpeed);
    }
    
}
