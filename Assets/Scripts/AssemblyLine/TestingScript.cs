using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [SerializeField] AssemblyPieceData conveyerBeltData;
    private AssemblyLineSystem assemblyLineSystem;
    private bool buildModeEnabled = false;

    private int facingIndex = 0;
    private void Start()
    {
        assemblyLineSystem = AssemblyLineSystem.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildModeEnabled = !buildModeEnabled;
            Debug.Log("Build Mode: " + (buildModeEnabled ? "Enabled" : "Disabled"));
        }

        if (buildModeEnabled)
        {
            //Rotate each time R is pressed between the facings
            if(Input.GetKeyDown(KeyCode.P))
            {
                facingIndex = (facingIndex + 1) % 4;

                switch(facingIndex)
                {
                    case 0:
                        conveyerBeltData.facing = Facing.North;
                        break;
                    case 1:
                        conveyerBeltData.facing = Facing.East;
                        break;
                    case 2:
                        conveyerBeltData.facing = Facing.South;
                        break;
                    case 3:
                        conveyerBeltData.facing = Facing.West;
                        break;
                }

                Debug.Log("Facing: " + conveyerBeltData.facing);
            }

            // Mouse click to place an assembly piece onto the grid
            if (Input.GetMouseButtonDown(0))
            {
                assemblyLineSystem.PlaceAssemblyPiece(Camera.main.ScreenToWorldPoint(Input.mousePosition), conveyerBeltData);
            }
        }
    }
}
