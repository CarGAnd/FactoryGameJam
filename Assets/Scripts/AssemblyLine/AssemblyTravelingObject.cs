using System.Collections;
using UnityEngine;

public class AssemblyTravelingObject : MonoBehaviour
{
    public void MoveToPiece(Vector3 startPosition, Vector3 endPosition)
    {
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
    }
}
