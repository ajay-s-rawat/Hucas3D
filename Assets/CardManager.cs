using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeReference] private DynamicGridLayout dynamicGridLayout;
    public GameObject cardPrefab; // Reference to your card prefab
 

    void Start()
    {
        GenerateCardLayout(dynamicGridLayout.rows, dynamicGridLayout.columns);
    }

    void GenerateCardLayout(int rows, int columns)
    {
        gridLayoutGroup.constraintCount = rows;
        for (int i = 0; i < rows * columns; i++)
        {
            // Instantiate card prefab
            GameObject card = Instantiate(cardPrefab, transform);
            // Optionally, you can assign unique identifiers or images to each card here
        }
    }
}
