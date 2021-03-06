using UnityEngine;
using System.Collections;
using System;

public class MPBase : MonoBehaviour 
{
    public string connectToIp = "127.0.0.1";
    public int connectPort = 25000;
    public bool useNAT = false;
    public string ipAddress = "";
    public string port = "";
    public string playerName = "<NAME ME>";
    public bool received=false;

    void Start()
    {
        MasterServer.ipAddress = "localhost";
        MasterServer.port = 25565;
    }

    void OnGUI()
    {
        if (Network.peerType == NetworkPeerType.Disconnected && !received)
        {
            if (GUILayout.Button("Find Server"))
            {
                MasterServer.ClearHostList();
                MasterServer.RequestHostList("Project_Flat");
                //received = false;
            }
            
            if (GUILayout.Button("Connect"))
            {
                if (playerName != "<NAME ME>")
                {
                    //Network.useNat = useNAT;
                    Network.Connect(connectToIp, connectPort);
                    PlayerPrefs.SetString("playerName", playerName);
                    
                }
            }
            if (GUILayout.Button("Start Server"))
            {
                if (playerName != "<NAME ME>")
                {
                    //Network.useNat = useNAT;
                    useNAT = !Network.HavePublicAddress();
                    Network.InitializeServer(32, connectPort,useNAT);
                    MasterServer.RegisterHost("Project_Flat", playerName);
                    foreach(GameObject go in FindObjectsOfType(typeof(GameObject)))
                    {
                        go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
                    }

                    PlayerPrefs.SetString("playerName", playerName);

                }
            }

            playerName = GUILayout.TextField(playerName);
            connectToIp = GUILayout.TextField(connectToIp);
            connectPort = Convert.ToInt32(GUILayout.TextField(connectPort.ToString()));
        }
        else if (received)
        {
            if (MasterServer.PollHostList().Length != 0)
            {
                HostData[] hostData = MasterServer.PollHostList();
                foreach (HostData host in hostData)
                {
                    Debug.Log("THere is a server here");
                    if (GUILayout.Button("Join "+host.gameName))
                    {
                        Network.Connect(host.ip, host.port);
                        PlayerPrefs.SetString("playerName", playerName);
                        received = false;
                        MasterServer.ClearHostList();
                        
                    }
                }
               
            }
            if(GUILayout.Button("Back to Menu"))
            {
                received = false;
                MasterServer.ClearHostList();
            }
        }
        else
        {
            if (Network.peerType == NetworkPeerType.Connecting) GUILayout.Label("Connect Status: Connecting");
            else if (Network.peerType == NetworkPeerType.Client)
            {
                GUILayout.Label("Connection Status: Client!");
                GUILayout.Label("Ping to Server: "+ Network.GetAveragePing(Network.connections[0]));
            }
            else if (Network.peerType == NetworkPeerType.Server)
            {
                GUILayout.Label("Connection Status: Server!");
                GUILayout.Label("Connections: "+ Network.connections.Length);
                if (Network.connections.Length >= 1)
                {
                    GUILayout.Label("Ping to Server: " + Network.GetAveragePing(Network.connections[0]));
                }
            }

            if (GUILayout.Button("Disconnect"))
            {
                Network.Disconnect(200);
            }

            ipAddress = Network.player.ipAddress;
            port = Network.player.port.ToString();
            GUILayout.Label("IP Address: " + ipAddress + ":" + port);

        }
        
    }

    void OnConnectedToServer()
    {
        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
        {
            go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMasterServerEvent(MasterServerEvent ms)
    {
        if (ms == MasterServerEvent.HostListReceived)
        {
            received = true;
        }
    }
}
