using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    int gold;
    Structure activeBuilding;
    Structure selectedBuilding;
    AICharacter selectedAI;
    PlayerResources resources;
    List<Structure> structures;
    Camera cam;

    [SerializeField] LayerMask placementLayerMask;
    [SerializeField] LayerMask selectionLayerMask;
    [SerializeField] EventSystem eventSystem;
    LocalNavMeshBuilder navBuilder;
    [SerializeField] WorldCanvas worldCanvas;

    public void StartPlacingBuilding(Structure buildingType)
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
        resources = GetComponent<PlayerResources>();
    }

    // Update is called once per frame
    void Update()
    {
        SelectedBuildingInput();
        TrySelect();
        TryPlacingBuilding();
        TryMoveAI();
    }

    void TryMoveAI()
    {
        if (Input.GetMouseButtonDown(1) && selectedAI != null)
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 5000))
            {
                Resource resource = hit.collider.GetComponent<Resource>();
                if (resource != null)
                {
                    selectedAI.SetResource(resource);
                    selectedAI.SetState(AIState.Collecting);
                }
                else
                {
                    selectedAI.SetState(AIState.Moving);
                    selectedAI.SetMovePosition(hit.point);
                }
            }
        }
    }

    void TrySelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedBuilding = null;
            if (selectedAI != null) selectedAI.Deselect();
            selectedAI = null;
            worldCanvas.ClearSelectedCharacter();
            if (activeBuilding != null) return;
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 5000, selectionLayerMask))
            {
                if (hit.collider.gameObject.layer == 7)
                {
                    StructureCollider sc = hit.collider.GetComponent<StructureCollider>();
                    if (sc != null)
                    {
                        Structure s = sc.GetParent();
                        if (s != null)
                        {
                            selectedBuilding = s;
                            selectedAI = null;
                            return;
                        }
                    }
                }

                else if (hit.collider.gameObject.layer == 6)
                {
                    AICharacter c = hit.collider.GetComponent<AICharacter>();
                    if (c != null)
                    {
                        selectedBuilding = null;
                        selectedAI = c;
                        selectedAI.Select();
                        worldCanvas.SetSelectedCharacter(c.gameObject);
                    }
                }
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
                navBuilder.FlagForUpdate();
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
            //Placement error here!

            if (canPlace && !eventSystem.IsPointerOverGameObject() && resources.CanAfford(activeBuilding.cost))
            {
                if (Input.GetMouseButtonDown(0))
                {

                    structures.Add(activeBuilding);
                    resources.TakeResources(activeBuilding.cost);
                    activeBuilding.PlaceBuilding();
                    activeBuilding = null;
                    //TODO: Replace with local update at object placement
                    navBuilder.UpdateNavMesh(true);
                }
            }
        }
    }
}
