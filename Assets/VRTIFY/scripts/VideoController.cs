using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VideoController : MonoBehaviour {

	public MediaPlayerCtrl MPC;
	private string VideoName;
	private string PATH = "file://storage/emulated/0/vrtify/vravel_videos/";
	private string FULL_PATH = "";

	public string HomeScreen;


	bool one_click = false;
	bool timer_running;
	float timer_for_double_click;
	float delay = 0.5f;


	void Awake(){
		
		DOTween.Init ();

	}

	void Start () {

		VideoName = GameObject.FindGameObjectWithTag ("INMORTAL").GetComponent<InmortalScript> ().VideoToPlay;

		//VideoName = "https://player.vimeo.com/external/213004364.hd.mp4?s=738f1be9ec7ee92f0e20d412facb2fd1737b9eee&profile_id=119";
        
		FULL_PATH = VideoName;

		MPC.m_strFileName = FULL_PATH;

		MPC.m_bAutoPlay = true;

		MPC.Load (FULL_PATH);

		showMsj ();

	}
		
	private void showMsj(){
		print ("");
	}

	void Update() {

		if (Input.GetMouseButtonDown (0)) {

			if (!one_click) {
				
				one_click = true;
				timer_for_double_click = Time.time; 

			} else {
				
				one_click = false;

				Application.LoadLevel (HomeScreen);

			}
		}

		if (one_click) {
			if ((Time.time - timer_for_double_click) > delay) {
				one_click = false;
			}
		}

	}



}
