using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour {

    public delegate void DownloadListDelegate(List<ImageDownloadRequest> list, ProgressDelegate onProgress , CompleteDelegate onComplete);
    public delegate void CompleteDelegate();
    public delegate void ProgressDelegate(float progress);
    public delegate Texture2D CacheDelegate(string id);

    public const int MAX_DOWNLOADS = 5;
    public const int MAX_TRY = 5;
    public const int TIMEOUT = 10;
    public static DownloadListDelegate DownloadList, ForceDownloadList;
    public static System.Action StopAllDownloads, StopRoutine;
    public static CacheDelegate FindTexture;

    private Dictionary<string, Texture2D> textureDictionary;
    private IEnumerator currentRoutine;


    void Awake() {
        StopAllDownloads  = StopAll;
        StopRoutine       = StopActualRoutine;
        DownloadList      = DownloadTextureList;
        ForceDownloadList = ForceDownloadTextureList;
        FindTexture       = GetTextureFromCache;
        textureDictionary = new Dictionary<string, Texture2D>();
    }

    public Texture2D GetTextureFromCache(string id) {
        if (textureDictionary.ContainsKey(id)) return textureDictionary[id];
        else return null;
    }

    public void DownloadTextureList(List<ImageDownloadRequest> requestList, ProgressDelegate OnProgress, CompleteDelegate OnComplete) {
        currentRoutine = DownloadTextList(requestList, OnProgress, OnComplete);
        StartCoroutine(currentRoutine);
    }

    public void ForceDownloadTextureList(List<ImageDownloadRequest> requestList, ProgressDelegate OnProgress, CompleteDelegate OnComplete) {
        StartCoroutine(DownloadTextList(requestList, OnProgress, OnComplete));
    }

    public IEnumerator DownloadTextList(List<ImageDownloadRequest> requestList, ProgressDelegate OnProgress, CompleteDelegate OnComplete) {
        ImageDownloadRequest imageRequest;
        List<ImageDownloadRequest> requests = new List<ImageDownloadRequest>();
        float totalRequests = requestList.Count;
        float downloadedCount = 0;
        float intervalTime = Time.realtimeSinceStartup, deltaTime = 0;

        while (requestList.Count > 0 || requests.Count > 0) {
            while (requests.Count < MAX_DOWNLOADS && requestList.Count > 0) {
                imageRequest = requestList[0];
                if (textureDictionary.ContainsKey(imageRequest.id)) {
                    imageRequest.OnComplete(textureDictionary[imageRequest.id]);
                    downloadedCount++;
                } else {
                    imageRequest.loader = new WWW(imageRequest.url);
                    requests.Add(imageRequest);
                }

                requestList.RemoveAt(0);
            }

            for (int i = 0; i < requests.Count; i++) {
                imageRequest = requests[i];
                imageRequest.time += deltaTime;

                if (!imageRequest.loader.isDone) {
                    if (imageRequest.time > TIMEOUT) {
                        if (imageRequest.retry == MAX_TRY) {
                            imageRequest.loader.Dispose();
                            requests.RemoveAt(i);
                            continue;
                        }

                        print("TIMEOUT: " + imageRequest.time + " || " + imageRequest.url);
                        requests[i].loader.Dispose();
                        requests[i].loader = new WWW(imageRequest.url);
                        imageRequest.time = 0;
                        imageRequest.retry++;
                    }
                    continue;
                }

                if (!string.IsNullOrEmpty(imageRequest.loader.error)) {
                    if(imageRequest.retry == MAX_TRY) {
                        imageRequest.loader.Dispose();
                        requests.RemoveAt(i);
                        continue;
                    }
                    print("error: " + imageRequest.loader.error + " || " + imageRequest.url);
                    requests[i].loader.Dispose();
                    requests[i].loader = new WWW(imageRequest.url);
                    imageRequest.time = 0;
                    imageRequest.retry++;
                    continue;
                }

                textureDictionary.Add(imageRequest.id, imageRequest.loader.texture);
                imageRequest.OnComplete(imageRequest.loader.texture);
                imageRequest.loader.Dispose();
                requests.RemoveAt(i);
                
                downloadedCount++;
                OnProgress((downloadedCount / totalRequests));
                yield return null;
            }

            yield return null;

            deltaTime    = Time.realtimeSinceStartup - intervalTime;
            intervalTime = Time.realtimeSinceStartup;
        }

        OnComplete();

        requestList.Clear();
        requests.Clear();

        requestList = null;
        requests    = null;
        currentRoutine = null;
    }

    void StopAll() { StopAllCoroutines(); }

    void StopActualRoutine() {
        if (currentRoutine == null) return;
        StopCoroutine(currentRoutine);
    }
}

public class ImageDownloadRequest {
    public string id, url;
    public System.Action<Texture2D> OnComplete;
    public WWW loader;
    public float time;
    public int retry = 0;

    public ImageDownloadRequest(string id, string url , System.Action<Texture2D> OnComplete) {
        this.id = id;
        this.url = url;
        this.OnComplete = OnComplete;
    }
}

