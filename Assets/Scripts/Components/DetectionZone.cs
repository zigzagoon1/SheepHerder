using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter = default, onExit = default;

    private void OnTriggerEnter(Collider other)
    {
        if (onEnter != null)
        {
            if (other.gameObject.CompareTag("Sheep"))
            {
                onEnter.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
        {
            onExit.Invoke();
        }
    }
}
