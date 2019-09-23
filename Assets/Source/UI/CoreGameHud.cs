using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGameHud : MonoBehaviour {

	public enum HudScreen {
		TurnStart,
		HoleStart,
		HoleEnd,
		MainHUD
	}

	public static CoreGameHud instance { get; private set; }

	public GameObject holeEndScreen;
	public GameObject holeStartScreen;
	public GameObject mainHUD;
	public TurnStartScreen turnStartScreen;

	private void Awake() {
		instance = this;
	}

	public T ShowScreen<T>(HudScreen scrn) {
		GameObject newScrn = ShowScreen( scrn );
		return newScrn.GetComponent<T>();
	}

	public GameObject ShowScreen(HudScreen scrn ) {

		turnStartScreen.gameObject.SetActive( false );
		holeStartScreen.gameObject.SetActive( false );
		holeEndScreen.gameObject.SetActive( false );
		mainHUD.gameObject.SetActive( false );

		GameObject newScrn = null;
		switch (scrn) {
			case HudScreen.TurnStart:
				newScrn = turnStartScreen.gameObject;
				break;
			case HudScreen.HoleStart:
				newScrn = holeStartScreen.gameObject;
				break;
			case HudScreen.HoleEnd:
				newScrn = holeEndScreen.gameObject;
				break;
			case HudScreen.MainHUD:
				newScrn = mainHUD.gameObject;
				break;
		}

		newScrn.SetActive( true );
		return newScrn;
	}
}