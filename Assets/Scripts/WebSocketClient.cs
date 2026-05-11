using NativeWebSocket;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;
    private bool isConnecting = false;
    private bool manuallyClosed = false;
    private string IP_Casa = "192.168.178.23";
    private string IP_HotSpot = "10.153.54.75";

    public AppManager manager;

    async void Start()
    {
        await ConnectSocket();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async Task ConnectSocket()
    {
        if (isConnecting)
            return;

        isConnecting = true;

        websocket = new WebSocket($"ws://{IP_HotSpot}:5000/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket opened");

            UnityMainThreadDispatcher.Instance().Enqueue(async () =>
            {
                await manager.RefreshArtifacts();
            });
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed");

            if (!manuallyClosed)
            {
                RetryConnection();
            }
        };

        websocket.OnMessage += (bytes) =>
        {
            string json = Encoding.UTF8.GetString(bytes);

            Debug.Log("Ricevuto: " + json);

            // parse base message
            WSMessage baseMsg = JsonUtility.FromJson<WSMessage>(json);

            if (baseMsg == null)
            {
                Debug.Log("WSMessage NULL");
                return;
            }

            Debug.Log("EVENT TYPE: " + baseMsg.eventType);

            switch (baseMsg.eventType)
            {
                //DELETE
                case "delete":

                    Debug.Log("WebSocket delete");

                    DeleteMessage msg = JsonUtility.FromJson<DeleteMessage>(json);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        manager.DeleteArtifact(msg.id);
                    });

                    break;
 

                //CREATE
                case "create":
                    
                    Debug.Log("WebSocket create");
                    
                    Artifact artifact = JsonUtility.FromJson<Artifact>(json);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        manager.SpawnSingleArtifact(artifact);
                    });

                    break;


                //UPDATE
                case "update":

                    Debug.Log("WebSocket UPDATE");

                    Artifact updatedArtifact =
                        JsonUtility.FromJson<Artifact>(json);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        manager.UpdateArtifact(updatedArtifact);
                    });

                    break;


                default:

                    Debug.LogWarning("Evento WS sconosciuto: " + baseMsg.eventType);

                    break;
            }

        };

        try
        {
            await websocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.Log("ECCEZIONE DI CONNESSIONE: " + ex.Message);

            RetryConnection();
        }

        isConnecting = false;
    }

    private async void RetryConnection()
    {
        Debug.Log("Retry tra 2 secondi...");

        await Task.Delay(2000);

        await ConnectSocket();
    }

    private async void OnApplicationQuit()
    {
        manuallyClosed = true;

        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}