using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGPUCompatibility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
