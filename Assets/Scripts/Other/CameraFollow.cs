using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public float camSpeed;
    public Transform focus;
    [SerializeField, Range(1f, 100f)] float orbitDistance;
    [SerializeField] Vector3 offset;
    [SerializeField, Min(0f)] float focusRadius = 5f;

    Vector3 focusPoint;

    private float inputRotation;
    private bool rotate = false;
    private float inputZoom;
    private bool zoom = false;
    private float rotationMultiplier = 3f;
    

    private void Awake()
    {
        focusPoint = focus.position;
        offset = focus.transform.position - transform.position;
    }
    void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            Debug.Log(distance);
            if (distance > focusRadius)
            {
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, focusRadius / distance);
            }
        }
        else
        {
            focusPoint = targetPoint;
        }
    }
    private void Start()
    {
        focus = FindObjectOfType<PlayerController>().transform;
    }

    void OnRotateCamera(InputValue input)
    {
        inputRotation = input.Get<float>();
        rotate = inputRotation == 1 || inputRotation == -1;
    }
    void OnZoomCamera(InputValue input)
    {
        inputZoom = input.Get<float>();
        zoom = inputZoom == 1 || inputZoom == -1;
    }
    private void LateUpdate()
    {
        UpdateFocusPoint();
        Vector3 lookDirection = Vector3.forward;
        transform.localPosition = focusPoint - offset;
    }
}
