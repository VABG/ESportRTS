using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingType
{
    None,
    Generic,
    Church,    
}

public class HUD : MonoBehaviour
{
    [SerializeField] Text gold;
    [SerializeField] Text wood;
    [SerializeField] Text rock;
    [SerializeField] Text food;

    [SerializeField] GameObject buildMenu;
    [SerializeField] GameObject churchMenu;
    BuildingType buildingState;

    Structure activeBuilding;
    // Start is called before the first frame update
    void Start()
    {
        churchMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResources(Resources r)
    {
        gold.text = r.gold.ToString();
        wood.text = r.wood.ToString();
        rock.text = r.rock.ToString();
        food.text = r.food.ToString();
    }

    public void SetUIBuildingState(BuildingType state, Structure building)
    {
        activeBuilding = building;
        buildingState = state;
        switch (state)
        {
            case BuildingType.Church:
                buildMenu.SetActive(false);
                churchMenu.SetActive(true);
                break;
            default:
                ClearState();
                break;
        }
    }

    public void ClearState()
    {
        switch (buildingState)
        {
            case BuildingType.Church:
                churchMenu.SetActive(false);
                break;
        }
        buildMenu.SetActive(true);
    }


    public void MakeHuman()
    {
        activeBuilding.GetComponent<BaseStructure>().MakeHuman();
    }
}
