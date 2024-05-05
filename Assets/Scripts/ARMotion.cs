using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARMotion : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private ARSessionOrigin _sessionOrigin;
    [SerializeField] private GameObject scenePrefab;

    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject _arCamera;
    [SerializeField] private StoryManager storyManager;

    private Transform trackedImageTransform = null;
    private Vector3 tracked_position;
    private Quaternion tracked_rotation;
    private GameObject scene = null;

    private float count = 1.0f;

    void OnEnable() { 
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        //ARSession.stateChanged += OnARSessionStateChanged;
    }
    void OnDisable() { 
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        //ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        _stateText.text = args.state.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if(scene != null)
        {
            scene.transform.position = tracked_position;
            scene.transform.rotation = tracked_rotation;
            //scene.transform.localScale = new Vector3(.25f, .25f, .25f);
            //Debug.Log($"Current scene transform: {scene.transform.position}, {scene.transform.rotation}, {scene.transform.localScale}");
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            storyManager.FoundQR();
            Vector3 pos = newImage.transform.position /*+ new Vector3(0,-3,0)*/;
            tracked_position = pos;
            // Handle added event
            //scene = GameObject.Instantiate(scenePrefab, pos, newImage.transform.rotation);
            //scene.transform.SetParent(_arCamera.transform);
            //_sessionOrigin.MakeContentAppearAt(scenePrefab.transform, newImage.transform.position, newImage.transform.rotation);
            //scene.transform.localScale = new Vector3(.25f, .25f, .25f);

            //merge
            scene = GameObject.Find("Scene");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (!storyManager.GetIsLocked())
            {
                scene = GameObject.Find("Scene");
                // Handle updated event
                tracked_position = updatedImage.transform.position;
                tracked_rotation = updatedImage.transform.rotation;
                //Debug.Log($"updatedImage.transform: {updatedImage.transform.position}, {updatedImage.transform.rotation}");
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            Destroy(scene);
            scene = null;
        }
    }
}
