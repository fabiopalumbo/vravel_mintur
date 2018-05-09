using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Executioner : MonoBehaviour {

    public UnityEvent Tapped;

    public void Execute()
    {
        Tapped.Invoke();
    }
}
