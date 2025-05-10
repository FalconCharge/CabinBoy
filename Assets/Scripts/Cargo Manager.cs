using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private float spaceBTWCargo = 2f;
    [SerializeField] private Vector2Int amountOfCrates = new Vector2Int(5, 5);
    private GameObject[,] crates;

    [Header("New Cargo method")]
    [SerializeField] private GameObject heavyCargo;
    [SerializeField] private int startAmountOfHeavyCargo;
    [SerializeField] private GameObject medCargo;
    [SerializeField] private int StartAmountOfMedCargo;
    [SerializeField] private GameObject lightCargo;
    [SerializeField] private int StartAmountOfLightCargo;

    [Header("Displaying private vars")]
    [SerializeField] int currentAmountOfLightCargo  = 0;
    [SerializeField] int currentAmountOfHeavyCargo = 0;
    [SerializeField] int currentAmountOfMedCargo = 0;


    void Start(){
        
        //Fill in the crates array with the amounts of crates and access will goto light crates

        currentAmountOfHeavyCargo = 0;
        currentAmountOfMedCargo = 0;
        currentAmountOfLightCargo = 0;

        crates = new GameObject[amountOfCrates.x, amountOfCrates.y];

        for(int i = 0; i < amountOfCrates.x; i++){
            for(int j = 0; j < amountOfCrates.y; j++){

                //Grab a ranom crate that's avaible
                crates[i, j] = GetRandomCrate();
            }
        }

    }

    private GameObject GetRandomCrate()
    {
        // Gather crate‐types that are still available
        List<int> availableTypes = new List<int>();
        if (currentAmountOfHeavyCargo < startAmountOfHeavyCargo)
            availableTypes.Add(0);
        if (currentAmountOfMedCargo   < StartAmountOfMedCargo)
            availableTypes.Add(1);
        if (currentAmountOfLightCargo < StartAmountOfLightCargo)
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
                currentAmountOfHeavyCargo++;
                return heavyCargo;
            case 1:
                currentAmountOfMedCargo++;
                return medCargo;
            case 2:
            default:
                currentAmountOfLightCargo++;
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
        if(currentAmountOfHeavyCargo > 0) return true;
        if(currentAmountOfLightCargo > 0) return true;
        if(currentAmountOfMedCargo > 0) return true;

        return false;
    }
    public void LostCargo(GameObject cargoType){
        if(cargoType.CompareTag("LightCargo")){
            currentAmountOfLightCargo -= 1;
        }else if(cargoType.CompareTag("MedCargo")){
            currentAmountOfMedCargo -= 1;
        }else if(cargoType.CompareTag("HeavyCargo")){
            currentAmountOfHeavyCargo -= 1;
        }else if(cargoType.CompareTag("Player")){
            LostPlayer();
        }else{
            Debug.LogWarning("Lost cargo without the tag LightCargo | MedCargo | HeavyCargo | Player");
        }
        CheckCargoLoss();

    }

    private void CheckCargoLoss(){
    
        if(currentAmountOfHeavyCargo <= 0 && currentAmountOfLightCargo <= 0 && currentAmountOfMedCargo <= 0){
            // Call the loss screen since we have no more cargo on the ship
            FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
        }
    }



    //Getter for the amount of current Crates incase of future mechanics
    public int AmountOfLightCargo(){
        return currentAmountOfLightCargo;
    }
    public int AmountOfMedCargo(){
        return currentAmountOfLightCargo;
    }
    public int AmountOfHeavyCargo(){
        return currentAmountOfLightCargo;
    }
    
}
