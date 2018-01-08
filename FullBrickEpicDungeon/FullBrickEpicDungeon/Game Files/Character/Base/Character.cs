﻿using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

abstract partial class Character : AnimatedGameObject
{
    //baseattributes contains the standard base stats and should not be changed, the values in attributes may be changes are used during the remainder of the level
    protected ClassType classType;
    protected BaseAttributes attributes, baseattributes;
    protected Weapon weapon;
    protected List<Equipment> inventory;
    protected Timer reviveTimer;
    protected Vector2 startPosition, movementSpeed, iceSpeed;
    protected int playerNumber;

    protected Dictionary<Keys, Keys> keyboardControls;
    protected Dictionary<Buttons, Buttons> xboxControls;
    protected bool keyboardControlled;
    protected bool xboxControlled = false;
    
    protected bool isOnIce = false, hasAKey = false;
    protected bool blockinput = false;
    Vector2 walkingdirection;

    //Constructor: sets up the controls given to the constructor for each player (xbox or keyboard)
    protected Character(int playerNumber, bool controllerControlled, ClassType classType, string baseAsset, string id = "") : base(0, id)
    {

        this.classType = classType;
        baseattributes = new BaseAttributes();
        inventory = new List<Equipment>();
        attributes = new BaseAttributes();
        reviveTimer = new Timer(10);
        this.velocity = Vector2.Zero;
        this.movementSpeed = new Vector2(3, 3);
        this.playerNumber = playerNumber;

        if (playerNumber == 1)
        {
            this.keyboardControlled = true; //player 1 en 2 kunnen naast xbox controls ook altijd nog op keyboard spelen
            this.xboxControlled = false;

            if (this.keyboardControlled) //opgeslagen controls staan in de txt bestandjes
                keyboardControls = GameEnvironment.SettingsHelper.GenerateKeyboardControls("Assets/KeyboardControls/player1controls.txt");
            if (this.xboxControlled)
                xboxControls = GameEnvironment.SettingsHelper.GenerateXboxControls("Assets/KeyboardControls/XboxControls/player1Xbox.txt");
        }
        else if (playerNumber == 2)
        {
            this.keyboardControlled = true; //player 1 en 2 kunnen naast xbox controls ook altijd nog op keyboard spelen
            this.xboxControlled = false;
            if (this.keyboardControlled)
                keyboardControls = GameEnvironment.SettingsHelper.GenerateKeyboardControls("Assets/KeyboardControls/player2controls.txt");
            if (this.xboxControlled)
                xboxControls = GameEnvironment.SettingsHelper.GenerateXboxControls("Assets/KeyboardControls/XboxControls/player2Xbox.txt");
        }
        else if (playerNumber == 3)
        {
            if (this.keyboardControlled)
                throw new ArgumentOutOfRangeException("Only Player 1 and 2 can play with a keyboard");
            if (this.xboxControlled)
                xboxControls = GameEnvironment.SettingsHelper.GenerateXboxControls("Assets/KeyboardControls/XboxControls/player3Xbox.txt");
        }
        else if (playerNumber == 4)
        {
            if (this.keyboardControlled)
                throw new ArgumentOutOfRangeException("Only Player 1 and 2 can play with a keyboard");
            if (this.xboxControlled)
                xboxControls = GameEnvironment.SettingsHelper.GenerateXboxControls("Assets/KeyboardControls/XboxControls/player4Xbox.txt");
        }

        this.xboxControlled = controllerControlled;
        this.iceSpeed = new Vector2(0, 0);
    }

    //Method for character input (both xbox controller and keyboard), for now dummy keys for 1 controller are inserted, but the idea should be clear
    //TO DO: a way to distinguish characters / players from each other.
    public override void HandleInput(InputHelper inputHelper)
    {
        Vector2 previousPosition = this.position;

        if (!IsDowned && !isOnIce && !blockinput)
        {
            velocity = Vector2.Zero;
            //Input keys for basic AA and abilities

            if (keyboardControlled)
            {
                HandleKeyboardInput(inputHelper);
            }
            if (xboxControlled)
            {
                HandleInputXboxController(inputHelper);
            }

        }
        // NOTE: the Ice method has to be updated to account for XBOX controls, maybe with a ||, but this will be a problem as keyboardcontrols will be null if a controller is used
        else if (!IsDowned && isOnIce)
        {
            KeyboardHandleIceMovement(inputHelper);
        }

        if (!SolidCollisionChecker())
        {
            this.iceSpeed = new Vector2(0, 0);
            this.position = previousPosition;
            PlayAnimation("idle");
            blockinput = false;
        }

        base.HandleInput(inputHelper);

    }

