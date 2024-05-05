using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] public bool mouseEnter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Mouse")
        {
            mouseEnter = true;
        }
    }
    public bool IsMouseEnter()
    {
        return mouseEnter;
    }
}
