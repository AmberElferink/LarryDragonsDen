﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

class PlayingState : IGameLoopObject
{
    // level list
    protected Level[] levelArray;
    private TimeSpan levelTimer;
    // current level that is being used
    protected int currentLevelIndex;

    /// <summary>
    /// A state that defines the playing state of the game
    /// </summary>
    public PlayingState()
    {
        currentLevelIndex = 13;
        levelArray = new Level[14]; //10 levels
    }

    /// <summary>
    /// Handles input, making it able for the player to pause the game
    /// </summary>
    public void HandleInput(InputHelper inputHelper)
    {
        CurrentLevel.HandleInput(inputHelper);
        
        if (inputHelper.KeyPressed(Keys.Space) || inputHelper.AnyPlayerPressed(Buttons.Start))
        {
            if(FullBrickEpicDungeon.DungeonCrawler.SFX)
                GameEnvironment.AssetManager.PlaySound("Assets/SFX/pause");

            GameEnvironment.GameStateManager.SwitchTo("pauseState");
        }
    }

    public void Initialize()
    {
        FullBrickEpicDungeon.DungeonCrawler.mouseVisible = false;
    }


    public void Reset()
    {
        levelTimer = TimeSpan.Zero;
        Level newlevel = new Level(currentLevelIndex);
        levelArray[currentLevelIndex] = newlevel;
    }

    public void Update(GameTime gameTime)
    {
        levelTimer += gameTime.ElapsedGameTime;
        CurrentLevel.Update(gameTime);
    }
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        CurrentLevel.Draw(gameTime, spriteBatch);
    }



    public void GoToNextLevel()
    {
        CurrentLevel.Reset();
        if (currentLevelIndex >= levelArray.Length - 1)
        {
            ResetLevelIndex();
            // if all the levels are over switch to the title state
            GameEnvironment.GameStateManager.SwitchTo("titleMenu");
        }
        else
        {
            currentLevelIndex++;
        }
    }

    public void ResetLevelIndex()
    {
        currentLevelIndex = 2;
    }

    // returns the current level being used in the state
    public Level CurrentLevel
    {
        get { return levelArray[currentLevelIndex]; }
    }

    public TimeSpan LevelTime
    {
        get { return levelTimer; }
        set { levelTimer = value; }
    }

}