    // Method that handles keyboard movement and input
    public void HandleKeyboardInput(InputHelper inputHelper)
    {
        if (inputHelper.KeyPressed(keyboardControls[Keys.Q]))
            this.weapon.Attack(GameWorld.Find("monsterLIST") as GameObjectList);
        if (inputHelper.KeyPressed(keyboardControls[Keys.R]))
            this.weapon.UseMainAbility(GameWorld.Find("monsterLIST") as GameObjectList);
        if (inputHelper.KeyPressed(keyboardControls[Keys.T]))
            this.weapon.UseSpecialAbility(GameWorld.Find("monsterLIST") as GameObjectList);

        //schuin linksboven
            if (inputHelper.IsKeyDown(keyboardControls[Keys.W]))
            {
                if (inputHelper.IsKeyDown(keyboardControls[Keys.A]))
                {
                    walkingdirection = MovementVector(this.movementSpeed, 225);
                }
                else if (inputHelper.IsKeyDown(keyboardControls[Keys.D]))
                {
                    walkingdirection = MovementVector(this.movementSpeed, 315);
                }
                else
                {
                    walkingdirection = MovementVector(this.movementSpeed, 270);
                }

            }
        //schuin rechtsboven
            else if (inputHelper.IsKeyDown(keyboardControls[Keys.S]))
            {
                if (inputHelper.IsKeyDown(keyboardControls[Keys.A]))
                {
                    walkingdirection = MovementVector(this.movementSpeed, 135);
                }
                else if (inputHelper.IsKeyDown(keyboardControls[Keys.D]))
                {
                    walkingdirection = MovementVector(this.movementSpeed, 45);

                }
                else
                {
                    walkingdirection = MovementVector(this.movementSpeed, 90);
                }
            }      
        //naar links
             else if (inputHelper.IsKeyDown(keyboardControls[Keys.A]))
            {
                walkingdirection = MovementVector(this.movementSpeed, 180);
            }
        //naar rechts
            else if (inputHelper.IsKeyDown(keyboardControls[Keys.D]))
            {
                walkingdirection = MovementVector(this.movementSpeed, 0);
            }
        //update position and animation
            this.position += walkingdirection;
            PlayAnimationDirection(walkingdirection);
            walkingdirection = Vector2.Zero;

            if (inputHelper.IsKeyDown(keyboardControls[Keys.E])) //Interact key
            {
                ObjectCollisionChecker();
            }
    }



    //handles xbox controller walking and input
    private void HandleInputXboxController(InputHelper inputHelper)
    {
        if (xboxControls != null) //xboxcontrols zijn niet ingeladen, dus wordt niet door xboxcontroller bestuurd.
        {
            if (inputHelper.ControllerConnected(playerNumber)) //check of controller connected is
            {
                //Attack and Main Ability
                if (inputHelper.ButtonPressed(playerNumber, Buttons.A))
                    this.weapon.Attack(GameWorld.Find("monsterLIST") as GameObjectList);
                if (inputHelper.ButtonPressed(playerNumber, Buttons.B))
                    this.weapon.UseMainAbility(GameWorld.Find("monsterLIST") as GameObjectList);

                //Interact button
                if (inputHelper.ButtonPressed(playerNumber, Buttons.X))
                    ObjectCollisionChecker();

                //Movement
                walkingdirection = inputHelper.WalkingDirection(playerNumber) * this.movementSpeed;
                walkingdirection.Y = -walkingdirection.Y;
                this.position += walkingdirection;

                PlayAnimationDirection(walkingdirection);
                walkingdirection = Vector2.Zero;

            }
        }
    }




