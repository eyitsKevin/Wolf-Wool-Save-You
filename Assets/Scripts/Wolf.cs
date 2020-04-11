using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public static Wolf player;

    [SerializeField] float playerSpeed = 3;
    [SerializeField] GameObject sweater;
    public Vector2 targetPosition;
    public bool woolHeld;

    void Start()
    {
        if (player == null)
        {
            player = this;
        }

        woolHeld = false;
        targetPosition = new Vector2(0, 0);
    }

    void Update()
    {
        //Basic WASD movement
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.Translate(-Vector2.right * playerSpeed * Time.deltaTime);
            transform.localScale = new Vector3(-2, 2, 2);
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
            transform.localScale = new Vector3(2, 2, 2);
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            transform.Translate(-Vector2.up * playerSpeed * Time.deltaTime);
        }
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);
        }        
        
        // Left-click to shear sheep, only succeeds when sufficiently close to it
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D mouseHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (mouseHit.collider != null)
            {
                if (mouseHit.collider.tag == "Unsheared")
                {
                    if ((transform.position - mouseHit.transform.position).magnitude < 3)
                    {
                        mouseHit.collider.tag = "Sheared";
                        woolHeld = true;
                    }
                }
            }
        }

        // Right-click to throw sweater to the indicated position
        if (woolHeld)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Instantiate(sweater, this.transform.position, Quaternion.identity);
                targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                woolHeld = false;
            }
        }
    }

    public static Vector2Int GetWolfPos()
    {
        return Vector2Int.RoundToInt(new Vector2(player.transform.position.x, player.transform.position.y));
    }
}
