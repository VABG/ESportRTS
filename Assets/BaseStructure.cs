using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Structure))]
public class BaseStructure : MonoBehaviour
{
    Structure s;
    PlayerResources pResources;
    // Start is called before the first frame update
    void Start()
    {
        s = GetComponent<Structure>();
        pResources = FindObjectOfType<PlayerResources>();
    }

    public void GiveResources(Resources r)
    {
        pResources.AddResources(r);
    }
}
