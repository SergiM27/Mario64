using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public LevelLoader levelLoader;
    bool isStarted;

    private void Start()
    {
        isStarted = false;

    }
    // Update is called once per frame
    void Update()
    {
        if (isStarted == false)
        {
            if (InputSystem.instance.StartGame || Input.GetMouseButtonDown(0) || InputSystem.instance.Jump)
            {
                levelLoader.StartGame();
                isStarted = true;
            }
        }
    }
}
