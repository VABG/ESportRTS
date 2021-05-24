using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Text gold;
    [SerializeField] Text wood;
    [SerializeField] Text rock;
    [SerializeField] Text food;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResources(Resources r)
    {
        gold.text = r.gold.ToString();
        wood.text = r.wood.ToString();
        rock.text = r.rock.ToString();
        food.text = r.food.ToString();
    }
}
