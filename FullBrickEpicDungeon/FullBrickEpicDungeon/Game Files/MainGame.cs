﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace FullBrickEpicDungeon
{
    /// <summary>
    /// Main Game file of our game
    /// </summary>
    public class DungeonCrawler : GameEnvironment
    {

        public static bool SFX = true, music = true, mouseVisible = true;
        //SpriteGameObject conversationFrame;
        public DungeonCrawler()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the screen and applies the correct resolution and adds all gamestates to the gamestate manager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            screen = new Point(1900, 1050);
            windowSize = new Point(1280, 720);
            FullScreen = false;
            ApplyResolutionSettings();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            gameStateManager.AddGameState("titleMenu", new TitleMenuState());
            GameStateManager.AddGameState("settingsState", new SettingsState());
            gameStateManager.AddGameState("characterSelection", new CharacterSelection());

            //Weghalen als characterselection eenmaal definitief aan staat, anders maakt hij al deze dingen opnieuw aan TO DO
            //Kan niet in plaats van in Characterselection hier aangemaakt worden, omdat dan de keuze niet goed wordt doorgegeven aan playingstate
            //als ik daar alleen playingstate zou zetten, is de volgorde niet meer goed.
            /*
            gameStateManager.AddGameState("playingState", new PlayingState()); //Deze VERWIJDEREN ALS CHAR SELECTION IS GEACTIVEERD
            gameStateManager.AddGameState("levelFinishedState", new LevelFinishedState());
            GameStateManager.AddGameState("pauseState", new PauseState());
            gameStateManager.AddGameState("conversation", new ConversationState());
            gameStateManager.AddGameState("cutscene", new CutsceneState());*/

            gameStateManager.SwitchTo("titleMenu");
        }

        /// <summary>
        /// Load the content for the game
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Unloads Content
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Update for the DungeonCrawler game, checks for mouse visiblity and has the ability to exit the game if the ESC key is pressed
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // If the global mousevisibility variable changed, update it accordingly.
            if (mouseVisible)
                IsMouseVisible = true;
            else
                IsMouseVisible = false;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        
    }
}
