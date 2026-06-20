using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private ParticleSystem _flowerBurst;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Flower _flower; // child
    [SerializeField] private TextMeshProUGUI _wordDisplay;
    [SerializeField] private int[] _wordsPerStage = { 2, 3, 4, 5};
    [SerializeField] private float wiltGrace = 20f;
    [SerializeField] private float wiltTime = 40f;
    [SerializeField] private float hoverAmplitude = 0.08f;
    [SerializeField] private float hoverSpeed = 1.5f;
   
    private GameManager _gameManager;

    private int _wordsCompletedInStage = 0;
    private string _typedSoFar = "";
    private bool _isTended = false;
    private bool _isDead = false;
    private bool _isFocused = false;
    private float _hoverPhase;
    private Coroutine _hover;
    private Coroutine _rainbow;

    private float _graceTimer = 0f;

    private Color _myColor;
    public bool IsReadyToRelease { get; private set; } = false;


    public bool IsDead => _isDead;
    public bool IsActive { get; private set; } = false;
    public bool IsPending { get; private set; } = false;
    public int CurrentStage { get; private set; } = 0;

    private string _currentWord = "";
    public string CurrentWord => _currentWord;
    private string _fullWord = "";
    public string FullWord => _fullWord;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    public void Init(bool isOffset)
    {
        _hoverPhase = Random.Range(0f, Mathf.PI * 2f);
        _myColor = isOffset ? _offsetColor : _baseColor;
        _flower.gameObject.SetActive(false);
        _wordDisplay.gameObject.SetActive(false);
        _renderer.color = _myColor; 

    }


    private void Update()
    {
        if (!IsActive || _isDead || IsReadyToRelease) return;

        if(_isTended)
        {
            _graceTimer = wiltGrace;
            _flower.StopWilting();
            return;
        }

        if(_graceTimer > 0f)
        {
            _graceTimer -= Time.deltaTime;
            return;
        }

        _flower.StartWilting(CurrentStage - 1, OnWiltStageComplete, KillFlower);
    }


    public bool CompleteWord()
    {
        if (CurrentStage >= _wordsPerStage.Length) return false;

        _wordsCompletedInStage++;

        if(_wordsCompletedInStage >= _wordsPerStage[CurrentStage])
        {
            _wordsCompletedInStage = 0;
            AdvanceStage();
            return true;
        }

        return false;
    }

    public void StartGrowing()
    {
        IsPending = false;
        StopAllCoroutines();

        IsActive = true;
        CurrentStage = 0;

        _flower.gameObject.SetActive(true);
        _flower.SetStage(0);
        _renderer.color = _myColor;
        // ShowWord(word);
        StartHover();
    }

    public void ShowWord(string word)
    {
       // _currentWord = word;
        _fullWord = word;
        _wordDisplay.gameObject.SetActive(true);
        _wordDisplay.text = word;
        StartHover();
    }

    public void UpdateDisplay(string typed, string remaining)
    {
        if(IsReadyToRelease) return;

        _wordDisplay.text = $"<b><color=#888888>{typed}</color>{remaining}</b>";
    }

    public void AssignNextWord(string word)
    {
        // ShowWord(word);
        _fullWord = word;
        _wordDisplay.gameObject.SetActive(true);
        UpdateDisplay("", word);
        StartHover();
    }

    public void HideWord()
    {
        _wordDisplay.gameObject.SetActive(false);
        StopHover();
    }

    public void AdvanceStage()
    {
        if(CurrentStage < 4)
        {
            CurrentStage++;
            _flower.SetStage(CurrentStage);
        }

        if(CurrentStage >= 4)
        {
            IsReadyToRelease = true;
            _fullWord = "collect";
            _typedSoFar = string.Empty;
            _wordDisplay.gameObject.SetActive(true);
            // StopHover();
            StartRainbow();
        }
    }

    private void KillFlower()
    {
        _isDead = true;
        IsActive = false;
        IsPending = false;

        StopRainbow();
        StopHover();

        _flower.gameObject.SetActive(false);
        _wordDisplay.gameObject.SetActive(false);

        _renderer.color = _myColor;

        _gameManager?.OnFlowerDied();
    }

    private void OnWiltStageComplete()
    {
        if(CurrentStage > 0)
        {
            CurrentStage--;
            _graceTimer = wiltGrace;
        }
    }

    public void SetObscured(bool obscured)
    {
        Color c = _wordDisplay.color;
        c.a = obscured ? 0.3f : 1f;
        _wordDisplay.color = c;
    }
    
    public void SetPending(string word)
    {
        IsPending = true;
        ShowWord(word);
        StartCoroutine(Pulse());
    }

    public void SetTending(bool tended)
    {
        _isTended = tended;
    }

    public void SetFocused(bool focused)
    {
        _isFocused = focused;
        if(!_isFocused)
        {
            _wordDisplay.fontStyle = TMPro.FontStyles.LowerCase;
            _wordDisplay.fontSize = 4.5f;
        }
        else
        {
            _wordDisplay.fontSize = 4.8f;
        }
        StartHover();
    }

    private void StartHover()
    {
        if(_hover != null)
        {
            StopCoroutine(_hover);
            _hover = null;
        }
        if(_wordDisplay.gameObject.activeInHierarchy)
        {
            _hover = StartCoroutine(Hover());
        }
    }

    private void StopHover()
    {
        if(_hover == null) return;
        StopCoroutine(_hover);

        _hover = null;
        RectTransform rt = _wordDisplay.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
    }

    private IEnumerator Hover()
    {
        RectTransform rt = _wordDisplay.GetComponent<RectTransform>();
        Vector2 basePos = rt.anchoredPosition;

        while(true)
        {
            float offset = Mathf.Sin(Time.time * hoverSpeed + _hoverPhase) * hoverAmplitude;
            rt.anchoredPosition = basePos + new Vector2(0f, offset);
            yield return null;
        }
    }

    private IEnumerator Pulse()
    {
        Color _base = _renderer.color;
        Color highlight = Color.Lerp(_base, Color.white, 0.4f);

        while (IsPending)
        {
            float t = (Mathf.Sin(Time.time * 3f) + 1f) / 2f; 
            _renderer.color = Color.Lerp(_base, highlight, t);
            yield return null;
        }
    }
        
    private IEnumerator RainbowWord()
    {
        while(true)
        {
            string colored = "";

            for(int i = 0; i < _typedSoFar.Length; i++)
            {
                colored += $"<color=#888888>{_fullWord[i]}</color>";
            }

            for (int i = _typedSoFar.Length; i < _fullWord.Length; i++)
            {
                float hue = (Time.time * 0.4f + i * 0.12f) % 1f;
                Color c = Color.HSVToRGB(hue, 0.8f, 1f);
                string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(c);
                colored += $"<color=#{hex}>{_fullWord[i]}</color>";
            }
            _wordDisplay.text = colored;
            yield return null;
        }
    }

    private void StartRainbow()
    {
        if (_rainbow != null) return;
        _rainbow = StartCoroutine(RainbowWord());
    }

    private void StopRainbow()
    {
        if (_rainbow == null) return;
        StopCoroutine(_rainbow);
        _rainbow = null;
    }
    
    public void SetTypedSoFar(string typed)
    {
        _typedSoFar = typed;
    }

    public void ResetTile()
    {
        StopAllCoroutines();
        StopRainbow();

        IsActive = false;
        IsPending = false;
        IsReadyToRelease = false;
        CurrentStage = 0;
        _wordsCompletedInStage = 0;
        _isDead = false;
        _isTended = false;
        _graceTimer = 0f;
        _fullWord = string.Empty;
        _currentWord = string.Empty;

        _flower.gameObject.SetActive(false);
        _wordDisplay.gameObject.SetActive(false);
        _renderer.color = _myColor;
    }

    public void PlayReleaseEffect()
    {
        _flowerBurst.Play();
    }


    public bool IsFullyGrown() => CurrentStage >= 4;

    public int WordsRemainingInStage() => _wordsPerStage[CurrentStage] - _wordsCompletedInStage;
    
}
