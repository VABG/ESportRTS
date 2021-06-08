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
    List<AICharacter> selectedAI;
    PlayerResources resources;
    List<Structure> structures;
    HUD hud;
    Camera cam;
    Vector3 mouseDragStartPos;

    [SerializeField] GameObject selectCube;

    [SerializeField] LayerMask placementLayerMask;
    [SerializeField] LayerMask selectionLayerMask;
    [SerializeField] LayerMask marqueeLayerMask;
    [SerializeField] EventSystem eventSystem;
    LocalNavMeshBuilder navBuilder;
    [SerializeField] WorldCanvas worldCanvas;
    [SerializeField] Raket raket;

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
        selectedAI = new List<AICharacter>();
        structures = new List<Structure>();
        cam = GetComponentInChildren<Camera>();
        navBuilder = FindObjectOfType<LocalNavMeshBuilder>();
        resources = GetComponent<PlayerResources>();
        hud = FindObjectOfType<HUD>();
    }

    // Update is called once per frame
    void Update()
    {
        SelectedBuildingInput();
        TrySelect();
        TryPlacingBuilding();
        TryMoveAI();
        ShootRocket();
    }

    void ShootRocket()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 5000))
            {
                Raket raketSpawn = Instantiate(raket, cam.transform.position, cam.transform.rotation);
                raketSpawn.SetTargetPosition(hit.point);
            }
        }
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
                    AIResource(resource);
                    //selectedAI.SetResource(resource);
                    AIState(global::AIState.Collecting);
                    return;
                }
                AICharacter c = hit.collider.GetComponent<AICharacter>();
                if (c != null)
                {
                    AIKillTarget(c);
                    //if (c == selectedAI) return;
                    //selectedAI.SetKillTarget(c);
                    //selectedAI.SetState(AIState.Fighting);
                    return;
                }

                AIState(global::AIState.Moving);
                AIMovePosition(hit.point);
            }
        }
    }

    void AIResource(Resource resource)
    {
        foreach (AICharacter ai in selectedAI)
        {
            ai.SetResource(resource);
        }
    }

    void AIKillTarget(AICharacter c)
    {
        if (selectedAI.Contains(c)) return;
        foreach (AICharacter ai in selectedAI)
        {
            ai.SetKillTarget(c);
            ai.SetState(global::AIState.Fighting);
        }
    }

    void AIMovePosition(Vector3 position)
    {
        foreach (AICharacter ai in selectedAI)
        {
            ai.SetMovePosition(position);
        }
    }

    void AIState(AIState state)
    {
        foreach (AICharacter ai in selectedAI)
        {
            ai.SetState(state);
        }
    }

    void AIDeselect()
    {
        foreach (AICharacter ai in selectedAI)
        {
            ai.Deselect();
        }
        selectedAI.Clear();
    }

    void TrySelect()
    {

        if (Input.GetMouseButtonDown(0) && !eventSystem.IsPointerOverGameObject())
        {
            mouseDragStartPos = Input.mousePosition;
            hud.ClearState();
            selectedBuilding = null;
            if (selectedAI.Count != 0 && !Input.GetKey(KeyCode.LeftShift)) AIDeselect();
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
                            selectedAI.Clear();
                            hud.SetUIBuildingState(s.GetBuildingType(), s);
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

                        selectedAI.Add(c);
                        c.Select();
                        worldCanvas.SetSelectedCharacter(c.gameObject);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseDragEndPos = Input.mousePosition;
            mouseDragEndPos.z += 10;
            mouseDragStartPos.z += 10;

            Ray startRay = cam.ScreenPointToRay(mouseDragStartPos);
            Ray endRay = cam.ScreenPointToRay(mouseDragEndPos);

            Vector3 mStart = startRay.origin + startRay.direction * (Mathf.Abs(startRay.origin.y)/Mathf.Abs(startRay.direction.y));
            Vector3 mEnd = endRay.origin + endRay.direction * (Mathf.Abs(endRay.origin.y) / Mathf.Abs(endRay.direction.y));
            
            //Debug.DrawLine(mStart, mEnd);
            SelectGroup(mStart + (mEnd - mStart)/2, (mEnd - mStart) / 2);
        }
    }

    void SelectGroup(Vector3 center, Vector3 halfExtent)
    {
        halfExtent = new Vector3(Mathf.Abs(halfExtent.x), Mathf.Abs(halfExtent.y), Mathf.Abs(halfExtent.z));
        halfExtent.y = 5;

        selectCube.transform.position = center;
        selectCube.transform.localScale = halfExtent * 2;


        RaycastHit[] rHits = Physics.BoxCastAll(center, halfExtent, Vector3.down, Quaternion.identity, 1000, marqueeLayerMask);

        bool foundAI = false;
        foreach (RaycastHit h in rHits)
        {
            AICharacter c = h.transform.GetComponent<AICharacter>();
            if (c && !selectedAI.Contains(c))
            {
                selectedAI.Add(c);
                c.Select();
                foundAI = true;
            }
        }
        if (selectedBuilding != null && foundAI)
        {
            selectedBuilding = null;
        }
    }

    void SelectedBuildingInput()
    {
        if (selectedBuilding != null)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                // Get funds back? Other actions?
                resources.AddResourcesFromDestroy(selectedBuilding.cost);
                Destroy(selectedBuilding.gameObject);
                navBuilder.FlagForUpdate();
                hud.ClearState();
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
