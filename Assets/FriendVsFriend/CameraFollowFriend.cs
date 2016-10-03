using UnityEngine;
using System.Collections;

public class CameraFollowFriend : MonoBehaviour 
{
	//public Transform target;        //target for the camera to follow (the arrow)
    public AimLineFriend aimLine;
    public Arrow arrow;
    private Vector3 targetVector;
    public SpriteRenderer LeftMostSpriteInView;
    public SpriteRenderer RightMostSpriteInView;
    public SpriteRenderer TopMostSpriteInView;
    public SpriteRenderer BottomMostSpriteInView;
    private float minXCoord = -80;
    private float maxXCoord = 80;
    private float minYCoord = 0;
    private float maxYCoord = 65;
    public bool Paused = false;
    private float setToX = 0f;
    private float setToY = 5f;
    
    void Awake()
    {
        if (NetworkManager.StartFromBeginingIfNotStartedYet())
        {
            return;
        }
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Start()
    {
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        arrow.ResetPosition(_networkManager.levelDef.IsPlayerLeftTurn);
    }

    const float minSize = 1f;
    const float maxSize = 20f;
    const float sensitivity = 0.5f;
    private NetworkManager _networkManager;

    void Update()
    {
        float size = Camera.main.orthographicSize;
        if (Input.mouseScrollDelta.y > 0)
        {
            size = Mathf.Clamp(size - sensitivity, minSize, maxSize);
            Camera.main.orthographicSize = size;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            size = Mathf.Clamp(size + sensitivity, minSize, maxSize);
            Camera.main.orthographicSize = size;
        }

        if (true) //!Paused)
        {
            /* Calculate X position */
            setToX = Mathf.Clamp(arrow.transform.position.x, minXCoord, maxXCoord);

            /* Calculate Y position */
            setToY = Mathf.Clamp(arrow.transform.position.y, minYCoord, maxYCoord);

            /* Apply what we've calculated */
            // If the arrow is shooting (flying) follow it with the camera instantly each frame
            if (aimLine != null && aimLine.IsShooting)
            {
                transform.position = new Vector3(setToX, setToY, transform.position.z);
            }
            // If the arrow isn't flying, slowly move the camera to the new arrow.
            else
            {
                float dampTime = 0.05f;
                float maxSpeed = 250f;
                Vector3 velocity = Vector3.zero;
                targetVector = new Vector3(setToX, setToY, transform.position.z);

                //Vector3 point = GameObject.FindObjectOfType<Camera>().WorldToViewportPoint(target.position);
                //Vector3 delta = target.position - GameObject.FindObjectOfType<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
                //Vector3 destination = transform.position + delta;
                //transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

                transform.position = Vector3.SmoothDamp(transform.position, targetVector, ref velocity, dampTime, maxSpeed, Time.deltaTime);
                //transform.position = Vector3.Lerp(, new Vector3(setToX, setToY, transform.position.z), Time.deltaTime * 5);
            }
        }

        if(Camera.main.orthographicSize > 2)
        {
            transform.position = new Vector3(transform.position.x, 6.0f + Camera.main.orthographicSize - 7.0f, transform.position.z);
        }
    }

    public void PauseInSeconds(float seconds)
    {
        StartCoroutine(this.PauseSeconds(seconds));
    }
    private IEnumerator PauseSeconds(float seconds)
    {
        Paused = true;
        yield return new WaitForSeconds(seconds);
        Paused = false;
    }

    // If the camera is within .1 of it's target, it is considered not moving
    public bool IsMoving()
    {
        if (Mathf.Abs(transform.position.x - arrow.transform.position.x) > .1f)
        {
            return true;
        }
        if (Mathf.Abs(transform.position.x - arrow.transform.position.x) > .1f)
        {
            return true;
        }
        return false;
    }
}
