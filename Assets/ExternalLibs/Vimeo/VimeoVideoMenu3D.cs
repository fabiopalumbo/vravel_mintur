using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;


public class VimeoVideoMenu3D : MonoBehaviour {

    private const string VIDEO_FORMAT = ".mp4";
    private string PATH , FOLDER = FileUtils.VIDEO_FOLDER;

    public delegate void SaveVideoDelegate();

    public static System.Action<VimeoVideo, string> Appear;
    public static System.Action<float> Disappear;

    public enum VideoQuality {
        VeryHigh = 2160,
        High = 1440,
        Medium = 1080,
        Low = 720,
        None = 0
    }

    [Header("Selector de Calidad")]
    public GameObject QualitySelection;

    [Header("Selector de Acciones")]
    public GameObject Actions;

    [Header("GameObject que muestra la descarga")]
    public GameObject Downloading;
    [Header("Barra de descarga")]
    public GameObject ProgressBar;

    [Header("Titulo")]
    public TextMesh TitleField;
    [Header("Tamaño del Archivo")]
    public TextMesh SizeField;
    [Header("Thumbnail")]
    public GameObject Thumbnail;

    [Header("Aparecen si el video ya fue descargado")]
    public GameObject[] LocalActions;

    [Header("Aparecen si el video no fue descargado")]
    public GameObject[] OnlineActions;

    public GameObject QualityButtonPrefab;

    [Header("Eventos")]
    [Space(5)]
    public UnityEvent OnDownload;
    public UnityEvent OnPlay, OnStream, OnDelete, OnBack;

    private InmortalScript inmortal;
    private VimeoVideo resp;
    private FileInfo videoInfo;
    private string ID , SelectedVideoURL, qualityHeight;
    private bool hidden = false;
    private List<VimeoMenuButton> buttons;
    private VideoQuality QualitySelected = VideoQuality.None;
    private int qualityIndex = 0;
    private SavedData dataToSave;

	public GameObject Blocker;

    public int Quality {
        set {
            QualitySelected = (VideoQuality)value;
        }
    }

    public void SetID(string id) {
        ID = id;
    }


