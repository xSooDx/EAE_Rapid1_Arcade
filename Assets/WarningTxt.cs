using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningTxt : MonoBehaviour
{
    public string WarningTag;

    private Animator _animmator;

    private void Start()
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.SetWarning.AddListener(SetWarning);
        }
        _animmator=gameObject.GetComponent<Animator>();
    }

    void SetWarning(string _tag,bool _Warning)
    {
        if (_animmator != null && (_tag.Equals(this.WarningTag)|| _tag.Equals("ALL")))
        {
            _animmator.SetBool("Warning", _Warning);
        }
    }
}
