using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName ="SFG/Create Hole Queue")]
public class HoleQueue : ScriptableObject {

	private int holeIdx;
	public HoleInfo[ ] holes;

	public static HoleQueue[] GetAllHoleQueues() {
		HoleQueue[ ] queues = Resources.LoadAll<HoleQueue>( "HoleQueues/" );
		return queues;
	}
}
