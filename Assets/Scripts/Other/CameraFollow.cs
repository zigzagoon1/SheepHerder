//followed this tutorial from CatlikeCoding: https://catlikecoding.com/unity/tutorials/movement/orbit-camera/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public float camSpeed;
    public Transform focus;
    public InputAction leftRight;
    public InputAction upDown;
    [SerializeField, Min(0f)] float focusRadius = 5f;
    [SerializeField, Range(0f, 1f)] float focusCentering = 0.5f;
    [SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(1f, 100f)] float distance = 50f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField, Min(0f)] float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45f;
    [SerializeField] LayerMask obstructionMask = -1;
    [SerializeField, Min(1)] float minZoom = 5f;
    [SerializeField] float maxZoom = 15f; 

    const float e = 0.001f;

    Vector3 focusPoint, previousFocusPoint;
    Vector2 orbitAngles = new Vector2(40, 108);
    Camera mainCamera;

    private Vector2 inputRotation;
    private float lastManualRotationTime;

    private float inputZoom;


    Vector3 CameraHalfExtends { 
        get {
            Vector3 halfExtends;
            halfExtends.y = mainCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCamera.fieldOfView);
            halfExtends.x = halfExtends.y * mainCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
            }
        }

    private void Awake()
    {
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
        mainCamera = GetComponent<Camera>();
    }
    private void OnEnable()
    {
        leftRight.Enable();
        upDown.Enable();
    }
    private void OnDisable()
    {
        leftRight.Disable();
        upDown.Disable();
    }
    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }
    private void Start()
    {
        focus = FindObjectOfType<PlayerController>().transform;
    }
    void UpdateFocusPoint()
    {
        previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
    }
    public bool ManualRotation()
    {
        float leftRightFloat = leftRight.ReadValue<float>();
        float upDownFloat = upDown.ReadValue<float>();
        inputRotation = new Vector2(upDownFloat, leftRightFloat);
        if (inputRotation.x < -e || inputRotation.x > e || inputRotation.y < -e || inputRotation.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * inputRotation;
            lastManualRotationTime = Time.unscaledDeltaTime;
            return true;
        }
        return false;
    }
    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }
        Vector2 movement = new Vector2(focusPoint.x - previousFocusPoint.x, focusPoint.z - previousFocusPoint.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.000001f)
        {
            return false;
        }
        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs > alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
            orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        }
        return true;
    }
    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
    void ConstrainAngles()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }
    void OnZoomCamera(InputValue input)
    {
        inputZoom = input.Get<float>();
    }
    private void LateUpdate()
    {
        UpdateFocusPoint();
        ManualRotation();
        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * mainCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
}
