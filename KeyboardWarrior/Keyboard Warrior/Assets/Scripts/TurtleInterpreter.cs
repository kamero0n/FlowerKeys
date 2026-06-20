using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleInterpreter : MonoBehaviour
{
    [Header("Segment Settings")]
    [SerializeField] private float segmentLength = 0.3f;
    [SerializeField] private float baseAngle = 25f;
    [SerializeField] private float angleVariance = 10f;
    [SerializeField] private float baseWidth = 0.08f;
    [SerializeField] private float tipWidth = 0.02f;

    [Header("Colors")]
    [SerializeField] private Color stemColorBase = new Color(0.2f, 0.6f, 0.2f);
    [SerializeField] private Color stemColorTip = new Color(0.4f, 0.6f, 0.2f);
    [SerializeField] private Color flowerColor = Color.blue;

    [Header("Prefabs")]
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject flowerheadPrefab;

    private struct TurtleState
    {
        public Vector2 position;
        public float angle;
        public float width;
        public int depth;
    }

    private List<LineRenderer> _segments = new List<LineRenderer>();
    private List<GameObject> _flowerHeads = new List<GameObject>();
    private List<(GameObject head, int segmentIndex)> _flowerHeadData = new List<(GameObject head, int segmentIndex)>();

    public List<LineRenderer> GetSegments() => _segments;
    public List<GameObject> GetFlowerHeads()
    {
        var heads = new List<GameObject>();
        foreach (var (head, _) in _flowerHeadData) heads.Add(head);
        return heads;
    }
    public List<(GameObject head, int segmentIndex)> GetFlowerHeadData() => _flowerHeadData;

    public void Interpret(string lsystem)
    {
        // clear
        foreach (var s in _segments) Destroy(s.gameObject);
        foreach (var f in _flowerHeads) Destroy(f);
        _segments.Clear();
        _flowerHeads.Clear();

        Stack<TurtleState> stateStack = new Stack<TurtleState>();

        TurtleState turtle = new TurtleState
        {
            position = (Vector2)transform.position,
            angle = 90f,
            width = baseWidth,
            depth = 0
        };

        foreach(char c in lsystem)
        {
            switch (c)
            {
                case 'F':
                    Vector2 direction = AngleToDirection(turtle.angle);
                    Vector2 newPos = turtle.position + direction * segmentLength;

                    DrawSegment(turtle.position, newPos, turtle.width, turtle.depth);

                    turtle.position = newPos;
                    turtle.width = Mathf.Max(turtle.width * 0.85f, tipWidth);
                    break;

                case '+':
                    turtle.angle += baseAngle + Random.Range(-angleVariance, angleVariance);
                    break;

                case '-':
                    turtle.angle -= baseAngle + Random.Range(-angleVariance, angleVariance);
                    break;

                case '[':
                    stateStack.Push(turtle);
                    turtle.depth++;
                    break;

                case ']':
                    SpawnFlowerHead(turtle.position);
                    turtle = stateStack.Pop();
                    break;
            }
        }


    }


    private void DrawSegment(Vector2 from, Vector2 to, float width, int depth)
    {
        if(branchPrefab == null)
        {
            Debug.LogError("branchPrefab is not assigned", this);
            return;
        }


        GameObject go = Instantiate(branchPrefab, transform);
        LineRenderer lr = go.GetComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        lr.startWidth = width;
        lr.endWidth = width * 0.7f;

        float t = (float)depth / 5f;
        lr.startColor = Color.Lerp(stemColorBase, stemColorTip, t);
        lr.endColor = Color.Lerp(stemColorBase, stemColorTip, t + 0.1f);

        lr.numCapVertices = 8;

        _segments.Add(lr);
    }


    private void SpawnFlowerHead(Vector2 pos)
    {
        if(flowerheadPrefab == null)
        {
            Debug.LogError("flowerhead is not assigned", this);
            return;
        }

        GameObject head = Instantiate(flowerheadPrefab, (Vector3)pos, Quaternion.identity, transform);
        head.GetComponent<SpriteRenderer>().color = RandomPastel();
        float scale = Random.Range(0.08f, 0.18f);
        head.transform.localScale = Vector3.one * scale;
        //_flowerHeads.Add(head);
        head.SetActive(false);

        _flowerHeadData.Add((head, _segments.Count));
    }


    private Vector2 AngleToDirection(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }


    private Color RandomPastel()
    {
        return Color.HSVToRGB(Random.value, 0.4f, 0.95f);
    }
}
