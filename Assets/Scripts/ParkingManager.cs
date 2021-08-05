using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkingManager : MonoBehaviour
{
    public GameObject sizeMessage;
    public InputField inputSize;


    public GameObject popup;
    public Text message;

    public GameObject lot;
    public GameObject spot;
    public GameObject parkButton;
    public GameObject mallButton;
    public GridLayoutGroup layoutGroup;
    public GridLayoutAutoExpand resizer;
    public Sprite available, occupied, recommended;

    private int parkingSize = 0;
    private List<SpotModel> spotList = new List<SpotModel>();
    void Start()
    {
        sizeMessage.SetActive(true); 
    }

    public void Submit()
    {
        if(!string.IsNullOrWhiteSpace(inputSize.text)) 
        {
            int size = int.Parse(inputSize.text);
            if(size % 2 == 0)
            {
                parkingSize = size;
                sizeMessage.SetActive(false);
                parkButton.SetActive(true);
                mallButton.SetActive(true);
                SetupParkingLot();
            } else 
            {
                message.text = "Please input even number.";
                popup.SetActive(true);
            }
        }
    }

    private void SetupParkingLot()
    {
        layoutGroup.constraintCount = parkingSize;   
        resizer.amountPerRow = parkingSize;

        int indexParkingSize = parkingSize - 1;
        List<SpotModel> temp = new List<SpotModel>();
        for(int y = 0; y <= indexParkingSize; y++)
        {
            for(int x = 0; x <= indexParkingSize; x++)
            {               
                GameObject newSpot = Instantiate(spot, new Vector3(0, 0, 0), Quaternion.identity);
                newSpot.transform.SetParent (lot.transform, false);

                Text title = newSpot.transform.Find("Title").gameObject.GetComponent<Text>();

                SpotModel item = new SpotModel();
                item.x = x;
                item.y = y;
                item.mallDistance = ComputeMallDistance(x,y);
                item.parkDistance = ComputeParklDistance(x,y);
                item.gameObject = newSpot;
                item.occupied = false;

                 newSpot.GetComponent<Image>().sprite = available;

                if(x==0 && y==0) 
                {
                    title.text = "Entrance";
                    newSpot.GetComponent<Image>().sprite = occupied;
                    item.occupied = true;
                }

                if(item.mallDistance == 0)
                {
                    title.text = "Mall Entrance";
                }

                if(item.parkDistance == 0)
                {
                    title.text = "Park Entrance";
                }

                spotList.Add(item);
            }
        }
        lot.SetActive(true);
    }

    public void ParkCar(string destination)
    {
        // set to occupied
        List<SpotModel> filter = new List<SpotModel>();
        filter.AddRange(spotList.FindAll(x => x.occupied));
        for(int i = 0; i < filter.Count; i++)
        {
            filter[i].gameObject.GetComponent<Image>().sprite = occupied;
        }
        
        // check if full
        List<SpotModel> check = new List<SpotModel>();
        check.AddRange(spotList.FindAll(x => !x.occupied));
        if(check.Count == 0)
        {
            message.text = "Parking is full.";
            popup.SetActive(true);
            return;
        }


        SpotModel lastSpot; 
        if(destination == "mall")
        {
            lastSpot = GetFurthestMall();
            for(int i = 1; i < spotList.Count; i++)
            {
                if (spotList[i].mallDistance < lastSpot.mallDistance && !spotList[i].occupied) lastSpot = spotList[i];
            }
            lastSpot.gameObject.GetComponent<Image>().sprite = recommended;
            lastSpot.occupied = true;
        }

        if(destination == "park")
        {
            lastSpot = GetFurthestPark();
            for(int i = 1; i < spotList.Count; i++)
            {
                if (spotList[i].parkDistance < lastSpot.parkDistance  && !spotList[i].occupied) lastSpot = spotList[i];
            }
            lastSpot.gameObject.GetComponent<Image>().sprite = recommended;
            lastSpot.occupied = true;
        }
    }

    private float ComputeMallDistance(int x, int y)
    {
        int n = parkingSize - 1;
        
        int mallX = n;
        int mallY = n/2;

        int xd = x - mallX;
        int yd = y - mallY;


        return Mathf.Sqrt(xd * xd + yd * yd);
    }

    private float ComputeParklDistance(int x, int y)
    {
        int n = parkingSize - 1;
        
        int parkX = n/2 + 1;
        int parkY = n;

        int xd = x - parkX;
        int yd = y - parkY;


        return Mathf.Sqrt(xd * xd + yd * yd);
    }

    private SpotModel GetFurthestMall()
    {
        SpotModel furthest = spotList[0];
        for(int i = 1; i < spotList.Count; i++)
        {
            if (spotList[i].mallDistance > furthest.mallDistance) furthest = spotList[i];
        }
        return furthest;
    }

    private SpotModel GetFurthestPark()
    {
        SpotModel furthest = spotList[0];
        for(int i = 1; i < spotList.Count; i++)
        {
            if (spotList[i].parkDistance > furthest.parkDistance) furthest = spotList[i];
        }
        return furthest;
    }
}
