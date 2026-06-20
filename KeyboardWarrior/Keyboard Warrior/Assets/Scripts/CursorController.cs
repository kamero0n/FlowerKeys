using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 5;

    private int currentX = 0;
    private int currentY = 0;

    private void Start()
    {
        UpdatePosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(1, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(-1, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(0, 1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(0, -1);
    }

    private void Move(int dx, int dy)
    {
        currentX = Mathf.Clamp(currentX + dx, 0, gridWidth - 1);
        currentY = Mathf.Clamp(currentY + dy, 0, gridHeight - 1);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(currentX, currentY, 0);
    }

    public int GetCurrentX()
    {
        return currentX;
    }

    public int GetCurrentY() { return currentY; }
}
