using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sacarMenaje : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (EraseAll ());	
	}
	
	IEnumerator EraseAll(){
		yield return new WaitForSeconds (3f);
		gameObject.SetActive (false);
	}
}
