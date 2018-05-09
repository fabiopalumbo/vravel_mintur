using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour {

    public GameObject[] Provincias;
    public GameObject Provincias_Container;
    public float AngleBase;

    private int MovementInitial = 0;
    private int FocusP = 0;


    public GameObject[] Destinos;

    
    public List<LoadedData> JsonData = new List<LoadedData>();

    private void Awake()
    {

        DOTween.Init();

        int rnd = Mathf.RoundToInt(Random.Range(0f, 100f));
        MovementInitial = rnd;
        Provincias_Container.transform.DORotate(new Vector3(0, rnd* AngleBase,0),0f);

        int counterPhase = 0;
        for(int i = 0; i < MovementInitial; i++)
        {
            counterPhase++;
            if(counterPhase > 22)
            {
                counterPhase = 0;
            }
        }

        FocusP = counterPhase;

        //http://www.alchemiagames.com/vravel_web/MINTUR.json

       // StartCoroutine(LoadJson());

    }

    void Start () {
        Provincias[FocusP].GetComponent<Provincia>().Show();
    }


    public GameObject Cam;
	
	void Update () {

        if (Input.GetButton("Fire1"))
        {
            RaycastHit hit;

            if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit))
            {
                if (hit.collider.gameObject.GetComponent<Executioner>())
                {
                    hit.collider.gameObject.GetComponent<Executioner>().Execute();
                }
            }
                
        }

        if (Input.GetKey("up"))
            MoveLeft();

        if (Input.GetKey("down"))
            MoveRight();

    }


    private bool SafeLock = false;

    public void MoveLeft()
    {
        if (SafeLock)
            return;
        SafeLock = true;

        MovementInitial--;
        Provincias_Container.transform.DORotate(new Vector3(0, MovementInitial * AngleBase, 0), 0.4f).OnComplete(()=> { SafeLock = false; });

        FocusP--;
        if(FocusP < 0)
        {
            FocusP = 22;
        }

        SetFocus(false);
    }

    public void MoveRight()
    {
        if (SafeLock)
            return;
        SafeLock = true;

        MovementInitial++;
        Provincias_Container.transform.DORotate(new Vector3(0, MovementInitial * AngleBase, 0), 0.4f).OnComplete(() => { SafeLock = false; });


        FocusP++;
        if(FocusP > 22)
        {
            FocusP = 0;
        }
        SetFocus(true);
    }

    public void SetFocus(bool direction)
    {

        if (!direction)
        {
            if(FocusP == 22)
            {
                Provincias[0].GetComponent<Provincia>().Hide();
            }
            else
            {
                Provincias[FocusP+1].GetComponent<Provincia>().Hide();
            }
        }
        else
        {
            if(FocusP == 0)
            {
                Provincias[22].GetComponent<Provincia>().Hide();
            }
            else
            {
                Provincias[FocusP - 1].GetComponent<Provincia>().Hide();
            }
        }

        Provincias[FocusP].GetComponent<Provincia>().Show();

    }


    IEnumerator LoadJson()
    {

        string url = "http://www.alchemiagames.com/vravel_web/MINTUR.json";
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {

            JSONObject JSON = new JSONObject(www.text);

          

            for(int i = 0; i < JSON.Count; i++)
            {

                LoadedData temp = new LoadedData();

                temp.NOMBRE         = JSON[i]["nombre"].ToString().Replace("\"", "");
                temp.PROVINCIA      = JSON[i]["provincia"].ToString().Replace("\"","");
                temp.DESCRIPTION    = JSON[i]["descripcion"].ToString().Replace("\"", "");
                temp.THUMB          = JSON[i]["thumb"].ToString().Replace("\"", "");
                temp.VIDEO          = JSON[i]["hls"].ToString().Replace("\"", "");
                temp.SKYBOX         = JSON[i]["skybox"].ToString().Replace("\"", "");
                
                JsonData.Add(temp);
            }

        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
        
    }

    public vravel_json_loader jdata;

    public List<LoadedData> fisk = new List<LoadedData>();

    public void ProvinceSelected(string Location)
    {

        Provincias_Container.SetActive(false);

        fisk.Clear();

        for(int i = 0; i< jdata.vault.Count; i++)
        {
            print(Location);
        
            if (jdata.vault[i].provincia.ToUpper() == Location)
            {
                LoadedData temp = new LoadedData();

                temp.PROVINCIA = jdata.vault[i].provincia;
                temp.DESCRIPTION = jdata.vault[i].descripcion;
                temp.THUMB = jdata.vault[i].thumb;
                temp.VIDEO = jdata.vault[i].hls;
                temp.NOMBRE = jdata.vault[i].nombre;
                temp.SKYBOX = jdata.vault[i].skybox;
				temp.dnd = jdata.vault[i].dnd;

                fisk.Add(temp);
            }
        }


        ShowShower();

    }


    public void ShowProvincias()
    {
        Provincias_Container.SetActive(true);
    }


    public GameObject ShowTemplate;
    public float xshower;
    public float yshower;


    public GameObject p_arrow_left;
    public GameObject p_arrow_right;

    public GameObject S_arrow_left;
    public GameObject S_arrow_right;


    public GameObject backbtn;

	public GameObject Skybox;
	public GameObject Skyboxfader;


    public void ShowShower()
    {

        ShowTemplate.SetActive(true);


        ShowTemplate.GetComponent<shower>().setImage(fisk[0].THUMB);
        ShowTemplate.GetComponent<shower>().SetTittle(fisk[0].NOMBRE);
        ShowTemplate.GetComponent<shower>().Setdescription(fisk[0].DESCRIPTION);
		ShowTemplate.GetComponent<shower> ().setState (fisk [0].PROVINCIA);
        
        p_arrow_left.SetActive(false);
        p_arrow_right.SetActive(false);
       

		ShowTemplate.GetComponent<VimeoVideoMenu3D> ().CheckExistance (fisk [0].SKYBOX);

		ShowTemplate.GetComponent<VimeoVideoMenu3D> ().currentDownloadLink = fisk [fiskCounter].dnd;
		ShowTemplate.GetComponent<VimeoVideoMenu3D> ().currentIDname = fisk [fiskCounter].SKYBOX;

		//aca setear el link de descarga en el vimeodownloader
		//aca serear el nombre de video de descarga

        if(fisk.Count > 1)
        {
            S_arrow_right.SetActive(true);
        }

        

        backbtn.SetActive(true);

		FadeInFader (fisk [0].SKYBOX);
    }



    public void BackToProvincias(){

		fiskCounter = 0;

        ShowTemplate.SetActive(false);
        p_arrow_left.SetActive(true);
        p_arrow_right.SetActive(true);
        Provincias_Container.SetActive(true);
        backbtn.SetActive(false);

        S_arrow_left.SetActive(false);
        S_arrow_right.SetActive(false);

		Skyboxfader.GetComponent<Renderer> ().material.DOFade (1, .5f).OnComplete(()=>{
			
			Skybox.SetActive (false);
			Skyboxfader.GetComponent<Renderer> ().material.DOFade (0, .5f).OnComplete(()=>{  });
		});

    }

	public void FadeInFader(string skybox){
		Skyboxfader.GetComponent<Renderer> ().material.DOFade (1, .5f).OnComplete(()=>{
			LoadingBlack.SetActive(true);
			StartCoroutine(loadSkybox(skybox));
		});
	}

	IEnumerator loadSkybox(string name){
		print ("http://www.vrtify.net.s3.amazonaws.com/vravel/mintur/skyboxes/" + name + ".jpg");
		WWW www = new WWW ("http://www.vrtify.net.s3.amazonaws.com/vravel/mintur/skyboxes/"+name+".jpg");
		yield return www;
		Skybox.SetActive (true);
		Skybox.GetComponent<Renderer> ().material.mainTexture = www.texture;

		LoadingBlack.SetActive(false);
		Skyboxfader.GetComponent<Renderer> ().material.DOFade (0, .5f).OnComplete(()=>{ lockMovement= false;});
	}


	public int fiskCounter = 0;


	public void ChangeShower(){
		
		Skyboxfader.GetComponent<Renderer> ().material.DOFade (1, .5f).OnComplete(()=>{
			LoadingBlack.SetActive(true);
			ShowTemplate.GetComponent<shower>().setImage(fisk[fiskCounter].THUMB);
			ShowTemplate.GetComponent<shower>().SetTittle(fisk[fiskCounter].NOMBRE);
			ShowTemplate.GetComponent<shower>().Setdescription(fisk[fiskCounter].DESCRIPTION);
			ShowTemplate.GetComponent<shower> ().setState (fisk [fiskCounter].PROVINCIA);


			ShowTemplate.GetComponent<VimeoVideoMenu3D> ().CheckExistance (fisk [fiskCounter].SKYBOX);

			ShowTemplate.GetComponent<VimeoVideoMenu3D> ().currentDownloadLink = fisk [fiskCounter].dnd;
			ShowTemplate.GetComponent<VimeoVideoMenu3D> ().currentIDname = fisk [fiskCounter].SKYBOX;
			//aca setear el link de descarga en el vimeodownloader
			//aca serear el nombre de video de descarga

			StartCoroutine(loadSkybox(fisk [fiskCounter].SKYBOX));

		});

	}

	private bool lockMovement = false;

	public GameObject LoadingBlack;

	public void NextLocation(){
		if (fiskCounter >= fisk.Count - 1)
			return;
		

		if (lockMovement)
			return;
		lockMovement = true;
		print ("FISK COUNTER " + fiskCounter + " Fisk Ammount " + fisk.Count);
		fiskCounter++;
		if (fiskCounter >= fisk.Count - 1) {
			S_arrow_right.SetActive (false);
		}
		S_arrow_left.SetActive (true);

		ChangeShower ();
	}

	public void PrevLocation(){
		if (fiskCounter == 0)
			return;


		if (lockMovement)
			return;
		lockMovement = true;
		fiskCounter--;
		if (fiskCounter == 0) {
			S_arrow_left.SetActive (false);
		}

		S_arrow_right.SetActive (true);

		ChangeShower ();
	}

	public void ShowVideo(){
		GameObject.FindObjectOfType<InmortalScript> ().VideoToPlay = fisk [fiskCounter].VIDEO;
		Skyboxfader.GetComponent<Renderer> ().material.DOFade (1, .5f).OnComplete (() => {
			Application.LoadLevel("Player");
		});
	}

	public void localvideo(string path){
		GameObject.FindObjectOfType<InmortalScript> ().VideoToPlay = path;
		Skyboxfader.GetComponent<Renderer> ().material.DOFade (1, .5f).OnComplete (() => {
			Application.LoadLevel("Player");
		});
	}

}

[System.Serializable]
public class LoadedData{

    public string PROVINCIA = "";
    public string NOMBRE = "";
    public string DESCRIPTION = "";
    public string THUMB = "";
    public string VIDEO = "";
    public string SKYBOX = "";
	public string dnd = "";


}
