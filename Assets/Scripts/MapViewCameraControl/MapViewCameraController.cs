using UnityEngine;

//[RequireComponent(typeof(Camera))]
public class MapViewCameraController : MonoBehaviour {

    //private Camera cam;

    private Vector3 target = Vector3.zero;
    //private float distance;

    public bool PanWhenMouseAtEdge = true;
    public float MousePanBounds = 10f;
    public float CardinalPanSpeed = 10f;

    void Awake() {
        //cam = GetComponent<Camera>();
    }

	// Use this for initialization
	void Start () {
        transform.LookAt(target);
        //distance = Vector3.Distance(transform.position, target);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M)) PanWhenMouseAtEdge = !PanWhenMouseAtEdge;

        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool leftClick = Input.GetMouseButton(1);
        bool rightClick = Input.GetMouseButton(2);

        if (leftClick && shift) {
            transform.RotateAround(target, transform.up, Input.GetAxis("Mouse X"));
            transform.RotateAround(target, transform.right, -Input.GetAxis("Mouse Y"));
            transform.LookAt(target,Vector3.up);
        }

        transform.Translate(Vector3.forward * Input.mouseScrollDelta.y, Space.Self);

        float x = 0f;
        float y = 0f;
        if (shift) {
            if (up) y += 1;
            if (down) y -= 1;
            if (left) x -= 1;
            if (right) x += 1;
        }


        if(rightClick || (shift && leftClick)) {
            x += Input.GetAxis("Mouse X");
            y += Input.GetAxis("Mouse Y");
            MoveAlongBoardNonNormalized(new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y")));
        }


        if (PanWhenMouseAtEdge) {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.y > (Screen.height - MousePanBounds)) up = true;
            if (mousePosition.y < MousePanBounds) down = true;
            if (mousePosition.x > (Screen.width - MousePanBounds)) right = true;
            if (mousePosition.x < MousePanBounds) left = true;
        }


        if(up || down || left || right && !shift) {
            Vector3 movement = Vector3.zero;
            if (up) movement += Vector3.forward;
            if (down) movement += Vector3.back;
            if (left) movement += Vector3.left;
            if (right) movement += Vector3.right;
            MoveAlongBoardNormalized(movement);
        }

	}

    /// <summary>
    /// Given a vector with only x and z components, pans the camera along the board relative to the camera's rotation
    /// This normalizes and multiplies by Time.deltaTime for use with digital inputs
    /// </summary>
    /// <param name="movement"></param>
    private void MoveAlongBoardNormalized(Vector3 movement) {
        Vector3 delta = transform.position - target;
        Vector3 mouseWorld = Vector3.Normalize(Vector3.ProjectOnPlane(transform.TransformDirection(movement), Vector3.up)) * CardinalPanSpeed * Time.deltaTime;
        target += mouseWorld;
        transform.position = target + delta;
    }

    /// <summary>
    /// Pans the camera along the board, does not normalize; for use with analog inputs
    /// </summary>
    /// <param name="movement"></param>
    private void MoveAlongBoardNonNormalized(Vector3 movement) {
        Vector3 delta = transform.position - target;
        Vector3 mouseWorld = Vector3.ProjectOnPlane(transform.TransformDirection(movement), Vector3.up);
        target += mouseWorld;
        transform.position = target + delta;
    }
}
