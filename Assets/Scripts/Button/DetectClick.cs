using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DetectClick : MonoBehaviour,IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData){
		GameplayController.instance.ShowHintLetter ();
	}
}
