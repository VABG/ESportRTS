using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacter : MonoBehaviour
{

    Animator animator;
    NavMeshAgent navMeshAgent;
    [SerializeField] GameObject selectedVisuals;
    int velocityID;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        velocityID = Animator.StringToHash("Velocity");
        navMeshAgent = GetComponent<NavMeshAgent>();
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

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(velocityID, navMeshAgent.velocity.magnitude);
    }

    public void SetMovePosition(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
    }
}
