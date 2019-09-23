using System;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

[System.Serializable]
public class PlayerInfo {
	public string playerName;
	public string playerId;
	public UnityEngine.Color playerColor;
	public Photon.Realtime.Player photonPlayer;
	public GolfBall playerGolfBall;
	public int strokes;
	public bool isInHole;

	public PlayerInfo() {

	}

	public PlayerInfo( string _playerId ) {
		playerId = _playerId;
	}
}