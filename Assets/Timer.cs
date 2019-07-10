using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class Timer : MonoBehaviour
{
    //public int minutes { get; set; }
    public int seconds { get; set; }
    public int hundredth { get; set; }

    public GameObject food;
    public GameObject bug;
    private GameObject[] objs;
    private TextAsset textFile;
    private string text;
    public string filoeLocation;
    public int bugNumber { get; set; }
    void Start()
    {
        Random.seed = 42 * System.DateTime.Now.Second / (System.DateTime.Now.Minute + 1);
        for (int i = 0; i < 100; i++)
        {
            GameObject.Instantiate(food);
            food.transform.name = "Food";
        }
        filoeLocation = Application.dataPath + "/Statistics.csv";

        //minutes = 0;
        seconds = 0;
        hundredth = 0;
    }
    private void Update()
    {
        
        
    }

    
    void FixedUpdate()
    {
        hundredth++;
        if (hundredth == 500)
        {
            seconds += 10;
            hundredth = 0;
            int bugNum = 0;
            int foodNum = 0;
            objs = GameObject.FindGameObjectsWithTag("Bug");
            foreach (GameObject Bug in objs)
            {
                bugNum++;
            }
            objs = GameObject.FindGameObjectsWithTag("Food");
            foreach (GameObject Food in objs)
            {
                foodNum++;
            }
            File.AppendAllText(filoeLocation, System.Environment.NewLine + seconds.ToString() + "," + bugNum.ToString() + "," + foodNum.ToString());
        }       
    }
}
