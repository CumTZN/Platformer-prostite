
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public Button[] levels;

    private void Start()
    {
        int levereached = PlayerPrefs.GetInt("levelreached", 1);

        for (int i = 0; i < levels.Length; i++)
            if (i + 1 > levereached)
                levels[i].interactable = false;
    }



    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Select(int numberInBuild)
    {
        SceneManager.LoadScene(numberInBuild);
        Destroy(GameObject.Find("Audio Sourse"));
    }
}
