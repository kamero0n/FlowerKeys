using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordBank : MonoBehaviour
{
    [SerializeField] private TextAsset _wordList;
    private List<string> originalWords = new List<string>();

    // old list
    /*private List<string> originalWords = new List<string>()
    {
        "Pankcakes", "are", "boom"
    };
*/
    private List<string> workingWords = new List<string>();

    private void Awake()
    {
        string[] lines = _wordList.text.Split('\n');
        originalWords = new List<string>(lines.Length);

        foreach(string line in lines)
        {
            string word = line.Trim();
            if(word.Length >= 3 && word.Length <= 8)
            {
                originalWords.Add(word);
            }
        }


        workingWords.AddRange(originalWords);
        Shuffle(workingWords);
        ConvertToLower(workingWords);
    }

    private void Shuffle(List<string> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            string temporary = list[i];

            list[i] = list[random];
            list[random] = temporary;
        }
    }

    private void ConvertToLower(List<string> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].ToLower();
        }
    }

    public string GetWord()
    {
       /* string newWord = string.Empty;

        if(workingWords.Count != 0)
        {
            newWord = workingWords.Last();
            workingWords.Remove(newWord);
        }

        return newWord;*/

        if(workingWords.Count == 0)
        {
            workingWords.AddRange(originalWords);
            Shuffle(workingWords);
            ConvertToLower(workingWords);
        }

        string newWord = workingWords.Last();
        workingWords.RemoveAt(workingWords.Count - 1);
        return newWord;
    }
}
