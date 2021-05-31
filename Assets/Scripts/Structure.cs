using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Structure : MonoBehaviour
{
    public Resources cost;
    public float health;
    int team;
    [SerializeField] GameObject placementCollidersParent;
    List<StructureCollider> colliders;
    [SerializeField] GameObject visualsParent;
    [SerializeField] bool randomScale = false;
    [SerializeField] float scaleRandomValue = .1f;
    [SerializeField] bool randomScaleAxis = true;
    [SerializeField] Transform accessPoint;
    [SerializeField] BuildingType buildingType;

    public Transform GetAccessPoint()
    {
        return accessPoint;
    }

    public BuildingType GetBuildingType()
    {
        return buildingType;
    }
    private void Awake()
    {
        if (SceneManager.GetActiveScene().isLoaded) return;
        colliders = new List<StructureCollider>(placementCollidersParent.GetComponentsInChildren<StructureCollider>());
        PlaceBuilding();
    }

    private void Start()
    {        
        if (randomScale)
        {
            if (randomScaleAxis)
            {
                transform.localScale += new Vector3(GetRandomScale(), GetRandomScale(), GetRandomScale());
            }
            else
            {
                float scaleAdd = ((Random.value * 2) - 1) * scaleRandomValue;
                transform.localScale += new Vector3(scaleAdd, scaleAdd, scaleAdd);
            }
        }
        colliders = new List<StructureCollider>(placementCollidersParent.GetComponentsInChildren<StructureCollider>());
        foreach(StructureCollider s in colliders)
        {
            s.SetParent(this);
        }
        if (colliders.Count <= 0) Debug.Log("Missing collider(s) for placement!");
    }

    float GetRandomScale()
    {
        return ((Random.value * 2) - 1) * scaleRandomValue;
    }

    public void PlaceBuilding()
    {
        // Deactivate plecement colliders
        foreach (StructureCollider s in colliders)
        {
            s.GetComponent<Collider>().isTrigger = false;
        }
    }

    public bool CanPlace()
    {
        if (colliders == null) return false;
        foreach (StructureCollider s in colliders)
        {
            if (!s.CanPlace) 
                return false;
        }
        return true;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Rotate(float rotation)
    {
        transform.Rotate(new Vector3(0, rotation, 0));
    }

}
