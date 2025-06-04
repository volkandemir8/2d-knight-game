using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject boss;
    public Image bossHealthBar;
    private AudioManager audioManager;
    private bool isPlayed = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        // Boss nesnesi devre dýþý býrakýldýðýnda ShowWinScreen metodunu çaðýr
        if (boss != null && !boss.activeInHierarchy && gameObject.GetComponent<UIManager>().lost == false)
        {
            StartCoroutine(ShowWinScreen());
        }

        bossHealthBar.fillAmount = boss.GetComponent<Enemy>().currentHealth / boss.GetComponent<Enemy>().maxHealth;
    }

    public IEnumerator ShowWinScreen()
    {
        gameObject.GetComponent<UIManager>().enabled = false;
        winScreen.SetActive(true);
        if(!isPlayed)
        {
            isPlayed = true;
            audioManager.PlaySFX(audioManager.win);
        }

        yield return new WaitForSeconds(6f);

        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
}
