using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private TMP_Text _planeText;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private ARPointCloudManager _arPointCloudManager;
    [SerializeField] private GameObject _button;

    private List<ARPlane> _activePlanes = new List<ARPlane>();
    private List<ARPointCloud> _activePointClouds = new List<ARPointCloud>();

    private GameObject _topPanel;

    // Start is called before the first frame update
    void Start()
    {
        _topPanel =  GameObject.Find("TopPanelElements");
        // Add callbacks
        ARSession.stateChanged += OnARSessionStateChanged;
        _arPlaneManager.planesChanged += OnPlanesChanged;
        _arPointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
    }

    // Update is called once per frame
    void Update()
    {
        int numOfPoints = 0;
        foreach (ARPointCloud cloud in _activePointClouds)
        {
            numOfPoints = numOfPoints + cloud.identifiers.Value.Length;
        }
        _planeText.text = $"% planes: {_activePlanes.Count}\n % points: {numOfPoints}";
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        _stateText.text = args.state.ToString();
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // handle added planes
        foreach (ARPlane plane in args.added)
        {
            if (!_activePlanes.Contains(plane))
            {
                _activePlanes.Add(plane);
            }
        }

        // handle removed planes
        foreach (ARPlane plane in args.removed)
        {
            if (_activePlanes.Contains(plane))
            {
                _activePlanes.Remove(plane);
            }
        }

        // handle merged planes
        foreach (ARPlane plane in args.updated)
        {
            if (plane.subsumedBy != null && _activePlanes.Contains(plane.subsumedBy))
            {
                _activePlanes.Remove(plane);
            }
            else if (plane.subsumedBy == null && !_activePlanes.Contains(plane))
            {
                _activePlanes.Add(plane);
            }
        }
    }

    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        // handle added clouds
        foreach (ARPointCloud cloud in args.added)
        {
            if (!_activePointClouds.Contains(cloud))
            {
                _activePointClouds.Add(cloud);
            }
        }

        // handle removed clouds
        foreach (ARPointCloud cloud in args.removed)
        {
            if (_activePointClouds.Contains(cloud))
            {
                _activePointClouds.Remove(cloud);
            }
        }

        // handle updated clouds
        foreach (ARPointCloud cloud in args.updated)
        {
            if (_activePointClouds.Contains(cloud))
            {
                int index = _activePointClouds.IndexOf(cloud);
                if (index != -1)
                {
                    _activePointClouds[index] = cloud;
                }
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("button clicked");
        if (_topPanel.activeSelf)  _topPanel.SetActive(false);
        else _topPanel.SetActive(true);
        // _button.SetActive(true);
    }

}



