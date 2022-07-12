
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour ,GameManager.IRestartGameElement
{
    public enum Type { health, coin};
    public Type type;

    public Canvas canvas;
    private Transform player;

    public AudioSource sound;

    public GameObject sparksPrefab;

    private void Start()
    {
        GameManager.instance.AddRestartGameElement(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canPickItem(other.gameObject))
            {
                player = other.transform;

            }
        }
    }



    private bool canPickItem(GameObject player)
    {
        switch (type)
        {
            case Type.health:
                if (FindObjectOfType<HealthController>().currentQuesitos < 8)
                {
                    Instantiate(sparksPrefab, transform.position, Quaternion.identity);
                    canvas.GetComponent<UIController>().HealthIn();
                    sound.Play();
                    DestroyItem();
                    FindObjectOfType<HealthController>().AddHealth();
                }
                return true;
            case Type.coin:
                Instantiate(sparksPrefab, transform.position, Quaternion.identity);
                GameManager.coinNumber++;
                sound.Play();
                canvas.GetComponent<UIController>().CoinIn();
                DestroyItem();
                return true;

            default:
                return false;
        }
    }

    public void DestroyItem()
    {
        gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }

    public void PlaySound()
    {
        sound.Play();
    }
}
