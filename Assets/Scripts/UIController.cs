using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text coinText;
    public Text lifesText;
    public Animator healthAnimator;
    public Animator coinAnimator;

    public void HealthIn()
    {
        healthAnimator.SetBool("IsIn", true);
        CancelInvoke("HealthOut");
        Invoke("HealthOut", 2.0f);
    }
    public void HealthOut()
    {
        healthAnimator.SetBool("IsIn", false);
    }

    public void CoinIn()
    {
        coinText.text = GameManager.coinNumber.ToString();
        coinAnimator.SetBool("IsIn", true);
        CancelInvoke("CoinOut");
        Invoke("CoinOut", 2.0f);
    }
    public void CoinOut()
    {
        coinAnimator.SetBool("IsIn", false);
    }

    public void UpdateLifes()
    {
        lifesText.text = GameManager.lifesNumber.ToString();
    }
}
