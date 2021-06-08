using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raket : MonoBehaviour
{
    [SerializeField] float forceMult = 50;
    public void SetTargetPosition(Vector3 position)
    {
        target = position;
        rb = GetComponent<Rigidbody>();
    }
    Vector3 target;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 direction = target - this.transform.position;
        direction.Normalize();
        rb.AddForce(direction * forceMult, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 20, Vector3.down);
        foreach (RaycastHit h in hits)
        {
            
            AICharacter c = h.transform.GetComponent<AICharacter>();
            if (c) c.Kill(transform.position);
            else
            {
                Rigidbody r = h.transform.GetComponent<Rigidbody>();
                if (r) r.AddForce((r.transform.position - transform.position).normalized * 10, ForceMode.VelocityChange);
            }
        }
        Destroy(this.gameObject);
    }
}
