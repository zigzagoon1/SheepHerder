using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public struct ObjectPosAndSizeInteractiveGrass
{
    public Vector3 position;

    public float size;


    public ObjectPosAndSizeInteractiveGrass(Vector3 pos, float size)
    {
        this.position = pos;
        this.size = size;
    }
}
[ExecuteAlways]
public class ObjectInteractsWithGrass : MonoBehaviour
{
    public GameObject target;
    public Material grassMaterial;

    int numInteractableObjects;
    ComputeBuffer buffer;
    ObjectPosAndSizeInteractiveGrass[] objectData;
    List<ObjectInteractsWithGrass> interactableObjects;
    private void Awake()
    {
        
        numInteractableObjects = FindObjectsOfType<ObjectInteractsWithGrass>().Length;
        interactableObjects = new List<ObjectInteractsWithGrass>();
        interactableObjects.AddRange(FindObjectsOfType<ObjectInteractsWithGrass>());

        objectData = new ObjectPosAndSizeInteractiveGrass[numInteractableObjects];
        buffer = new ComputeBuffer(numInteractableObjects, sizeof(float) * 4);


    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        if (buffer != null)
        {
            buffer.Release();
        }
    }
    private void Update()
    {

        int j = 0;

        foreach (ObjectInteractsWithGrass obj in interactableObjects)
        {

            BoxCollider collider = obj.GetComponent<BoxCollider>();
            float size = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z);
            objectData[j].position = obj.transform.position;
            objectData[j].size = size;
            j++;
        }
        if (buffer == null)
        {
            buffer = new ComputeBuffer(numInteractableObjects, sizeof(float) * 4);
        }

        buffer.SetData(objectData);
        int obDataID = Shader.PropertyToID("objectData1");
        Shader.SetGlobalFloat("_numInteractableObjects", numInteractableObjects);
        Shader.SetGlobalBuffer(obDataID, buffer);
        grassMaterial.SetBuffer(obDataID, buffer);
        
    }
}
