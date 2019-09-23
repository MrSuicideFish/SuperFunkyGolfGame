using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINametag : MonoBehaviour {

	public Text nameTagText;

	[HideInInspector]
	public RectTransform rectTransform;

	[HideInInspector]
	public GolfBall ball;

	[HideInInspector]
	public string ownerId;

	private void Awake() {
		rectTransform = this.GetComponent<RectTransform>();
	}

	public void SetBall(GolfBall newBall ) {
		ball = newBall;
		ownerId = newBall.owner.playerId;
	}

	public void SetName(string playerName) {
		nameTagText.text = playerName;
	}
}
