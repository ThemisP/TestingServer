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
    private bool reset = true;
    private float timer = 0;
    private float timer2 = 0;
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
        switch (this._state) {
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

                //update every 2 seconds
                if (timer > 2f) {
                    Player1InLobby.text = Network.instance.player.GetUsername();
                    string p2 = Network.instance.player.GetTeammateUsername();
                    if (p2 != null)
                        Player2InLobby.text = p2;
                    RoomTitle.text = "Room Index: " + Network.instance.player.GetRoomIndex();
                    timer = 0;
                } else {
                    timer += Time.deltaTime;
                }

                //update every 16 seconds
                if(timer2 > 16f) {
                    Network.instance.GetPlayersInRoom();
                    timer2 = 0;
                    reset = true;
                } else {
                    timer2 += Time.deltaTime;
                }
                
                break;
        }
    }
    private void UpdateLobbyNames() {

    }

    private void SetMenuState(MenuState state) {
        this._state = state;
    }

    public void Login() {
        if (string.IsNullOrEmpty(username.text)) return;
        Network.instance.Login(username.text);
        SetMenuState(MenuState.Main);
    }

    public void CreateGame() {
        Network.instance.CreateGame(2);
    }
    public void CreateGameSuccessfull() {
        SetMenuState(MenuState.Lobby);
    }

    public void JoinGame() {
        if(string.IsNullOrEmpty(roomIndexSelect.text)) {
            Debug.Log("Enter room index to join");
            return;
        }
        int roomNum;
        int.TryParse(roomIndexSelect.text, out roomNum);
        Debug.Log(roomNum);
        
        Network.instance.JoinRoom(roomNum);
    }
    public void JoinGameSuccessfull() {
        SetMenuState(MenuState.Lobby);
    }

    public void Quit() {
        SetMenuState(MenuState.Login);
    }

    public void QuitMain() {
        SetMenuState(MenuState.Main);
    }

}
