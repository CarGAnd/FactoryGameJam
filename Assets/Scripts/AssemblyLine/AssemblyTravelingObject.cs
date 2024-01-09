using System.Collections;
using UnityEngine;

public class AssemblyTravelingObject : MonoBehaviour
{
    public void MoveToPiece(Vector3 startPosition, Vector3 endPosition)
    {
        StartCoroutine(MoveToPieceCoroutine(new Vector3(startPosition.x, 0.4f, startPosition.z), new Vector3(endPosition.x, 0.4f, endPosition.z)));
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
