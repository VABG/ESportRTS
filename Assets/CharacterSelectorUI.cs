using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] Color activeColor;
    Transform targetTransform;
    Image img;
    bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 90);
            transform.position = targetTransform.position;
        }
    }

    public void Activate(Transform t)
    {
        targetTransform = t;
        img.color = activeColor;
        active = true;
        transform.position = targetTransform.position;
    }

    public void Deactivate()
    {
        img.color = Color.clear;
        active = false;
        targetTransform = null;
    }
}
