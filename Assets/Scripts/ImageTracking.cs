using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTracking : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] imagePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();


        // Instantiate prefabs and add this go's in dictionary
        foreach (GameObject p in imagePrefabs)
        {
            GameObject newPrefab = Instantiate(p, Vector3.zero, Quaternion.identity);
            newPrefab.name = p.name;
            spawnedPrefabs.Add(p.name, newPrefab);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            //Debug.Log($"Removed tracked image: {trackedImage.referenceImage.name}");
            spawnedPrefabs[trackedImage.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        if (spawnedPrefabs == null)
        {
            //Debug.LogError("spawnedPrefabs == null");
            return;
        }

        string name = trackedImage.referenceImage.name;

        if (trackedImage.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            spawnedPrefabs[name].SetActive(false);
            return;
        }
        //Debug.Log($"trackedImage.referenceImage.name: {name}");
        //GameObject prefab = spawnedPrefabs[name];
        //prefab.transform.position = trackedImage.transform.position;
        //prefab.transform.rotation = trackedImage.transform.rotation;
        //prefab.SetActive(true);

        spawnedPrefabs[name].SetActive(true);
        spawnedPrefabs[name].transform.position = trackedImage.transform.position;
        spawnedPrefabs[name].transform.rotation = trackedImage.transform.rotation;
    }
}
