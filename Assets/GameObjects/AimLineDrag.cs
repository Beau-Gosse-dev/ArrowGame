using UnityEngine;
using System.Collections;


public class AimLineOld : MonoBehaviour
{

    public Arrow activeArrow;
    public CameraFollow cameraFollow;
    public LevelManager levelManager;
    private LineRenderer lineRender;
    private bool IsDragging = false; // If the aim/power line is visible and the player is dragging
    private bool IsValidShot = false; // If the distance is too drag is too small, it's not valid so the user can reset.
    private Vector3 startPoint; // Start point of the drag for the aim/power line
    private Vector3 endPoint; // End (or current) point of the drag for the aim/power line

    public Canvas worldCanvas;

    public bool IsShooting = false; // The arrow is moving through the air


    // private Vector3 computerAim = new Vector3(6f, 4f, -.05f);

    void Start()
    {
        lineRender = gameObject.GetComponent<LineRenderer>();
        lineRender.SetColors(Color.black, Color.black);
        lineRender.SetWidth(0.01f, 0.3f);
        lineRender.SetVertexCount(2);
        startPoint = new Vector3(0f, 0f, 100f);
        endPoint = new Vector3(10f, 10f, 100f);
        lineRender.SetPosition(0, startPoint);
        lineRender.SetPosition(1, endPoint);
    }

    void Update()
    {
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
                    if (Vector2.Distance(startPoint, endPoint)  > 3)
                    {
                        // Since it is valid, show the line
                        IsValidShot = true;
                        startPoint.z = -0.5f;
                        endPoint.z = -0.5f;
                    }
                    else
                    {
                        IsValidShot = false;
                        // Since it isn't valid, hide the line
                        startPoint.z = 100f;
                        endPoint.z = 100f;
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
                startPoint.z = 100f;
                endPoint.z = 100f;

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
}
