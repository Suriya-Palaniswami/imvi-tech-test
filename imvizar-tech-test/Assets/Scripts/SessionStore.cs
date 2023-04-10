using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SessionDetails
{
    [SerializeField] public string SessionName;
    [SerializeField] public string CreatorName;

    [SerializeField] public List<ObjectInSession> ObjectListInSession;

    public SessionDetails(string _sessionName, string _creatorName, List<ObjectInSession> _objectListInSession)
    {
        SessionName = _sessionName;
        CreatorName = _creatorName;
        ObjectListInSession = _objectListInSession;
    }

}
[Serializable]
public struct ObjectInSession
{
    [SerializeField] public GameObject Object;
    [SerializeField] public ObjectDataRecord EndTransform;

    public ObjectInSession(GameObject _object,  ObjectDataRecord _endObjectDataRecord)
    {
        Object = _object;
        EndTransform = _endObjectDataRecord;
    }
}

[Serializable]
public struct ObjectDataRecord
{
    [SerializeField] public  Vector3 Position;
    [SerializeField] public  Quaternion Rotation;

    public ObjectDataRecord( Vector3 _position, Quaternion _rotation)
    {
        Position = _position;
        Rotation = _rotation;
    }
}

[CreateAssetMenu]
public class SessionStore : ScriptableObject
{
    //Details of all the sessions conducted
    [SerializeField] public List<SessionDetails> SessionDetailsList;

    //Prefab of Session Display to be Instantiated in ScrollView
    [SerializeField] public GameObject SessionDisplayTemplate;

    //Objects in the Project that can be instantiated
    [SerializeField] public List<GameObject> objectList;

}
