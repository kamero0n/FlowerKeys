using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelTransition((SceneManager.GetActiveScene().buildIndex + 1)));
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadLevelTransition((SceneManager.GetActiveScene().buildIndex)));
    }

    IEnumerator LoadLevelTransition(int levelIndex)
    {
        // play anim
        transition.SetTrigger("Start");


        yield return new WaitForSeconds(transitionTime);


        SceneManager.LoadScene(levelIndex);
    }
}
