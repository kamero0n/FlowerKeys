using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Typer : MonoBehaviour
{
    // [SerializeField] Flower flower;
    [SerializeField] private CursorController cursor;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameManager gameManager;

    private Tile currentTile;
    private Tile lastTile;

    public WordBank wordBank = null;
    public TextMeshProUGUI wordOutput = null;

    private string remainingWord = string.Empty;
    private string typedSoFar = string.Empty;
    private string currentWord = string.Empty;

   // private int currentStage = 0;

    private void Start()
    {
       // SetCurrentWord();
    }

    private void SetCurrentWord()
    {
        currentWord = wordBank.GetWord();
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string newString)
    {
        remainingWord = newString;
        wordOutput.text = remainingWord;
    }

    private void Update()
    {
        Vector2 cursorPos = new Vector2(cursor.GetCurrentX(), cursor.GetCurrentY());
        currentTile = gridManager.GetTileAtPosition(cursorPos);

        if(currentTile != lastTile)
        {
            if (lastTile != null)
            {
                lastTile.SetTending(false);
                lastTile.SetFocused(false);
            }
            lastTile = currentTile;
            OnTileChanged();
        }

        // for now use space to activate tile
       /* if(Input.GetKeyDown(KeyCode.Space))
        {
            ActivateTile();
        }*/

        // check typing if there is active tile w word
        if(currentTile != null && currentTile.IsActive)
        {
            currentTile.SetTending(true);
            CheckInput();
        }
    }

    private void ActivateTile()
    {
        if (currentTile == null) return;
        if (currentTile.IsActive) return;

        string word = wordBank.GetWord();
        if (word == string.Empty) return;

        // currentTile.StartGrowing(word);
        remainingWord = word;
    }

    private void CheckInput()
    {
        if(Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if(keysPressed.Length == 1)
            {
                EnterLetter(keysPressed);
            }
        }
    }

    private void EnterLetter(string typedLetter)
    {
        if(IsCorrectLetter(typedLetter))
        {
            AudioManager.instance.PlayTyping();
            RemoveLetter();

            if(IsWordComplete())
            {
                /*currentStage++;
                if (currentStage <= 4)
                {
                    flower.SetStage(currentStage);
                }*//*
                // SetCurrentWord();

                // currentTile.AdvanceStage();
                bool stageAdvanced = currentTile.CompleteWord();

                if(currentTile.IsFullyGrown())
                {
                    // currentTile.HideWord();
                    currentTile.AdvanceStage();
                }
                else
                {
                    string nextWord = wordBank.GetWord();
                    currentTile.AssignNextWord(nextWord);
                    // remainingWord = nextWord;
                    ResetTypingState(nextWord);
                }

               // SetCurrentWord();*/

                if(currentTile.IsReadyToRelease)
                {
                    CollectFlower();
                }
                else
                {
                    AudioManager.instance.PlayCompleteWord();
                    currentTile.CompleteWord();

                    if (currentTile.IsReadyToRelease)
                    {
                        ResetTypingState("collect");
                    }
                    else
                    {
                        string nextWord = wordBank.GetWord();
                        currentTile.AssignNextWord(nextWord);
                        ResetTypingState(nextWord);
                    }
                }
            }
        }
        else
        {
            AudioManager.instance.PlayWrongKey();
        }
    }

    private void CollectFlower()
    {
        AudioManager.instance.PlayCollect();
        currentTile.PlayReleaseEffect();
        gameManager.OnFlowerCollected();
        currentTile.ResetTile();
        ResetTypingState(string.Empty);
    }

    private bool IsCorrectLetter(string letter)
    {

        return remainingWord.Length > 0 && remainingWord.IndexOf(letter) == 0;
    }

    private void RemoveLetter()
    {
        // string newString = remainingWord.Remove(0, 1);
        // SetRemainingWord(newString);
        typedSoFar += remainingWord[0];
        remainingWord = remainingWord.Remove(0, 1);
        currentTile.UpdateDisplay(typedSoFar, remainingWord);
        currentTile.SetTypedSoFar(typedSoFar);
        // SetRemainingWord(remainingWord);
    }

    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }

    private void ResetTypingState(string word)
    {
        remainingWord = word;
        typedSoFar = string.Empty;
        if (currentTile != null) currentTile.SetTypedSoFar(string.Empty);
    }

    private void OnTileChanged()
    {
        // if (lastTile != null) lastTile.SetFocused(false);

        ClearAllObscured();

        if(currentTile == null)
        {
            ResetTypingState(string.Empty);
            return;
        }

        Vector2 focusPos = new Vector2(cursor.GetCurrentX(), cursor.GetCurrentY());
        Vector2[] neighborDirs =
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1)
        };
        foreach (var dir in neighborDirs)
        {
            Tile neighbor = gridManager.GetTileAtPosition(focusPos + dir);
            if (neighbor != null && neighbor != currentTile && neighbor.FullWord != string.Empty) neighbor.SetObscured(false);
        }


        currentTile.SetFocused(true);

        if (currentTile.IsPending)
        {
            currentTile.StartGrowing();
            ResetTypingState(currentTile.FullWord);
            currentTile.UpdateDisplay("", currentTile.FullWord);
            return;
        }


        if(currentTile.IsActive && currentTile.FullWord != string.Empty)
        {
            ResetTypingState(currentTile.FullWord);
            currentTile.UpdateDisplay("", currentTile.FullWord);
        }
        else
        {
            ResetTypingState(string.Empty);
        }
    }

    private void ClearAllObscured()
    {
        if (lastTile == null) return;
        Vector2 lastPos = new Vector2(lastTile.transform.position.x, lastTile.transform.position.y);
        Vector2[] neighborDirs =
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right,
            new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1)
        };
        foreach(var dir in neighborDirs)
        {
            Tile neighbor = gridManager.GetTileAtPosition(lastPos + dir);
            if (neighbor != null) neighbor.SetObscured(true);
        }
    }
}
