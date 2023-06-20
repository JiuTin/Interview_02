using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionBehaviour : MonoBehaviour,IInteraction
{
    protected bool _canInteraction;

    private void Start()
    {
        _canInteraction = true;
    }
    public void InteractionAction()
    {
        Interaction();
    }

    public bool CanInteraction()
    {
        return _canInteraction;
    }
    //½»»¥Âß¼­
    protected abstract void Interaction();
}
