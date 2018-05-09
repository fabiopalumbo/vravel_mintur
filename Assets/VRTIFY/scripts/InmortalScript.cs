using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InmortalScript : MonoBehaviour {

	void Awake () { DontDestroyOnLoad (transform.gameObject); }

	public bool Selection;
	public string LevelToLoad = "";
	public string IsVR = "";

	public string VideoToPlay = "";
	public string ExtraSound = "";

	public bool Spotify = false;

	public string MySong = "";

	public string MySongTitle = "";
	public List<string> Destinations;
	public bool HasOtherSounds = false;
	public bool IsVrtify = false;
	public bool IsExperience = false;
	public string LastScene = "";

	public string SocialThumb = "";
	public string SocialText = "";

	public string SceneDestination = "";


	public string LastSettedScene = "null";


	public List<string> destinations;

	//----------------------------------//

	public string Avatar = "";
	public string UsrName = "";
	public string Followers = "";
	public string videos = "";

	public int HARD_CHANNEL = 0;


	public string FeedToPlay = "";
	public string cam_360 = "";
	public string cam_360_2 = "";
	public string cam_180 = "";


}
