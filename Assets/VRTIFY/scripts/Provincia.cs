using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Provincia : MonoBehaviour { 



    public GameObject Graph_Provincia;
    public string ID;
    public TextMesh Titulo;
    public UnityEvent OnClick;

    private void Awake()
    {
        DOTween.Init();

        Titulo.text = ID.Replace("_", " ");
        Titulo.gameObject.SetActive(false);

    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        Titulo.gameObject.SetActive(true);
        Graph_Provincia.transform.DOLocalMoveZ(4f, .3f);
        
    }

    public void Hide()
    {
        Titulo.gameObject.SetActive(false);
        Graph_Provincia.transform.DOLocalMoveZ(5.14f, .3f);
       
    }

    void OnMouseDown()
    {
        OnClick.Invoke();

    }

    public void ExecuteTap()
    {
        OnClick.Invoke();
    }


}
