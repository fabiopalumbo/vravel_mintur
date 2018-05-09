using UnityEngine;
using System.Collections;

public class VimeoCover : MonoBehaviour {
   

    private VimeoVideo data;

	public void SetData(VimeoVideo videoData) {
        data = videoData;
        StartCoroutine(LoadImage(data.pictures.sizes[(int)VimeoPictures.PictureSize.MEDIUM].link)) ;
    }

    IEnumerator LoadImage(string url) {
        WWW loader = new WWW(url);
        yield return loader;

        GetComponent<MeshRenderer>().material.mainTexture = loader.texture;
    }

    void OnMouseDown() {
        VimeoVideoMenu3D.Appear(data, data.id);
    }

}
