using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController activePlayer;
    public RoomGenerator activeGenerator;
    public GameObject menu;
    
    private void Awake() {
        activePlayer.enabled = false;
        menu.SetActive(true);
    }
    public void StartLevel() {
        menu.SetActive(false);
        activePlayer.transform.position = activeGenerator.GetStartingPosition();
        activePlayer.enabled = true;
    }
}
