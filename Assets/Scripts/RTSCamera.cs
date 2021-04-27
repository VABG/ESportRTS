using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    Camera cam;  
    [SerializeField] Transform nearCamPosition;
    [SerializeField] Transform farCamTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] LayerMask rayTraceMask;
    float cameraHeightLerp;
    float cameraHeightLerpTarget;
    Vector3 rayCastOffset = new Vector3(0, 20, 0);
    Vector3 targetPosition;
    [SerializeField] float movementLerpMultiplier = 4.0f;
    [SerializeField] float cameraSpeedMult = 100.0f;
    [SerializeField] float scrollCamSpeedMult = .1f;
    [SerializeField] float cameraGroundOffset = 5.0f;
    [SerializeField] float xLimMin, xLimMax, zLimMin, zLimMax;


    Vector3 camGroundAvoidanceTargetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {   
        InputWASD();
        LimitCameraToArea();
        InputMouseScroll();
        InputCameraMouseRotation();
        LerpCamera();
    }

    void LerpCamera()
    {
        // Move w Lerp Camera Parent
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementLerpMultiplier * Time.deltaTime);

        // Lerp ground avoidance
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camGroundAvoidanceTargetPosition, movementLerpMultiplier * Time.deltaTime);
    }

    void InputMouseScroll()
    {
        // Scroll to move closer/further away(within limits)
        cameraHeightLerpTarget += Input.mouseScrollDelta.y * scrollCamSpeedMult;
        cameraHeightLerpTarget = Mathf.Clamp01(cameraHeightLerpTarget);
        cameraHeightLerp = Mathf.Lerp(cameraHeightLerp, cameraHeightLerpTarget, 8.0f * Time.deltaTime);
        cameraTransform.position = Vector3.Lerp(farCamTransform.position, nearCamPosition.position, cameraHeightLerp);
        cameraTransform.rotation = Quaternion.Lerp(farCamTransform.rotation, nearCamPosition.rotation, Mathf.Pow(cameraHeightLerp, 3));
    }

    void InputWASD()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement.z += 1;
        if (Input.GetKey(KeyCode.S)) movement.z -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        movement.Normalize();
        movement *= cameraSpeedMult * Time.deltaTime;
        targetPosition += transform.forward * movement.z;
        targetPosition += transform.right * movement.x;
    }

    void LimitCameraToArea()
    {
        if (targetPosition.x < xLimMin) targetPosition.x = xLimMin;
        if (targetPosition.x > xLimMax) targetPosition.x = xLimMax;
        if (targetPosition.z < zLimMin) targetPosition.z = zLimMin;
        if (targetPosition.z > zLimMax) targetPosition.z = zLimMax;
    }

    void InputCameraMouseRotation()
    {
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        }

        if (Input.GetMouseButton(2))
        {
            targetPosition -= transform.right * Input.GetAxis("Mouse X");
            targetPosition -= transform.forward * Input.GetAxis("Mouse Y");
        }
    }

    private void FixedUpdate()
    {
        TraceTargetHeight();
        CameraAvoidGround();
    }

    void TraceTargetHeight()
    {
        // Camera center height adjustment
        float targetHeight = 0;

        Ray r = new Ray(targetPosition + rayCastOffset, Vector3.down);
        if (Physics.Raycast(r, out RaycastHit hitInfo, 100, rayTraceMask))
        {
            targetHeight = hitInfo.point.y;
        }
        targetPosition.y = targetHeight;
    }

    void CameraAvoidGround()
    {
        camGroundAvoidanceTargetPosition = Vector3.zero;
        Ray r = new Ray(cameraTransform.position + rayCastOffset, Vector3.down);
        if (Physics.Raycast(r, out RaycastHit hitInfo, 100, rayTraceMask))
        {
            float d = hitInfo.distance - rayCastOffset.y;
            if (d < cameraGroundOffset) camGroundAvoidanceTargetPosition = Vector3.up * (cameraGroundOffset - d);            
        }
    }
}
