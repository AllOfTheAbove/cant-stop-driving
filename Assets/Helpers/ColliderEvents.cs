using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvents : MonoBehaviour {

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter()
    {
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit()
    {
        onTriggerExit.Invoke();
    }
}
