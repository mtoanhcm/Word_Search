using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharController : MonoBehaviour, IPointerClickHandler {

	[SerializeField] private Image _choosenRect;

	private bool _isChoose;
	private int _myPosX;
	private int _myPosY;

	// Use this for initialization
	void Awake () {
		_isChoose = false;
	}

	/// <summary>
	/// Create Choosen Rect When Click on Character
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick(PointerEventData eventData){
		if (!_isChoose) {
			Image _tempChoosenRect = Instantiate (_choosenRect, Vector3.zero, Quaternion.identity) as Image;
			_tempChoosenRect.transform.SetParent (transform, false);
			_tempChoosenRect.transform.position = transform.position;

			//Re-position to better show
			Vector3 _tempPos = _tempChoosenRect.transform.localPosition;
			_tempPos.y += 2;
			_tempChoosenRect.transform.localPosition = _tempPos;
		} else {
			if (transform.childCount > 0) {
				GameObject _tempChoosenRect = transform.GetChild (0).gameObject;
				Destroy (_tempChoosenRect);
			}
		}

		_isChoose = !_isChoose;
	}

	public void SetMyPosInBoard(int _posX, int _posY){
		_myPosX = _posX;
		_myPosY = _posY;
	}

	public int[] GetMyPosInBoard(){
		int[] _myPos = new int[2]{ _myPosX, _myPosY };
		return _myPos;
	}

	public void SetIsChooose(bool _value){
		_isChoose = _value;
	}
}
