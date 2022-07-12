using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager instance;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    public PlayerController player;

    public LevelLoader levelLoader;

    public static int coinNumber = 0;
    public static int lifesNumber = 5;

    //CheckPoint
    public bool checkPoint;
    public bool fromStart;
    public bool isDead;
    public GameObject currentCheckpoint;
    public int checkPointHealth;
    public int checkPointCoins;

    public List<IRestartGameElement> restartGameElements = new List<IRestartGameElement>();


    private void Start()
    {
        lifesNumber = 5;
        isDead = false;
    }
    public interface IRestartGameElement
    {
        void RestartGame();
    }

    public void AddRestartGameElement(IRestartGameElement restartGameElement)
    {
        restartGameElements.Add(restartGameElement);
    }

    public void RestartGame()
    {
        foreach(IRestartGameElement restartGameElement in restartGameElements)
        {
            restartGameElement.RestartGame();
        }
    }

}