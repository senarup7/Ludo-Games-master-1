
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;

public enum PhotonConnectingStatus
{
    CONNECTED_TO_MASTER,
    DISCONNECTED,
    NO_ROOM_FOUND,
    JOINED_ROOM,
    READY_TO_GO,
    TIMED_OUT

}

public class NetworkHandler : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";

    public const int ExpectedMaxPlayers = 2;
    public const string LobbyName = "LudoLobby";

    public static Action<PhotonConnectingStatus> OnStatusChange;
    public static Action<Photon.Realtime.Player> OnClientFound, OnHostFound;


    private int mPlayerLevel = 0;
   // private int mMinOpponentlLevel = 0;
   // private int mMaxOpponentLevel = 0;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ConnectRoom()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnectedAndReady)
        {
            JoinRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            if (PhotonNetwork.NetworkClientState != ClientState.ConnectingToMasterserver)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    /// <summary>
    /// Joins a random room matching passed in query.
    /// </summary>
    void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom(null, ExpectedMaxPlayers);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = "GameRoom";
        OnStatusChange?.Invoke(PhotonConnectingStatus.CONNECTED_TO_MASTER);

        JoinRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        OnStatusChange?.Invoke(PhotonConnectingStatus.DISCONNECTED);
    }

    public override void OnJoinedRoom()
    {
        OnStatusChange?.Invoke(PhotonConnectingStatus.JOINED_ROOM);

        if (PhotonNetwork.PlayerList.Length == 2)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!PhotonNetwork.PlayerList[i].IsLocal)
                {
                    OnHostFound?.Invoke(PhotonNetwork.PlayerList[i]);
                    break;
                }
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        OnStatusChange?.Invoke(PhotonConnectingStatus.NO_ROOM_FOUND);

        // create room options with sql like query 
        // more - https://doc.photonengine.com/en-us/pun/current/lobby-and-matchmaking/matchmaking-and-lobby#skill_based_matchmaking
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = ExpectedMaxPlayers;
        roomOptions.PlayerTtl = 20000;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", mPlayerLevel } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0" };

        PhotonNetwork.CreateRoom("room " + PhotonNetwork.LocalPlayer.UserId, roomOptions);
        StartCoroutine(RunWaitTimer());
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        StopAllCoroutines();
        if (PhotonNetwork.PlayerList.Length == ExpectedMaxPlayers)
        {
            OnClientFound?.Invoke(newPlayer);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                OnStatusChange?.Invoke(PhotonConnectingStatus.READY_TO_GO);
            }
        }
    }

    IEnumerator RunWaitTimer()
    {
        float timer = 0;

        while (timer < 20)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        OnStatusChange?.Invoke(PhotonConnectingStatus.TIMED_OUT);
    }
}