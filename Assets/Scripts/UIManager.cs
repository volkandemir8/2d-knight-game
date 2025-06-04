using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject loseScreen;
    public Image healthBar;

    [HideInInspector]
    public bool lost = false;

    private AudioManager audioManager;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        audioManager.PlaySFX(audioManager.newScene, 0.3f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && lost == false)
        {
            Cursor.lockState = CursorLockMode.None;
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            audioManager.PlaySFX(audioManager.click);
        }

        healthBar.fillAmount = GameObject.Find("Player").GetComponent<PlayerMovement>().currentHealth / 100f;
    }

    public void ReturnToGame()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        audioManager.PlaySFX(audioManager.click);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator ShowLoseScreen()
    {
        lost = true;
        loseScreen.SetActive(true);
        audioManager.PlaySFX(audioManager.defeat);

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
