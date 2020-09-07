using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShowTips : MonoBehaviour
{
    [SerializeField] GameObject moveTip;
    [SerializeField] GameObject tapTip;
    [SerializeField] ObjectPlacer objectPlacer;
    [SerializeField] ARPlaneManager planeManager;
    bool isCompleteAllStages = false;

    void Update()
    {
        if (isCompleteAllStages)
            return;
        // if object is created
        if (objectPlacer.isObjectCreated())
        {
            tapTip.SetActive(false);
            isCompleteAllStages = true;
            return;
        }
        // if we have planes
        if (planeManager.trackables.count > 0)
        {
            moveTip.SetActive(false);
            tapTip.SetActive(true);
        }

    }
}
