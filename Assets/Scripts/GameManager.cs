﻿using UnityEngine;

public enum GameState {LOADING, START, PLAYING, GAME_OVER, PAUSE, CONTINUE, WIN}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentState;

    public delegate void ChangeState();
    public ChangeState ChangeStateEvent;

    void Awake()
    {
        instance = this;
        currentState = GameState.START;
        ChangeStateEvent += NewStateEvent;
    }

    public void ChangeToNewState(GameState newState)
    {
        currentState = newState;
        ChangeStateEvent();
    }

    void NewStateEvent()
    {
        Debug.Log("New game state: " + currentState);
    }
}
