using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocationThumb : MonoBehaviour {



    public string Imagen;
    public GameObject Fade;
    public string Video;
    public TextMesh Titulo;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadData(string title, string thumb, string video)
    {
        Titulo.text = title;
        Video = video;
        Imagen = thumb;
        StartCoroutine(LoadThumb());

    }


    IEnumerator LoadThumb()
    {

        WWW www = new WWW(Imagen);
        yield return www;
        GetComponent<Renderer>().material.mainTexture = www.texture;

    }

    public void Execute()
    {

    }

}
