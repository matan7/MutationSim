using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
//using System;

public class BugScript : MonoBehaviour
{
    //dodati levele i mutacije tipova
    //dodati ostalu hranu i otrove itd npr ljek isto, ostala svojstva (brzina, vece mutacije itd...)
    // implementirati postupno skretanje?

    public float maxSize = 160f;        //Velicina
    public float maxEnergy = 100;       //Energija   
    public float maxSpeed = 50f;        //Brzina
    public float maxLifeSpan = 200;     //Zivotni vjek
    public int maxMemory = 3;           //Memorija
    public float maxFieldOfView = 100;  //Velicina vidnog polja
    public float maxDistanceOfView = 100; //Daljina vida

    public float sizeBug;
    public float energyBug;
    public float lifeSpanBug;
    public float currentSpeed = 0;
    private float directionAngle = 0;
    private float rotationAngle = 0;
    public int changeDirInterval = 2; // ako nema hrane u listi ili memoriji, svakih X sekundi promjeni random smjer kretanja
    

    private int memoryCount; //dodati izmjene ovisno o energiji
    public CircleCollider2D fieldOfWiew;
    public List<GameObject> foodMemory;
    public List<GameObject> foodList;
    public SpriteRenderer sprite;
    public CapsuleCollider2D colider;
    private int hunderth;
    private bool findClosestTrigger = false;
    public Timer timerCs;

    void Start()
    {
        
        //Evolucija
        maxSize += Random.Range(Percent(maxSize, 10), Percent(maxSize, -10));
        maxEnergy += Random.Range(Percent(maxEnergy, 10f), Percent(maxEnergy, -10f));
        maxLifeSpan += Random.Range(Percent(maxLifeSpan, 5), Percent(maxLifeSpan, -5));
        maxSpeed += Random.Range(Percent(maxSpeed, 5), Percent(maxSpeed, -5));
        maxMemory += Random.Range(1, -1);
        maxFieldOfView += Random.Range(Percent(maxSpeed, 5), Percent(maxSpeed, -5));
        maxDistanceOfView += Random.Range(Percent(maxSpeed, 5), Percent(maxSpeed, -5));

        //Spawn postavke
        energyBug = maxEnergy / 100 / 2;
        lifeSpanBug = maxLifeSpan / 10;
        sizeBug = maxSize / 2000;
        fieldOfWiew.radius = maxFieldOfView / 100;
        fieldOfWiew.offset = new Vector2(maxFieldOfView / 100 - (maxDistanceOfView * 0.005f),0f);
        memoryCount = maxMemory;

        //Istanciranje
        colider = GetComponent<CapsuleCollider2D>();
        foodList = new List<GameObject>();
        foodMemory = new List<GameObject>();
        sprite = GetComponent<SpriteRenderer>();
        colider.size = new Vector2( sizeBug + maxSpeed/100 * Time.deltaTime, sizeBug);
        sprite.size = new Vector2(sizeBug, sizeBug);

        hunderth = 0;

        //RANDOM SPAWN:
        //float x = Random.Range(-10f, 10f);
        //float y = Random.Range(-10f, 10f);
        //this.transform.position = new Vector2(x, y);

        //statistika
        timerCs.bugNumber += 1;
        File.AppendAllText(Application.dataPath + "/Bugs.csv", System.Environment.NewLine + timerCs.bugNumber.ToString() + "," + maxSize.ToString() + "," + maxEnergy.ToString() + "," + maxLifeSpan.ToString() + "," + maxSpeed.ToString() + "," + maxMemory.ToString() + "," + maxFieldOfView.ToString() + "," + maxDistanceOfView.ToString());
    }

