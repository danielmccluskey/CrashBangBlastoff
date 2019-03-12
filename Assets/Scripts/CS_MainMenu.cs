using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_MainMenu : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;

    private void Update()
    {
        if (Input.GetButtonDown("A" + 1))
        {
            OnPlayClicked();
        }
        if (Input.GetButtonDown("B" + 1))
        {
            OnQuitClicked();
        }
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}