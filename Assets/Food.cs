using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float Energy { get; private set; }

    private float x;
    private float y;
    private bool eadable;

    public int readyToEat;
    private int hunderth;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = this.transform.GetComponent<SpriteRenderer>();
        Energy = Random.Range(1f, 10f);
        readyToEat = (int)Mathf.Round(Random.Range(1,10) * Energy / 10);
        if(readyToEat == 0)
        {
            readyToEat = 1;
        }
        x = Random.Range(-17.5f, 17.5f);
        y = Random.Range(-9f, 9f);
        this.transform.position = new Vector2(x, y);
        eadable = false;
        this.transform.tag = "NotFood";
        sprite.color = Color.gray;
    }
    void OnDestroy()
    {
        GameObject go = GameObject.Instantiate(this.gameObject);
        go.transform.name = "Food";
        go.SetActive(true);
        x = Random.Range(-17.5f, 17.5f);
        y = Random.Range(-9f, 9f);
        go.transform.position = new Vector2(x, y);
        go.GetComponent<Food>().enabled = true;
        go.GetComponent<CircleCollider2D>().enabled = true;
    }
    void FixedUpdate() //TIMER
    {
        hunderth++; //STOTINKE  x2
        if (hunderth == readyToEat * 100)
        {
            hunderth = 0;
            Growth();
        }
    }
    void Growth()
    {
        this.transform.tag = "Food";
        sprite.color = new Color(1, 0.6751f, 0);
    }
}
