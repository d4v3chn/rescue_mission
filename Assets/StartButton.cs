using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEditor.Tilemaps;


public class ClickExample : MonoBehaviour {
	public Button yourButton;

    private Button btn;
    public GameManager GM;  
    //public TextMeshProUGUI PlatText;

	void Start () {
		btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		GM.NewGame();
        btn.gameObject.SetActive(false);
	}
}