using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public List<InteractionBehaviour> _lights=new List<InteractionBehaviour>();
    bool _canControl;
    private void Start()
    {
        _canControl = true;
    }
    private void Update()
    {
        Control();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out IInteraction interaction))
            {
                if (!interaction.CanInteraction())
                {
                    _canControl = false;
                }
                else
                    _canControl = true;
            }
           
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _canControl = false;
        }
    }
    public void Control()
    {
        if (!_canControl) return;
        if (_lights.Count == 0) return;
        if (GameInputManager.MainInstance.TakeOut)
        {
            foreach (var light in _lights)
            {
                if (light.CanInteraction())
                {
                    //Debug.Log(light.gameObject.name);
                    light.InteractionAction();
                }
            }
        }
    }
}
