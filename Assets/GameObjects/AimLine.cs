using UnityEngine;
using System;
using Assets.Mangers;

public class AimLine : MonoBehaviour
{

    public Arrow activeArrow;
    public CameraFollow cameraFollow;
    private LineRenderer lineRender;
    public Player playerLeft;
    private bool IsDragging = false; // If the aim/power line is visible and the player is dragging
    private bool IsValidShot = false; // If the distance is too drag is too small, it's not valid so the user can reset.
    private Vector3 startPoint; // Start point of the drag for the aim/power line
    private Vector3 endPoint; // End (or current) point of the drag for the aim/power line

    public Player playerRight;
    public Canvas worldCanvas;

    public bool IsShooting = false; // The arrow is moving through the air
    private Vector3 computerAimPoint; // Where we will tell the computer to shoot (including randomness)
    private Vector3 computerAimPointWithoutRandomness; // Where the computer needs to drag to to shoot

    // private Vector3 computerAim = new Vector3(6f, 4f, -.05f);

    void Start()
    {
        lineRender = gameObject.GetComponent<LineRenderer>();
        lineRender.SetColors(Color.black, Color.black);
        lineRender.SetWidth(0.01f, 0.3f);
        lineRender.SetVertexCount(2);

        this.SetupMatch();
    }

    public void SetupMatch()
    {
        lineRender = gameObject.GetComponent<LineRenderer>();

        if (LevelDefinition.gameState == GameState.ShowLastMove)
        {
            startPoint.x = LevelDefinition.LastShotStartX;
            startPoint.y = LevelDefinition.LastShotStartY;
            endPoint = startPoint;
            computerAimPoint.x = LevelDefinition.LastShotEndX;
            computerAimPoint.y = LevelDefinition.LastShotEndY;

        }
        else
        {
            startPoint = new Vector3(0f, 0f, 0f);
            endPoint = new Vector3(10f, 10f, 0f);
            lineRender.enabled = false;
            // Set up the first shot for the computer
            computerAimPoint = new Vector3(playerRight.transform.position.x + 5.0f, playerRight.transform.position.y, playerRight.transform.position.z);
            computerAimPointWithoutRandomness = new Vector3(playerRight.transform.position.x + 5.0f, playerRight.transform.position.y, playerRight.transform.position.z);
        }

        lineRender.SetPosition(0, startPoint);
        lineRender.SetPosition(1, endPoint);
    }

