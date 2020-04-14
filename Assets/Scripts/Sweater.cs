using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweater : MonoBehaviour
{
    [SerializeField] float sweaterSpeed = 20;
    [SerializeField] float sheepSpeed = 3;
    public GameObject nearestSheep;
    float proximity;
    Vector2 targetPosition;

    void Update()
    {
        // Player sets target position and sweater moves towards it
        targetPosition = GameObject.Find("Wolf").GetComponent<Wolf>().targetPosition;
        this.transform.position = Vector2.MoveTowards(this.transform.position, targetPosition, sweaterSpeed * Time.deltaTime);

        // Acquire "Sweater" tag once sweater reaches target position
        if (new Vector2(this.transform.position.x, this.transform.position.y) == targetPosition)
        {
            this.tag = "Sweater";
        }

        // Only closest naked sheep moves to this sweater once it acquires the "Sweater" tag
        if (GameObject.FindGameObjectsWithTag("Sheared").Length > 0 && this.tag == "Sweater")
        {
            GameObject[] nakedSheep = GameObject.FindGameObjectsWithTag("Sheared");
            proximity = (nakedSheep[0].transform.position - this.transform.position).magnitude;
            nearestSheep = nakedSheep[0];

            foreach (GameObject sheep in nakedSheep)
            {
                if ((sheep.transform.position - this.transform.position).magnitude < proximity)
                {
                    proximity = (sheep.transform.position - this.transform.position).magnitude;
                    nearestSheep = sheep;
                }
            }

            // Move nearest sheep to this sweater
            //nearestSheep.transform.position = Vector2.MoveTowards(nearestSheep.transform.position, this.transform.position, sheepSpeed * Time.deltaTime);
            nearestSheep.GetComponent<SheepBehavior>().pathingType = SheepBehavior.SheepPathingType.ToSweater;
            nearestSheep.GetComponent<SheepBehavior>().sweaterPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);

            // Face nearest sheep in direction of this sweater
            if (nearestSheep.transform.position.x < this.transform.position.x)
            {
                nearestSheep.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                nearestSheep.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cannot throw sweater beyond obstacles
        if (collision.gameObject.layer == 8)
        {
            sweaterSpeed = 0;
            this.tag = "Sweater";
        }

        // Destroy sweater once nearest sheep reaches it
        if (collision.gameObject == nearestSheep)
        {
            nearestSheep.tag = "Clothed";
            Destroy(gameObject);
        }
    }
}
