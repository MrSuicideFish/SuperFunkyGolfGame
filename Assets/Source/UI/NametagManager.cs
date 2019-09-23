using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NametagManager : MonoBehaviour {

	private static NametagManager _instance;
	public static NametagManager instance {
		get {
			if(_instance == null) {
				NametagManager nameTagCanvasRes = Resources.Load<NametagManager>("NametagCanvas");
				_instance = NametagManager.Instantiate( nameTagCanvasRes );
			}
			return _instance;
		}
	}

	public UINametag nametagPrefab;
	public RectTransform panelRectTransform;
	public Font labelFont;
	public float labelXOffset;
	public float labelYOffset;

	private void OnGUI() {

		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.fontStyle = FontStyle.Normal;
		style.font = labelFont;

		for (int i = 0; i < PlayerManager.instance.playerCount; i++) {
			PlayerInfo p = PlayerManager.instance.GetPlayerInfoByIdx( i );
			if(p != null) {
				GolfBall ball = p.playerGolfBall;
				if(ball != null) {

					style.normal.textColor = p.playerColor;

					Vector3 labelWorldPos = ball.transform.position + (Vector3.up * labelYOffset);
					Vector2 viewportPos = Camera.main.WorldToScreenPoint( labelWorldPos );
					viewportPos.y = Screen.height - viewportPos.y;

					int labelLength = p.playerName.Length;
					Rect rect = new Rect( viewportPos.x - ((labelLength / 2) * labelXOffset), viewportPos.y, 300, 50 );
					GUI.Label( rect, p.playerName, style );
				}
			}
		}
	}
}
