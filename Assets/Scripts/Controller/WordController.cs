using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordController : MonoBehaviour, IPointerClickHandler {

	private Text _myText;

	// Use this for initialization
	void Awake () {
		_myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	public void OnPointerClick(PointerEventData eventData){
		GameplayController.instance.SetWordNeedToDedinition (_myText);
	}
}
