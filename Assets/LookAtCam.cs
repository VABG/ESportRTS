using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = target.rotation;
    }

    public void UpdateLookAt()
    {
        transform.rotation = target.rotation;
    }
}