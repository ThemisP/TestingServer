using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    [Header("Menus")]
    public GameObject LoginMenu;
    public GameObject MainScreenMenu;
    public GameObject LobbyMenu;

    [Header("InputFields")]
    public InputField username;
    public InputField roomIndexSelect;

    [Header("TextFields")]
    public Text RoomTitle;
    public Text Player1InLobby;
    public Text Player2InLobby;

    private MenuState _state;
    private float timer = 0;
    public enum MenuState {
        Login,
        Main,
        Lobby
    }

	// Use this for initialization
	void Start () {
        SetMenuState(MenuState.Login);
	}

    void Update() {
        if(timer > 4f) {
            Network.instance.GetPlayersInRoom();
            timer = 0;
        } else {
            timer += Time.deltaTime;
        }
        
    }
    private void UpdateLobbyNames() {

    }

    private void SetMenuState(MenuState state) {
        this._state = state;
        switch (state) {
            case MenuState.Login:
                LoginMenu.SetActive(true);
                MainScreenMenu.SetActive(false);
                LobbyMenu.SetActive(false);
                break;
            case MenuState.Main:
                LoginMenu.SetActive(false);
                MainScreenMenu.SetActive(true);
                LobbyMenu.SetActive(false);
                break;
            case MenuState.Lobby:
                LoginMenu.SetActive(false);
                MainScreenMenu.SetActive(false);
                LobbyMenu.SetActive(true);
                break;
        }
    }

    public void Login() {
        if (string.IsNullOrEmpty(username.text)) return;
        Network.instance.Login(username.text);
        SetMenuState(MenuState.Main);
    }

    public void CreateGame() {
        Network.instance.CreateGame(2);
        SetMenuState(MenuState.Lobby);
    }
    
    public void Quit() {
        SetMenuState(MenuState.Login);
    }

    public void QuitMain() {
        SetMenuState(MenuState.Main);
    }

}
