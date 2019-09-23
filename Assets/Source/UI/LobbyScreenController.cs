using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyScreenController : MonoBehaviour {

	public LobbyPlayerListItem lobbyPlayerListItemPrefab;
	public VerticalLayoutGroup playerListParent;
	public Button startBtn;

	private List<LobbyPlayerListItem> lobbyPlayerList;

	private void Awake() {
		lobbyPlayerList = new List<LobbyPlayerListItem>();
		PlayerManager.instance.onPlayerListChanged.AddListener( RefreshPlayerList );
	}

	private void OnEnable() {
		Routine.CoroutineHelper.DoAfter( () => {
			startBtn.gameObject.SetActive( PhotonNetwork.IsMasterClient );
		}, 1f );
	}

	private void ClearPlayerList() {
		for(int i = 0; i < lobbyPlayerList.Count; i++) {
			GameObject.Destroy( lobbyPlayerList[i].gameObject );
		}
		lobbyPlayerList.Clear();
	}

	private void AddPlayerToList(string playerName, Color playerColor ) {
		LobbyPlayerListItem newListItem = GameObject.Instantiate( lobbyPlayerListItemPrefab );
		newListItem.transform.SetParent( playerListParent.transform, true );
		newListItem.transform.localPosition = Vector3.zero;
		newListItem.transform.localEulerAngles = Vector3.zero;
		newListItem.transform.localScale = Vector3.one;
		newListItem.playerNameText.text = playerName;
		newListItem.backgroundImg.color = playerColor;
		newListItem.gameObject.SetActive( true );
		lobbyPlayerList.Add( newListItem );
	}

	public void RefreshPlayerList() {
		ClearPlayerList();
		for(int i = 0; i < PlayerManager.instance.playerCount; i++) {
			PlayerInfo player = PlayerManager.instance.GetPlayerInfoByIdx( i );
			AddPlayerToList( player.playerName, player.playerColor );
		}
	}
}