    // Method that handles keyboard movement when the character is on ice
    public void KeyboardHandleIceMovement(InputHelper inputHelper)
    {
        if (blockinput)
        {
            if (this.iceSpeed != new Vector2(0, 0))
                this.position += iceSpeed;
        }
        else
        {
            if (inputHelper.IsKeyDown(keyboardControls[Keys.W]))
            {
                blockinput = true;
                iceSpeed = MovementVector(this.movementSpeed * 2, 270);
            }
            else if (inputHelper.IsKeyDown(keyboardControls[Keys.A]))
            {
                blockinput = true;
                iceSpeed = MovementVector(this.movementSpeed * 2, 180);
            }
            else if (inputHelper.IsKeyDown(keyboardControls[Keys.S]))
            {
                blockinput = true;
                iceSpeed = MovementVector(this.movementSpeed * 2, 90);
            }
            else if (inputHelper.IsKeyDown(keyboardControls[Keys.D]))
            {
                blockinput = true;
                iceSpeed = MovementVector(this.movementSpeed * 2, 0);
            }
            PlayAnimationDirection(iceSpeed);
        }
    }

    // Calculates the new movementVector for a character for keyboard
    public Vector2 MovementVector(Vector2 movementSpeed, float angle)
    {
        float adjacent = movementSpeed.X;
        float opposite = movementSpeed.Y;

        float hypotenuse = (float)Math.Sqrt(adjacent * adjacent + opposite * opposite);
        adjacent = (float)Math.Cos(angle * (Math.PI / 180)) * hypotenuse;
        opposite = (float)Math.Sin(angle * (Math.PI / 180)) * hypotenuse;

        return new Vector2(adjacent, opposite);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        this.weapon.Update(gameTime);
        MonsterCollisionChecker();
        IsOnIceChecker();
        if (IsDowned)
        {
            reviveTimer.IsPaused = false;
            if (reviveTimer.IsExpired)
            {
                this.Reset();
                // when the revivetimer expires, the character dies :( sadly he will lose some of his gold after dying (currently 25% might be higher in later versions)
                this.attributes.Gold = this.attributes.Gold - (this.attributes.Gold / 4);
            }
        }
    }


   
    
    public override void Reset()
    {
        this.attributes.HP = this.baseattributes.HP;
        this.position = startPosition;
    }


    // Transfers money to another character
    public void TransferGold(int amount, Character target)
    {
        if (target == this)
        {
            this.attributes.Gold += amount;
        }
        else
        {
            this.attributes.Gold -= amount;
            target.attributes.Gold += amount;
        }
    }

    // Checks if the Character collides with monsters
    public void MonsterCollisionChecker()
    {
        GameObjectList monsterList = GameWorld.Find("monsterLIST") as GameObjectList;
        // TODO: Add Tilefield collision with walls puzzles etc, (not doable atm as it isn't programmed as of writing this)
        foreach (Monster monsterobj in monsterList.Children)
        {
            if (monsterobj.CollidesWith(this))
            {
                this.TakeDamage(monsterobj.Attributes.Attack);
            }
        }
    }

    //Checks if the character collides with interactive objects
    public void ObjectCollisionChecker()
    {
        GameObjectList objectList = GameWorld.Find("objectLIST") as GameObjectList;
        // If a character collides with an interactive object, set the target character to this instance and tell the interactive object that it is currently interacting

        foreach (var intObj in objectList.Children)
        {
            if (intObj is Trap)
                continue;
            //hierboven kun je nog niet naar InteractiveObject vragen omdat je anders een casting error krijgt bij andere objecten waar je mee interact.
            else if (intObj is InteractiveObject)
            {
                if (((InteractiveObject)intObj).CollidesWith(this))
                {
                    ((InteractiveObject)intObj).TargetCharacter = this;
                    ((InteractiveObject)intObj).IsInteracting = true;

                    if (intObj is KeyItem)
                    {
                        HasAKey = true;
                        intObj.Position = this.Position;
                    }
                }
            }
        }
    }

    //Dikke collision met muren/andere solid objects moet ervoor zorgen dat de player niet verder kan bewegen.
    public bool SolidCollisionChecker()
    {
        GameObjectGrid Field = GameWorld.Find("TileField") as GameObjectGrid;
        Rectangle quarterBoundingBox = new Rectangle((int)this.BoundingBox.X, (int)(this.BoundingBox.Y + 0.75 * Height), this.Width, (int)(this.Height / 4));
        foreach (Tile tile in Field.Objects)
        {
            if (tile.IsSolid && quarterBoundingBox.Intersects(tile.BoundingBox))
            {
                return false;
            }

            if (tile.IsDoor && quarterBoundingBox.Intersects(tile.BoundingBox) && tile.Sprite.SheetIndex == 0)
                return false;
        }
        return true;
    }

