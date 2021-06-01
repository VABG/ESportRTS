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
        int food = TakeResource(ref resources.food, takePerCollection.food);
        int gold = TakeResource(ref resources.gold, takePerCollection.gold);
        int rock = TakeResource(ref resources.rock, takePerCollection.rock);
        int wood = TakeResource(ref resources.wood, takePerCollection.wood);

        CheckIsEmpty();
        return new Resources { food = food, gold = gold, rock = rock, wood = wood };
    }

    public int TakeResource(ref int resource, int takeAmount)
    {
        resource -= takeAmount;
        int take = takeAmount;
        if (resource < 0)
        {
            take += resource;
            resource = 0;
        }
        return take;
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
