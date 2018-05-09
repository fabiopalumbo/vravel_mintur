using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeScene : MonoBehaviour {


	void Start () {
		StartCoroutine (ChangeScene ());
	}
	
	IEnumerator ChangeScene(){
		yield return new WaitForSeconds (1);
		Application.LoadLevel ("Home");
	}
}
