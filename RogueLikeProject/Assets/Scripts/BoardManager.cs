using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //Cuantas filas y columnas tendra cada nivel, esto sera muy util para el tamano del nivel
    public int columns = 8;
    public int rows = 8;

    //Esto podra contar las paredes y la comida que se encontraran en el nivel
    //Count se usara para especificar un rango aleatorio para cuantas paredes y comida queremos engendrar en cada nivel
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    //la salida, sera una sola porque solo habra una salida
    public GameObject exit;

    //Aqui colocaremos como array porque hay vario de ellos o pueden haber varios en cada nivel 
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    //Esto es algo que vamos a usar para mantener limpia la jerarquia 
    private Transform boardHolder;

    //Esto se usara para rastrear las posiciones posibles en nuestro tablero de juego
    private List<Vector3> gridPositions = new List<Vector3>();

    //Esto sera para llenar el nivel con paredes, enemigos entre otros
    void InitialiseList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 0; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //Esto es para configurar la pared exterior y el piso de nuestro tablero de juego
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //Utilizamos la longitud de el array y asi sabremos que sprite agarrar
                GameObject ToInstatiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    ToInstatiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(ToInstatiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }
    //Coloca un objeto random en el mapa y elimina el index para que no exista duplicado
    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Este va a controlar cuantos de un objeto dado vamos a generar
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
    //Este sera llamado por el game manager ya que sera el unico 
    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns -1, rows -1, 0f), Quaternion.identity);
    }
}
