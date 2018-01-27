﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameStateManager : IGameLoopObject
{
    Dictionary<string, IGameLoopObject> gameStates;
    IGameLoopObject currentGameState;
    IGameLoopObject previousGameState;

    public GameStateManager()
    {
        gameStates = new Dictionary<string, IGameLoopObject>();
        currentGameState = null;
    }

    public void AddGameState(string name, IGameLoopObject state)
    {
        gameStates[name] = state;
    }

    public IGameLoopObject GetGameState(string name)
    {
        return gameStates[name];
    }

    public void SwitchTo(string name)
    {
        if (gameStates.ContainsKey(name))
        {
            previousGameState = currentGameState;
            currentGameState = gameStates[name];
            currentGameState.Initialize();
        }
        else
        {
            throw new KeyNotFoundException("Could not find game state: " + name);
        }
    }

    public IGameLoopObject CurrentGameState
    {
        get
        {
            return currentGameState;
        }
    }

    public IGameLoopObject PreviousGameState
    {
        get
        {
            return previousGameState;
        }
    }

    public void HandleInput(InputHelper inputHelper)
    {
        if (currentGameState != null)
        {
            currentGameState.HandleInput(inputHelper);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (currentGameState != null)
        {
            currentGameState.Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (currentGameState != null)
        {
            currentGameState.Draw(gameTime, spriteBatch);
        }
    }

    public void Initialize() { }

    public void Reset()
    {
        if (currentGameState != null)
        {
            currentGameState.Reset();
        }
    }
}
