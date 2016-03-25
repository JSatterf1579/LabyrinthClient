using UnityEngine;

//[RequireComponent(typeof(Camera))]
public class MapViewCameraController : MonoBehaviour {

    //private Camera cam;

    private Vector3 target = Vector3.zero;
    private float distance;
    private float orbitX = 0f; //since this parameter has no absolute bounds, we don't care what value it starts at
    private float orbitY;

    public bool PanWhenMouseAtEdge = true;
    public float MousePanBounds = 10f;
    public float CardinalPanSpeed = 10f;
    public float MinZoomDistance = 5f;
    public float MaxZoomDistance = 30f;
    public float MinAngle = 20f;
    public float MaxAngle = 80f;

    void Awake() {
        //cam = GetComponent<Camera>();
    }

	// Use this for initialization
	void Start () {
        transform.LookAt(target);
        distance = Vector3.Distance(transform.position, target);
        orbitY = CalculateAngle();
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        Gizmos.DrawSphere(target, 0.1f);
    }

#endif

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.M)) PanWhenMouseAtEdge = !PanWhenMouseAtEdge;

        DebugHUD.setValue("Mouse X", Input.GetAxis("Mouse X"));
        DebugHUD.setValue("Mouse Y", Input.GetAxis("Mouse Y"));
        string stateString = "";

        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool leftClick = Input.GetMouseButton(2);
        bool rightClick = Input.GetMouseButton(1);

        float x = 0f;
        float y = 0f;
        if (shift) {
            if (up) y += 1;
            if (down) y -= 1;
            if (left) x -= 1;
            if (right) x += 1;
            stateString += "Keyboard Orbit, ";
        }
        if(leftClick && !shift) {
            x += Input.GetAxis("Mouse X");
            y -= Input.GetAxis("Mouse Y");
            stateString += "Mouse Orbit, ";
        }
        //transform.RotateAround(target, transform.up, x);
        //transform.RotateAround(target, transform.right, y);
        //transform.LookAt(target,Vector3.up);
        ShiftOribitAngles(x, y);

        DoScrollZoom();

        DebugHUD.setValue("OrbitX", x);
        DebugHUD.setValue("OrbitY", y);


        if(rightClick) {
            MoveAlongBoardNonNormalized(new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y")));
            stateString += "Mouse Pan, ";
        }


        if (PanWhenMouseAtEdge && !leftClick && !rightClick) {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.y > (Screen.height - MousePanBounds)) up = true;
            if (mousePosition.y < MousePanBounds) down = true;
            if (mousePosition.x > (Screen.width - MousePanBounds)) right = true;
            if (mousePosition.x < MousePanBounds) left = true;
            stateString += "Checking Edges, ";
        }


        if((up || down || left || right) && !shift) {
            Vector3 movement = Vector3.zero;
            if (up) movement += Vector3.forward;
            if (down) movement += Vector3.back;
            if (left) movement += Vector3.left;
            if (right) movement += Vector3.right;
            MoveAlongBoardNormalized(movement);
            stateString += "Panning, ";
        }

        DebugHUD.setValue("Camera States", stateString);

	}

    private void ShiftOribitAngles(float x, float y) {
        //SetOribitAngles(x + orbitX, y + orbitY);
        float ny = y + orbitY;
        if(ny > MaxAngle) {
            float over = ny - MaxAngle;
            y -= over;
        } else if (ny < MinAngle) {
            float over = MinAngle - ny;
            y += over;
        }
        orbitY += y;
        transform.RotateAround(target, transform.up, x);
        transform.RotateAround(target, transform.right, y);
        transform.LookAt(target, Vector3.up);
    }

    /// <summary>
    /// reads the scroll wheel delta value for this frame and adjusts the current zoom level accordingly
    /// </summary>
    private void DoScrollZoom() {
        float scroll = Input.mouseScrollDelta.y;
        if(scroll != 0f) {
            SetZoom(distance - scroll);
        }
    }

    /// <summary>
    /// Sets a new zoom (distance) value and moves the camera accordingly
    /// </summary>
    /// <param name="zoom"></param>
    private void SetZoom(float zoom) {
        zoom = Mathf.Clamp(zoom, MinZoomDistance, MaxZoomDistance);
        if (zoom == distance) return; //there was no change
        //by always moving the camera relative to its current position, we never have to recalculate the distance to the target
        float delta = distance - zoom;
        transform.Translate(Vector3.forward * delta, Space.Self);
        distance = zoom;
    }

    /// <summary>
    /// Given a vector with only x and z components, pans the camera along the board relative to the camera's rotation
    /// This normalizes and multiplies by Time.deltaTime for use with digital inputs
    /// </summary>
    /// <param name="movement"></param>
    private void MoveAlongBoardNormalized(Vector3 movement) {
        Vector3 mouseWorld = Vector3.Normalize(Vector3.ProjectOnPlane(transform.TransformDirection(movement), Vector3.up)) * CardinalPanSpeed * Time.deltaTime;
        SetTargetPosition(target + mouseWorld);
    }

    /// <summary>
    /// Pans the camera along the board, does not normalize; for use with analog inputs
    /// </summary>
    /// <param name="movement"></param>
    private void MoveAlongBoardNonNormalized(Vector3 movement) {
        Vector3 mouseWorld = Vector3.ProjectOnPlane(transform.TransformDirection(movement), Vector3.up);
        SetTargetPosition(target + mouseWorld);
    }

    private float CalculateAngle() {
        Vector3 firstVector = transform.forward;
        Vector3 secondVector = Vector3.ProjectOnPlane(firstVector, Vector3.up);
        return Vector3.Angle(firstVector, secondVector);
    }

    /// <summary>
    /// Moves the target to the given point and moves the camera accordingly
    /// </summary>
    /// <param name="newTargetPos"></param>
    private void SetTargetPosition(Vector3 newTargetPos) {
        //clamp to map bounds
        newTargetPos.x = Mathf.Clamp(newTargetPos.x, Map.Current.MinXBound, Map.Current.MaxXBound);
        newTargetPos.z = Mathf.Clamp(newTargetPos.z, Map.Current.MinZBound, Map.Current.MaxZBound);
        newTargetPos.y = 0f;
        Vector3 delta = transform.position - target;
        target = newTargetPos;
        transform.position = target + delta;
    }
}
