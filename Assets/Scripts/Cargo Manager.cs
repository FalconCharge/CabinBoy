using System.Collections.Generic;
using UnityEngine;

public class CargoManager : MonoBehaviour
{

    [SerializeField] private GameObject[] products;
    [SerializeField] private float spaceBTWCargo = 2f;
    [SerializeField] private Vector2Int amountOfCrates = new Vector2Int(5, 5);
    private GameObject[,] crates;

    [SerializeField] private float cratesLeft;
    //[SerializeField] private GameObject gameOverUI;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crates = new GameObject[amountOfCrates.x, amountOfCrates.y];

        for(int row = 0; row < amountOfCrates.x; row++){
            for(int col = 0; col < amountOfCrates.y; col++){
                crates[row, col] = products[Random.Range(0, products.Length)];
            }
        }

        cratesLeft = amountOfCrates.x * amountOfCrates.y;

        //gameOverUI.SetActive(false);
    }


    public void LostCrate(){
        if(cratesLeft - 1 <= 0){
            // TODO: Disable Normal UI (The score)
            //gameOverUI.SetActive(true);
            // TODO: Game Over does not mean game over the UI could say you won if you have enough crates left
            // Prob should pass in a boolean saying whether it a win or not for the Game UI to adjust to that
            FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
            
        }else{
            cratesLeft -= 1;
        }
    }

    public void LostPlayer(){
        //gameOverUI.SetActive(true);

        FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
    }

    public void SpawnCrates(){
        // TODO Spawn in the crates
        // Go through array 1 by 1 and spawn then in 
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
        if(cratesLeft > 0){
            return true;
        }
        return false;
    }
}
