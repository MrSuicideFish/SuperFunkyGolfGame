using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HomeScreenController : MonoBehaviour{

	private ColorText currentColor = ColorText.BLUE;
	private enum ColorText : int {
		BLUE = 0, //BLUE
		RED = 1, //RED
		GREEN = 2, //GREEN
		PURPLE = 3 //PURPLE
	}

	public Text playerColorText;

	private void Start() {
		ToggleNextColor();
	}

	private Color EnumToColor(ColorText textColor) {

		Color newColor = Color.white;
		switch (textColor) {
			case ColorText.BLUE:
				newColor = Color.blue;
				break;
			case ColorText.RED:
				newColor = Color.red;
				break;
			case ColorText.GREEN:
				newColor = Color.green;
				break;
			case ColorText.PURPLE:
				newColor = new Color( 1.0f, 0.0f, 1.0f );
				break;
		}

		return newColor;
	}

	public void ToggleNextColor() {
		ColorText nextColor;
		int colorIdx = currentColor.GetHashCode();
		if(++colorIdx > ColorText.PURPLE.GetHashCode()) {
			colorIdx = 0;
		}

		nextColor = (ColorText)colorIdx;
		Color c = EnumToColor( nextColor );

		playerColorText.color = c;

		Hashtable playerColorHash = new Hashtable();
		playerColorHash.Add( "Color", new Vector3( c.r, c.g, c.b ) );
		Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties( playerColorHash );

		playerColorText.text = nextColor.ToString();
		currentColor = nextColor;
	}

	public void TogglePrevColor() {
		ColorText prevColor;
		int colorIdx = currentColor.GetHashCode();
		if(--colorIdx < 0) {
			colorIdx = ColorText.PURPLE.GetHashCode();
		}

		prevColor = (ColorText)colorIdx;
		Color c = EnumToColor( prevColor );

		playerColorText.color = c;

		Hashtable playerColorHash = new Hashtable();
		playerColorHash.Add( "Color", new Vector3( c.r, c.g, c.b ) );
		Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties( playerColorHash );

		playerColorText.text = prevColor.ToString();
		currentColor = prevColor;
	}

	public void SetNickname( string nickname ) {
		Photon.Pun.PhotonNetwork.LocalPlayer.NickName = nickname;
	}

	public void CreateGame() {
		MainMenuController.instance.GoToHoleSelect();
	}

	public void JoinGame() {
		MainMenuController.instance.GoToJoinGame();
	}

	public void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
