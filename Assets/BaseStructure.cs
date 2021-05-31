using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Structure))]
public class BaseStructure : MonoBehaviour
{
    Structure s;
    PlayerResources pResources;
    [SerializeField] GameObject AICharacterToBuild;
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

    public void MakeHuman()
    {
        Transform t = s.GetAccessPoint();
        t.Rotate(Vector3.up, Random.value * 360);
        GameObject g = Instantiate(AICharacterToBuild, t.position, t.rotation);
        g.GetComponent<AICharacter>().SetMovePosition(t.position + t.forward * (1+ Random.value *3));
    }    
}
