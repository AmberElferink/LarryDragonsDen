﻿using System;
using Microsoft.Xna.Framework;

class LittlePenguin : AImonster
{
    float slideSpeed;
    Vector2 targetPos, movementVector;
    int animationCheck = 0;
    float idleCounter;
    /// <summary>
    /// Class that defines a Penguin that slides around
    /// </summary>
    /// <param name="currentLevel">current level the penguin is in</param>
    public LittlePenguin(Level currentLevel) : base(0f, currentLevel, "LittlePenguin", 800)
    {
        this.baseattributes.HP = 80;
        this.baseattributes.Armour = 0;
        this.baseattributes.Attack = 20;
        this.baseattributes.Gold = 75;
        attributes.HP = baseattributes.HP;
        attributes.Armour = baseattributes.Armour;
        attributes.Attack = baseattributes.Attack;
        attributes.Gold = baseattributes.Gold;

        //The total speed of movement of the penguin
        slideSpeed = 10f;

        //Because the penguin will be idle for a while after sliding, the movement vector will have to be set (0,0) from time to time
        movementVector = new Vector2(0, 0);
        idleCounter = 3;
        //Load the animations
        LoadAnimation("Assets/Sprites/Enemies/PenguinSide@4", "sideWalk", true, 0.2f);
        LoadAnimation("Assets/Sprites/Enemies/PenguinFront@4", "sideFront", true, 0.2f);
        LoadAnimation("Assets/Sprites/Enemies/PenguinBack@4", "sideBack", true, 0.2f);
        LoadAnimation("Assets/Sprites/Enemies/PenguinSlideUp@4", "slideUp", true, 0.2f);
        LoadAnimation("Assets/Sprites/Enemies/PenguinSlideSide@4", "slideSide", true, 0.2f);
        LoadAnimation("Assets/Sprites/Enemies/PenguinSlideDown@4", "slideDown", true, 0.2f);

        PlayAnimation("sideFront");
    }

    /// <summary>
    /// Method that updates the penguin
    /// </summary>
    /// <param name="gameTime">current game time</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (AI.CurrentTarget != null && idleCounter <= 0 && movementVector == new Vector2(0, 0))
            SlideDirection();
        else if (movementVector != new Vector2(0, 0))
        {
            previousPos = Position;
            Position += movementVector;

            if(Math.Abs(movementVector.X) > Math.Abs(movementVector.Y))
            {
                PlayAnimation("slideSide");
                animationCheck = 3;
                if (movementVector.X > 0)
                    Mirror = false;
                else
                    Mirror = true;
            }
            else
            {
                if (movementVector.Y > 0)
                {
                    PlayAnimation("slideDown");
                    animationCheck = 2;
                }
                else
                {
                    PlayAnimation("slideUp");
                    animationCheck = 1;
                }
            }
            CheckCollision();
        }
        else if (idleCounter > 0)
        {
            idleCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationCheck == 1)
                PlayAnimation("sideFront");
            else if (animationCheck == 2)
                PlayAnimation("sideBack");
            else
                PlayAnimation("sideWalk");
        }
    }

    //The penguin is special in the way that it attacks when it is sliding from 1 point to another.
    //This means that the attack method for the penguin is obsolete. The AttackHit, however, is still important.
    public override void Attack()
    {

    }

    /// <summary>
    /// Collision checker for the Penguin
    /// </summary>
    public void CheckCollision()
    {
        //Take the grid to check for all tiles that are solid or doors, and player list for collision with players
        GameObjectGrid field = GameWorld.Find("TileField") as GameObjectGrid;
        GameObjectList players = GameWorld.Find("playerLIST") as GameObjectList;

        Rectangle tileBoundingBox = new Rectangle((int)this.BoundingBox.X, (int)(this.BoundingBox.Y + 0.75 * Height), this.Width, (int)(this.Height / 4));
        //First check if monster collides with player
        foreach (Character player in players.Children)
            if (this.BoundingBox.Intersects(player.BoundingBox) && !playersHit.Contains(player))
                AttackHit(player);

        //Then check if monster collides with solid tile
        foreach (Tile tile in field.Objects)
        {
            if (tile.IsSolid && tileBoundingBox.Intersects(tile.BoundingBox))
            {
                movementVector = new Vector2(0, 0);
                Position = previousPos;
                idleCounter = 3;
                // TargetRandomObject (currently removed but might add back later, but as a local method?
            }
        }
    }

    float totalDistance;
    
    /// <summary>
    /// Method that calculates the sliding direction
    /// </summary>
    public void SlideDirection()
    {

        //Take the position of the target
        GameObjectList players = GameWorld.Find("playerLIST") as GameObjectList;
        foreach(Character player in players.Children)
            if(player == AI.CurrentTarget)
            {
                targetPos = player.Position;
            }

        Vector2 differencePos = targetPos - position;
        movementVector = GetMovementVector(differencePos);
    }

    /// <summary>
    /// Method that calculates the movement vector of the penguin
    /// </summary>
    /// <param name="difference">Amount of distance between the target and the penguin</param>
    /// <returns></returns>
    public Vector2 GetMovementVector(Vector2 difference)
    {
        totalDistance = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
        float scaling = slideSpeed / totalDistance;

        return new Vector2(difference.X * scaling, difference.Y * scaling);
    }


}

