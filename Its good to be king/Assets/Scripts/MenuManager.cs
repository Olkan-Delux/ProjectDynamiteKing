using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MenuData
{
    public static string ourCharacterName = "";
    public static bool ShouldLoad = false;
}

public class MenuManager : MonoBehaviour
{
    public TMP_InputField input;

    public void Start()
    {
        MenuData.ourCharacterName = "";
    }

    public void CreateGame()
    {
        MenuData.ourCharacterName = input.text;
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadGame()
    {
        MenuData.ShouldLoad = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
