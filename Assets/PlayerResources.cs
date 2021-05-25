using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resources
{
    public int gold, wood, rock, food;
}

public class PlayerResources : MonoBehaviour
{
    [SerializeField] HUD hud;
    [SerializeField] float returnDivider = 2;
    Resources resources;

    // Start is called before the first frame update
    private void Start()
    {
        hud.SetResources(resources);
    }
    public bool CanAfford(Resources cost)
    {
        if (cost.gold > resources.gold) return false;
        if (cost.wood > resources.wood) return false;
        if (cost.rock > resources.rock) return false;
        if (cost.food > resources.food) return false;
        return true;
    }
    public void AddResources(Resources r)
    {
        resources.gold += r.gold;
        resources.wood += r.wood;
        resources.rock += r.rock;
        resources.food += r.food;
        hud.SetResources(resources);
    }
    public void AddResourcesFromDestroy(Resources r)
    {
        resources.gold += (int)(r.gold / returnDivider);
        resources.wood += (int)(r.wood / returnDivider);
        resources.rock += (int)(r.rock / returnDivider);
        resources.food += (int)(r.food / returnDivider);
        hud.SetResources(resources);
    }

    public void TakeResources(Resources r)
    {
        resources.gold -= r.gold;
        resources.wood -= r.wood;
        resources.rock -= r.rock;
        resources.food -= r.food;
        hud.SetResources(resources);
    }
}
