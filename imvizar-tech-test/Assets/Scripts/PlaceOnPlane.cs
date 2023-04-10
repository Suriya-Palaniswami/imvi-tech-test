using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using DG.Tweening;
using System;
using TMPro;
/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    public GameObject m_PlacedPrefab;

    UnityEvent placementUpdate;

    [SerializeField]
    public GameObject visualObject;

    [SerializeField]public List<GameObject> placedList = new List<GameObject>();

    [SerializeField]
    private int maxPrefabSpwanCount = 0;
    public int placedPrefabCount;


    [SerializeField] public bool GoTo = false;

    [SerializeField] public GameObject GoToButton;

    [SerializeField] private TextMeshProUGUI hintText;

    public bool AllObjectsSpawned = false;
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject AnchorSpawnedObject { get;  set; }
    public GameObject SpawnedObject { get;  set; }
    public GameObject ActorSpawnedObject { get; set; }

    public bool readyToInstantiate = false;
    

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();

        if (placementUpdate == null)
            placementUpdate = new UnityEvent();

        placementUpdate.AddListener(DiableVisual);
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {

    }
    
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;
            //Spawn a prefab if the placedCount is less that max
            if (placedPrefabCount < maxPrefabSpwanCount)
            {
                    SpawnPrefab(hitPose);
            }
            placementUpdate.Invoke();

        }


    }

    public void SetPrefab(GameObject prefabType)
    {
        m_PlacedPrefab = prefabType;
    }
    private void SpawnPrefab(Pose hitPose)
    {
        if (placedList.Contains(m_PlacedPrefab))
        {
            //D0 Nothing
        }
        else
        {
            //Instantiate and Assign
            //Add to List
            //Raise new Event to Indicate the System of Spawn
            //This marks the end of One Instantiation Cycle
            SpawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
            placedList.Add(SpawnedObject);
            EventManager.DoObjectInstantiation(m_PlacedPrefab, null);
        }
        placedPrefabCount++;
    }

    public void LerpTo()
    {
        hintText.text = "Trying?"; 
        try
        {
            hintText.text = "Walking";
            ActorSpawnedObject = GameObject.FindGameObjectWithTag("Character");
            AnchorSpawnedObject = GameObject.FindGameObjectWithTag("Anchor");
            ActorSpawnedObject.transform.DOMove(AnchorSpawnedObject.transform.position, 3.5f).OnComplete(() =>SetTransform());
           

        }
        catch (Exception e)
        {
            hintText.text = "Exception"+ActorSpawnedObject.name+AnchorSpawnedObject.name;
        }
    }

    private void SetTransform()
    {
        ActorSpawnedObject.transform.SetParent(AnchorSpawnedObject.transform);
        GetTaskCScriptAndCall();
    }

    private void GetTaskCScriptAndCall()
    {
        GameObject.FindGameObjectWithTag("TaskC").GetComponent<TaskC>().DoTaskC(ActorSpawnedObject);
    }
    public void DiableVisual()
    {
        visualObject.SetActive(false);
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}