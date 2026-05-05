using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class SignalRClient : MonoBehaviour
{
    private HubConnection connection;
    static private string ipV4 = "10.153.54.75";

    public AppManager manager; // riferimento al tuo spawner

    async void Start()
    {
        Debug.Log("START SIGNALR");

        connection = new HubConnectionBuilder()
            .WithUrl("http://10.153.54.75:5000/artifactHub", options =>
            {
                // 🔥 FORZA WEBSOCKETS (fondamentale per HoloLens)
                options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();

        connection.On<Artifact>("ArtifactCreated", (artifact) =>
        {
            Debug.Log("Artifact ricevuto: " + artifact.name);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                manager.SpawnSingleArtifact(artifact);
            });
        });

        connection.Closed += async (error) =>
        {
            Debug.LogError("CLOSED: " + error);
            await Task.Delay(2000);
            await connection.StartAsync();
        };

        connection.Reconnecting += (error) =>
        {
            Debug.LogWarning("RECONNECTING...");
            return Task.CompletedTask;
        };

        try
        {
            await connection.StartAsync();
            Debug.Log("✅ Connesso a SignalR");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ ERRORE CONNESSIONE: " + e);
        }
    }

    //async void Start()
    //{
    //    Debug.Log("START SIGNALR");
    //    connection = new HubConnectionBuilder()
    //        .WithUrl("http://10.153.54.75:5000/artifactHub")
    //        .Build();

    //    Debug.Log(UnityMainThreadDispatcher.Instance() == null
    //        ? "DISPATCHER NULL"
    //        : "DISPATCHER OK");

    //    connection.On<Artifact>("ArtifactCreated", (artifact) =>
    //    {
    //        Debug.Log("Nuovo artifact ricevuto: " + artifact.ToString());
    //        //Artifact newArtifact = new Artifact();
    //        //newArtifact = artifact;

    //        UnityMainThreadDispatcher.Instance().Enqueue(() =>
    //        {
    //            Debug.Log("Eseguito nel main thread");

    //            manager.SpawnSingleArtifact(artifact);
    //        });
    //    });

    //    //connection.On<string>("ArtifactCreated", (json) =>
    //    //{
    //    //    Debug.Log("RAW JSON: " + json);

    //    //    try
    //    //    {
    //    //        Debug.Log("RAW: " + json);

    //    //        UnityMainThreadDispatcher.Instance().Enqueue(() =>
    //    //        {
    //    //            Artifact artifact = JsonUtility.FromJson<Artifact>(json);
    //    //            manager.SpawnSingleArtifact(artifact);
    //    //        });
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        Debug.LogError("HANDLER ERROR: " + e);
    //    //    }
    //    //});

    //    await connection.StartAsync();
    //    Debug.Log(connection.ToString());
    //    Debug.Log("Connesso a SignalR");

    //    connection.Closed += async (error) =>
    //    {
    //        Debug.LogError("SIGNALR CLOSED: " + error);
    //    };

    //    connection.Reconnecting += error =>
    //    {
    //        Debug.LogWarning("SIGNALR RECONNECTING");
    //        return Task.CompletedTask;
    //    };
    //}
}