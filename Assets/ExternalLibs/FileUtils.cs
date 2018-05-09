using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class FileUtils {
    public delegate void ProgressDelegate(float progress);
    public delegate void CompleteSaveDelegate(FileInfo info);
    public delegate void CompleteDownloadDelegate(string info);

    public const string DATA_FOLDER = "/data/", IMAGES_FOLDER = "/images/", VIDEO_FOLDER = "/video/" , DATA_EXTENSION = ".dat";


    public static IEnumerator DownloadAndSave(string url, string fullPath , ProgressDelegate OnProgress = null , CompleteSaveDelegate OnComplete = null) {


        UnityEngine.Networking.UnityWebRequest req = UnityEngine.Networking.UnityWebRequest.Get(url);
        req.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        req.Send();

        float p = 0;
        while (!req.downloadHandler.isDone) {
            while (p == 0 && req.downloadProgress == 0.5f) yield return null;
            p = req.downloadProgress;
            if (OnProgress != null) OnProgress(req.downloadProgress);
            yield return null;
        }

        File.WriteAllBytes(fullPath, req.downloadHandler.data);

        if (OnComplete != null) OnComplete(new FileInfo(fullPath));
    }

    public static IEnumerator GetDownloadSize(string url , CompleteDownloadDelegate OnComplete) {
        UnityEngine.Networking.UnityWebRequest req = UnityEngine.Networking.UnityWebRequest.Head(url);
        
        yield return req.Send();

        OnComplete(req.GetResponseHeader("Content-Length"));

        req.Dispose();
    }

    public static void DeleteFile(FileInfo file) {
        if (file != null && file.Exists) file.Delete();
    }

    public static string SaveJSONData(object json, string ID) {
        string textToSave = JsonUtility.ToJson(json, true);
        if (!Directory.Exists(Application.persistentDataPath + DATA_FOLDER)) Directory.CreateDirectory(Application.persistentDataPath + DATA_FOLDER);
        string path = Application.persistentDataPath + DATA_FOLDER + ID + DATA_EXTENSION;

        try {
            using (StreamWriter writer = new StreamWriter(path, false)) {
                writer.Write(textToSave);
                writer.Close();
            }
        }
        finally { }

        return path;
    }

    public static string SaveImage(Texture2D texture, string id) {
        if (!Directory.Exists(Application.persistentDataPath + IMAGES_FOLDER)) Directory.CreateDirectory(Application.persistentDataPath + IMAGES_FOLDER);
        string path = Application.persistentDataPath + IMAGES_FOLDER + id + ".jpg";
        File.WriteAllBytes(path, texture.EncodeToJPG(90));
        return path;
    }

    public static SavedData GetSavedData(string ID) {
        if (!Directory.Exists(Application.persistentDataPath + DATA_FOLDER)) Directory.CreateDirectory(Application.persistentDataPath + DATA_FOLDER);
        string path = Application.persistentDataPath + DATA_FOLDER + ID + DATA_EXTENSION;

        string result;
        try {
            result = File.ReadAllText(path);
        }
        catch (System.Exception e) {
            result = null;
        }

        if (result == null) return null;

        return JsonUtility.FromJson<SavedData>(result);
    }

    public static List<SavedData> GetDownloadedVideos() {
        List<SavedData> videos = new List<SavedData>();
        if (!Directory.Exists(Application.persistentDataPath + DATA_FOLDER)) return videos;
        string result;
        foreach (string file in  Directory.GetFiles(Application.persistentDataPath + DATA_FOLDER, "*" + DATA_EXTENSION)) {
            try {
                result = File.ReadAllText(file);
            }
            catch (System.Exception e) {
                result = null;
            }

            if (result == null) continue;

            videos.Add(JsonUtility.FromJson<SavedData>(result));
        }

        return videos;
    }
}


[System.Serializable]
public class SavedData {
    public string title, thumbnail, audio;
    public string[] videos, qualities;

    public SavedData() {
        videos = new string[8];
        qualities = new string[8];
    }

    public void AddVideo(int index, string videoPath , string quality) {
        videos[index] = videoPath;
        qualities[index] = quality;
    }
}