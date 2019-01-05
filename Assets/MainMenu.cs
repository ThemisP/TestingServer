using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    public GameObject LoginMenu;
    public GameObject MainScreenMenu;

    public InputField username;
    

	// Use this for initialization
	void Start () {
        LoginMenu.SetActive(true);
        MainScreenMenu.SetActive(false);
	}

    public void Login() {
        if (string.IsNullOrEmpty(username.text)) return;
        Network.instance.Login(username.text);
        LoginMenu.SetActive(false);
        MainScreenMenu.SetActive(true);
        
    }
    
    public void Quit() {
        LoginMenu.SetActive(true);
        MainScreenMenu.SetActive(false);
    }

}