    public void IsOnIceChecker()
    {
        GameObjectGrid Field = GameWorld.Find("TileField") as GameObjectGrid;
        Rectangle feetBoundingBox = new Rectangle((int)(this.BoundingBox.X + 0.33 * Width), 
            (int)(this.BoundingBox.Y + 0.9 * Height), (int)(this.Width / 3), (Height/10));
        foreach (Tile tile in Field.Objects)
        {
            if (tile.IsIce && tile.BoundingBox.Intersects(feetBoundingBox))
            {
                isOnIce = true;
                return;
            }
       }
        isOnIce = false;
        blockinput = false;
    }

    // Changes the weapon of a Character and drops the weapon on the ground
    public void ChangeWeapon(Weapon newweapon)
    {
        DroppedItem droppedWeapon = new DroppedItem(this.weapon, "DROPPED" + weapon.Id);
        this.weapon = newweapon;
    }

    // Changes items in the characters inventory, also allows to remove it
    public void ChangeItems(Equipment item, bool remove = false)
    {
        if (item == null)
        {
            return;
        }

        if (remove)
        {
            try
            {
                inventory.Remove(item);
            }
            catch
            {
                throw new ArgumentOutOfRangeException("No such item was found in " + this.classType + "'s inventory!");
            }
        }
        else
        {
            inventory.Add(item);
        }

    }

    // Checks if a character owns an item (only for equipment)
    public bool OwnsItem(Equipment item)
    {
        return inventory.Contains(item);
    }

    public void TakeDamage(int damage)
    {
        int totalitemdefense = 0;
        foreach (Equipment item in inventory)
        {
            totalitemdefense += item.Armour;
        }
        int takendamage = (damage - (int)(0.3F * this.attributes.Armour + totalitemdefense));
        if (takendamage < 5)
        {
            takendamage = 0;
        }
        this.attributes.HP -= takendamage;
        if (this.attributes.HP < 0)
        {
            this.attributes.HP = 0;
        }
    }

    //when called with the walkingdirection, it plays the correct animation with the movement.
    public void PlayAnimationDirection(Vector2 walkingdirection)
    {
        if (Math.Abs(walkingdirection.X) >= Math.Abs(walkingdirection.Y))
        {
            if (walkingdirection.X > 0)
            {
                this.PlayAnimation("rightcycle");
                this.Mirror = true;
            }
            else if (walkingdirection.X < 0)
            {
                this.PlayAnimation("leftcycle");
                this.Mirror = false;
            }
            else
                this.PlayAnimation("idle");
        }
        else if (Math.Abs(walkingdirection.Y) > Math.Abs(walkingdirection.X))
        {
            if(walkingdirection.Y > 0)
            {
                this.PlayAnimation("frontcycle");
            }
            else if(walkingdirection.Y < 0)
            {
                this.PlayAnimation("backcycle");
            }
            else
                this.PlayAnimation("idle");

        }
        else
            this.PlayAnimation("idle");

    }


    // returns if the character has gone into the "downed" state
    public bool IsDowned
    {
        get { return this.attributes.HP == 0; }
    }

    public bool HasAKey
    {
        get { return hasAKey; }
        set { hasAKey = value; }
    }
    // returns the startPosition of the character. Will be newly set when entering a level
    public Vector2 StartPosition
    {
        get { return startPosition; }
        set { startPosition = value; }
    }
    public Vector2 MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = value; }
    }
    // returns the weapon of the character
    public Weapon CurrentWeapon
    {
        get { return weapon; }
        set { weapon = value; }
    }
    // returns the attributes of the character
    public BaseAttributes Attributes
    {
        get { return attributes; }
        set { attributes = value; }
    }

    public Dictionary<Keys, Keys> KeyboardControlScheme
    {
        get { return keyboardControls; }
        set { keyboardControls = value; }
    }

    public ClassType Type
    {
        get { return classType; }
    }

    public int PlayerNumber
    {
        get { return playerNumber; }
    }
    // returns the facing direction of the character
}