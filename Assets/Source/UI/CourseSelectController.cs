using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class CourseSelectController : MonoBehaviour{

	public void SelectCourse(int course) {
		MainMenuController.instance.TryJoinGame();
	}
}
