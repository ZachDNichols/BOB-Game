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

    public static event Action<GameState> OnGameStateChanged;

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
        UpdateGameState(GameState.WaitingToStart);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.WaitingToStart:
                break;
            case GameState.Playing:
                _score = 0;
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
        Debug.Log("Successes: " + _successesInRow.ToString() + "\nScore: " + _score.ToString());
        if (_successesInRow >= maxSuccessesInRow)
        {
            _successesInRow = 0;
            Spawner.Instance.itemSpeed -= 3f;
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
    
}
