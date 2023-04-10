using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntantiateAndPlaceOnPlane : MonoBehaviour
{
    [SerializeField] public SessionStore sessionStore;

    [SerializeField] public Image _blueCheck;
    [SerializeField] public Image _greenCheck;

    public bool _selected = false;
    public bool _placed = false;



    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    private void Update()
    {

    }

    public void ButtonPress()
    {
        if (!_selected && !_placed) 
        { 
            _selected = true; 
            _blueCheck.enabled =true; 
            EventManager.DoObjectInstantiation(sessionStore.objectList[sessionStore.objectList.FindIndex(x=> x.gameObject.name == gameObject.name)], gameObject.name); 
        }
        
    }

}
