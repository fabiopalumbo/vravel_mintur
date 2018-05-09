using UnityEngine;
using System.Collections;

public class ManualLock : MonoBehaviour {

	public VimeoVideoMenu3D VimeoDown;
	//public Core_Controller Core;

	void EraseVideos(){
		//Core.EraseAll ();
	}

	public void ActivateVimeo(string val){
		VimeoDown.gameObject.SetActive (true);
		//Core.SimpleVideoHIde ();
		EraseVideos ();
		VimeoDown.ManualLoadID (val);
	}
}
