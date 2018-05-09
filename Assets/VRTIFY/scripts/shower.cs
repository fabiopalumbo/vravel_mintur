using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shower : MonoBehaviour {

    public GameObject fade;
    public GameObject Title;
    public GameObject descripcion;
    public GameObject button;
    public GameObject provincia;



    private void Start()
    {
        //setImage("https://i.vimeocdn.com/video/651684710_640x427.jpg?r=pad");
    }


    public void setImage(string val)
    {
        StartCoroutine(loadImage(val));
    }

    IEnumerator loadImage(string url)
    {

        WWW www = new WWW(url);

        yield return www;

        gameObject.GetComponent<Renderer>().material.mainTexture = www.texture;

    }


	public void setState(string val){
		
		provincia.GetComponent<TextMesh> ().text = val;

	}

    public void SetTittle(string val)
    {
		Title.GetComponent<TextMesh>().text = WrapText(val,70,2);;
    }

    public void Setdescription(string val)
    {
		descripcion.GetComponent<TextMesh>().text = WrapText(val,70,4);
    }


	public string WrapText(string s, int maxCharacters, int maxLines = 2 , string elpisis = "...") {
		string temp = "";
		int lineLength = 0, lines = 1;
		string[] parts = s.Split(' ');
		for (int i = 0; i < parts.Length; i++) {
			lineLength += parts[i].Length;
			if (lineLength >= maxCharacters) {
				temp.TrimEnd(' ');

				if(lines >= maxLines) {
					int offset = parts[i].Length - ((lineLength - maxCharacters) + elpisis.Length); 
					if (offset >= 0) temp += parts[i].Substring(0, offset);
					else temp = temp.Substring(0, temp.Length + offset);

					temp += elpisis;
					return temp;
				}

				lines++;
				temp += System.Environment.NewLine;
				lineLength = parts[i].Length;
			}

			lineLength += 1;
			temp += parts[i] + " ";
		}

		return temp;
	}



	public void CheckDownload(){

	}
    
}

