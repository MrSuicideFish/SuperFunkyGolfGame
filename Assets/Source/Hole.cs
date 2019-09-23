using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
	private void OnTriggerEnter2D( Collider2D collision ) {
		if(collision.gameObject.tag == "GolfBall") {
			GolfBall ball = collision.gameObject.GetComponent<GolfBall>();
			CoreGameController.Instance.PlayerFinish( ball );
		}
	}
}
