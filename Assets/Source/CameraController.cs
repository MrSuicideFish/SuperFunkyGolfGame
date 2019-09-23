using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float lerpPow;
	public GolfBall targetBall;

	private void Update() {
		if(targetBall != null) {
			Vector3 newPos = new Vector3( targetBall.transform.position.x, targetBall.transform.position.y, -10.0f );
			transform.position = Vector3.Lerp( transform.position, newPos, lerpPow );
		}
	}
}