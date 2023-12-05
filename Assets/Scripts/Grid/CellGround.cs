using UnityEngine;

public class CellGround : MonoBehaviour
{
    private Cell cellReference;
    public void SetCellReference(Cell cell)
    {
        cellReference = cell;
    }

    // Unity's method called when the mouse clicks this GameObject
    void OnMouseDown()
    {
        if (cellReference != null)
        {
            if(cellReference.GetState() == CellState.EMPTY)
            {}
            else if(cellReference.GetState() == CellState.OCCUPIED)
            {
                if(cellReference.GetOccupant() is IGridInteractable gridInteractable)
                {
                    gridInteractable.OnSelected();
                }
            }

        }
    }

}
