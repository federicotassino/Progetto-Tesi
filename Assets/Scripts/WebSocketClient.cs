using NativeWebSocket;
using UnityEngine;
using System.Text;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;

    public AppManager manager;

    async void Start()
    {
        websocket = new WebSocket("ws://10.153.54.75:5000/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket aperto");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket errore: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket chiuso");
        };

        websocket.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);

            Debug.Log("Ricevuto: " + json);

            Artifact artifact = JsonUtility.FromJson<Artifact>(json);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                manager.SpawnSingleArtifact(artifact);
            });
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}