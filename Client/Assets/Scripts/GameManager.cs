using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();


    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Update() {
        //Debug.Log(players.Count);
    }

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation) {
        //Debug.Log("2ndCall");
        GameObject _player;
        if (_id == Client.instance.myId) { 
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        } else {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponentInChildren<PlayerManager>().id = _id;
        _player.GetComponentInChildren<PlayerManager>().username = _username;

        players.Add(_id, _player.GetComponent<PlayerManager>());

    }
}
