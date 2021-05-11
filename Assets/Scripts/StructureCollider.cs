using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureCollider : MonoBehaviour
{
    public bool CanPlace { get { return collisions == 0; } private set { } }
    int collisions = 0;
    Structure parent;

    public void SetParent(Structure parent)
    {
        this.parent = parent;
    }

    public Structure GetParent()
    {
        return parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        collisions++;
    }

    private void OnTriggerExit(Collider other)
    {
        collisions--;
    }
}
