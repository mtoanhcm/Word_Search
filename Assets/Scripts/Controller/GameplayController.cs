using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Helpers;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;

	//Use for Word Board
	[SerializeField] private Transform _wordBoard;
	[SerializeField] private Transform _wordListBoard;
	[SerializeField] private Transform _wordOnListBoard;
	[SerializeField] private Text _textRemaining;
	[SerializeField] private Text _textTimer;
	[SerializeField] private RectTransform _bearHint;
	[SerializeField] private TextAsset _wordDic;
	[SerializeField] private TextAsset _wordDefinition;
	[SerializeField] private Text[] _textAnswers;
	[SerializeField] private Transform[] _penguinList;
	[SerializeField] private Transform _definitionBoard;
	[SerializeField] private Transform _wordPannel;
	[SerializeField] private Transform _definitionPannel;
	[SerializeField] private GameObject _resultPannel;
	[SerializeField] private Text _textScore;
	[SerializeField] private TextMesh _textScorePlus;
	[SerializeField] private AudioClip _winClip;
	[SerializeField] private AudioClip _wrongClip;
	[SerializeField] private AudioClip _rightClip;
	[SerializeField] private Text _textBoardDefinitionTitle;
	[SerializeField] private Text _textTypeOfWord;
	[SerializeField] private Button _buttonSubmit;
	public AudioSource audioSource;

	private DictionaryHelper _dictionary;
	private List<Text> _listWordsPlayable;
	private List<Text> _listWordDefine;
	private List<Text> _listFirstChar;
	private List<string> _listStringPlayable;
	private Text[,] _wordOnBoard;
	private List<Vector3> _penguinPos;
	private Dictionary<string,string> _mainDictionary;
	private Text _wordChoose;
	private int _numWords;
	private int _wordPosX;
	private int _wordPosY;
	private string _answers;
	private bool _escapeCoroutine;
	private bool _isEndTime;
	private int _score;

	void MakeInstance(){
		if (instance == null) {
			instance = this;
		}
	}

	// Use this for initialization
	void Awake(){
		MakeInstance ();
		_dictionary = new DictionaryHelper ("words");
		_mainDictionary = new Dictionary<string, string> ();
		_listWordsPlayable = new List<Text> ();
		_listWordDefine = new List<Text> ();
		_listFirstChar = new List<Text> ();
		_listStringPlayable = new List<string> ();
		_numWords = Random.Range (15, 21);
		//_numWords = 3;
		_score = 0;
	}

	void Start () {
		CreateDictionary ();
		_wordOnBoard = (Text[,])_wordBoard.parent.GetComponent<WordBoardGenerate> ().GetWordOnBoard ().Clone ();
		CreateListWords (_numWords);
		ShowListWordOnBoard ();
		NumWordRmainning ();
		PlaceWordInListOnBoard ();
		CreateRandomWordOnBoard ();

		StartCoroutine (CountdownClock (15, 0));
	}

	void CreateDictionary(){
		List<string> _tempWordDic = new List<string> (_wordDic.text.Split('\n'));
		List<string> _tempWordDefi = new List<string> (_wordDefinition.text.Split ('\n'));

		for (int i = 0; i < _tempWordDic.Count; i++) {
			_tempWordDic [i] = _tempWordDic [i].Trim ();
		}

		for (int i = 0; i < _tempWordDic.Count; i++) {
			string _tempDic = _tempWordDic [i];
			string _tempDicDefi = _tempWordDefi [i];
			_mainDictionary.Add (_tempDic, _tempDicDefi);

		}

		//Debug.Log (_mainDictionary [a]);

	}

	#region wordboard area

	/// <summary>
	/// Creates the list words.
	/// </summary>
	/// <param name="_numOfWord">Number of word.</param>
	void CreateListWords(int _numOfWord){

		int _tempNumOfWord = _numOfWord;
		float _numLongWord = Mathf.Round(_numOfWord * 0.1f);
		float _numMediumWord = Mathf.Round(_numWords * 0.03f);
		float _numShortWord = _tempNumOfWord - _numLongWord - _numMediumWord;

		while (_tempNumOfWord > 0) {
			string _randomWord = "";
			while (_randomWord == "") {
				_randomWord = _dictionary.RandomWord ();
				if(_listStringPlayable.Contains(_randomWord)){
					_randomWord = "";	
				}
			}

			if (_randomWord.Length > 6 && _numLongWord > 0) {//long word
				_listStringPlayable.Add (_randomWord);
				_numLongWord--;
				_tempNumOfWord--;
			} else if (_randomWord.Length >= 4 && _randomWord.Length < 6 && _numMediumWord > 0) {//medium word
				_listStringPlayable.Add (_randomWord);
				_numMediumWord--;
				_tempNumOfWord--;
			} else if (_randomWord.Length > 0 && _randomWord.Length < 4 && _numShortWord > 0) {
				_listStringPlayable.Add (_randomWord);
				_tempNumOfWord--;
				_numShortWord--;
			}
		}
	}

	void PlaceWordInListOnBoard(){
		foreach (string _word in _listStringPlayable) {
			int _typeOfWord = Random.Range (0, 3);
			if (_typeOfWord == 0) {
				PlaceVerticalWord (_word);
			} else if (_typeOfWord == 1) {
				PlaceHorizontalWord (_word);
			} else {
				PlaceX (_word);
			}
		}
	}

	/// <summary>
	/// Places the vertical word.
	/// </summary>
	/// <param name="_word">Word.</param>
	void PlaceHorizontalWord(string _word){
		char[] _chars = _word.ToCharArray ();
		int _charAtX = 0;
		int _charAtY = 0;
		int _escape = 100;

		while (_charAtX == 0 && _charAtY == 0) {
			//Generat word on board
			int _tempRandomX = Random.Range (0, 14);
			int _tempRandomY = Random.Range (0, 14);

			if (_word.Length + _tempRandomY < 14) {
				bool _isNotEmpty = true;
				for (int i = 0; i < _word.Length; i++) {
					var _temp= _wordOnBoard [_tempRandomX, _tempRandomY + i].GetComponent<Text> ();
					if (_temp.text != "") {
						_isNotEmpty = false;
						break;
					}
				}

				if (_isNotEmpty) {
					_charAtX = _tempRandomX;
					_charAtY = _tempRandomY;
				}
			}

			_escape--;
			if (_escape <= 0) {
				_escape = 100;
				PlaceVerticalWord (_word);
				return;
			}
		}

		_listFirstChar.Add (_wordOnBoard [_charAtX, _charAtY]);

		for (int i = 0; i < _chars.Length; i++) {

			var _letterPos = _wordOnBoard [_charAtX, _charAtY + i];
			var _textAtLetterPos = _letterPos.GetComponent<Text> ();

			_textAtLetterPos.text = _chars [i].ToString ();
			//_textAtLetterPos.color = Color.magenta;
		}
	}

	void PlaceVerticalWord(string _word){
		char[] _chars = _word.ToCharArray ();
		int _charAtX = 0;
		int _charAtY = 0;
		int _escape = 100;

		while (_charAtX == 0 && _charAtY == 0) {
			//Generat word on board
			int _tempRandomX = Random.Range (0, 14);
			int _tempRandomY = Random.Range (0, 14);

			if (_word.Length + _tempRandomX < 14) {
				bool _isNotEmpty = true;
				for (int i = 0; i < _word.Length; i++) {
					var _temp= _wordOnBoard [_tempRandomX + i, _tempRandomY].GetComponent<Text> ();
					if (_temp.text != "") {
						_isNotEmpty = false;
						break;
					}
				}

				if (_isNotEmpty) {
					_charAtX = _tempRandomX;
					_charAtY = _tempRandomY;
				}
			}

			_escape--;
			if (_escape <= 0) {
				_escape = 100;
				PlaceVerticalWord (_word);
				return;
			}
		}

		_listFirstChar.Add (_wordOnBoard [_charAtX, _charAtY]);

		for (int i = 0; i < _chars.Length; i++) {
			var _letterPos = _wordOnBoard [_charAtX + i, _charAtY];
			var _textAtLetterPos = _letterPos.GetComponent<Text> ();

			_textAtLetterPos.text = _chars [i].ToString ();
			//_textAtLetterPos.color = Color.magenta;
		}
	}

	void PlaceX(string _word){
		char[] _chars = _word.ToCharArray ();
		int _charAtX = 0;
		int _charAtY = 0;
		int _escape = 100;

		while (_charAtX == 0 && _charAtY == 0) {
			//Generat word on board
			int _tempRandomX = Random.Range (0, 14);
			int _tempRandomY = Random.Range (0, 14);

			if (_word.Length + _tempRandomX < 14 && _word.Length + _tempRandomY < 14) {
				bool _isNotEmpty = true;
				for (int i = 0; i < _word.Length; i++) {
					var _temp= _wordOnBoard [_tempRandomX + i, _tempRandomY+i].GetComponent<Text> ();
					if (_temp.text != "") {
						_isNotEmpty = false;
						break;
					}
				}

				if (_isNotEmpty) {
					_charAtX = _tempRandomX;
					_charAtY = _tempRandomY;
				}
			}

			_escape--;
			if (_escape <= 0) {
				_escape = 100;
				PlaceVerticalWord (_word);
				return;
			}
		}

		_listFirstChar.Add (_wordOnBoard [_charAtX, _charAtY]);

		for (int i = 0; i < _chars.Length; i++) {
			var _letterPos = _wordOnBoard [_charAtX + i, _charAtY+i];
			var _textAtLetterPos = _letterPos.GetComponent<Text> ();

			_textAtLetterPos.text = _chars [i].ToString ();
			//_textAtLetterPos.color = Color.magenta;
		}
	}

	/// <summary>
	/// Numbers the word rmainning.
	/// </summary>
	void NumWordRmainning(){
		int _tempNum = _listWordsPlayable.Count;
		_textRemaining.text = "" + _tempNum;
	}

	/// <summary>
	/// Shows the list word on board.
	/// </summary>
	void ShowListWordOnBoard(){
		foreach (string _word in _listStringPlayable) {
			var _tempText = Instantiate (_wordOnListBoard, Vector3.zero, Quaternion.identity) as Transform;
			var _tempTextComp = _tempText.GetComponent<Text> ();
			_tempText.SetParent (_wordListBoard, false);
			_tempTextComp.text = _word;
			_listWordsPlayable.Add (_tempTextComp);
			_listWordDefine.Add (_tempTextComp);

			_tempTextComp.raycastTarget = false;
		}
	}

	/// <summary>
	/// Creates the random word on board.
	/// </summary>
	void CreateRandomWordOnBoard(){
		foreach (Text _char in _wordOnBoard) {
			if (_char.text == "") {
				_char.text = RandomLetter ();
			}
		}
	}

	/// <summary>
	/// Randoms the letter.
	/// </summary>
	/// <returns>The letter.</returns>
	string RandomLetter(){
		int _num = Random.Range (0, 26);
		char _let = (char)('a' + _num);

		return _let.ToString ();
	}

	/// <summary>
	/// Clicks the check word choosen.
	/// </summary>
	public void ClickCheckWordChoosen(){

		List<Text> _charNeedChecking = new List<Text> ();
		Text _wordNeedRemove = null;
		string _wordNeedToCompare = "";
		string _wordInput = "";
		bool _isCheckComplete = false;
		bool _isChoosenByHint = false;

		for (int i = 0; i < 14; i++) {
			for (int j = 0; j < 14; j++) {
				Text _char = _wordOnBoard [i, j];
				if (_char.transform.childCount > 0) {
					_charNeedChecking.Add (_char);
					_wordInput += _char.text;
				}
			}
		}
			
		List<Text> _wordNeeded = new List<Text> ();
		var _firstChar = _charNeedChecking [0];
		var _firstCharScript = _firstChar.GetComponent<CharController> ();
		var _firstCharPos = _firstCharScript.GetMyPosInBoard ();

		for (int i = 0; i < _charNeedChecking.Count; i++) {
			var _nchar = _charNeedChecking [i];
			var _ncharScript = _nchar.GetComponent<CharController> ();
			var _ncharPos = _ncharScript.GetMyPosInBoard ();

			//Check if word stay at next
			if (_firstCharPos [0] == _ncharPos [0] && _firstCharPos [1] + i == _ncharPos [1]) {
				_wordNeeded.Add (_nchar);
			} else if (_firstCharPos [1] == _ncharPos [1] && _firstCharPos [0] + i == _ncharPos [0]) {
				_wordNeeded.Add (_nchar);
			} else if (_firstCharPos [0] + i == _ncharPos [0] && _firstCharPos [1] + i == _ncharPos [1]) {
				_wordNeeded.Add (_nchar);
			}

		}

		foreach (Text _text in _wordNeeded) {
			_wordNeedToCompare += _text.text;
		}
			
		foreach (Text _word in _listWordsPlayable) {
			if (_word.text == _wordNeedToCompare) {
				_word.color = Color.green;
				_wordNeedToCompare = "";
				_wordNeedRemove = _word;
				_isCheckComplete = true;
			}
		}

		_listWordsPlayable.Remove (_wordNeedRemove);
		_textRemaining.text = "" + _listWordsPlayable.Count;

		if (_isCheckComplete) {
			audioSource.PlayOneShot (_rightClip);
			_isCheckComplete = false;
			foreach (Text _char in _wordNeeded) {

				if (_char.color == Color.red) {
					_isChoosenByHint = true;
				}

				_char.color = Color.blue;
				_char.raycastTarget = false;
				Destroy (_char.transform.GetChild (0).gameObject);

				if (_charNeedChecking.Contains (_char)) {
					_charNeedChecking.Remove (_char);
				}
			}

			if (!_isChoosenByHint && !_isEndTime) {
				_score++;
			}
		} else {
			audioSource.PlayOneShot (_wrongClip);
			foreach (Text _char in _charNeedChecking) {
				var _child = _char.transform.GetChild (0);
				_char.transform.GetComponent<CharController> ().SetIsChooose (false);
				Destroy (_child.gameObject);
			}
				
				
			StartCoroutine (TimeToShowHint (60));
		}

		/////Complete finding all word
		if (_listWordsPlayable.Count <= 0) {
			ChangeSector ();
		}
	}

	void ChangeSector(){
		_buttonSubmit.gameObject.SetActive (false);
		_escapeCoroutine = true;
		StartCoroutine (AutoChangeWord (0.5f,true));
		_wordPannel.DOMoveY (13f,2f);
		_definitionPannel.DOMoveY (0f,2f).OnComplete(()=>RunWhenCompleteMovePannel());

		foreach (Text _word in _listWordDefine) {
			_word.color = Color.white;
		}
	}

	/// <summary>
	/// Runs the when complete move pannel.
	/// </summary>
	void RunWhenCompleteMovePannel(){
		GetPenguinPos ();
		_wordPannel.gameObject.SetActive (false);
		_textRemaining.text = "" + _listWordDefine.Count;
		_escapeCoroutine = false;

		foreach (Text _word in _listWordDefine) {
			_word.raycastTarget = true;
		}
		_isEndTime = false;
		//StartCoroutine (CountdownClock (15,0));
	}

	/// <summary>
	/// Countdowns the clock.
	/// </summary>
	/// <returns>The clock.</returns>
	/// <param name="_minute">Minute.</param>
	/// <param name="_second">Second.</param>
	IEnumerator CountdownClock(int _minute, int _second){
		if (_escapeCoroutine) {
			yield break;
		}
		yield return new WaitForSeconds (1f);
		if (_second <= 0) {
			if (_minute <= 0) {
				_minute = 0;
				_second = 0;
				yield break;
			} else {
				_minute--;
				_second = 60;
			}
		}

		_second--;
		_textTimer.text = _minute + ":" + _second;

		if (_minute <= 0 && _second <= 0) {
			if (!_wordPannel.gameObject.activeSelf) {//Check if word pannel is active or not

				FillRedAllWordHaveNotDefine ();
				MoveWrongPenguins ("all");
				_listWordDefine.Clear ();
				CheckResult ();

				foreach (Transform _penguin in _penguinList) {
					_penguin.gameObject.SetActive (false);
				}
			}

			_isEndTime = true;
			yield break;
		}

		StartCoroutine (CountdownClock (_minute, _second));
	}

	public void ShowHintLetter(){
		if (_listFirstChar.Count > 0) {
			int _random = Random.Range (0, _listFirstChar.Count);
			Text _firstChar = _listFirstChar [_random];

			if (_firstChar.color != Color.blue) {
				_firstChar.color = Color.red;
			} else {
				ShowHintLetter ();
			}

			_listFirstChar.Remove (_firstChar);
		}
	}
		
	IEnumerator TimeToShowHint(int _second){
		yield return new WaitForSeconds (1f);
		_second--;
		if (_second <= 0) {
			_bearHint.DOLocalMoveX (-417, 1f, true);
			yield break;
		}

		StartCoroutine (TimeToShowHint (_second));
	}

	#endregion

	#region word definition area

	/// <summary>
	/// Checks the word was answered.
	/// </summary>
	/// <returns><c>true</c>, if word was answered was checked, <c>false</c> otherwise.</returns>
	/// <param name="_word">Word.</param>
	bool CheckWordWasAnswered(Text _word){
		if (_word.color != Color.white) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Sets the word need to dedinition.
	/// Use when click on word in board
	/// </summary>
	/// <param name="_word">Word.</param>
	public void SetWordNeedToDedinition(Text _word){
		string _tempWord = _word.text;
		string _rightAnswer = "";
		string _wordType = "";
		List<string> _allAnswer = new List<string>();
		_wordChoose = _word;

		if (_mainDictionary.ContainsKey (_tempWord)) {
			var _desContent = JsonHelper.WordInfoCreateFromJson (_mainDictionary [_tempWord]);
			int _random = 0;

			if (_desContent.verb != null) {
				_random = Random.Range (0, _desContent.verb.Length);
				_rightAnswer = _desContent.verb [_random];
				_wordType = "verb";
			} else if (_desContent.noun != null) {
				_random = Random.Range (0, _desContent.noun.Length);
				_rightAnswer = _desContent.noun [_random];
				_wordType = "noun";
			} else if (_desContent.adjective != null) {
				_random = Random.Range (0, _desContent.adjective.Length);
				_rightAnswer = _desContent.adjective [_random];
				_wordType = "adjective";
			} else if (_desContent.adverb != null) {
				_random = Random.Range (0, _desContent.adverb.Length);
				_rightAnswer = _desContent.adverb [_random];
				_wordType = "adverb";
			} else if (_desContent.pronoun != null) {
				_random = Random.Range (0, _desContent.pronoun.Length);
				_rightAnswer = _desContent.pronoun [_random];
				_wordType = "pronoun";
			} else if (_desContent.preposition != null) {
				_random = Random.Range (0, _desContent.preposition.Length);
				_rightAnswer = _desContent.preposition [_random];
				_wordType = "preposition";
			} else if (_desContent.exclamation != null) {
				_random = Random.Range (0, _desContent.exclamation.Length);
				_rightAnswer = _desContent.exclamation [_random];
				_wordType = "exclamation";
			} else if (_desContent.possessive_determiner != null) {
				_random = Random.Range (0, _desContent.possessive_determiner.Length);
				_rightAnswer = _desContent.possessive_determiner [_random];
				_wordType = "possessive_determiner";
			} else if (_desContent.plural_noun != null) {
				_random = Random.Range (0, _desContent.plural_noun.Length);
				_rightAnswer = _desContent.plural_noun [_random];
				_wordType = "plural_noun";
			} else if (_desContent.proper_noun != null) {
				_random = Random.Range (0, _desContent.proper_noun.Length);
				_rightAnswer = _desContent.proper_noun [_random];
				_wordType = "proper_noun";
			} else if (_desContent.ordinal_number != null) {
				_random = Random.Range (0, _desContent.ordinal_number.Length);
				_rightAnswer = _desContent.ordinal_number [_random];
				_wordType = "ordinal_number";
			} else if (_desContent.cardinal_number != null) {
				_random = Random.Range (0, _desContent.cardinal_number.Length);
				_rightAnswer = _desContent.cardinal_number [_random];
				_wordType = "cardinal_number";
			} else if (_desContent.phrase != null) {
				_random = Random.Range (0, _desContent.phrase.Length);
				_rightAnswer = _desContent.phrase [_random];
				_wordType = "phrase";
			} else if (_desContent.determiner != null) {
				_random = Random.Range (0, _desContent.determiner.Length);
				_rightAnswer = _desContent.determiner [_random];
				_wordType = "determiner";
			}
		}

		_textBoardDefinitionTitle.text = _tempWord;
		_textTypeOfWord.text = "" + _wordType;
		_allAnswer.AddRange(CreateAnswersList (_wordType,_rightAnswer));
		_allAnswer.Add (_rightAnswer);
		_answers = _rightAnswer;

		SuffleWord (_allAnswer);

		for (int i = 0; i < _allAnswer.Count; i++) {
			if (_allAnswer [i] == "") {
				_allAnswer [i] = "Undefine.";
			}

			_textAnswers [i].text = _allAnswer [i];
		}

		if (CheckWordWasAnswered (_word)) {
			ToggleChecker (true);
			MoveWrongPenguins ("all");
		} else {
			ToggleChecker (false);
			if (_penguinPos != null) {
				ResetPenguinPos ();
			}
		}
	}

	/// <summary>
	/// Creates the answers list.
	/// </summary>
	/// <returns>The answers list.</returns>
	/// <param name="_typeOfWord">Type of word.</param>
	List<string> CreateAnswersList(string _typeOfWord,string _rightAnswer){
		List<string> _answerList = new List<string> ();
		int _numAnswers = 3;
		int _escape = 50;

		while (_numAnswers > 0) {
			string _desOfWord = _dictionary.RandomWord ();
			//Debug.Log (_desOfWord);
			if (_desOfWord != _rightAnswer) {
				var _desContent = JsonHelper.WordInfoCreateFromJson (_mainDictionary [_desOfWord]);

				if (_typeOfWord == "verb") {
					if (_desContent.verb != null) {
						int _random = Random.Range (0, _desContent.verb.Length);
						string _des = _desContent.verb [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "noun") {
					if (_desContent.noun != null) {
						int _random = Random.Range (0, _desContent.noun.Length);
						string _des = _desContent.noun [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "adjective") {
					if (_desContent.adjective != null) {
						int _random = Random.Range (0, _desContent.adjective.Length);
						string _des = _desContent.adjective [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "adverb") {
					if (_desContent.adverb != null) {
						int _random = Random.Range (0, _desContent.adverb.Length);
						string _des = _desContent.adverb [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "pronoun") {
					if (_desContent.pronoun != null) {
						int _random = Random.Range (0, _desContent.pronoun.Length);
						string _des = _desContent.pronoun [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "preposition") {
					if (_desContent.preposition != null) {
						int _random = Random.Range (0, _desContent.preposition.Length);
						string _des = _desContent.preposition [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "exclamation") {
					if (_desContent.exclamation != null) {
						int _random = Random.Range (0, _desContent.exclamation.Length);
						string _des = _desContent.exclamation [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "possessive_determiner") {
					if (_desContent.possessive_determiner != null) {
						int _random = Random.Range (0, _desContent.possessive_determiner.Length);
						string _des = _desContent.possessive_determiner [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "plural_noun") {
					if (_desContent.plural_noun != null) {
						int _random = Random.Range (0, _desContent.plural_noun.Length);
						string _des = _desContent.plural_noun [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "proper_noun") {
					if (_desContent.proper_noun != null) {
						int _random = Random.Range (0, _desContent.proper_noun.Length);
						string _des = _desContent.proper_noun [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "ordinal_number") {
					if (_desContent.ordinal_number != null) {
						int _random = Random.Range (0, _desContent.ordinal_number.Length);
						string _des = _desContent.ordinal_number [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "cardinal_number") {
					if (_desContent.cardinal_number != null) {
						int _random = Random.Range (0, _desContent.cardinal_number.Length);
						string _des = _desContent.cardinal_number [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "phrase") {
					if (_desContent.phrase != null) {
						int _random = Random.Range (0, _desContent.phrase.Length);
						string _des = _desContent.phrase [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				} else if (_typeOfWord == "determiner") {
					if (_desContent.determiner != null) {
						int _random = Random.Range (0, _desContent.determiner.Length);
						string _des = _desContent.determiner [_random];
						_answerList.Add (_des);
						_numAnswers--;
					}
				}

				_escape--;
				if (_escape < 0) {
					_typeOfWord = "noun";
				}
			}
		}
			
		return _answerList;
	}

	void SuffleWord(List<string> _deck){
		for (int i = _deck.Count - 1; i > 0; i--) {
			int _random = Random.Range(0,i+1);
			var _temp = _deck[i];
			_deck[i] = _deck[_random];
			_deck[_random] = _temp;
		}
	}

	public void ClickChooseAnswer(string _myAnswer){
		for (int i = 1; i < _definitionBoard.childCount; i++) {
			var _posOfAnswer = _definitionBoard.GetChild (i).GetChild (0).name;

			//Choose right letter answer (A || B || C || D)
			if (_posOfAnswer == _myAnswer) {
				var _answerContent = _definitionBoard.GetChild (i).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text;

				if (_answerContent == _answers) {
					ChooseRightAnswer (_myAnswer);
				} else {
					ChooseWrongAnswer ();
				}
			}
		}

		CheckResult ();
	}

	void ChooseRightAnswer(string _myAnswer){
		if (_wordChoose.color != Color.red) {
			var _tempText = Instantiate(_textScorePlus,new Vector3(1.20f,-4f,0),Quaternion.identity) as TextMesh;
			_tempText.GetComponent<MeshRenderer> ().sortingOrder = 5;
			_tempText.transform.DOMoveY (-1.40f, 0.5f);
			Destroy (_tempText, 0.5f);

			if (!_isEndTime) {
				_score++;
			}

			audioSource.PlayOneShot (_rightClip);
		}
		
		_wordChoose.color = Color.green;
		_listWordDefine.RemoveAll (x => x.text == _wordChoose.text);
		_textRemaining.text = "" + _listWordDefine.Count;

		MoveWrongPenguins (_myAnswer);
		ToggleChecker (true);
		StartCoroutine (AutoChangeWord (2f,false));
	}

	void ChooseWrongAnswer(){
		_listWordDefine.RemoveAll (x => x.text == _wordChoose.text);
		_textRemaining.text = "" + _listWordDefine.Count;
		audioSource.PlayOneShot (_wrongClip);
		_wordChoose.color = Color.red;
		MoveWrongPenguins ("all");
		ToggleChecker (true);
		StartCoroutine (AutoChangeWord (2f,false));
	}

	/// <summary>
	/// Moves the wrong penguin.
	/// </summary>
	/// <param name="_rightPenguin">Right penguin.</param>
	void MoveWrongPenguins(string _rightPenguin){
		foreach (Transform _penguin in _penguinList) {
			_penguin.GetComponent<Button> ().interactable = false;
			if (_penguin.name != _rightPenguin) {
				float _posMove = _definitionBoard.transform.position.y;
				_penguin.DOMoveY (_posMove, 2f); 
			}
		}
	}

	void GetPenguinPos(){
		_penguinPos = new List<Vector3> ();
		foreach (Transform _penguin in _penguinList) {
			_penguinPos.Add (_penguin.position);
		}
	}

	void ResetPenguinPos(){
		for (int i = 0; i < _penguinList.Length; i++) {
			var _tempPenguin = _penguinList [i];
			var _tempPos = _penguinPos;
			_tempPenguin.DOMove (_tempPos[i], 1f);
			_tempPenguin.GetComponent<Button> ().interactable = true;
		}
	}

	/// <summary>
	/// Autos the change word.
	/// </summary>
	/// <returns>The change word.</returns>
	/// <param name="_time">Time.</param>
	/// <param name="_isFirstme">If set to <c>true</c> is firstme.</param>
	IEnumerator AutoChangeWord(float _time,bool _isFirstme){
		yield return new WaitForSeconds (_time);
		if (_listWordDefine.Count > 0) {
			int _random = Random.Range (0, _listWordDefine.Count);
			SetWordNeedToDedinition (_listWordDefine [_random]);
		}

		if (!_isFirstme) {
			ResetPenguinPos ();
		}
	}

	/// <summary>
	/// Toggles the checker.
	/// </summary>
	void ToggleChecker(bool _isCheck){
		List<Transform> _listAnswer = new List<Transform> ();
		for (int i = 0; i < 4; i++) {
			var _tempHeadText = _textAnswers [i].transform.parent.parent.parent.parent;

			_listAnswer.Add (_tempHeadText);
		}

		//Use _isCheck to disable all right or wrong check --> use with word haven't defined
		if (_isCheck) {
			foreach (Transform _ans in _listAnswer) {
				var _answerContent = _ans.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text> ().text;

				if (_answerContent == _answers) {
					_ans.GetChild (0).GetChild (0).gameObject.SetActive (true);
					_ans.GetChild (0).GetChild (1).gameObject.SetActive (false);
				} else {
					_ans.GetChild (0).GetChild (0).gameObject.SetActive (false);
					_ans.GetChild (0).GetChild (1).gameObject.SetActive (true);
				}
			}
		} else {
			foreach (Transform _ans in _listAnswer) {
				_ans.GetChild (0).GetChild (0).gameObject.SetActive (false);
				_ans.GetChild (0).GetChild (1).gameObject.SetActive (false);
			}
		}
	}

	void FillRedAllWordHaveNotDefine(){
		foreach (Text _word in _listWordDefine) {
			if (_word.color != Color.green) {
				_word.color = Color.red;
			}
		}
	}

	#endregion

	void CheckResult(){
		if (CheckWinCondition()) {
			_escapeCoroutine = true;
			_resultPannel.SetActive (true);
			_textScore.text = "" + _score;
			audioSource.PlayOneShot (_winClip);
		}
	}

	bool CheckWinCondition(){
		if (_listWordDefine.Count <= 0) {
			return true;
		} else {
			return false;
		}
	}

	public void ClickTutButton(){
		SystemController.instance.ClickTutorialButton ();
	}

	public void ClickFinishedButton(){
		
	}
		
}
