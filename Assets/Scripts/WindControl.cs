using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WindControl : MonoBehaviour
{
    public Camera RTcamera;

    private void Update()
    {
        if (RTcamera != null)
        {
            Shader.SetGlobalVector("RTCameraPosition", RTcamera.transform.position);
            Shader.SetGlobalFloat("RTCameraSize", RTcamera.orthographicSize);
        }
    }
}