    void Update()
    {
        //RUB EKRANA
        if (this.transform.position.x < -17.51)
        {
            this.transform.position = new Vector2(17.50f, this.transform.position.y);
        }
        if (this.transform.position.x > 17.51)
        {
            this.transform.position = new Vector2(-17.50f, this.transform.position.y);
        }
        if (this.transform.position.y < -9.01)
        {
            this.transform.position = new Vector2(this.transform.position.x, 9.0f);
        }
        if (this.transform.position.y > 9.01)
        {
            this.transform.position = new Vector2(this.transform.position.x, -9.0f);
        }
        //RUB EKRANA end

        //DOK JE BUBA ZIVA

        currentSpeed = maxSpeed / (sizeBug) * (maxEnergy / 1000 + 1) / 100; //Brzina se smanjuje u odnosu na velicinu i povecava u odnosu na kolicinu energije

        transform.position = VectorCreator(currentSpeed * Time.deltaTime, directionAngle, transform.position); //Kretanje dok nema hrane

        if (energyBug > maxEnergy / 100 * 0.8) //Razmnozavanje ako je energija veca od 80%
        {
            if (sizeBug > maxSize/1000) // ako je veci od "maksimalne" velicine
            {
                Reproduction();
                sizeBug /= 2;
                energyBug /= 2;
                colider.size = new Vector2(sizeBug + currentSpeed * Time.deltaTime, sizeBug);
                sprite.size = new Vector2(sizeBug, sizeBug);
            }
        }
        //DOK JE BUBA ZIVA end

        //MEMORIJA HRANE
        if (foodList.Count > 0 && foodList[0] != null) //ako ima hrane u listi ide prema hrani
        {
            rotationAngle = DirectionFinder(foodList[0].transform.position, this.transform.position) - transform.eulerAngles.z;
            this.transform.Rotate(0, 0, rotationAngle);
            directionAngle = AngleCalc(directionAngle, rotationAngle);
        }
        if (foodList.Count > 0 && foodList[0] == null) //brise ak je null u listi hrane
        {
            foodList.RemoveAt(0);
        }
        if (foodList.Count == 0 && foodMemory.Count > 0 && foodMemory[0] != null)
        {
            rotationAngle = DirectionFinder(foodMemory[0].transform.position, this.transform.position) - transform.eulerAngles.z;
            this.transform.Rotate(0, 0, rotationAngle);
            directionAngle = AngleCalc(directionAngle, rotationAngle);
        }
        if(foodList.Count == 0 && foodMemory.Count > 0 && foodMemory[0] == null)
        {
            foodMemory.RemoveAt(0);
        }
        //MEMORIJA HRANE end
    }
    void OnCollisionEnter2D(Collision2D col) // GLAVNI COLIDER - JEDENJE
    {
        if (col.gameObject.CompareTag("Food"))
        {
            if (foodList.Count > 0 && foodList[0] == null)
            {
                foodList.RemoveAt(0);
            }
            energyBug += col.gameObject.GetComponent<Food>().Energy;
            if (energyBug > maxEnergy / 100)
            {
                energyBug = maxEnergy / 100;
            }
            if(sizeBug < maxSize/1000)
            {
                sizeBug += Percent(sizeBug,10);
                colider.size = new Vector2(sizeBug + currentSpeed * Time.deltaTime, sizeBug);
                sprite.size = new Vector2(sizeBug, sizeBug);
            }
            GameObject.Destroy(col.gameObject);
        }
        if (col.gameObject.CompareTag("Bug"))
        {
            float colSize = col.transform.GetComponent<BugScript>().sizeBug;
            if (this.sizeBug > col.transform.GetComponent<BugScript>().sizeBug * 2)
            {
                energyBug += col.transform.GetComponent<BugScript>().energyBug * 0.8f;
                GameObject.Destroy(col.gameObject);
                if (foodList.Count > 0 && foodList[0] == null)
                {
                    foodList.RemoveAt(0);
                }

                if (energyBug > maxEnergy / 100)
                {
                    energyBug = maxEnergy / 100;
                }
            }
            else
            {
                if(foodList.Count > 0)
                foodList.RemoveAt(0); 
            }
        }
        findClosestTrigger = true;
    }
    void OnTriggerEnter2D(Collider2D col) //COLIDER ZA "GLEDANJE"
    {
        if (col.gameObject.CompareTag("Food"))
        {
            foodList.Add(col.gameObject);
            findClosestTrigger = true;
        }
        if (col.gameObject.CompareTag("Bug"))
        {
            if (this.sizeBug > col.transform.GetComponent<BugScript>().sizeBug * 2)
            {
                foodList.Add(col.gameObject);
                findClosestTrigger = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Food"))
        {
            foodList.Remove(col.gameObject);
            if(foodMemory.Count <= memoryCount)
            {
                foodMemory.Add(col.gameObject);
            }
            findClosestTrigger = true;
        }
        if (col.gameObject.CompareTag("Bug"))
        {
            if (this.sizeBug > col.transform.GetComponent<BugScript>().sizeBug * 2)
            {
                foodList.Remove(col.gameObject);
                if (foodMemory.Count <= memoryCount)
                {
                    foodMemory.Add(col.gameObject);
                }

                findClosestTrigger = true;
            }
        }
        
    }
    void LateUpdate()
    {       
        if (findClosestTrigger)
        {
            FindClosest(foodList);
            MemoryCleaner(foodMemory);
            findClosestTrigger = false;
        }
    }
    void FixedUpdate() //TIMER
    {
        hunderth++; //STOTINKE  x2
        
        if (hunderth == 50)//SEKUNDE
        {
            hunderth = 0;
            DoEverySec();
        }
        if (energyBug <= 0 || lifeSpanBug <= 0)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
    private void DoEverySec() //TIMER ZA SVAKU SEKUNDU
    {
        changeDirInterval--;
        energyBug -= (sizeBug * currentSpeed + maxFieldOfView);      // potrosnja energije po sekundi 
        lifeSpanBug -= 1 / (energyBug * 4);        // starenje - sto manje energije ima brze stari
        if (changeDirInterval == 0)             // mjenjanje smjera kretnje
        {
            changeDirInterval = 2;
            if (foodList.Count == 0)
            {
                rotationAngle = Random.Range(-120, 120);
                directionAngle = AngleCalc(directionAngle, rotationAngle);
                this.transform.Rotate(0, 0, rotationAngle);
            }
        }      
    }
    //MEMORY  CLEANER cisti listu od null objekata
    void MemoryCleaner(List<GameObject> list) 
    {
        for(int i = 0; i <= list.Count - 1; i++)
        {
            if(list[i] == null)
            {
                list.RemoveAt(i);
            }
        }
    }
    
    void Reproduction() //REPRODUCIRANJE
    {
        Transform newBug = GameObject.Instantiate(this.transform, new Vector3(this.transform.position.x, this.transform.position.y), Quaternion.identity, this.transform.parent);
        newBug.transform.name = "Bug";
    }
    void FindClosest(List<GameObject> list) //PO FOODLISTI TRAZI IDUCU NAJBLIZU 
    {
        if (list.Count > 1)
        {
            int index = -1;
            int indexS = 0;
            GameObject closest = list[0];
            
            foreach (GameObject food in list)
            {
                index++;
                if(index != 0) {
                    try
                    {  
                        if (Calculator(food.transform.position) < Calculator(closest.transform.position))
                        {
                            closest = food;
                            indexS = index;
                        }                       
                    }
                    catch{
                    }
                }  
            }
            foodList.RemoveAt(indexS);
            foodList.Insert(0, closest);
        }

        float Calculator(Vector3 obj1)
        {
            Vector3 obj2 = this.transform.position;
            float aSide;
            float bSide = obj1.y - obj2.y;
            if (obj2.x >= obj1.x)
            {
                aSide = obj2.x - obj1.x;
            }
            else
            {
                aSide = obj1.x - obj2.x;
            }
            if (obj2.y >= obj1.y)
            {
                bSide = obj2.y - obj1.y;
            }
            else
            {
                bSide = obj1.y - obj2.y;
            }
            return Mathf.Sqrt(Mathf.Pow(aSide, 2) + Mathf.Pow(bSide, 2));
        }
    }
    
    Vector3 VectorCreator(float speed, float direction, Vector3 position) //ENGINE ZA KRETANJE
    {
        float bSide = 0;
        float aSide = 0;
        Vector3 dirPosition = new Vector3();
        if (direction <= 180)
        {
            bSide = Mathf.Cos(direction * Mathf.Deg2Rad) * speed;
            aSide = Mathf.Sqrt(Mathf.Pow(speed, 2) - Mathf.Pow(bSide, 2));
            dirPosition = new Vector3(position.x + bSide, position.y + aSide, 0);
        }

        else if (direction > 180)
        {
            bSide = Mathf.Cos(direction * Mathf.Deg2Rad) * speed;
            aSide = Mathf.Sqrt(Mathf.Pow(speed, 2) - Mathf.Pow(bSide, 2));
            dirPosition = new Vector3(position.x + bSide, position.y + aSide * -1, 0);
        }

        return dirPosition;
    }

    float DirectionFinder(Vector3 obj1, Vector3 obj2) //USMJERAVA NA ODREDJENU POZICIJU
    {
        float aSide;
        float bSide = obj1.y - obj2.y;
        if (obj2.x >= obj1.x)
        {
            aSide = obj2.x - obj1.x;
        }
        else
        {
            aSide = obj1.x - obj2.x;
        }
        if (obj2.y >= obj1.y)
        {
            bSide = obj2.y - obj1.y;
        }
        else
        {
            bSide = obj1.y - obj2.y;
        }
        float cSide = Mathf.Sqrt(Mathf.Pow(aSide, 2) + Mathf.Pow(bSide, 2));
        float cAngle = Mathf.Atan(aSide / bSide) * Mathf.Rad2Deg;
        if (obj2.x >= obj1.x && obj2.y >= obj1.y)
        {
            cAngle = Mathf.Atan(aSide / bSide) * Mathf.Rad2Deg * -1 - 90;
        }
        else if (obj2.x < obj1.x && obj2.y >= obj1.y)
        {
            cAngle = Mathf.Atan(aSide / bSide) * Mathf.Rad2Deg - 90;
        }
        else if (obj2.x >= obj1.x && obj2.y < obj1.y)
        {
            cAngle = Mathf.Atan(aSide / bSide) * Mathf.Rad2Deg + 90;
        }
        else
        {
            cAngle = Mathf.Atan(aSide / bSide) * Mathf.Rad2Deg * -1 + 90;
        }
        if (float.IsNaN(cAngle))
        {
            cAngle = 0f;
        }

        return cAngle;
    }
    float AngleCalc(float curentAngle, float rotation) //ROTACIJA BUBE
    {
        float newAngle = 0;
        if (curentAngle + rotation > 360)
        {
            newAngle = (curentAngle + rotation) % 360;
        }
        else if (curentAngle + rotation < 360 && curentAngle + rotation >= 0)
        {
            newAngle = curentAngle + rotation;
        }
        else if (curentAngle + rotation < 0)
        {
            newAngle = 360 - (curentAngle + rotation) % 360 * -1;
        }
        if (newAngle == 360)
        {
            newAngle = 0;
        }
        return newAngle;
    }
    float Percent(float num, float percent) //RACUNA POSTOTAK PRVOG BROJA
    {
        float a = num / 100 * percent;
        return a;
    }
}