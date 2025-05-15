using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ServerEndpointsEnum
{
    public static string
        BASE_URL = "http://localhost:5005",
        //HAND_DATA = "/hand-data",
        HAND_DATA = "/et-hand-data",
        HAND_DATA_PLAY_MODE = "/et-hand-data-play-mode",
        END_TRAINING = "/save-model",
        NEW_MODEL = "/new-model",
        TRAIN = "/et-train32",
        START_NOTE ="/start-note",

        RTT_HAND_DATA = "/rtt-hand-data",
        RTT_TRAIN = "/rtt-train",
        RTT_END_TRAINING = "/rtt-save-model",
        RTT_NEW_MODEL = "/rtt-new-model";
}

public class ServerGateway : MonoBehaviour
{
    [SerializeField] private string _serverUrl;

    public void SetServerUrl(string serverUrl)
    {
        _serverUrl = serverUrl;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
    
    public void SendStartNote(string startnote, System.Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.START_NOTE, "POST", "{\"start_note\": \" " + startnote + " \" }", callback));
    }

    public void CreateNewModel(int outputDimension, Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.NEW_MODEL, "POST",
            "{\"output_dimension\": " + (outputDimension + 1) + "}", callback));
    }

    public void SendHandData(RequestWrapper requestWrapper, Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.HAND_DATA, "POST",
            JsonConvert.SerializeObject(requestWrapper), callback));
    }

    public void SendHandDataPlayMode(RequestWrapper requestWrapper, Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.HAND_DATA_PLAY_MODE, "POST",
            JsonConvert.SerializeObject(requestWrapper), callback));
    }

    public void Train(Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.TRAIN, "GET", "{}", callback));
    }

    public void EndTraining(Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.END_TRAINING, "GET", "{}", callback));
    }
    
    //Real time training---------------------------------
    public void RTTSendHandData(RequestWrapper requestWrapper, Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.RTT_HAND_DATA, "POST",
            JsonConvert.SerializeObject(requestWrapper), callback));
    }
    
    public void RTTTrain(Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.RTT_TRAIN, "GET", "{}", callback));
    }
    public void RTTEndTraining(Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.RTT_END_TRAINING, "GET", "{}", callback));
    }
    public void RTTCreateNewModel(int outputDimension, Action<string> callback)
    {
        StartCoroutine(ExecRequest(_serverUrl + ServerEndpointsEnum.RTT_NEW_MODEL, "POST",
            "{\"output_dimension\": " + (outputDimension + 1) + "}", callback));
    }
    
    private IEnumerator ExecRequest(string uri, string method, string jsonData, Action<string> callback)
    {
        var webRequest = new UnityWebRequest(uri, method);
        var jsonToSend = new UTF8Encoding().GetBytes(jsonData);

        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.DataProcessingError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + webRequest.downloadHandler.text);
            callback?.Invoke(null);
        }
        else
        {
            callback?.Invoke(webRequest.downloadHandler.text);
        }
    }
}