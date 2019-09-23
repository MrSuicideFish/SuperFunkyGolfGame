using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFloor : MonoBehaviour {

	public void OnTriggerEnter2D( Collider2D collision ) {
		if(collision.gameObject.tag == "GolfBall") {
			GolfBall ball = collision.gameObject.GetComponent<GolfBall>();
			CoreGameController.Instance.KillGolfBall( ball );
		}
	}
}