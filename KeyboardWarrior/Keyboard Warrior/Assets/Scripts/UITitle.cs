using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITitle : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;

    public float hoverSpeed = 1.0f;
    public float hoverAmplitude = 1.0f;

    private Vector2 _basePos;

    private void Start()
    {
        _basePos = titleText.GetComponent<RectTransform>().anchoredPosition;
    }
    
    void Update()
    {
        RectTransform rt = titleText.GetComponent<RectTransform>();

        float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        rt.anchoredPosition = _basePos + new Vector2(0f, offset);
    }
}
