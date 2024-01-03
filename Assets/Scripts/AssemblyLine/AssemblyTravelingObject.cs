using System.Collections;
using UnityEngine;

public class AssemblyTravelingObject : MonoBehaviour
{
    public void MoveToPiece(Cell startPiece, Cell endPiece)
    {
        // Assuming each AssemblyPiece has a method to get its world position
        Vector3 startPosition = startPiece.GetCellCenter();
        Vector3 endPosition = endPiece.GetCellCenter();
        StartCoroutine(MoveToPieceCoroutine(startPosition, endPosition));
    }

    private IEnumerator MoveToPieceCoroutine(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        float duration = 1f; // Duration of the movement, adjust as needed

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
            yield return null;
        }

        // Optionally, trigger an event or callback when the movement is complete
    }
}
