using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ObjectTimelineHandler : MonoBehaviour
{
    public AudioSource AudioSource;

    [SerializeField]private Animator _animator;

    public string prior;

    private ExperienceBuilder _experienceBuilder;

    public bool _animationCompleted = false;
    public bool _audioCompleted = false;

    private bool playedOnce = false;
    private AnimationClip _currentAnimationClip;

    [SerializeField] public ObjectDataRecord EndObjectRecord;
    private void Awake()
    {
        

    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        AudioSource.enabled = false;
        _animator.enabled = false;
        _experienceBuilder = FindObjectOfType<ExperienceBuilder>();
        GetPrior();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (prior == "" && !_animationCompleted && !_audioCompleted && !_experienceBuilder.isNonLinear)
        {
            InitiateObjectLifeCycle(); 
        }
        else if (_animationCompleted && _audioCompleted && playedOnce) { playedOnce = false; KnockPriorOff(); }
    }

    private void OnEnable()
    {
        EventManager.OnPriorAction += ListenToPriors;
        EventManager.OnObjectAction += HandleImviObject;
    }

    private void OnDisable()
    {
        EventManager.OnPriorAction -= ListenToPriors;
        EventManager.OnObjectAction -= HandleImviObject;


    }

    private void HandleImviObject(ImviObject imviObject, object _o)
    {
        Debug.Log("Reaching Indi Obj Name: "+gameObject.name + " ImviObjec: "+imviObject.RootObject.name);
        if (imviObject.RootObject.name == gameObject.name) { _currentAnimationClip = imviObject.ObjectAnimationClip; InitiateObjectLifeCycle(); }
    }

    private void ListenToPriors(object _o)
    {
        try
        {
            if (_o.ToString() == prior)
            {
                prior = "";
            }

        }
        catch (Exception e)
        { 
            
        }
    }

    private void GetPrior()
    {
        try
        {
            prior = _experienceBuilder.ObjectsInExperience[_experienceBuilder.ObjectsInExperience.FindIndex(x => x.gameObject.name == this.gameObject.name) - 1].name;
        }
        catch (Exception e)
        {
            prior = "";
        }
    }

    private void InitiateObjectLifeCycle()
    {
        StartCoroutine("PlayAnimator");
        StartCoroutine("PlayAudio");
    }

    IEnumerator PlayAnimator()
    {
        _animator.enabled = true;
        if (_experienceBuilder.isNonLinear) _animator.Play(_currentAnimationClip.name, 0, 0.0f);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animationCompleted = true;
    }

    IEnumerator PlayAudio()
    {
        AudioSource.enabled = true;
        //Debug.Log(_currentAnimationClip.name.ToString());
        
        if (!_experienceBuilder.isNonLinear)
        {
            AnimatorClipInfo[] m_CurrentClipInfo;
            m_CurrentClipInfo = _animator.GetCurrentAnimatorClipInfo(0);
            _currentAnimationClip = m_CurrentClipInfo[0].clip;
            
        }
        var clip = Resources.Load<AudioClip>("Audios/" + _currentAnimationClip.name.ToString());
        if (clip == null)
        {
            Debug.Log(GetType().Name + ": " + gameObject.name.ToString() + " has no audio."+ _currentAnimationClip.name.ToString());
        }
        var audioClip = Instantiate(clip);
        AudioSource.clip = audioClip;
        if (AudioSource.isPlaying == false) AudioSource.Play();
        yield return new WaitForSeconds(AudioSource.clip.length);
        _audioCompleted = true;
        playedOnce = true;

    }



    private void KnockPriorOff()
    {
        Debug.Log("Prior "+gameObject.name);
        _animator.enabled = false;
        AudioSource.enabled = false;
        if(_experienceBuilder.isNonLinear)EventManager.DoNextAction(StateManagement.Instance._currentTimelineIndex,name);
        else EventManager.DoPriorAction(gameObject.name);
    }

}
