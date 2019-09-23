using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnStartScreen : MonoBehaviour {

	public Text playerNameText;
	public void Show(string playerName ) {
		playerNameText.text = string.Format( "{0}'s Turn!", playerName );
	}
}
