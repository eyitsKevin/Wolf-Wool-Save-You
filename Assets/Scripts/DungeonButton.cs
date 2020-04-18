using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonButton : MonoBehaviour
{
    private Animator animator;
    private GameObject door;
    private bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        isPressed = false;
        animator = GetComponent<Animator>();
        door = GameObject.Find("Dungeon Door");
    }

    private void Pressed()
    {
        animator.SetTrigger("Pressed");
        door.SetActive(false); // Opens door
        SoundEffectManager.instance.PlayDoorSound();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Equals("Wolf") && isPressed == false)
        {
            isPressed = true;
            Pressed();
        }
    }
}
