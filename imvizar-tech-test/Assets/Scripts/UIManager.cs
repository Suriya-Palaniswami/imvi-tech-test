using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEditor;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI welcome, to, stageReality;

    [SerializeField] private SessionStore sessionStore;

    [SerializeField] private Transform scrollViewContent;

    [SerializeField] private GameObject ViewAndInitiateCreateScreen;

    [SerializeField] private GameObject backButton;

    [SerializeField] private GameObject saveInfoScreen;

    [SerializeField] public GameObject _prefabPreviewRawImage;  

    [SerializeField] private Transform horizontalScrollViewContent;

    [SerializeField] private GameObject ARScreen;

    [SerializeField] private GameObject backgroundImage;

    [SerializeField] private ARSessionOrigin _arSessionOrigin;

    [SerializeField] private ARSession _arSession;

    [SerializeField] private GameObject logo;

    [SerializeField] private List<GameObject> _rootScreens;

    [SerializeField] private TMP_InputField _sessionName;

    [SerializeField] private TMP_InputField _creatorName;

    [SerializeField] private List<GameObject> _objectsInSessionList;

    [SerializeField] private List<GameObject> _objectsInSessionListPrivate;

    [SerializeField] private List<ObjectInSession> sessionStoreList;

    [SerializeField] RawImage PrefabPreviewImageInARScene;

    [SerializeField] private PlaceOnPlane _placeOnPlane;

    [SerializeField] private List<IntantiateAndPlaceOnPlane> previewIcons;

    // Start is called before the first frame update
    void Start()
    {
        AnimateWelcomeText();
        _objectsInSessionListPrivate = new List<GameObject>();
        ReadPersistentSOData();


    }
    private void OnEnable()
    {
        EventManager.OnButtonAction += ManageUIEvents;
        EventManager.OnObjectInstantiationAction += ManageObjectEvents;
    }
   
    private void OnDisable()
    {
        EventManager.OnButtonAction -= ManageUIEvents;
        EventManager.OnObjectInstantiationAction -= ManageObjectEvents;
    
    }

    private void ManageObjectEvents(GameObject prefabToInstantiate, object _o)
    {
        //Two Different Types of Calls
        if (_o != null && prefabToInstantiate != null)//On Object Selection In Creation Scene
        {
            _objectsInSessionList.Add(prefabToInstantiate);
            _objectsInSessionListPrivate.Add(prefabToInstantiate);
        }
        if (_o == null)//On Object Instantiation
        {
            _objectsInSessionList.Remove(prefabToInstantiate);
            if (_objectsInSessionList.Count != 0)
            {
                //Create Prefab Preview for Scroll Content
                PrefabPreviewImageInARScene.texture = RuntimePreviewGenerator.GenerateModelPreview(_objectsInSessionList[0].gameObject.transform);
                _placeOnPlane.m_PlacedPrefab = _objectsInSessionList[0].gameObject;

            }
        }
    }

    private void ManageUIEvents(ButtonType buttonType, object _o)
    {
        TurnOffAllScreens();
        switch (buttonType)
        {
            case ButtonType.Back:
                BackButton();
                break;
            case ButtonType.SaveCreationInfo:
                PopulateARScene();
                break;
            case ButtonType.CreateNewExperience:
                PopulateInfoScreen();
                break;
            case ButtonType.Save:
                GetEndTransformAndSaveData();
                break;
        }
    }
    private void AnimateWelcomeText()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(welcome.DOFade(1f,2f));
        sequence.Append(to.DOFade(1f, 2f));
        sequence.Append(welcome.DOFade(0f, 0.5f));
        sequence.Append(to.DOFade(0f, 0.5f));
        sequence.Append(stageReality.DOFade(1f,2f));
        sequence.Append(stageReality.DOFade(0f,2f));
        sequence.Play().OnComplete(SetupMenuScreen);
    }

    private void SetupMenuScreen()
    {
        ViewAndInitiateCreateScreen.SetActive(true);
        
        backButton.SetActive(true);
        PopulateSessionList();

    }
    private void BackButton()
    {
        ViewAndInitiateCreateScreen.SetActive(true);
        ARScreen.SetActive(false);
        backgroundImage.SetActive(true);
        _arSessionOrigin.gameObject.SetActive(false);
        _arSessionOrigin.camera.enabled = false;
    }

    private void TurnOffAllScreens()
    {
        foreach (GameObject rootScreen in _rootScreens)
        {
            rootScreen.SetActive(false);
        }
    }

    private void PopulateSessionList()
    {
        foreach (SessionDetails sessionDetails in sessionStore.SessionDetailsList)
        {
            GameObject newSessionEntry = Instantiate(sessionStore.SessionDisplayTemplate);
            newSessionEntry.transform.SetParent(scrollViewContent);
            SessionDisplayRefHolder newSessionDisplayRefHolder =  newSessionEntry.GetComponent<SessionDisplayRefHolder>();
            newSessionDisplayRefHolder.gameObject.name = sessionDetails.SessionName;
            newSessionDisplayRefHolder._sessionName.text = sessionDetails.SessionName;
            newSessionDisplayRefHolder._cretorName.text = sessionDetails.CreatorName;

        }
    }

    private void PopulateInfoScreen()
    {
        saveInfoScreen.SetActive(true);

        foreach (GameObject child in sessionStore.objectList)
        {
            GameObject newRawImage = Instantiate(_prefabPreviewRawImage);
            newRawImage.transform.SetParent(horizontalScrollViewContent);
            newRawImage.GetComponent<RawImage>().texture = RuntimePreviewGenerator.GenerateModelPreview(child.transform);
            newRawImage.name = child.name;
            previewIcons.Add(newRawImage.GetComponent<IntantiateAndPlaceOnPlane>());
        }
    }

    private void PopulateARScene()
    {
        backgroundImage.SetActive(false);
        _arSessionOrigin.gameObject.SetActive(true);
        _arSessionOrigin.camera.enabled = true;
        ARScreen.SetActive(true);
        logo.SetActive(false);
        PrefabPreviewImageInARScene.texture = RuntimePreviewGenerator.GenerateModelPreview(_objectsInSessionList[0].gameObject.transform);
        _placeOnPlane.m_PlacedPrefab = _objectsInSessionList[0].gameObject;
        _placeOnPlane.readyToInstantiate = true;
    }

    private void GetEndTransformAndSaveData()
    {
        foreach (GameObject obj in _objectsInSessionListPrivate)
        {
            sessionStoreList.Add(new ObjectInSession(obj, new ObjectDataRecord(obj.transform.position,obj.transform.rotation)));
        }
        sessionStore.SessionDetailsList.Add(new SessionDetails(_sessionName.text.ToString(), _creatorName.text.ToString(), sessionStoreList));
        sessionStoreList = new List<ObjectInSession>();
        TurnOffAllScreens();
        backgroundImage.SetActive(true);
        _arSessionOrigin.gameObject.SetActive(false);
        _arSessionOrigin.camera.enabled = false;
        _arSession.Reset();//Reset AR Session 
        SetupMenuScreen();
        SaveSODataToBePersistent();
        _placeOnPlane.placedList.Clear();
        _placeOnPlane.placedPrefabCount = 0;
        foreach (IntantiateAndPlaceOnPlane intantiateAndPlaceOnPlane in previewIcons)
        {
            intantiateAndPlaceOnPlane._blueCheck.enabled = false;
            intantiateAndPlaceOnPlane._placed = false;
            intantiateAndPlaceOnPlane._selected = false;
        }

        
    }

    private void SaveSODataToBePersistent()
    {
        Debug.Log("Call "+ Application.persistentDataPath + "/GameData.json");
        string sO = JsonUtility.ToJson(sessionStore);
        File.WriteAllText(Application.persistentDataPath + "/GameData.json", sO);
        var json = JsonUtility.ToJson(sessionStore);
        
    }

    private void ReadPersistentSOData()
    {
        if (File.Exists(Application.persistentDataPath +  "GameData"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "GameData", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), sessionStore);
            file.Close();
        }


    }





}
