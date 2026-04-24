using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private Transform _playerTransform;
    private Transform _playerIcon;
    private Transform _doorIcon;
    //private float _playerIconOffset = 1.5f;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        GameObject playerIcon = GameObject.Find("PlayerIcon");
        GameObject doorIcon = GameObject.Find("DoorIcon");

        if (player != null)
            _playerTransform = player.GetComponent<Transform>();

        if (playerIcon != null)
            _playerIcon = playerIcon.GetComponent<Transform>();

        if (doorIcon != null)
        {
            _doorIcon = doorIcon.GetComponent<Transform>();
        }
    }

    void Update()
    {
        if (_playerTransform != null && _playerIcon != null)
        {
            // Match the sprite's position to the player's position
            _playerIcon.transform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z);

        }
    }
}
