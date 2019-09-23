using UnityEngine;
using System.Collections;

public class GolfBall : MonoBehaviour {

	public PlayerInfo owner { get; private set; }
	public SpriteRenderer ballSprite;

	public void SetOwner( PlayerInfo newOwner ) {
		owner = newOwner;
		ballSprite.color = newOwner.playerColor;
	}
}