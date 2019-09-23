using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class MainMenuController : MonoBehaviour, IMatchmakingCallbacks, IOnEventCallback{

	public static MainMenuController instance { get; private set; }

	public GameObject homeScreen;
	public GameObject levelSelect;
	public GameObject lobbyScreen;

	private void Start() {
		PhotonNetwork.AddCallbackTarget( this );
		instance = this;
		homeScreen.SetActive( false );
		levelSelect.SetActive( false );
		StartCoroutine( WaitForConnect() );
	}

	private IEnumerator WaitForConnect() {
		Photon.Pun.PhotonNetwork.ConnectUsingSettings();
		while (!Photon.Pun.PhotonNetwork.IsConnectedAndReady) {
			yield return null;
		}
		GoToHome();
	}

	public void GoToHome() {
		levelSelect.SetActive( false );
		homeScreen.SetActive( true );
		lobbyScreen.SetActive( false );
	}

	public void GoToHoleSelect() {
		levelSelect.SetActive( true );
		homeScreen.SetActive( false );
		lobbyScreen.SetActive( false );
	}

	public void GoToJoinGame() {
		RoomOptions op = new RoomOptions();
		op.MaxPlayers = 6;
		op.PublishUserId = true;
		GoToLobby();
		PhotonNetwork.JoinRoom( "Debug Room" );
	}

	public void GoToLobby() {
		levelSelect.SetActive( false );
		homeScreen.SetActive( false );
		lobbyScreen.SetActive( true );
	}

	public void TryJoinGame() {
		RoomOptions op = new RoomOptions();
		op.MaxPlayers = 6;
		op.PublishUserId = true;
		MainMenuController.instance.GoToLobby();
		PhotonNetwork.JoinOrCreateRoom( "Debug Room", op, TypedLobby.Default );
	}

	public void StartGame() {
		if (PhotonNetwork.IsMasterClient) {
			RaiseEventOptions evOp = new RaiseEventOptions();
			evOp.CachingOption = EventCaching.DoNotCache;
			evOp.Receivers = ReceiverGroup.All;
			PhotonNetwork.RaiseEvent( (byte)10, null, evOp, SendOptions.SendReliable );
		}
	}

	public void OnFriendListUpdate( List<FriendInfo> friendList ) {
	}

	public void OnCreatedRoom() {
	}

	public void OnCreateRoomFailed( short returnCode, string message ) {
	}

	public void OnJoinedRoom() {
	}

	public void OnJoinRoomFailed( short returnCode, string message ) {
		GoToHome();
	}

	public void OnJoinRandomFailed( short returnCode, string message ) {
	}

	public void OnLeftRoom() {
	}

	public void OnEvent( EventData photonEvent ) {
		if(photonEvent.Code == (byte)10) {
			PhotonNetwork.LoadLevel( 1 );
		}
	}
}