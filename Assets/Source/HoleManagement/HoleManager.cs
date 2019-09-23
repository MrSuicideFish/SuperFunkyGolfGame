using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HoleManager  {

	private static HoleManager _instance;
	public static HoleManager instance {
		get {
			return _instance;
		}
	}

	private int _currentHoleIdx;
	private HoleQueue _holeQueue;

	public bool IsFirstHole {
		get {
			return _currentHoleIdx <= 0;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnAppStart() {
		_instance = new HoleManager();
	}

	public HoleManager() {
		// load default hole queue
		_holeQueue = HoleQueue.GetAllHoleQueues()[0];
	}

	public bool CanGoToNextHole() {
		HoleInfo[ ] holesQueued = _holeQueue.holes;
		return _currentHoleIdx + 1 < holesQueued.Length;
	}

	public void GoToNextHole() {
		Photon.Pun.PhotonNetwork.IsMessageQueueRunning = false;
		if(_currentHoleIdx < 0) {
			_currentHoleIdx = 0;
		} else {
			_currentHoleIdx++;
		}

		HoleInfo[ ] holesQueued = _holeQueue.holes;
		HoleInfo nextHole = holesQueued[_currentHoleIdx];
		SceneManager.LoadScene( nextHole.sceneBuildIdx );
		Routine.CoroutineHelper.DoAfter( () => {
			Photon.Pun.PhotonNetwork.IsMessageQueueRunning = true;
		}, 0.1f );
	}
}
