using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController {

	private GolfBall _playerBall;

	private LineRenderer _strokeGauge;
	private LineRenderer strokeGauge {
		get {
			if (!_strokeGauge) {
				LineRenderer lineRenderRes = Resources.Load<LineRenderer>( "StrokeGauge" );
				_strokeGauge = LineRenderer.Instantiate( lineRenderRes );
				//_dragDebugLine.startColor = Color.green;
				//_dragDebugLine.endColor = Color.green;
				//_dragDebugLine.startWidth = 0.1f;
				//_dragDebugLine.endWidth = 0.1f;
				//_dragDebugLine.positionCount = 2;
			}
			return _strokeGauge;
		}
	}

	public PlayerController(GolfBall playerBall) {
		_playerBall = playerBall;
	}

	private Vector3 dragStartPos;
	private Vector3 dragPos;
	private Vector3 hitNormal;
	private float strength;
	private bool isDragging;
	public void TickInput() {

		if (Input.GetMouseButtonDown( 0 )) {
			dragStartPos = Input.mousePosition;
			dragPos = dragStartPos;
			isDragging = true;
		} else if (Input.GetMouseButtonUp( 0 )) {
			CoreGameController.Instance.TryHitBall( hitNormal, strength );
			strokeGauge.enabled = false;
			isDragging = false;
		} else if (Input.GetMouseButton( 0 )) {
			dragPos = Input.mousePosition;
			strokeGauge.enabled = true;
			isDragging = true;
		} else {
			isDragging = false;
		}

		if (isDragging) {
			hitNormal = (dragPos - dragStartPos).normalized;
			float rawDist = Vector3.Distance( dragStartPos, dragPos );
			float dist = rawDist * 0.01f;
			Vector3 worldStartPos = _playerBall.transform.position;

			Vector3 rawWorldNormalDir = hitNormal * dist;
			Vector3 worldNormalDir = Vector3.ClampMagnitude( rawWorldNormalDir, 1.0f );
			strength = Vector3.ClampMagnitude( rawWorldNormalDir, 3.0f ).magnitude / 3.0f;
			Vector3 worldEndPos = worldStartPos + Vector3.ClampMagnitude( rawWorldNormalDir, 3.0f );

			strokeGauge.SetPosition( 0, worldStartPos );
			strokeGauge.SetPosition( 1, worldEndPos );

			Color gaugeColor = Color.Lerp( Color.white, Color.red, strength );
			strokeGauge.startColor = gaugeColor;
			strokeGauge.endColor = gaugeColor;
		} else {
			strokeGauge.SetPosition( 0, Vector3.zero );
			strokeGauge.SetPosition( 1, Vector3.zero );
		}
	}
}