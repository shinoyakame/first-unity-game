using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArea : MonoBehaviour {

    public Transform player;
    public GameStage gameStage;
    public int index;
	
	void Update () {
        if (gameStage.isGameOver) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    player.position = new Vector3(hit.collider.transform.position.x, player.position.y, player.position.z);
                    gameStage.PlayerAction(index);
                }
            }
        }
	}
}