    void Start() {
        PATH = Application.persistentDataPath + FOLDER;

        if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);

      
    }


	public void ManualLoadID(string ID){
		gameObject.SetActive (true);
		StartCoroutine (GetVideo(ID));
	}

    IEnumerator GetVideo(string ID) {
        UnityEngine.Networking.UnityWebRequest req = UnityEngine.Networking.UnityWebRequest.Get(VimeoApi.GetVideoURL(ID));
        req.SetRequestHeader(VimeoApi.getHeader()[0], VimeoApi.getHeader()[1]);
        yield return req.Send();

        resp = JsonUtility.FromJson<VimeoVideo>(req.downloadHandler.text);
        
        ShowQuality(resp, ID);
    }

    public void ShowQuality(VimeoVideo response , string id = "") {
        if (response == null) return;

        Show();

        if (Downloading) Downloading.SetActive(false);

        if (SizeField) SizeField.text = "";

        if (TitleField) TitleField.text = response.name.Replace(" - " , "\n");

        dataToSave = new SavedData();
        dataToSave.title = response.name;

        inmortal = FindObjectOfType<InmortalScript>();

        if (inmortal) {
            if(inmortal.HasOtherSounds == false) inmortal.SocialText = response.name;
            inmortal.SocialThumb = response.pictures.GetPicture();
        }

        resp = response;
        ID = id;

        ImageLoader.DownloadList(
            new List<ImageDownloadRequest> {new ImageDownloadRequest(id, response.pictures.GetPicture(), (Texture2D texture) => {
                if (Thumbnail) Thumbnail.GetComponent<Renderer>().material.mainTexture = texture;
                dataToSave.thumbnail = FileUtils.SaveImage(texture,ID);
                FileUtils.SaveJSONData(dataToSave , ID);
            }) },
			(float progress) => { } , () => { }
        );
        
        QualitySelected = VideoQuality.Low;
        GetVideoInfo();
    }

    public void ShowQuality(string title, string url , string thumb) {
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title)) return;

        Show();

        if (Downloading) Downloading.SetActive(false);
        if (QualitySelection) QualitySelection.SetActive(false);

        if (SizeField) SizeField.text = "";

        if (TitleField) TitleField.text = title.Replace(" - ", "\n");

        inmortal = FindObjectOfType<InmortalScript>();

        if (inmortal) {
            if (inmortal.HasOtherSounds == false) inmortal.SocialText = title;
            inmortal.SocialThumb = thumb;
        }

        dataToSave = new SavedData();
        dataToSave.title = title;

        QualitySelected = VideoQuality.None;

        SelectedVideoURL = url;
        int l = url.Split('/').Length;
        ID = url.Split('/')[l - 1].Split('.')[0];

        ImageLoader.DownloadList(
            new List<ImageDownloadRequest> {new ImageDownloadRequest(url, thumb, (Texture2D texture) => {
                if (Thumbnail) Thumbnail.GetComponent<Renderer>().material.mainTexture = texture;
                dataToSave.thumbnail = FileUtils.SaveImage(texture,ID);
                FileUtils.SaveJSONData(dataToSave , ID);
            }) },
            (float progress) => { }, () => { }
        );

        
        qualityHeight = "MAX";

        ShowActions();
        GetSize();
    }

    public void GetVideoInfo() {
        int count = 0;
        GameObject Button;
        Vector3 ButtonScale;
        VimeoMenuButton MenuButton;

		List<VimeoVideoData> auxiliar = new List<VimeoVideoData> ();

		int counteaux = 0;
		for (int p = 0; p < resp.files.Length; p++) {
			if (resp.files [p].fps.ToString() == "29.97") {
				if (resp.files [p].height > 720) {
					auxiliar.Add (resp.files [p]);
				}

			}
		}
		print (auxiliar.Count);
		print (resp.files.Length);


        QualitySelection.SetActive(true);
		int l = (auxiliar.Count > buttons.Count) ? auxiliar.Count : buttons.Count;

        for (int i = 0; i < l; i++) {
            //if (resp.files[i].quality != "hd") {
			//if (resp.files [i].fps < 30) {




				if (auxiliar[i].quality == "hls") {
					if (buttons.Count > i)
						buttons [i].gameObject.SetActive (false);
					continue;
				}



				if (buttons.Count <= i) { 
					Button = Instantiate (QualityButtonPrefab);
					ButtonScale = Button.transform.localScale;

					Button.SetActive (true);
					Button.transform.parent = QualitySelection.transform;
					Button.transform.localPosition = Vector3.right * count;
					Button.transform.localPosition += (Vector3.right * .4f);

					MenuButton = Button.GetComponent<VimeoMenuButton> ();

					Button.transform.localScale = ButtonScale;
					MenuButton.scale = ButtonScale;
					MenuButton.OnClickWithInt.AddListener (SetSelection);
					MenuButton.Index = count;
					buttons.Add (MenuButton);
				} else {
					MenuButton = buttons [i];
					MenuButton.transform.localPosition = Vector3.right * count;
					MenuButton.transform.localPosition += (Vector3.right * .4f);
				}


				MenuButton.Text = (auxiliar [i].height + "");

				buttons [i].gameObject.SetActive (true);

			
				count++;

			/*

				if (resp.files [i].quality == "hls") {
					if (buttons.Count > i)
						buttons [i].gameObject.SetActive (false);
					continue;
				}



				if (buttons.Count <= i) { 
					Button = Instantiate (QualityButtonPrefab);
					ButtonScale = Button.transform.localScale;

					Button.SetActive (true);
					Button.transform.parent = QualitySelection.transform;
					Button.transform.localPosition = Vector3.right * count;
					Button.transform.localPosition += (Vector3.right * .4f);

					MenuButton = Button.GetComponent<VimeoMenuButton> ();

					Button.transform.localScale = ButtonScale;
					MenuButton.scale = ButtonScale;
					MenuButton.OnClickWithInt.AddListener (SetSelection);
					MenuButton.Index = count;
					buttons.Add (MenuButton);
				} else {
					MenuButton = buttons [i];
					MenuButton.transform.localPosition = Vector3.right * count;
					MenuButton.transform.localPosition += (Vector3.right * .4f);
				}


				MenuButton.Text = (resp.files [i].height + "");

				buttons [i].gameObject.SetActive (true);

			
				count++;

			*/


			//}
        }

        Vector3 position = QualitySelection.transform.localPosition;
        position.x = -(count - .2f) /2;
        QualitySelection.transform.localPosition = position;

        buttons[0].ManualOverride();

        ShowActions();
    }

    void SetSelection(int index) {
        SelectedVideoURL = resp.files[index].link;
        qualityHeight    = resp.files[index].height + "";
        if (SizeField) SizeField.text = (resp.files[index].size / 1048576) + "MB";

        for (int i = 0; i < buttons.Count; i++) {
            if (i == index) buttons[i].Select();
            else buttons[i].DeSelect();
        }

        qualityIndex = index;
        ShowActions();
    }


    public void GetSize() {
        if (SizeField) SizeField.text = "";
        if (!videoInfo.Exists) {
            StartCoroutine(FileUtils.GetDownloadSize(SelectedVideoURL, (string vsize) => {
                if (SizeField) SizeField.text = (long.Parse(vsize) / 1048576) + "MB";
            }));
        } else {
            if (SizeField) SizeField.text = (videoInfo.Length / 1048576) + "MB";
        }
    }

    public void ShowActions() {
        if (QualitySelected == VideoQuality.None) {
            videoInfo = new FileInfo(PATH + ID + VIDEO_FORMAT);
        } else {
            videoInfo = new FileInfo(PATH + ID + "_" + qualityHeight + "_V" + VIDEO_FORMAT);
        }

        foreach (GameObject go in LocalActions) {
            go.SetActive(videoInfo.Exists);
        }

        foreach (GameObject go in OnlineActions) {
            go.SetActive(!videoInfo.Exists);
        }

        if (Actions) Actions.SetActive(true);
    }


	public string currentDownloadLink ="";
	public string currentIDname = "";
	public void Download() {
		/*
        if (Actions) Actions.SetActive(false);
        if (Downloading) Downloading.SetActive(true);

        string videoPath = "";

        if (QualitySelected == VideoQuality.None) {
            videoPath = PATH + ID + VIDEO_FORMAT;
        } else {
            videoPath = PATH + ID + "_" + qualityHeight + "_V" + VIDEO_FORMAT;
        }

        if (ProgressBar) ProgressBar.transform.DOScaleX(0, 0);

        StartCoroutine(FileUtils.DownloadAndSave(SelectedVideoURL, videoPath, 
            (float progress) => {
                if (ProgressBar) ProgressBar.transform.DOScaleX(progress,.1f);
            }, 
            (FileInfo file) => {
                if (Downloading) Downloading.SetActive(false);
                dataToSave.AddVideo(qualityIndex, file.ToString(), qualityHeight);
                FileUtils.SaveJSONData(dataToSave, ID);

                ShowActions();
                return;
            }
        ));

        OnDownload.Invoke();

*/


		string video_name = currentIDname;

		SelectedVideoURL = currentDownloadLink;

		if (Actions) Actions.SetActive(false);
		if (Downloading) Downloading.SetActive(true);

		string videoPath = "";


		videoPath = PATH + video_name + VIDEO_FORMAT;


		if (ProgressBar) ProgressBar.transform.DOScaleX(0, 0);

		StartCoroutine(FileUtils.DownloadAndSave(SelectedVideoURL, videoPath, 
			(float progress) => {



				if (ProgressBar) downloadtime.text = "Download - "+Mathf.Round( progress*10000)+"%";

				if(progress == 1){
					OnDownload.Invoke();
				}
			}
		));




    }


	private string currentvideo = "";

	public void CheckExistance(string video_name){
		string filePath = PATH + video_name + VIDEO_FORMAT;
		currentvideo = filePath;
		if (System.IO.File.Exists(filePath))
		{
			OnDownload.Invoke();
		}
		else
		{
			botondownload.SetActive (true);
			downloadtime.text = "Download";
			botonplay.SetActive (false);
		}
	}

	public TextMesh downloadtime;
	public GameObject botondownload;
	public GameObject botonplay;
	public void DC(){


		botondownload.SetActive (false);
		botonplay.SetActive (true);


	}

	public void PlayVideoFromLocal(){

		cn.localvideo (currentvideo);


	}


	public Controller cn;


    public void Stream() {
        if (!inmortal) inmortal = FindObjectOfType<InmortalScript>();
        if (!inmortal) return;

        inmortal.VideoToPlay = SelectedVideoURL;
        
        OnStream.Invoke();
    }

    public void Play() {

       

        if (!inmortal) inmortal = FindObjectOfType<InmortalScript>();
        if (!inmortal) return;

#if UNITY_ANDROID
        if (QualitySelected == VideoQuality.None) {
            inmortal.VideoToPlay = "file:/" + PATH + ID + VIDEO_FORMAT;
        } else {
            inmortal.VideoToPlay = "file:/" + PATH + ID + "_" + qualityHeight + "_V" + VIDEO_FORMAT;
        }
#else
        if (QualitySelected == VideoQuality.None) {
            inmortal.VideoToPlay = PATH + ID + VIDEO_FORMAT;
        } else {
            inmortal.VideoToPlay = PATH + ID + "_" + qualityHeight + "_V" + VIDEO_FORMAT;
        }
#endif
        OnPlay.Invoke();
    }

    public void Delete() {
        FileUtils.DeleteFile(videoInfo);
        OnDelete.Invoke();

        ShowActions();
    }

    public void Back() {
		Blocker.SetActive (false);
        Hide();
        StopAllCoroutines();
        OnBack.Invoke();
    }

    public void Show(float time = .3f) {
        if (!hidden) return;
        gameObject.SetActive(true);
        hidden = false;
        transform.DOScale(0, time).From();
    }

    public void Hide(float time = .3f) {
        if (hidden) return;
        hidden = true;
        gameObject.SetActive(false);
    }

}