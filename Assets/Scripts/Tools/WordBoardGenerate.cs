using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WordBoardGenerate : MonoBehaviour {

	public int _numRow;
	public int _numColumn;

	public float _offSetX;
	public float _offSetY;

	[SerializeField] private Text _letterPrefab;
	[SerializeField] private Transform _letterBox;

	private Text[,] _board;
	private char _letter;

	// Use this for initialization
	void Awake () {
		_board = new Text[_numRow, _numColumn];
		CreateWordBoard ();
	}

	void Start(){
		
	}

	/// <summary>
	/// Creates the word board.
	/// </summary>
	void CreateWordBoard(){
		Vector3 _tempPos = _letterBox.position;
		for (int i = 0; i < _numRow; i++) {
			_tempPos.y = -i * _offSetY;
			for (int j = 0; j < _numColumn; j++) {
				_tempPos.x = j * _offSetX;
				Text _tempText = Instantiate (_letterPrefab, _tempPos, Quaternion.identity) as Text;
				//_tempText.text = RandomLetter ();
				var _tempTextScript = _tempText.GetComponent<CharController>();
				_tempTextScript.SetMyPosInBoard (i, j);
				_tempText.text = "";
				_tempText.transform.SetParent (_letterBox, false);
				_board [i, j] = _tempText;
			}
		}
	}

	/// <summary>
	/// Make Random Letter
	/// </summary>
	/// <returns>The letter.</returns>
	string RandomLetter(){
		int _num = Random.Range (0, 26);
		char _let = (char)('a' + _num);

		return _let.ToString ();
	}

	public Text[,] GetWordOnBoard(){
		return _board;
	}
}
