using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridLayout : MonoBehaviour
{
    public int rows = 2;                // Number of rows
    public int columns = 2;             // Number of columns
    public float spacing = 10f;         // Space between cells

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        UpdateGridLayout();
    }

    void UpdateGridLayout()
    {
        // Get container dimensions
        float containerWidth = rectTransform.rect.width;
        float containerHeight = rectTransform.rect.height;

        // Calculate the best cell size to fit the grid within the container
        float cellWidth = (containerWidth - (spacing * (columns - 1)) - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / columns;
        float cellHeight = (containerHeight - (spacing * (rows - 1)) - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom) / rows;

        // Use the smaller of the two to keep cells square
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        // Apply calculated cell size and spacing to the Grid Layout Group
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);

        // Calculate the extra padding needed to center the grid within the container
        float totalGridWidth = cellSize * columns + spacing * (columns - 1);
        float totalGridHeight = cellSize * rows + spacing * (rows - 1);

        int extraPaddingHorizontal = Mathf.Max(0, Mathf.RoundToInt((containerWidth - totalGridWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / 2));
        int extraPaddingVertical = Mathf.Max(0, Mathf.RoundToInt((containerHeight - totalGridHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom) / 2));

        // Set padding while keeping minimum values as set in the inspector
        gridLayoutGroup.padding.left += extraPaddingHorizontal;
        gridLayoutGroup.padding.right += extraPaddingHorizontal;
        gridLayoutGroup.padding.top += extraPaddingVertical;
        gridLayoutGroup.padding.bottom += extraPaddingVertical;
    }

    // This function can be called whenever you need to update the grid
    public void RefreshGrid(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        UpdateGridLayout();
    }
}
