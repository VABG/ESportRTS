using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSync : MonoBehaviour
{
    [SerializeField] Camera syncCam;
    Camera thisCam;
    // Start is called before the first frame update
    void Start()
    {
        thisCam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        thisCam.fieldOfView = syncCam.fieldOfView;
    }
}
