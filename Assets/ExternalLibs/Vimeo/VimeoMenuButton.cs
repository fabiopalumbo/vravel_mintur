using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class VimeoMenuButton : MonoBehaviour {

    private bool imSelected = false;
   
    public Color32 BackgroundColor, TextColor;
    public UnityEvent OnClick;

    [HideInInspector]
    public UnityIntEvent OnClickWithInt;
    [HideInInspector]
    public int Index = 0;
    [HideInInspector]
    public Vector3 scale;

    public string Text {
        set { 
            GetComponentInChildren<TextMesh>().text = value;
        }

        get {
            return GetComponentInChildren<TextMesh>().text;
        }
    }
    
    void Update() {
        if (!imSelected) return;

        if (Input.GetButtonDown("Fire1")) {
            OnClick.Invoke();
            OnClickWithInt.Invoke(Index);
            OnTriggerExit();
        }
    }

    public void ManualOverride() {
        OnClick.Invoke();
        OnClickWithInt.Invoke(Index);
    }

    public void Select() {
        GetComponent<Renderer>().material.color = TextColor;
        transform.GetChild(0).GetComponent<Renderer>().material.color = BackgroundColor;
    }

    public void DeSelect() {
        GetComponent<Renderer>().material.color = BackgroundColor;
        transform.GetChild(0).GetComponent<Renderer>().material.color = TextColor;
    }

    void OnMouseUp() {
        OnClick.Invoke();
        OnClickWithInt.Invoke(Index);
    }

    void OnTriggerEnter() {
        transform.DOScale(scale * 1.2f, .3f);
        imSelected = true;
    }

    void OnTriggerExit() {
        transform.DOScale(scale, .3f);
        imSelected = false;
    }
}

[System.Serializable]
public class UnityIntEvent : UnityEvent<int> { }