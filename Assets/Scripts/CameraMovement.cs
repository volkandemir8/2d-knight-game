using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public float cameraSpeed;
    public Transform minCameraPos, maxCameraPos;

    private void FixedUpdate()
    {
        gameObject.transform.position = Vector3.Slerp(
            gameObject.transform.position,
            new Vector3(player.position.x, player.position.y + 2f, gameObject.transform.position.z), 
            cameraSpeed);

        if (gameObject.transform.position.x > maxCameraPos.position.x)
            gameObject.transform.position = new Vector3(maxCameraPos.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

        if (gameObject.transform.position.x < minCameraPos.position.x)
            gameObject.transform.position = new Vector3(minCameraPos.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
    }
}
