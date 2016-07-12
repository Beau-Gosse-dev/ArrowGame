using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Mangers;

public class Arrow : MonoBehaviour
{
    Rigidbody2D arrow;
    public AimLineFriend aimLine;
    public Transform playerLeftTransform;
    public Transform playerRightTransform;
    public Player playerLeft;
    public Player playerRight;
    private const float arrowPlayerXOffset = 1.7f;
    private const float arrowPlayerYOffset = .5f;
    public CameraFollow cameraFollow;
    public List<Arrow> arrowsShot = new List<Arrow>();

    private const int BodyDamageAmount = 55;
    private const int HeadDamageAmount = 100;
    void Awake()
    {
        arrow = gameObject.GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    void Start()
    {
        //arrow = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set rotation to where the arrow is moving if it is moving
        if (arrow.velocity.x != 0 || arrow.velocity.y != 0)
        {
            var dir = arrow.velocity;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 50 * Time.deltaTime);
        }
    }

    public void SetRotation(float angle)
    {
        if (!arrow.isKinematic)
        {
            GetComponent<Rigidbody2D>().rotation = angle;
        }
    }

    public void Shoot(Vector2 startPoint, Vector2 endPoint)
    {
        if (!arrow.isKinematic)
        {
            arrow.gravityScale = 1;
            arrow.AddForce(new Vector2((startPoint.x - endPoint.x) * 3, (startPoint.y - endPoint.y) * 3), ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!arrow.isKinematic)
        {
            #region gameTypeComputer
            if (LevelDefinition.gameType == GameType.Computer)
            {
                // Tell the aim line where it hit, so if it was the computer's turn, it can update it's aim
                aimLine.ArrowCollision(transform.position.x, transform.position.y, col.gameObject.name.StartsWith("Brick"), this.GetComponent<Rigidbody2D>().transform.rotation);

                // Create new arrow to stay where this hit
                // Set this arrow to no movement and kinematic before cloning, the reset this arrow
                this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                this.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                this.gameObject.GetComponent<Collider2D>().enabled = false;
                Arrow newArrowObject = Instantiate(this);
                this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                this.gameObject.GetComponent<Collider2D>().enabled = true;
                // Save the new arrow to our array of gameObjects (so we can destroy them)
                arrowsShot.Add(newArrowObject);
                // Save the new arrow to the level definition to save to the server
                LevelDefinition.ShotArrows.Add(
                    new ShotArrow(newArrowObject.GetComponent<Rigidbody2D>().rotation, newArrowObject.transform.position.x, newArrowObject.transform.position.y)
                    );

                // Move the active arrow to the new reset position
                arrow.velocity = new Vector2(0f, 0f);
                arrow.angularVelocity = 0f;
                arrow.gravityScale = 0;

                if (col.gameObject.name == "Head")
                {
                    if (col.gameObject.transform.parent.gameObject.name == "PlayerLeft")
                    {
                        playerLeft.Hit(HeadDamageAmount);
                    }
                    else if (col.gameObject.transform.parent.gameObject.name == "PlayerRight")
                    {
                        playerRight.Hit(HeadDamageAmount);
                    }
                }
                else if (col.gameObject.name == "Body")
                {
                    if (col.gameObject.transform.parent.gameObject.name == "PlayerLeft")
                    {
                        playerLeft.Hit(BodyDamageAmount);
                    }
                    else if (col.gameObject.transform.parent.gameObject.name == "PlayerRight")
                    {
                        playerRight.Hit(BodyDamageAmount);
                    }
                }
                else if (LevelDefinition.RebuttleTextEnabled) // A body part wasn't hit, so if we were in rebuttal, right lost
                {
                    // If the rebuttal wasn't successful, just kill right player, he will handle removing the rebuttle text and logging.
                    playerRight.Hit(100);
                }

                // Reset the arrow location on hit
                ResetPosition(LevelDefinition.IsPlayerLeftTurn);

                // Allow another shot
                aimLine.ArrowHit(col);
                
            } // End If Computer
            #endregion gameTypeComputer
            #region gameTypeHuman
            else if(LevelDefinition.gameState == GameState.ShowLastMove)
            {
                // Since this is just showing the last shot, don't add a new arrow.                
                // Reset the arrow location on hit
                ResetPosition(!LevelDefinition.IsPlayerLeftTurn);

                LevelDefinition.gameState = GameState.Playing;

                // Allow another shot
                aimLine.ArrowHit(col);
            }
            else // Not Computer, so Human vs Human
            {
                // Create new arrow to stay where this hit
                // Set this arrow to no movement and kinematic before cloning, the reset this arrow
                this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                this.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                this.gameObject.GetComponent<Collider2D>().enabled = false;
                Arrow newArrowObject = Instantiate(this);
                this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                this.gameObject.GetComponent<Collider2D>().enabled = true;
                // Save the new arrow to our array of gameObjects (so we can destroy them)
                arrowsShot.Add(newArrowObject);
                // Save the new arrow to the level definition to save to the server
                LevelDefinition.ShotArrows.Add(
                    new ShotArrow(newArrowObject.GetComponent<Rigidbody2D>().rotation, newArrowObject.transform.position.x, newArrowObject.transform.position.y)
                    );

                if (col.gameObject.name == "Head")
                {
                    if (col.gameObject.transform.parent.gameObject.name == "PlayerLeft")
                    {
                        playerLeft.Hit(HeadDamageAmount);
                    }
                    else if (col.gameObject.transform.parent.gameObject.name == "PlayerRight")
                    {
                        playerRight.Hit(HeadDamageAmount);
                    }
                }
                else if (col.gameObject.name == "Body")
                {
                    if (col.gameObject.transform.parent.gameObject.name == "PlayerLeft")
                    {
                        playerLeft.Hit(BodyDamageAmount);
                    }
                    else if (col.gameObject.transform.parent.gameObject.name == "PlayerRight")
                    {
                        playerRight.Hit(BodyDamageAmount);
                    }
                }
                else if (LevelDefinition.RebuttleTextEnabled) // A body part wasn't hit, so if we were in rebuttal, right lost
                {
                    // If the rebuttal wasn't successful, just kill right player, he will handle removing the rebuttle text and logging.
                    playerRight.Hit(100);
                }

                // Reset the arrow location on hit
                ResetPosition(LevelDefinition.IsPlayerLeftTurn);

                // Allow another shot
                aimLine.ArrowHit(col);

                // Upload the new state to Parse so when the next player comes they can play now.
                LevelDefinition.saveCurrentToServer();
            }

            #endregion gameTypeHuman
            cameraFollow.PauseInSeconds(1f);
        }
    }
    

    internal void AddShotArrows(List<ShotArrow> shotArrows)
    {
        // Set this arrow to no movement and kinematic before cloning, then reset this arrow
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        foreach (ShotArrow newShotArrow in shotArrows)
        {
            Arrow newArrowObject = Instantiate(this);
            newArrowObject.transform.position = new Vector3(newShotArrow.X, newShotArrow.Y, 0);
            newArrowObject.GetComponent<Rigidbody2D>().rotation = newShotArrow.Angle;
            arrowsShot.Add(newArrowObject);
        }
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        this.gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void ResetPosition(bool IsPlayerLeftTurn)
    {
        arrow.velocity = new Vector2(0f, 0f);
        arrow.angularVelocity = 0f;
        arrow.gravityScale = 0;
        if ((IsPlayerLeftTurn && LevelDefinition.gameState!=GameState.ShowLastMove) || (!IsPlayerLeftTurn && LevelDefinition.gameState == GameState.ShowLastMove))
        {
            arrow.rotation = 0;
            arrow.transform.position = new Vector2(playerLeftTransform.position.x + arrowPlayerXOffset, playerLeftTransform.position.y + arrowPlayerYOffset);
        }
        else
        {
            arrow.rotation = 180;
            arrow.transform.position = new Vector2(playerRightTransform.position.x - arrowPlayerXOffset, playerRightTransform.position.y + arrowPlayerYOffset);
        }
    }

    public void RemoveAllShotArrows()
    {
        while(arrowsShot.Count > 0)
        {
            Arrow arrow = arrowsShot[0];
            arrowsShot.RemoveAt(0);
            if (arrow != null)
            {
                DestroyObject(arrow.gameObject);
            }
        }
    }
}
