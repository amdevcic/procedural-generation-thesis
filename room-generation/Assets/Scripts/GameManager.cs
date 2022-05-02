using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController activePlayer;
    public RoomGenerator activeGenerator;
    public GameObject menu;
    public GameObject pauseMenu;
    bool paused = false;
    
    private void Awake() {
        activePlayer.enabled = false;
        menu.SetActive(true);
    }
    public void StartLevel() {
        menu.SetActive(false);
        activePlayer.transform.position = activeGenerator.GetStartingPosition();
        activePlayer.enabled = true;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && menu.activeSelf == false) {
            TogglePause();
        }
    }

    public void OpenMenu() {
        menu.SetActive(true);
        activePlayer.enabled = false;
        pauseMenu.SetActive(false);
        paused = false;
    }

    public void TogglePause() {
        paused = !paused;
        pauseMenu.SetActive(paused);
        activePlayer.enabled = !paused;
    }
}
