using UnityEngine;
using System.Collections;

public class BugBehaviour : MonoBehaviour
{
    public float maxSize = 160f;
    public float maxEnergy = 100;
    public float maxSpeed = 200f; 
    public float maxLifeSpan = 500;


    // Use this for initialization
    void Start()
    {

        maxEnergy *= maxSize;
        maxSpeed = maxSpeed / maxSize * maxEnergy;
        

    }

    // Update is called once per frame
    void Update()
    {

    }
}