    void Update()
    {
        if (LevelDefinition.gameType == GameType.Computer)
        {
            #region LeftPlayer
            // Left Player
            // Only allow dragging when the camera is not paused and on it's target(not moving) and we aren't shooting. Also when game state is playing.
            if (!IsShooting && !cameraFollow.Paused && !cameraFollow.IsMoving() && LevelDefinition.gameState == GameState.Playing && LevelDefinition.IsPlayerLeftTurn)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!IsDragging)
                    {
                        // If we weren't dragging, set the new start point to here. Also set endpoint here because we're dragging now.
                        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        IsDragging = true;
                    }
                    else
                    {
                        // We were already dragging and the mouse is down still, update the end point                
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        // If the player drags the end point to where the start point is, allow them to reset the start point
                        if (Vector2.Distance(startPoint, endPoint) > 3)
                        {
                            // Since it is valid, show the line
                            IsValidShot = true;
                            lineRender.enabled = true;
                        }
                        else
                        {
                            IsValidShot = false;
                            lineRender.enabled = false;
                        }
                        if (IsValidShot)
                        {
                            var deltaX = startPoint.x - endPoint.x;
                            var deltaY = startPoint.y - endPoint.y;
                            var rad = System.Math.Atan2(deltaY, deltaX); // In radians
                            var deg = rad * (180 / System.Math.PI);
                            activeArrow.SetRotation(System.Convert.ToSingle(deg));
                        }
                    }
                    this.setLine(startPoint, endPoint);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    worldCanvas.enabled = false; // Disable the instruction text if they have shot.

                    IsDragging = false;
                    // Hide the line
                    lineRender.enabled = false;

                    // If it's a valid shot, shoot, otherwise we wanted to just reset
                    if (IsValidShot)
                    {
                        activeArrow.Shoot(startPoint, endPoint);
                        IsShooting = true;
                        LevelDefinition.IsPlayerLeftTurn = false;
                        IsValidShot = false;
                        InitializeComputerShot();
                    }
                    this.setLine(startPoint, endPoint);
                }
            }
            #endregion LeftPlayer 

            // Right player's turn
            // Translate the start and end points to show the arrow
            // Once in line, shoot
            #region ComputerPlayer
            else if (!IsShooting && !cameraFollow.Paused && !cameraFollow.IsMoving() && LevelDefinition.gameState == GameState.Playing && !LevelDefinition.IsPlayerLeftTurn)
            {
                endPoint = Vector3.MoveTowards(endPoint, computerAimPoint, Time.deltaTime * 5f);
                if (Vector2.Distance(startPoint, endPoint) > 0)
                {
                    // Since it is valid, show the line
                    IsValidShot = true;
                    lineRender.enabled = true;
                }
                else
                {
                    IsValidShot = false;
                    // Since it isn't valid, hide the line
                    lineRender.enabled = false;
                }
                if (IsValidShot)
                {
                    var deltaX = startPoint.x - endPoint.x;
                    var deltaY = startPoint.y - endPoint.y;
                    var rad = System.Math.Atan2(deltaY, deltaX); // In radians
                    var deg = rad * (180 / System.Math.PI);
                    activeArrow.SetRotation(System.Convert.ToSingle(deg));
                }
                this.setLine(startPoint, endPoint);
                if (Math.Abs(endPoint.x - computerAimPoint.x) < .01 && Math.Abs(endPoint.y - computerAimPoint.y) < .01)
                {
                    worldCanvas.enabled = false; // Disable the instruction text if they have shot.

                    IsDragging = false;
                    // Hide the line
                    lineRender.enabled = false;

                    // Shoot
                    activeArrow.Shoot(startPoint, endPoint);
                    IsShooting = true;
                    IsValidShot = false;
                    LevelDefinition.IsPlayerLeftTurn = true;
                    this.setLine(startPoint, endPoint);
                }
            }
            #endregion ComputerPlayer
        }
        else if(LevelDefinition.gameType == GameType.Local)
        {
            #region HumanVsHuman
            // Left Player
            //if (IsLeftPlayerTurn && !IsShooting)
            // Only allow dragging when the camera is not paused and on it's target(not moving) and we aren't shooting. Also when game state is playing.
            if (!IsShooting && !cameraFollow.Paused && !cameraFollow.IsMoving() && LevelDefinition.gameState == GameState.Playing)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!IsDragging)
                    {
                        // If we weren't dragging, set the new start point to here. Also set endpoint here because we're dragging now.
                        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        IsDragging = true;
                    }
                    else
                    {
                        // We were already dragging and the mouse is down still, update the end point                
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        // If the player drags the end point to where the start point is, allow them to reset the start point
                        if (Vector2.Distance(startPoint, endPoint) > 3)
                        {
                            // Since it is valid, show the line
                            IsValidShot = true;
                            lineRender.enabled = true;
                        }
                        else
                        {
                            IsValidShot = false;
                            // Since it isn't valid, hide the line
                            lineRender.enabled = false;
                        }
                        if (IsValidShot)
                        {
                            var deltaX = startPoint.x - endPoint.x;
                            var deltaY = startPoint.y - endPoint.y;
                            var rad = System.Math.Atan2(deltaY, deltaX); // In radians
                            var deg = rad * (180 / System.Math.PI);
                            activeArrow.SetRotation(System.Convert.ToSingle(deg));
                        }
                    }
                    this.setLine(startPoint, endPoint);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    worldCanvas.enabled = false; // Disable the instruction text if they have shot.

                    IsDragging = false;
                    // Hide the line
                    lineRender.enabled = false;

                    // If it's a valid shot, shoot, otherwise we wanted to just reset
                    if (IsValidShot)
                    {
                        activeArrow.Shoot(startPoint, endPoint);
                        IsShooting = true;
                        LevelDefinition.IsPlayerLeftTurn = !LevelDefinition.IsPlayerLeftTurn;
                        IsValidShot = false;
                    }
                    this.setLine(startPoint, endPoint);
                }
            }

            #endregion HumanVsHuman
        }
        else if (LevelDefinition.gameType == GameType.Online)
        {
            #region ShowingLastShot
            if (LevelDefinition.gameState == GameState.ShowLastMove && !IsShooting)
            {
                endPoint = Vector3.MoveTowards(endPoint, computerAimPoint, Time.deltaTime * 5f);
                if (Vector2.Distance(startPoint, endPoint) > 0)
                {
                    // Since it is valid, show the line
                    IsValidShot = true;
                    lineRender.enabled = true;
                }
                else
                {
                    IsValidShot = false;
                    // Since it isn't valid, hide the line
                    lineRender.enabled = false;
                }
                if (IsValidShot)
                {
                    var deltaX = startPoint.x - endPoint.x;
                    var deltaY = startPoint.y - endPoint.y;
                    var rad = System.Math.Atan2(deltaY, deltaX); // In radians
                    var deg = rad * (180 / System.Math.PI);
                    activeArrow.SetRotation(System.Convert.ToSingle(deg));
                }
                this.setLine(startPoint, endPoint);
                if (Math.Abs(endPoint.x - computerAimPoint.x) < .01 && Math.Abs(endPoint.y - computerAimPoint.y) < .01)
                {
                    worldCanvas.enabled = false; // Disable the instruction text if they have shot.

                    IsDragging = false;
                    // Hide the line
                    lineRender.enabled = false;

                    // Shoot
                    if (IsShooting == false)
                    {
                        activeArrow.Shoot(startPoint, endPoint);
                        LevelDefinition.LastShotEndX = endPoint.x;
                        LevelDefinition.LastShotEndY = endPoint.y;
                        LevelDefinition.LastShotStartX = startPoint.x;
                        LevelDefinition.LastShotStartY = startPoint.y;
                        IsShooting = true;
                        IsValidShot = false;
                        this.setLine(startPoint, endPoint);
                    }
                }
            }
            #endregion ShowingLastShot
            #region HumanVsHumanOnline
            // If it's left player and the current user is left, or it's right and the curr is right, allow shooting.
            // Only allow dragging when the camera is not paused and on it's target(not moving) and we aren't shooting. Also when game state is playing.
            if (!IsShooting 
                && !cameraFollow.Paused 
                && !cameraFollow.IsMoving() 
                && LevelDefinition.gameState == GameState.Playing
                && ((LevelDefinition.IsPlayerLeftTurn)
                    || (!LevelDefinition.IsPlayerLeftTurn))
                )
            {
                if (Input.GetMouseButton(0))
                {
                    if (!IsDragging)
                    {
                        // If we weren't dragging, set the new start point to here. Also set endpoint here because we're dragging now.
                        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        IsDragging = true;
                    }
                    else
                    {
                        // We were already dragging and the mouse is down still, update the end point                
                        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        // If the player drags the end point to where the start point is, allow them to reset the start point
                        if (Vector2.Distance(startPoint, endPoint) > 3)
                        {
                            // Since it is valid, show the line
                            IsValidShot = true;
                            lineRender.enabled = true;
                        }
                        else
                        {
                            IsValidShot = false;
                            // Since it isn't valid, hide the line
                            lineRender.enabled = false;
                        }
                        if (IsValidShot)
                        {
                            var deltaX = startPoint.x - endPoint.x;
                            var deltaY = startPoint.y - endPoint.y;
                            var rad = System.Math.Atan2(deltaY, deltaX); // In radians
                            var deg = rad * (180 / System.Math.PI);
                            activeArrow.SetRotation(System.Convert.ToSingle(deg));
                        }
                    }
                    this.setLine(startPoint, endPoint);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    worldCanvas.enabled = false; // Disable the instruction text if they have shot.

                    IsDragging = false;
                    // Hide the line with z index
                    lineRender.enabled = false;

                    // If it's a valid shot, shoot, otherwise we wanted to just reset
                    if (IsValidShot)
                    {
                        activeArrow.Shoot(startPoint, endPoint);
                        IsShooting = true;
                        LevelDefinition.IsPlayerLeftTurn = !LevelDefinition.IsPlayerLeftTurn;
                        IsValidShot = false;

                        // Save the shot so we can recreate it for the other player to watch (for online)
                        LevelDefinition.LastShotStartX = startPoint.x;
                        LevelDefinition.LastShotStartY = startPoint.y;
                        LevelDefinition.LastShotEndX = endPoint.x;
                        LevelDefinition.LastShotEndY = endPoint.y;
                    }
                    this.setLine(startPoint, endPoint);
                }
            }

            #endregion HumanVsHumanOnline
        }

    }

    private void setLine(Vector3 startPoint, Vector3 endPoint)
    {
        lineRender.SetPosition(0, startPoint);
        lineRender.SetPosition(1, endPoint);
    }

    public void ArrowHit(Collision2D col)
    {
        this.IsShooting = false;
    }

    private void InitializeComputerShot()
    {
        startPoint = new Vector3(playerRight.transform.position.x, playerRight.transform.position.y + 6.0f, playerRight.transform.position.z);
        endPoint = new Vector3(playerRight.transform.position.x, playerRight.transform.position.y + 6.0f, playerRight.transform.position.z);
    }

    public void ArrowCollision(float x, float y, bool hitWall, Quaternion arrowRotation)
    {
        // If it's the left player's turn (because the computer just shot and we want to know how that shot did)
        if (LevelDefinition.IsPlayerLeftTurn)
        {
            // 4 states,
            //  No wall, too far
            //  No wall, too short
            //  Wall, arrow was on it's way down
            //  Wall, arrow was on it's way up

            // Create randomness from -0.2 to 0.2 extra drag distance
            float xRandomness = (UnityEngine.Random.value * 0.4f) - 0.2f;
            float yRandomness = (UnityEngine.Random.value * 0.4f) - 0.2f;
            
            
            if (hitWall)
            {
                // If the arrow is pointing downward
                if (arrowRotation.eulerAngles.z > 180)
                {
                    Debug.Log("Downward Arrow");
                    // Shoot higher
                    computerAimPoint.y = computerAimPointWithoutRandomness.y - 0.2f;

                    // pull the line 1/10th shorter than we missed by
                    // add randomness to make it fair.
                    computerAimPointWithoutRandomness.x = computerAimPointWithoutRandomness.x + 0.1f;
                    computerAimPoint.x = computerAimPointWithoutRandomness.x;
                }
                // If the arrow is pointing up or straight
                else
                {
                    Debug.Log("Upward Arrow");
                    // Shoot higher
                    computerAimPoint.y = computerAimPointWithoutRandomness.y - 0.2f;

                    // pull the line 1/10th shorter than we missed by
                    // add randomness to make it fair.
                    computerAimPointWithoutRandomness.x = computerAimPointWithoutRandomness.x - 0.1f;
                    computerAimPoint.x = computerAimPointWithoutRandomness.x;
                }
            }
            else
            {
                // Adjust power considering how far away the miss is.
                // Add randomness to make it fair.
                computerAimPointWithoutRandomness.x = computerAimPointWithoutRandomness.x + ((x - playerLeft.transform.position.x) / 10);
                computerAimPoint.x = computerAimPointWithoutRandomness.x + xRandomness;
                computerAimPoint.y = computerAimPointWithoutRandomness.y + yRandomness;
            }

            // Verify the computer isn't shooting backwards or down.
            Mathf.Clamp(computerAimPoint.x, startPoint.x, 100);
            Mathf.Clamp(computerAimPoint.y, -100, startPoint.y);
        }
    }
}
