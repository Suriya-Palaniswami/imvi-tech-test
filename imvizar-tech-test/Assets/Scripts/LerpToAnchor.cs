using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LerpToAnchor : MonoBehaviour
{

    
    public GameObject to;
    public bool go = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LerpTo()
    {
        if(go)this.transform.DOMove(to.transform.position, 1.5f);
    }
}
