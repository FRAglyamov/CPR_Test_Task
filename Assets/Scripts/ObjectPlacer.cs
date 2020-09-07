using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject gameObjectToInstanstiate;

    private GameObject _spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 _touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Camera cam;
    [SerializeField]
    private ObjectMode mode = ObjectMode.Move;

    public enum ObjectMode
    {
        Move = 0,
        Rotate = 1,
        Scale = 2
    };

    private void Awake()
    {
        _arRaycastManager = FindObjectOfType<ARRaycastManager>();
        cam = Camera.main;
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out _touchPosition))
        {
            return;
        }
        switch (mode)
        {
            case ObjectMode.Move:
                MoveObject();
                break;
            case ObjectMode.Rotate:
                if(isObjectCreated() == false)
                    return;
                RotateObject();
                break;
            case ObjectMode.Scale:
                if (isObjectCreated() == false)
                    return;
                ScaleObject();
                break;
            default:
                break;
        }
    }

    public bool isObjectCreated()
    {
        if (_spawnedObject == null)
        {
            // Show error message in UI what object is not created yet
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Перемещаем объект в точку касания, если там есть поверхность. Если объекта нет - создаём его.
    /// </summary>
    private void MoveObject()
    {
        if (_arRaycastManager.Raycast(_touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if (_spawnedObject == null)
            {
                _spawnedObject = Instantiate(gameObjectToInstanstiate, hitPose.position, hitPose.rotation);
            }
            else
            {
                _spawnedObject.transform.position = hitPose.position;
            }
        }
    }

    /// <summary>
    /// Вращение объекта
    /// </summary>
    void RotateObject()
    {
        Vector3 touchDir = GetTouchDirection();
        _spawnedObject.transform.Rotate(0f, touchDir.x * -300f, 0f);
    }
    /// <summary>
    /// Изменение размера объекта
    /// </summary>
    void ScaleObject()
    {
        Vector3 touchDir = GetTouchDirection();

        if (_spawnedObject.transform.localScale.x < 0.1f && touchDir.x < 0)
        {
            // Если объект меньше 1/10 своего размера, то не уменьшаем его
        }
        else
        {
            _spawnedObject.transform.localScale += Vector3.one * touchDir.x;
        }
    }

    private Vector3 GetTouchDirection()
    {
        Vector3 curPos = cam.ScreenToViewportPoint(Input.touches[0].position);
        Vector3 lastPos = cam.ScreenToViewportPoint(Input.touches[0].position - Input.touches[0].deltaPosition);
        return curPos - lastPos;
    }

    public void ChangeObjectMode(int newMode)
    {
        mode = (ObjectMode)newMode;
    }
}
