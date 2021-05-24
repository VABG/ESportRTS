using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Moving,
    Collecting,
    Returning,
    Fighting,
    Running,
}

public class AICharacter : MonoBehaviour
{
    AIState state;
    Resource activeResource;
    Resources resources = new Resources();
    BaseStructure nearestBase;

    Animator animator;
    NavMeshAgent navMeshAgent;
    [SerializeField] GameObject selectedVisuals;
    int velocityID;
    bool doingAction = false;
    float actionTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        velocityID = Animator.StringToHash("Velocity");
        navMeshAgent = GetComponent<NavMeshAgent>();
        resources = new Resources();
    }

    public void SetState(AIState state)
    {
        this.state = state;
        if (state == AIState.Collecting)
        {
            SetNearestBase();
            if (HasResources())
            {
                this.state = AIState.Returning;
                navMeshAgent.SetDestination(nearestBase.transform.position);
            }
            else if (activeResource.IsEmpty()) state = AIState.Idle;
            else navMeshAgent.SetDestination(activeResource.transform.position);
        }
    }

    private void SetNearestBase()
    {
        BaseStructure[] b = FindObjectsOfType<BaseStructure>();
        if (b == null) Debug.LogError("No base found!");
        float nearest = 100000000;
        int nearestArrayPos = 0;
        for (int i = 0; i < b.Length; i++)
        {
            float d = (b[i].transform.position - activeResource.transform.position).magnitude;
            if (d < nearest)
            {
                nearest = d;
                nearestArrayPos = i;
            }
        }
        nearestBase = b[nearestArrayPos];
    }

    public void Select()
    {
        if (selectedVisuals != null)
            selectedVisuals.SetActive(true);
    }

    public void Deselect()
    {
        if (selectedVisuals != null)
            selectedVisuals.SetActive(false);
    }

    public void SetResource(Resource r)
    {
        activeResource = r;
    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
            {
            case AIState.Idle:
                IdleBehaviour();
                break;
            case AIState.Moving:
                MovingBehaviour();
                break;
            case AIState.Fighting:
                break;
            case AIState.Collecting:
                CollectingBehaviour();
                break;
            case AIState.Returning:
                ReturningBehaviour();
                break;
            case AIState.Running:
                break;
        }
        animator.SetFloat(velocityID, navMeshAgent.velocity.magnitude);
    }

    private void IdleBehaviour()
    {

    }

    private void MovingBehaviour()
    {
        if (navMeshAgent.isStopped) state = AIState.Idle;
    }

    private void CollectingBehaviour()
    {       
        if (doingAction)
        {
            actionTime -= Time.deltaTime;
            if (actionTime <= 0)
            {
                resources = activeResource.CollectResources();
                navMeshAgent.SetDestination(nearestBase.transform.position);
                doingAction = false;
                state = AIState.Returning;
            }
            return;
        }
        if (DistanceToTarget(activeResource.transform.position) < 3)
        {
            navMeshAgent.ResetPath();
            doingAction = true;
            actionTime = activeResource.CollectionTime();
        }
        if (activeResource.IsEmpty())
        {
            state = AIState.Idle;
            navMeshAgent.ResetPath();
        }
    }

    private void ReturningBehaviour()
    {
        if (doingAction)
        {
            actionTime -= Time.deltaTime;
            if (actionTime <= 0)
            {
                nearestBase.GiveResources(resources);
                ClearResources();

                navMeshAgent.SetDestination(activeResource.transform.position);
                doingAction = false;
                if (activeResource.IsEmpty())
                {
                    state = AIState.Idle;
                    navMeshAgent.ResetPath();
                }
                else state = AIState.Collecting;
            }
            return;
        }
        if (DistanceToTarget(nearestBase.transform.position) < 2)
        {
            navMeshAgent.ResetPath();
            doingAction = true;
            actionTime = 1.0f;
        }
    }

    float DistanceToTarget(Vector3 target)
    {
        return (transform.position - target).magnitude;
    }

    public void SetMovePosition(Vector3 position)
    {
        //state = AIState.Moving;
        navMeshAgent.SetDestination(position);
    }

    public bool HasResources()
    {
        if (resources.food > 0) return true;
        if (resources.gold > 0) return true;
        if (resources.rock > 0) return true;
        if (resources.wood > 0) return true;
        return false;
    }

    public void ClearResources()
    {
        resources.food = 0;
        resources.gold = 0;
        resources.rock = 0;
        resources.wood = 0;
    }
}
