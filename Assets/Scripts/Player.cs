using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    int funds;
    Structure activeBuilding;
    Structure selectedBuilding;

    List<Structure> structures;
    Camera cam;
    [SerializeField] LayerMask placementLayerMask;
    [SerializeField] LayerMask selectionLayerMask;
    [SerializeField] EventSystem eventSystem;
    LocalNavMeshBuilder navBuilder;
    public void PlaceBuilding(Structure buildingType)
    {
        CancelBuildingPlacing();
        activeBuilding = Instantiate(buildingType);
    }

    public void CancelBuildingPlacing()
    {
        if (activeBuilding != null) Destroy(activeBuilding.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        structures = new List<Structure>();
        cam = GetComponentInChildren<Camera>();
        navBuilder = FindObjectOfType<LocalNavMeshBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        SelectedBuildingInput();
        TrySelectBulding();
        TryPlacingBuilding();
    }

    void TrySelectBulding()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (activeBuilding != null) return;
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 5000, selectionLayerMask))
            {
                StructureCollider sc = hit.collider.GetComponent<StructureCollider>();
                if (sc == null) return;
                Structure s = sc.GetParent();
                if (s != null)
                {
                    selectedBuilding = s;
                    return;
                }
            }
            else
            {
                selectedBuilding = null;
            }
        }
    }

    void SelectedBuildingInput()
    {
        if (selectedBuilding != null)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                // Get funds back? Other actions?
                Destroy(selectedBuilding.gameObject);
            }
        }
    }

    void TryPlacingBuilding()
    {
        if (activeBuilding != null)
        {
            selectedBuilding = null;
            if (Input.GetKey(KeyCode.Q))
            {
                activeBuilding.Rotate(-Time.deltaTime * 180);
            }
            if (Input.GetKey(KeyCode.E))
            {
                activeBuilding.Rotate(Time.deltaTime * 180);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuildingPlacing();
                return;
            };

            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 5000, placementLayerMask))
            {
                activeBuilding.SetPosition(hit.point);
            }
            bool canPlace = activeBuilding.CanPlace();
            //Error here!

            if (canPlace && !eventSystem.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {

                    structures.Add(activeBuilding);
                    funds -= activeBuilding.cost;
                    activeBuilding.PlaceBuilding();
                    activeBuilding = null;
                    //TODO: Replace with local update at object placement
                    navBuilder.UpdateNavMesh(true);
                }
            }
        }
    }
}
