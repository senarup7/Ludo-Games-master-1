
using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
public class RTM_PlayerConnection : MonoBehaviour
{
    public Text connectionStatusText;

    private void OnEnable()
    {
        StartConnecting();
        NetworkHandler.OnStatusChange += OnStatusChangeAction;


    }

    void StartConnecting()
    {
        FindObjectOfType<NetworkHandler>().ConnectRoom();
    }

    void OnStatusChangeAction(PhotonConnectingStatus status)
    {
        switch (status)
        {
            case PhotonConnectingStatus.CONNECTED_TO_MASTER:
                connectionStatusText.text = "Connecting Server";

                break;
            case PhotonConnectingStatus.DISCONNECTED:
                connectionStatusText.text = "Server Connected Failed";
    
                break;
            case PhotonConnectingStatus.JOINED_ROOM:
                connectionStatusText.text = "Joined Room-Waiting For Players";
               
                if (!PhotonNetwork.IsMasterClient)
                {
                    //_CloseButton.interactable = false;
                }
                break;
            case PhotonConnectingStatus.NO_ROOM_FOUND:
               
                break;
            case PhotonConnectingStatus.READY_TO_GO:
               
                connectionStatusText.text = "New Player Joined";
                // _CloseButton.interactable = false;
                StartCoroutine(WaitForBothPlayerContributionAndLoadGameplay());
                break;
            case PhotonConnectingStatus.TIMED_OUT:
                Debug.LogError("Time out");
                connectionStatusText.text = " Time Out, Try Again";
                break;
        }
    }

    IEnumerator WaitForBothPlayerContributionAndLoadGameplay()
    {
        yield return new WaitForSeconds(2);

        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        MainMenuUIManager.Instance.OnPlay();
    }

    public void CancelSearch()
    {
        PhotonNetwork.Disconnect();
        connectionStatusText.text = "";
        StopAllCoroutines();
        FindObjectOfType<NetworkHandler>().StopAllCoroutines();
    }

    private void OnDisable()
    {
        NetworkHandler.OnStatusChange -= OnStatusChangeAction;
    }
}
