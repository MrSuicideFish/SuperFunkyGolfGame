using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Routine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CoreGameController : MonoBehaviourPunCallbacks, IPunObservable {

	public static CoreGameController Instance { get; private set; }

	public bool DebugJoinRoom;
	public GolfBall debugGolfBallPrefab;
	public float ballHitPower;

	private int _playerTurnIdx = -1;
	private string _playerTurnId;
	private PlayerController _localPlayerController;
	private bool _turnHasStarted;
	private bool _turnHasEnded;
	private bool _isBallInHole;
	private bool _gameHasStarted;
	private bool _gameHasEnded;

	private CameraController _gameCam;
	private CameraController gameCam {
		get {
			if(_gameCam == null) {
				_gameCam = CameraController.FindObjectOfType<CameraController>();
			}
			return _gameCam;
		}
	}

	public bool IsMyTurn {
		get {
			return _gameHasStarted && !_gameHasEnded && !_turnHasEnded && _playerTurnId == PhotonNetwork.LocalPlayer.UserId;
		}
	}

	public void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info ) {
		if (stream.IsWriting) {
			stream.SendNext( _playerTurnId );
			stream.SendNext( _playerTurnIdx );
			stream.SendNext( _turnHasEnded );
			stream.SendNext( _turnHasStarted );
			stream.SendNext( _isBallInHole );
			stream.SendNext( _gameHasStarted );
			stream.SendNext( _gameHasEnded );
		} else if (stream.IsReading) {
			_playerTurnId = (string)stream.ReceiveNext();
			_playerTurnIdx = (int)stream.ReceiveNext();
			_turnHasEnded = (bool)stream.ReceiveNext();
			_turnHasStarted = (bool)stream.ReceiveNext();
			_isBallInHole = (bool)stream.ReceiveNext();
			_gameHasStarted = (bool)stream.ReceiveNext();
			_gameHasEnded = (bool)stream.ReceiveNext();
		}
	}

	public void Awake() {
		_gameHasStarted = false;
		_gameHasEnded = false;
		Instance = this;
		PlayerManager.instance.ResetAllHoleData();
	}

	public void Start() {
		if(NametagManager.instance != null) {
			// NASTY SHIT
		}
		if (DebugJoinRoom && HoleManager.instance.IsFirstHole) {
			PhotonNetwork.ConnectUsingSettings();
		} else if(PhotonNetwork.IsMasterClient){
			StartCoroutine( StartGame_Server() );
		}
	}

	public void OnGUI() {
		if (PhotonNetwork.InRoom) {
			if (PhotonNetwork.IsMasterClient && HoleManager.instance.IsFirstHole && DebugJoinRoom) {
				if (GUI.Button( new Rect( 10, 10, 140, 40 ), "Start Game" )) {
					StartCoroutine( StartGame_Server() );
				}
			}

			for (int i = 0; i < PlayerManager.instance.playerCount; i++) {
				PlayerInfo p = PlayerManager.instance.GetPlayerInfoByIdx( i );
				string info = string.Format( "{0} (Strokes: {1})", p.playerName, p.strokes );
				GUI.Label( new Rect( 10, 180 + (16 * i), 350, 23 ), info );
			}
		}
	}

	private IEnumerator StartGame_Server() {
		_isBallInHole = false;
		_turnHasStarted = false;
		_turnHasEnded = false;
		_gameHasStarted = true;
		_gameHasEnded = false;
		yield return new WaitForSeconds( 1.0f );
		yield return GoToNextTurn_Server();
	}

	public override void OnConnectedToMaster() {
		if (DebugJoinRoom) {
			RoomOptions op = new RoomOptions();
			op.MaxPlayers = 4;
			op.PublishUserId = true;
			PhotonNetwork.JoinOrCreateRoom( "Debug Room", op, TypedLobby.Default );
		}
	}

	private bool AllPlayersFinish() {
		bool allFinish = true;
		for(int i = 0; i < PlayerManager.instance.playerCount; i++) {
			PlayerInfo p = PlayerManager.instance.GetPlayerInfoByIdx( i );
			if (!p.isInHole) {
				allFinish = false;
			}
		}

		return allFinish;
	}

	public IEnumerator GoToNextTurn_Server() {
		if (!AllPlayersFinish()) {

			if (_playerTurnIdx == -1) {
				_playerTurnIdx = 0;
			} else {
				_playerTurnIdx += 1;
				if (_playerTurnIdx >= PlayerManager.instance.playerCount) {
					_playerTurnIdx = 0;
				}
			}

			PlayerInfo player = PlayerManager.instance.GetPlayerInfoByIdx( _playerTurnIdx );
			while (player.isInHole) {
				_playerTurnIdx += 1;
				if (_playerTurnIdx >= PlayerManager.instance.playerCount) {
					_playerTurnIdx = 0;
				}

				player = PlayerManager.instance.GetPlayerInfoByIdx( _playerTurnIdx );
			}
			
			_playerTurnId = player.playerId;

			/// spawn player's ball if needed
			if (player.playerGolfBall == null) {
				this.photonView.RPC( "SpawnPlayerBall", RpcTarget.All, player.playerId );
				while (player.playerGolfBall == null) {
					yield return null;
				}
			}

			yield return new WaitForSeconds( 0.2f );

			/// send start turn remote
			this.photonView.RPC( "StartTurn", RpcTarget.All, _playerTurnId );
		} else {

			_gameHasEnded = true;

			/// do end hole
			this.photonView.RPC( "EndHole", RpcTarget.All );
		}

		yield return null;
	}

	[PunRPC]
	public void SpawnPlayerBall( string playerId ) {
		PlayerInfo player = PlayerManager.instance.GetPlayerInfoById( playerId );
		GolfBall playerBall = GolfBall.Instantiate( debugGolfBallPrefab, GetLevelSpawnPosition(), Quaternion.identity );
		playerBall.SetOwner( player );
		PhotonView ballView = playerBall.GetComponent<PhotonView>();
		ballView.TransferOwnership( player.photonPlayer );
		player.playerGolfBall = playerBall;

		if (player.photonPlayer.IsLocal) {
			_localPlayerController = new PlayerController( playerBall );
		}
	}

	[PunRPC]
	public void StartTurn(string playerId) {

		Debug.Log( "Starting turn for player: " + playerId );
		PlayerInfo p = PlayerManager.instance.GetPlayerInfoById( playerId );

		/// find player's ball
		GolfBall playerBall = p.playerGolfBall;

		/// cam go to ball
		gameCam.targetBall = playerBall;

		_isBallInHole = false;
		_turnHasEnded = false;
		_turnHasStarted = true;

		TurnStartScreen turnStartScrn = CoreGameHud.instance.ShowScreen<TurnStartScreen>( CoreGameHud.HudScreen.TurnStart );
		turnStartScrn.Show( p.playerName );
		CoroutineHelper.DoAfter( () => {
			CoreGameHud.instance.ShowScreen( CoreGameHud.HudScreen.MainHUD );
		}, 1.5f );
	}
	
	public void TryHitBall( Vector3 normal, float power ) {
		this.photonView.RPC( "HitBall_Client", RpcTarget.All, normal, power );
	}

	[PunRPC]
	public void HitBall_Client(Vector3 normal, float power ) {
		PlayerInfo p = PlayerManager.instance.GetPlayerInfoById( _playerTurnId );
		GolfBall playerBall = p.playerGolfBall;
		if(playerBall != null) {
			// todo: play hit sound

			normal.x = -normal.x;
			Rigidbody2D rb = playerBall.GetComponent<Rigidbody2D>();
			rb.AddForce( normal * power * ballHitPower, ForceMode2D.Impulse );
			_turnHasEnded = true;
			_turnHasStarted = true;

			p.strokes++;
			StartCoroutine( WaitForBallStop() );
		}
	}

	public void PlayerFinish(GolfBall ball) {
		if (PhotonNetwork.IsMasterClient) {
			string playerId = null;
			for(int i = 0; i < PlayerManager.instance.playerCount; i++) {
				PlayerInfo p = PlayerManager.instance.GetPlayerInfoByIdx( i );
				if(p.playerGolfBall != null && p.playerGolfBall == ball) {
					playerId = p.playerId;
				}
			}
			this.photonView.RPC( "PlayerFinish_Client", RpcTarget.All, playerId );
		}
	}

	[PunRPC]
	public void PlayerFinish_Client(string playerId) {

		PlayerInfo p = PlayerManager.instance.GetPlayerInfoById( playerId );
		p.isInHole = true;

		// destroy player ball
		GameObject.Destroy( p.playerGolfBall.gameObject );

		// TODO: show celebration fx
		//..

		if(playerId == _playerTurnId) {
			_turnHasEnded = true;
			_isBallInHole = true;
		}
	}

	public void KillGolfBall( GolfBall ball ) {
		if(ball != null) {
			KillGolfBall( ball.owner.playerId );
		}
	}

	public void KillGolfBall(string playerId) {
		this.photonView.RPC( "KillGolfBall_Client", RpcTarget.All, playerId );
	}

	[PunRPC]
	public void KillGolfBall_Client(string playerId) {
		PlayerInfo p = PlayerManager.instance.GetPlayerInfoById( playerId );
		if(p.playerGolfBall != null) {
			GameObject.Destroy( p.playerGolfBall.gameObject );
			p.playerGolfBall = null;
		}
	}

	private IEnumerator WaitForBallStop() {
		float stopTimer = 0.0f;
		while (true) {

			PlayerInfo currentPlayer = PlayerManager.instance.GetPlayerInfoById( _playerTurnId );
			if(currentPlayer == null) {
				break;
			}

			GolfBall ball = currentPlayer.playerGolfBall;
			if(ball == null) {
				break;
			}

			Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
			if (ballRB == null) {
				break;
			}

			if (_isBallInHole) {
				break;
			}

			if(ballRB.velocity.magnitude <= 0.05f) {
				stopTimer += Time.deltaTime;
				if(stopTimer >= 1.0f) {
					break;
				}
			} else {
				stopTimer = 0.0f;
			}

			yield return null;
		}

		if (PhotonNetwork.IsMasterClient) {
			StartCoroutine( GoToNextTurn_Server() );
		}
	}

	private Vector3 GetLevelSpawnPosition() {
		LevelSpawn spawnPoint = FindObjectOfType<LevelSpawn>();
		if(spawnPoint != null) {
			return spawnPoint.transform.position;
		}
		return Vector3.zero;
	}

	[PunRPC]
	public void EndHole() {
		CoreGameHud.instance.ShowScreen( CoreGameHud.HudScreen.HoleEnd );
		CoroutineHelper.DoAfter( () => {
			if (PhotonNetwork.IsMasterClient) {
				photonView.RPC( "GoToNextHole", RpcTarget.All );
			}
		}, 3.0f );
	}

	[PunRPC]
	private void GoToNextHole() {
		if (HoleManager.instance.CanGoToNextHole()) {
			HoleManager.instance.GoToNextHole();
		} else {
			// show match end
			Debug.Log( "~~~ALL HOLES COMPLETE!" );
		}
	}

	private void Update() {
		if(IsMyTurn && _localPlayerController != null) {
			_localPlayerController.TickInput();
		}
	}
}