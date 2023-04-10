using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImviUIButton : MonoBehaviour
{
    [SerializeField] private ButtonType buttonType;
    public void ButtonPress()
    {
        EventManager.DoButtonAction(buttonType,null);
    }
}
