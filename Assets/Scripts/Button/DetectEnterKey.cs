using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DetectEnterKey : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			GameplayController.instance.ClickCheckWordChoosen ();
		}
	}
}
