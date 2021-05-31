using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] Resources resources;
    [SerializeField] Resources takePerCollection;
    [SerializeField] float collectionTime = 2;
    bool isEmpty = false;
    
    public Resources CollectResources()
    {
        return CollectResourcesFromSource();
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    private Resources CollectResourcesFromSource()
    {
        resources.food -= takePerCollection.food;
        int food = takePerCollection.food;
        if (resources.food < 0)
        {
            food += resources.food;
            resources.food = 0;
        }

        resources.wood -= takePerCollection.wood;
        int wood =  takePerCollection.wood;
        if (resources.wood < 0)
        {
            wood += resources.wood;
            resources.wood = 0;
        }

        resources.rock -= takePerCollection.rock;
        int rock = takePerCollection.rock;
        if (resources.rock < 0)
        {
            rock += resources.rock;
            resources.rock = 0;
        }
        
        resources.gold -= takePerCollection.gold;
        int gold = takePerCollection.gold;
        if (resources.gold < 0)
        {
            gold += resources.gold;
            resources.gold = 0;
        }
        CheckIsEmpty();
        return new Resources { food = food, gold = gold, rock = rock, wood = wood };
    }

    public float CollectionTime()
    {
        return collectionTime;
    }

    void CheckIsEmpty()
    {
         isEmpty = resources.food == 0 && resources.gold == 0 && resources.rock == 0 && resources.wood == 0;
    }
}
