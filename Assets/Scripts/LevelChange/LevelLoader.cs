using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public GameObject pauseMenu;
    public GameObject normalMenu;
    public GameObject gameOverMenu;
    public GameObject selectedButton;
    public AudioSource sound;
    public AudioSource menuPressSound;

    public float transitionTime=1f;
    public float transitionTimeEnd = 3f;



    public void StartGame()
    {
        StartCoroutine(GoToLevel());
        Invoke(nameof(StartPlaying), transitionTimeEnd);
    }

    public void BackToMenu()
    {
        StartCoroutine(GoToMenu());
    }

    public void Death()
    {
        StartCoroutine(DeathAnimation());
    }
    public void RestartGame()
    {
        StartCoroutine(LoadLevelReset());
    }

    public void RestartGameCheckpoint()
    {
        StartCoroutine(LoadLevelCheckpoint());
    }

    IEnumerator LoadLevelCheckpoint()
    {
        if (GameManager.lifesNumber > 0)
        {
            GameManager.instance.checkPoint = true;
            transition.SetBool("Start", true);
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yield return new WaitForSeconds(transitionTime);
            GameManager.lifesNumber--;
            GameManager.coinNumber = 0;
            FindObjectOfType<UIController>().coinText.text = GameManager.coinNumber.ToString(); 
            FindObjectOfType<UIController>().UpdateLifes();
            GameManager.instance.RestartGame();
            transition.SetBool("Start", false);
            GameManager.instance.checkPoint = false;
            GameManager.instance.isDead = false;
        }
    }

    IEnumerator LoadLevelReset()
    {
        if(GameManager.lifesNumber > 0)
        {
            GameManager.instance.fromStart = true;
            transition.SetBool("Start", true);
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yield return new WaitForSeconds(transitionTimeEnd);
            GameManager.coinNumber = 0;
            GameManager.lifesNumber--;
            FindObjectOfType<UIController>().UpdateLifes();
            FindObjectOfType<UIController>().coinText.text=GameManager.coinNumber.ToString();
            GameManager.instance.RestartGame();
            transition.SetBool("Start", false);
            GameManager.instance.fromStart = false;
            Invoke(nameof(StartPlaying), transitionTimeEnd);
        }
    }

    void StartPlaying()
    {
        GameManager.instance.isDead = false;
        GameManager.coinNumber = 0;
        GameManager.lifesNumber = 5;
    }

    IEnumerator DeathAnimation()
    {
        transition.SetBool("Start", true);

        yield return new WaitForSeconds(transitionTimeEnd);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenu.SetActive(true);
        if (GameManager.lifesNumber <= 0)
        {
            normalMenu.SetActive(false);
            gameOverMenu.SetActive(true);
        }
        else
        {
            gameOverMenu.SetActive(false);
        }
        sound.Play();
        GameManager.instance.isDead = true;
        EventSystem.current.SetSelectedGameObject(selectedButton);

    }

    IEnumerator GoToLevel()
    {
        transition.SetBool("Start", true);
        menuPressSound.Play();

        yield return new WaitForSeconds(transitionTimeEnd);
        GameManager.lifesNumber = 5;
        SceneManager.LoadScene(1);

    }

    IEnumerator GoToMenu()
    {
        transition.SetBool("Start", true);

        yield return new WaitForSeconds(transitionTimeEnd);

        SceneManager.LoadScene(0);

    }
}
