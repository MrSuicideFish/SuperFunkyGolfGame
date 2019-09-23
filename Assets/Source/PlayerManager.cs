using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : IInRoomCallbacks, IMatchmakingCallbacks, IConnectionCallbacks {

	public static PlayerManager instance { get; private set; }

	private List<PlayerInfo> _playerInfos;
	private List<PlayerInfo> playerInfos {
		get {
			if (_playerInfos == null) {
				_playerInfos = new List<PlayerInfo>();
			}
			return _playerInfos;
		}
	}

	public int playerCount { get {
			return playerInfos.Count;
		}
	}

	public UnityEvent onPlayerListChanged = new UnityEvent();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnAppStart() {
		instance = new PlayerManager();
		PhotonNetwork.AddCallbackTarget( instance );
	}

	public PlayerInfo GetPlayerInfoById( string id ) {
		for (int i = 0; i < playerInfos.Count; i++) {
			PlayerInfo p = playerInfos[i];
			if (p.playerId == id) {
				return p;
			}
		}
		return null;
	}

	public PlayerInfo GetPlayerInfoByIdx(int idx ) {
		if (idx >= 0 && idx < playerInfos.Count) {
			return playerInfos[idx];
		}
		return null;
	}

	public void SetLocalPlayerColor(Color color) {
		Hashtable newHash = new Hashtable();
		newHash.Add( "Color", new Vector3( color.r, color.g, color.b ) );
		PhotonNetwork.LocalPlayer.SetCustomProperties( newHash );
	}

	public bool IsPlayerLoggedIn(string playerId ) {
		for(int i = 0; i < playerInfos.Count; i++) {
			if(playerInfos[i].playerId == playerId) {
				return true;
			}
		}
		return false;
	}

	public void LoginPlayer( Player player ) {
		if (IsPlayerLoggedIn( player.UserId )) {
			return;
		}

		PlayerInfo newPlayer = new PlayerInfo( player.UserId );
		newPlayer.playerName = player.NickName == "" ? "Noob Player" : player.NickName;
		newPlayer.photonPlayer = player;

		// set player color
		Vector3 playerColor = (Vector3)player.CustomProperties["Color"];
		newPlayer.playerColor = new Color( playerColor.x, playerColor.y, playerColor.z );

		playerInfos.Add( newPlayer );
		if(onPlayerListChanged != null) {
			onPlayerListChanged.Invoke();
		}
		
		Debug.Log( "Logged in player: " + newPlayer.playerId );
	}

	private void LogoutPlayer(Player player) {
		for (int i = 0; i < playerInfos.Count; i++) {
			PlayerInfo p = playerInfos[i];
			if (p.playerId == player.UserId) {

				// destroy golf ball
				if(p.playerGolfBall != null) {
					GameObject.Destroy( p.playerGolfBall );
				}
				
				playerInfos.RemoveAt( i );
				if (onPlayerListChanged != null) {
					onPlayerListChanged.Invoke();
				}
				return;
			}
		}
	}

	private void ClearAllPlayers() {
		playerInfos.Clear();
	}

	public void ResetAllHoleData( ) {
		for(int i = 0; i < playerCount; i++) {
			PlayerInfo p = GetPlayerInfoByIdx( i );
			p.isInHole = false;
			p.strokes = 0;
		}
	}

	public void OnPlayerEnteredRoom( Player newPlayer ) {
		LoginPlayer( newPlayer );
	}

	public void OnPlayerLeftRoom( Player otherPlayer ) {
		LogoutPlayer( otherPlayer );
	}

	public void OnJoinedRoom() {
		Player[ ] players = PhotonNetwork.PlayerList;
		for(int i = 0; i < players.Length; i++) {
			LoginPlayer( players[i] );
		}
	}

	public void OnLeftRoom() {
		ClearAllPlayers();
	}

	public void OnCreatedRoom() {
		ClearAllPlayers();
	}

	#region USELESS
	public void OnRoomPropertiesUpdate( ExitGames.Client.Photon.Hashtable propertiesThatChanged ) {

	}

	public void OnPlayerPropertiesUpdate( Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps ) {

	}

	public void OnMasterClientSwitched( Player newMasterClient ) {

	}

	public void OnFriendListUpdate( List<FriendInfo> friendList ) {
	}

	public void OnCreateRoomFailed( short returnCode, string message ) {
	}

	public void OnJoinRoomFailed( short returnCode, string message ) {
	}

	public void OnJoinRandomFailed( short returnCode, string message ) {
	}

	public void OnConnected() {
	}

	public void OnConnectedToMaster() {
		SetLocalPlayerColor( Color.blue );
	}

	public void OnDisconnected( DisconnectCause cause ) {
	}

	public void OnRegionListReceived( RegionHandler regionHandler ) {
	}

	public void OnCustomAuthenticationResponse( Dictionary<string, object> data ) {
	}

	public void OnCustomAuthenticationFailed( string debugMessage ) {
	}

	#endregion

}
