using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private float spaceBTWCargo = 2f;
    [SerializeField] private Vector2Int amountOfCrates = new Vector2Int(5, 5);

    [SerializeField] TextMeshProUGUI cargoScoreText;
    private GameObject[,] crates;

    [Header("Cargo Win/Loss Details")]
    [SerializeField] GameObject heavyCargo;
    [SerializeField] int startAmountOfHeavyCargo;
    [SerializeField] int needAmountOfHeavyCargo;
    [SerializeField] GameObject medCargo;
    [SerializeField] int startAmountOfMedCargo;
    [SerializeField] int needAmountOfMedCargo;
    [SerializeField] GameObject lightCargo;
    [SerializeField] private int startAmountOfLightCargo;
    [SerializeField] int needAmountOfLightCargo;

    [Header("Displaying private vars")]
    [SerializeField] int lightCargoAmount  = 0;
    [SerializeField] int medCargoAmount = 0;
    [SerializeField] int heavyCargoAmount = 0;

    // Show player they are about to lose   
    [SerializeField] private int warningBuffer = 1;


    void Start(){
        
        //Fill in the crates array with the amounts of crates and access will goto light crates

        medCargoAmount = 0;
        heavyCargoAmount = 0;
        lightCargoAmount = 0;

        crates = new GameObject[amountOfCrates.x, amountOfCrates.y];

        for(int i = 0; i < amountOfCrates.x; i++){
            for(int j = 0; j < amountOfCrates.y; j++){

                //Grab a ranom crate that's avaible
                crates[i, j] = GetRandomCrate();
            }
        }

        UpdateCargoText();

    }

    private GameObject GetRandomCrate()
    {
        // Gather crate‐types that are still available
        List<int> availableTypes = new List<int>();
        if (heavyCargoAmount < startAmountOfHeavyCargo)
            availableTypes.Add(0);
        if (medCargoAmount   < startAmountOfMedCargo)
            availableTypes.Add(1);
        if (lightCargoAmount < startAmountOfLightCargo)
            availableTypes.Add(2);

        int choice;
        if (availableTypes.Count > 0)
        {
            // Pick a random crate from avaible 
            int randIndex = Random.Range(0, availableTypes.Count);
            choice = availableTypes[randIndex];
        }
        else
        {
            // If full give a light crate
            choice = 2;
        }

        // Increment the current counter and return the prefab
        switch (choice)
        {
            case 0:
                heavyCargoAmount++;
                return heavyCargo;
            case 1:
                medCargoAmount++;
                return medCargo;
            case 2:
            default:
                lightCargoAmount++;
                return lightCargo;
        }
    }

    public void LostPlayer(){
        // TODO Pause the game somehow
        FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
    }

    public void SpawnCrates(){
        // Note that this spawning I don't like but it works enough
        Vector3 startPos = transform.position;

        for (int row = 0; row < amountOfCrates.x; row++)
        {
            for (int col = 0; col < amountOfCrates.y; col++)
            {
                if (crates[row, col] == null)
                {
                    Debug.LogWarning($"[Spawner] Missing crate at ({row},{col})");
                    continue;
                }

                // Calculate position based on spacing
                Vector3 spawnPos = startPos + new Vector3(row * spaceBTWCargo, transform.position.y, col * spaceBTWCargo);

                Instantiate(crates[row, col], spawnPos, Quaternion.identity);
            }
        }
    }


    public bool HasCargo(){
        if(medCargoAmount > 0) return true;
        if(lightCargoAmount > 0) return true;
        if(heavyCargoAmount > 0) return true;

        return false;
    }
    public void LostCargo(GameObject cargoType){
        if(cargoType.CompareTag("LightCargo")){
            lightCargoAmount -= 1;
        }else if(cargoType.CompareTag("MedCargo")){
            medCargoAmount -= 1;
        }else if(cargoType.CompareTag("HeavyCargo")){
            heavyCargoAmount -= 1;
        }else if(cargoType.CompareTag("Player")){
            LostPlayer();
        }else{
            Debug.LogWarning("Lost cargo without the tag LightCargo | MedCargo | HeavyCargo | Player");
        }
        CheckCargoLoss();
        UpdateCargoText();

    }


    private void CheckCargoLoss()
    {
        // If any one type dips below the required “need” amount → LOSS
        if (medCargoAmount < needAmountOfMedCargo ||
            heavyCargoAmount   < needAmountOfHeavyCargo   ||
            lightCargoAmount < needAmountOfLightCargo)
        {
            FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
        }
    }

    private void CheckCargoWin()
    {
        // If all three meet or exceed their needs → WIN
        if (medCargoAmount >= needAmountOfHeavyCargo &&
            heavyCargoAmount   >= needAmountOfMedCargo   &&
            lightCargoAmount >= needAmountOfLightCargo)
        {
            FindFirstObjectByType<GameOverUI>().ShowGameOverUI(true);
        }
    }


    //Getter for the amount of current Crates incase of future mechanics
    public int AmountOfHeavyCargo() => medCargoAmount;
    public int AmountOfMedCargo()   => heavyCargoAmount;
    public int AmountOfLightCargo() => lightCargoAmount;
    

    private void UpdateCargoText()
    {
        // Build each segment with optional coloring
        string H = FormatSegment(heavyCargoAmount, needAmountOfHeavyCargo);
        string M = FormatSegment(medCargoAmount,   needAmountOfMedCargo);
        string L = FormatSegment(lightCargoAmount, needAmountOfLightCargo);

        cargoScoreText.text = $"H: {H}   M: {M}   L: {L}";
    }

    // Returns "5/2" or "<color=red>1/2</color>" if at or below need,
    // or "<color=yellow>2/2</color>" when within warningBuffer
    private string FormatSegment(int current, int need)
    {
        string raw = $"{current}/{need}";

        if (current < need)
            return $"<color=red>{raw}</color>";
        if (current <= need + warningBuffer)
            return $"<color=yellow>{raw}</color>";
        return raw;
    }
}
