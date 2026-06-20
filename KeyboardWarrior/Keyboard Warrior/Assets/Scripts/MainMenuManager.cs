using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playText;
    [SerializeField] private TextMeshProUGUI _quitText;

    private int _selected = -1;
    private string _input = "";

    public float hoverSpeed = 2.0f;
    public float hoverAmplitude = 5.0f;

    private Vector2 _playBasePos;
    private Vector2 _quitBasePos;

    private void Start()
    {
        _playBasePos = _playText.GetComponent<RectTransform>().anchoredPosition;
        _quitBasePos = _quitText.GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selected = _selected == 0 ? 1 : 0;
            UpdateHighlight();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (_selected == 0) SceneManager.LoadScene("Main");
            else Application.Quit();
        }*/

        RectTransform rtPlay = _playText.GetComponent<RectTransform>();
        RectTransform rtQuit = _quitText.GetComponent<RectTransform>();
        float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        rtPlay.anchoredPosition = _playBasePos + new Vector2(0f, offset);
        rtQuit.anchoredPosition = _quitBasePos + new Vector2(0f, offset) ;

        if(Input.anyKeyDown)
        {
            
            string key = Input.inputString;
            if (key.Length != 1) return;

            string candidate = _input + key;


            if ("play".StartsWith(candidate))
            {
                _selected = 0;
                _input = candidate;
                AudioManager.instance.PlayTyping();
            }
            else if ("quit".StartsWith(candidate))
            {
                _selected = 1;
                _input = candidate;
                AudioManager.instance.PlayTyping();
            }
            else
            {
                _input = "";
                _selected = -1;
                AudioManager.instance.PlayWrongKey();

                if ("play".StartsWith(key)) { _input = key; _selected = 0; }
                else if("quit".StartsWith(key)) { _input = key; _selected = 1;  }
            }

            UpdateHighlight();

            if (_input == "quit")
            {
                AudioManager.instance.PlayCompleteWord();
                Application.Quit();
            }
            if (_input == "play")
            {
                AudioManager.instance.PlayCompleteWord();
                SceneManager.LoadScene("Main");
            }
        }
    }

    private void UpdateHighlight()
    {
        string playTyped = _selected == 0 ? _input : "";
        string quitTyped = _selected == 1 ? _input : "";

        _playText.text = $"<color=#888888>{playTyped}</color>{"play".Substring(playTyped.Length)}";
        _quitText.text = $"<color=#888888>{quitTyped}</color>{"quit".Substring(quitTyped.Length)}";
    }
}
