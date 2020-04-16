using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweater : MonoBehaviour
{
    [SerializeField] float sweaterSpeed = 20;
    public GameObject nearestSheep;
    SheepBehavior nearestSheepBehaviour;
    public float proximity;
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
            nearestSheepBehaviour = nearestSheep.GetComponent<SheepBehavior>();
            nearestSheepBehaviour.pathingType = SheepBehavior.SheepPathingType.ToSweater;
            nearestSheepBehaviour.sweaterPos = nearestSheepBehaviour.PositionToWorldVector2Int(new Vector2(transform.position.x, transform.position.y));
            
            proximity = (nearestSheep.transform.position - this.transform.position).magnitude;

            if (proximity < 3) // change layer only if sheep is about to pick up the shirt
            {
                this.gameObject.layer = 0; // can throw sweater past sheep; sheep can only collide with sweater once it lands
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cannot throw sweater beyond obstacles
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            sweaterSpeed = 0;
            this.tag = "Sweater";
        }

        // Destroy sweater once a sheared sheep reaches it
        if (collision.gameObject.tag == "Sheared")
        {
            collision.gameObject.tag = "Clothed";
            Destroy(gameObject);
        }
    }
}
