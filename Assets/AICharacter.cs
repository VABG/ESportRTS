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
    [SerializeField]GameObject healthBar;
    AIState state;
    Resource activeResource;
    AICharacter toKill;
    Resources resources = new Resources();
    BaseStructure nearestBase;

    Animator animator;
    NavMeshAgent navMeshAgent;
    [SerializeField] GameObject selectedVisuals;
    int velocityID;

    bool doingAction = false;
    float actionTime = 0;
    [SerializeField] float health = 10;
    float maxHealth = 0;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        animator = GetComponentInChildren<Animator>();
        velocityID = Animator.StringToHash("Velocity");
        navMeshAgent = GetComponent<NavMeshAgent>();
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

    public void SetKillTarget(AICharacter ai)
    {
        toKill = ai;
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
                AttackBehaviour();
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

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        SetHealthScale(health / maxHealth);
        if (health <= 0) Destroy(this.gameObject);
    }

    public void SetHealthScale(float scale)
    {
        healthBar.transform.localScale = 
            new Vector3(scale, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

    }

    private void AttackBehaviour()
    {
        if (doingAction)
        {
            actionTime -= Time.deltaTime;
            if (actionTime <= 0)
            {
                doingAction = false;
            }
            return;
        }

        if (toKill == null)
        {
            state = AIState.Idle;
        }
        else
        {
            if (DistanceToTarget(toKill.transform.position) > 2)
            {
                navMeshAgent.SetDestination(toKill.transform.position);
            }
            else
            {
                toKill.TakeDamage(2);
                animator.SetTrigger("Attack");
                doingAction = true;
                actionTime = .5f;
            }
        }
    }

    private void IdleBehaviour()
    {

    }

    private void MovingBehaviour()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete) state = AIState.Idle;
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
            animator.SetTrigger("Attack");
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
            animator.SetTrigger("Attack");
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
