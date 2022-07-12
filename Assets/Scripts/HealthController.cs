using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthController : MonoBehaviour, GameManager.IRestartGameElement
{
    // Start is called before the first frame update
    public float currentQuesitos;
    public float currentFillAmount;
    public GameObject canvas;
    public GameObject levelLoader;
    public Animator player;

    void Start()
    {
        FindObjectOfType<UIController>().UpdateLifes();
        GameManager.instance.AddRestartGameElement(this);
        currentQuesitos = 8;
        currentFillAmount=gameObject.GetComponent<Image>().fillAmount = 1;
    }

    public void UpdateHealthHud()
    {
        currentFillAmount = currentQuesitos * 0.125f;
        gameObject.GetComponent<Image>().fillAmount = currentFillAmount;
        canvas.GetComponent<UIController>().HealthIn();
        Die();
    }

    public void SubstractHealth()
    {
        currentQuesitos--;
        player.SetTrigger("GetHit");
        UpdateHealthHud();
    }

    public void Kill()
    {
        currentQuesitos = 0;
        UpdateHealthHud();
    }

    public void AddHealth()
    {
        currentQuesitos++;
        Debug.Log(currentQuesitos);
        UpdateHealthHud();
    }

    public void Die()
    {
        if (GameManager.instance.isDead==false)
        {
            //player.SetTrigger("GetHit");
            if (currentQuesitos <= 0)
            {
                player.SetTrigger("GetHit");
                player.SetBool("isDead",true);
                Invoke(nameof(LevelLoaderIn), 0.5f);
                GameManager.instance.isDead = true;
                //Game Over
            }
        }

    }

    void LevelLoaderIn()
    {
        levelLoader.GetComponent<LevelLoader>().Death();
    }

    public void RestartGame()
    {
        currentQuesitos = 8;
        GameManager.instance.isDead = false;
        UpdateHealthHud();
        player.SetTrigger("Restart");
    }
}
