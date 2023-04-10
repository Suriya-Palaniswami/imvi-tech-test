using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class TaskC : MonoBehaviour
{
    //Get And Change Attached Model
    //Set Threshold
    //Check for Threshold

    [SerializeField] private GameObject hangingActorPrefab;

    [SerializeField] private GameObject hangingActorInGame;

    public Rigidbody anchorObjectRigidbody;

    [SerializeField] private float maxSpeed;

    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private TextMeshProUGUI hintTextMax;




    // Start is called before the first frame update
    void Start()
    {
       // DoTaskC(hangingActorPrefab,cube);
    }

    // Update is called once per frame
    void Update()
    {

        if (anchorObjectRigidbody == null)
        {
            hintText.text = "Null";
        }

        if (anchorObjectRigidbody != null) {

            hintText.text = "Not Null";
            hintText.text = ((int)anchorObjectRigidbody.velocity.magnitude).ToString();
            if(((int)anchorObjectRigidbody.velocity.magnitude) > maxSpeed)
            {
                hintTextMax.text = "Max Speedd";
                hangingActorInGame.transform.parent = null;
                hangingActorInGame.transform.DOMoveY(-0.561f, 1.5f);
                hangingActorInGame.transform.DOLocalRotate(new Vector3(-21.51f, 0f, 0f), 1.5f).OnComplete(() => SetRigidBodyNull());
                hintText.text = anchorObjectRigidbody.velocity.magnitude.ToString();
            }
        }
    }

    public void DoTaskC(GameObject OldWalkingActor)
    {
        OldWalkingActor.SetActive(false);
        hangingActorInGame = Instantiate(hangingActorPrefab, OldWalkingActor.transform.position, OldWalkingActor.transform.rotation);

        hangingActorInGame.transform.SetParent(GameObject.FindGameObjectWithTag("Anchor").transform);

        hintText.text = "Trying to Get RigidBody";
        try
        {
            anchorObjectRigidbody = GameObject.FindGameObjectWithTag("Anchor").GetComponent<Rigidbody>();
        }
        catch (Exception e)
        {
            hintText.text = e.ToString();
        }
    }

    private void SetRigidBodyNull()
    {
        anchorObjectRigidbody = null;
    }
}
