using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInstructionScript : MonoBehaviour
{
    [SerializeField] private GameObject textGameObject;
    private TextMeshProUGUI textDescription;
    // Start is called before the first frame update
    void Start()
    {
        textDescription = textGameObject.GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string s)
    {
        textGameObject.GetComponent<TextMeshProUGUI>().text = s;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
