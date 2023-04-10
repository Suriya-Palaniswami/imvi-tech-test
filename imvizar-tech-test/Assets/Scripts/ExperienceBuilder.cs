using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ImviObject
{
    [SerializeField] public GameObject RootObject;
    [SerializeField] public AnimationClip ObjectAnimationClip;

    public ImviObject(GameObject _rootObject, AnimationClip _objectAnimationClip)
    {
        RootObject = _rootObject;
        ObjectAnimationClip = _objectAnimationClip;
    }

}

public class StateManagement
{
    private static StateManagement _instance;
    public static StateManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StateManagement();
            }
            return _instance;
        }
    }

    public StateManagement()
    {
        _currentTimelineIndex = 0;
    }

    public int _currentTimelineIndex;
}

public class ExperienceBuilder : MonoBehaviour
{
    [SerializeField] public bool isNonLinear = false;

    public List<GameObject> ObjectsInExperience;

    [SerializeField]private List<string> _objectsInExperience;

    [SerializeField] private List<ImviObject> allObjects;

    

    private void Awake()
    {
        _objectsInExperience.Clear();
        foreach (GameObject objectInExperience in ObjectsInExperience)
        {
            _objectsInExperience.Add(objectInExperience.name.ToString());
            objectInExperience.GetComponent<ObjectTimelineHandler>().AudioSource = objectInExperience.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        EventManager.OnNextAction += HandleNextAction;
    }

    private void OnDisable()
    {
        EventManager.OnNextAction -= HandleNextAction;
    }

    private void HandleNextAction(int nextCall,object _o)
    {
        if (_o.ToString() != gameObject.name)
        {
            PlayOneByOne();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateImviObjectsForEditor()
    {
        foreach (GameObject objectInExperience in ObjectsInExperience)
        {
            PopulateImviObjectsList(objectInExperience, objectInExperience.GetComponent<Animator>());
        }
    }

    private void PopulateImviObjectsList(GameObject objectToAdd, Animator animator)
    {
        foreach (AnimationClip animationClip in animator.runtimeAnimatorController.animationClips)
        {
            allObjects.Add(new ImviObject(objectToAdd,animationClip));
        }
    }

    public void PlayOneByOneCall()
    {
        StartCoroutine("PlayOneByOne");

    }

    public void PlayOneByOne()
    {
        EventManager.DoObjectAction(new ImviObject(allObjects[StateManagement.Instance._currentTimelineIndex].RootObject,allObjects[StateManagement.Instance._currentTimelineIndex].ObjectAnimationClip),null);
        StartCoroutine("WaitAndIncrement");
        //allObjects[start].RootObject.GetComponent<Animator>().enabled = true;
        //allObjects[start].RootObject.GetComponent<Animator>().Play(allObjects[start].ObjectAnimationClip.name, 0, 0.0f);
        //yield return new WaitForSeconds(allObjects[start].RootObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        //start++;
        //if(start <allObjects.Count) StartCoroutine("PlayOneByOne");

    }

    IEnumerator WaitAndIncrement()
    {
        yield return new WaitForSeconds(allObjects[StateManagement.Instance._currentTimelineIndex].RootObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        if(StateManagement.Instance._currentTimelineIndex< allObjects.Count)StateManagement.Instance._currentTimelineIndex++;
    }
}
