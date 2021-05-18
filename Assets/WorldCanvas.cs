using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : MonoBehaviour
{
    [SerializeField] CharacterSelectorUI charSelect;
    // Start is called before the first frame update
    void Start()
    {
        charSelect.Deactivate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSelectedCharacter(GameObject g)
    {
        charSelect.Activate(g.transform);
    }

    public void ClearSelectedCharacter()
    {
        charSelect.Deactivate();
    }
}
