using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ButtonType : byte
{ 
    Back,
    SaveCreationInfo,
    CreateNewExperience,
    Record,
    Stop,
    Save
}

public static class EventManager {

    

    public delegate void PriorAction(object _o);

    public static event PriorAction OnPriorAction;

    public static void DoPriorAction(object _o)
    {
        OnPriorAction(_o);
    }

    public delegate void ImviObjectAction(ImviObject imviObject, object _o);

    public static event ImviObjectAction OnObjectAction;

    public static void DoObjectAction(ImviObject imviObject, object _o)
    {
        OnObjectAction(imviObject, _o);
    }

    public delegate void NextAction(int nextCall, object _o);

    public static event NextAction OnNextAction;

    public static void DoNextAction(int nextCall, object _o)
    {
        OnNextAction(nextCall, _o);
    }

    public delegate void ButtonAction(ButtonType buttonType, object _o);

    public static event ButtonAction OnButtonAction;

    public static void DoButtonAction(ButtonType buttonType, object _o)
    {
        OnButtonAction(buttonType, _o);
    }

    public delegate void ObjectInstantiationAction(GameObject prefabToInstantiate, object _o);

    public static event ObjectInstantiationAction OnObjectInstantiationAction;

    public static void DoObjectInstantiation(GameObject _prefabToInstantiate, object _o)
    {
        OnObjectInstantiationAction(_prefabToInstantiate,_o);
    }


}
