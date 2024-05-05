using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RaycastManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager _arRaycastManager;
    [SerializeField] private Camera _arCamera;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private StoryManager _storyManager;
    // Store raycast hits
    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

    // GameObject tobe spawned; null at first
    private GameObject _spawnedObject = null;

    [SerializeField] private LayerMask _layerMask;

    // Update is called once per frame
    void Update()
    {
        // handle screen touches
        if (Input.touchCount > 0)
        {
            if (!_storyManager.GetIsInDialogue()) { 
                Touch touch = Input.GetTouch(0);
                if (Physics.Raycast(_arCamera.ScreenPointToRay(Input.mousePosition), out var hitObject, Mathf.Infinity, _layerMask))
                {
                    Vector3 newPoint = hitObject.point;
                    if (_spawnedObject == null)
                    {
                        //_spawnedObject = GameObject.Instantiate(_prefab, newPoint, Quaternion.identity);
                        //_spawnedObject.transform.SetParent(GameObject.Find("Scene1").transform);

                        _spawnedObject = GameObject.Find("Mouse Movement");
                        //_spawnedObject.transform.LookAt(GameObject.Find("Trigger Area Exit").transform);
                    }
                    else
                    {
                        // Object already instantiated. Move it at touch position 
                        _spawnedObject.transform.position = newPoint;
                    }
                }
            }
        }
    }
}
