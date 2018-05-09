using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vravel_json_loader : MonoBehaviour {


	public string url = "";

	public List<video_data> vault;




    IEnumerator start_download(){

        JSONObject jobj;
      
        WWW www = new WWW("http://www.vrtify.net/vravel/mintur/mintur.json");

        yield return www;

        print(www.text);

        jobj = new JSONObject(www.text);

		vault = new List<video_data> ();

		for (int i = 0; i < jobj [0].Count; i++) {

			video_data temp = new video_data ();
        
            temp.provincia = jobj[0][i]["provincia"].ToString().Replace("\"", "");
            temp.nombre = jobj[0][i]["nombre"].ToString().Replace("\"", "");
            temp.descripcion = jobj[0][i]["descripcion"].ToString().Replace("\"", "");
            temp.hls = jobj[0][i]["hls"].ToString().Replace("\"", "");
            temp.thumb = jobj[0][i]["thumb"].ToString().Replace("\"", "");
            temp.skybox = jobj[0][i]["skybox"].ToString().Replace("\"", "");
			temp.dnd = jobj[0][i]["dnd"].ToString().Replace("\"", "");

            vault.Add (temp);


		}

       //Loaded.Invoke();


	}

   
	void Start () {
		StartCoroutine (start_download ());


			

	}
	

}
