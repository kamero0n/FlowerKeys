using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    /*[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite[] stageSprites;*/
    [SerializeField] private float swayAmount = 3f;
    [SerializeField] private float swaySpeed = 1.2f;

    [SerializeField] private TurtleInterpreter turtle;
    [SerializeField] private int lsystems = 3;

    [SerializeField] private float segmentRevealSpeed = 0.15f;

    private List<LineRenderer> _allSegments;
    private List<(GameObject head, int segmentIndex)> _flowerHeadData;
    private int _revealedUpTo = 0;
    private int currentStage = 0;
    private float _swayOffset;


    private Coroutine _reveal;
    private Coroutine _wilt;

    private void Awake()
    {
        _swayOffset = Random.Range(0f, Mathf.PI * 2f);

        var rules = new Dictionary<char, string[]>
        {
            {'F', new[] { "F[+F]F[-F]F", "F[+F]F", "F[-F]F"} }
        };
        var probs = new Dictionary<char, float[]> {

            { 'F', new[]{0.33f, 0.33f, 0.34f} }
            
        };

        var generator = new LSystemGenerator("F", rules, probs);
        string lstring = generator.Generate(lsystems);

        turtle.Interpret(lstring);

        // hide segments
        _allSegments = turtle.GetSegments();
        _flowerHeadData = turtle.GetFlowerHeadData();

        foreach (var seg in _allSegments) seg.enabled = false;
        foreach (var head in turtle.GetFlowerHeads()) head.SetActive(false);

    }
    

    public void Update()
    {
        if(_revealedUpTo >= 0)
        {
            float sway = Mathf.Sin(Time.time * swaySpeed + _swayOffset) * swayAmount;
            transform.rotation = Quaternion.Euler(0f, 0f, sway);
        }
    }

    public void StartWilting(int targetStage, System.Action onStagedWilted, System.Action onFullyWilted = null)
    {
        if (_wilt != null) return;
        if (_reveal != null) StopCoroutine(_reveal);
        AudioManager.instance.PlayWither();
        _wilt = StartCoroutine(WiltSegments(targetStage, onStagedWilted, onFullyWilted));
    }

    public void StopWilting()
    {
        if(_wilt != null)
        {
            StopCoroutine(_wilt);
            _wilt = null;
        }
    }

    public void SetStage(int stage)
    {
        /*currentStage = stage;
        sprite.sprite = stageSprites[currentStage];*/

        int totalSegments = _allSegments.Count;
        int targetRevealed = Mathf.RoundToInt((stage / 4f) * totalSegments);

        if (_reveal != null) StopCoroutine(_reveal);
        _reveal = StartCoroutine(RevealSegments(targetRevealed));
    }

    private IEnumerator WiltSegments(int targetStage, System.Action onStageWilted, System.Action onFullyWilted)
    {
        int targetSegments = Mathf.RoundToInt((targetStage / 4f) *  _allSegments.Count);

        while (_revealedUpTo > 0 && _revealedUpTo > targetSegments)
        {
            _revealedUpTo--;
            _allSegments[_revealedUpTo].enabled = false;

            foreach(var (head, segIndex) in _flowerHeadData)
            {
                if(head.activeSelf && segIndex > _revealedUpTo)
                {
                    head.SetActive(false);
                }
            }

            yield return new WaitForSeconds(segmentRevealSpeed);
        }

        _wilt = null;
        
        if (_revealedUpTo == 0)
        {
            onFullyWilted?.Invoke();
        }
        else
        {
            onStageWilted?.Invoke();
        }
    }

    private IEnumerator RevealSegments(int targetCount)
    {
        while (_revealedUpTo < targetCount && _revealedUpTo < _allSegments.Count)
        {
            _allSegments[_revealedUpTo].enabled = true;
            _revealedUpTo++;

            foreach (var (head, segIndex) in _flowerHeadData)
            {
                if (!head.activeSelf && segIndex <= _revealedUpTo)
                {
                    head.SetActive(true);
                }
            }

            yield return new WaitForSeconds(segmentRevealSpeed);
        }
    }
}
