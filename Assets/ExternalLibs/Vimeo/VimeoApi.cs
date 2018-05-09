using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System.Linq;

public class VimeoApi : MonoBehaviour {
    public const string TOKEN = "6ba4b7ac492382279e222912d314ca23";
    public GameObject CoverPrefab;

    public VimeoResponseObject response;

    /*void Start () {
       // StartCoroutine(VimeoRequest());
    }*/

    IEnumerator VimeoRequest() {
        string url = "https://api.vimeo.com/me/videos?fields=uri,pictures,name,files,download&per_page=5";
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url);
        request.SetRequestHeader(getHeader()[0], getHeader()[1]);
        request.Send();

        while (!request.isDone) { 
            yield return null;
        }
        
        response = JsonUtility.FromJson<VimeoResponseObject>(request.downloadHandler.text);

        GameObject go;
        for (int i = 0; i < response.data.Length; i++) {
            go = Instantiate(CoverPrefab, (Vector3.right * 5) * i, Quaternion.identity) as GameObject;
            go.GetComponent<VimeoCover>().SetData(response.data[i]);
        }

    }

    public static string GetVideoURL(string id) {
        return "https://api.vimeo.com/me/videos/"+ id + "?fields=uri,pictures,name,files,download";
    }


    public static string[] getHeader() {
        return new string[] { "Authorization", "Bearer " + TOKEN };
    }

}

[System.Serializable]
public class VimeoResponseObject {
    public int total, page, per_page;
    public VimeoVideo[] data;
    public VimeoPaging paging;
}

[System.Serializable]
public class VimeoPaging {
    public string next, previous, first, last;
}

[System.Serializable]
public class VimeoVideo : ISerializationCallbackReceiver {
    public string name, uri, id;
    public VimeoVideoData[] files, download;
    public VimeoPictures pictures;

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize() {
        id = uri.Replace("/videos/", "");
        if (id.Contains(":")) id = id.Split(':')[0];

        files = files.OrderByDescending(x => x.height).ToArray();
        download = download.OrderByDescending(x => x.height).ToArray();
    }
}

[System.Serializable]
public class VimeoVideoData {
    public string quality, type, link, md5, link_secure, created_time, expires;
    public int size , width, height;
    public float fps;
}

[System.Serializable]
public class VimeoPictures {
    public string uri, type;
    public bool active;
    public VimeoPictureData[] sizes;

    public enum PictureSize {
        TINY,
        EXTRA_SMALL,
        SMALL,
        MEDIUM,
        LARGE,
        EXTRA_LARGE
    }

    public string GetPicture(PictureSize size = PictureSize.MEDIUM) {
        return sizes[(int)size].link;
    }

    public void SetPicturePath(string path, PictureSize size = PictureSize.MEDIUM) {
        sizes[(int)size].link = path;
    }
}


[System.Serializable]
public class VimeoPictureData {
    public int width, height;
    public string link, link_with_play_button;
}


