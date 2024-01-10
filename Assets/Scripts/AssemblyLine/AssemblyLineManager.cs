using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyLineManager
{
    private List<AssemblyLine> assemblyLines;
    public AssemblyLineManager(List<AssemblyLine> assemblyLines)
    {
        this.assemblyLines = assemblyLines;
    }
    public void PlaceAssemblyPiece(AssemblyPiece newPiece)
    {
        if(newPiece != null)
        {
            AddPieceToAssemblyLine(newPiece);
        }
    }

    private void AddPieceToAssemblyLine(AssemblyPiece piece)
    {
        AssemblyPiece intersectedPiece;
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out intersectedPiece);
        List<AssemblyLine> endLines = FindLinesPieceIsEndOf(piece);

        if(intersectedLine != null && endLines.Count > 0)
        {
            HandleMultipleIntersections(piece, intersectedPiece, intersectedLine, endLines);
        }
        else if(intersectedLine != null)
        {
            HandleSingleIntersection(piece, intersectedPiece, intersectedLine);
        }
        else if(endLines.Count > 0)
        {
            HandleEndConnections(piece, endLines);
        }
        else
        {
            CreateNewLine(piece);
        }
    }

    private void HandleEndConnections(AssemblyPiece piece, List<AssemblyLine> endLines)
    {
        AssemblyLine correctEndLine = FindCorrectEndLine(endLines, piece);
        if (correctEndLine != null)
        {
            correctEndLine.AddPiece(piece);
            HandleConnectingLines(correctEndLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);
        }
        else
        {
            AssemblyLine newLine = CreateNewLine(piece);
            HandleConnectingLines(newLine, endLines, piece);
        }
    }

    private void HandleSingleIntersection(AssemblyPiece piece, AssemblyPiece intersectedPiece, AssemblyLine intersectedLine)
    {
        //The case where the new piece is just the new start of an intersected line.
        if (intersectedPiece.facing == piece.facing)
        {
            intersectedLine.AddPiece(piece);
        }
        //If the new piece is intersecting at the middle of a line without being the end of other lines
        else
        {
            AssemblyLine newLine = CreateNewLine(piece);
            HandleConnectingLines(intersectedLine, new List<AssemblyLine> { newLine }, intersectedPiece);
        }
    }

    private void HandleMultipleIntersections(AssemblyPiece piece, AssemblyPiece intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        //We start by considering the scenario where there are two lines are facing the same way.
        AssemblyLine correctEndLine = FindCorrectEndLine(endLines, intersectedPiece, piece);
        if (correctEndLine != null)
        {
            CombineLinesFacingSameWay(piece, intersectedLine, endLines, correctEndLine);
        }
        //Alright we assume the endlines do not face the same way as the intersected line. 
        else
        {
            //In which case we first need to see if the new piece is part of an existing endline.
            correctEndLine = FindCorrectEndLine(endLines, piece);
            if (correctEndLine != null)
            {
                CombineEndLineFacingPiece(piece, intersectedPiece, intersectedLine, endLines, correctEndLine);
                return;
            }
            //However the new piece might still face the same way as the intersected line.
            else if(piece.facing == intersectedPiece.facing)
            {
                CombineEndLinesWithIntersectedLine(piece, intersectedLine, endLines);
            }
            //In case they don't face the same way as the piece, and the piece doesn't face the same way as the line..
            else
            {
                CombineEndLinesFacingDifferentThanPiece(piece, intersectedPiece, intersectedLine, endLines);
            }
        }
    }

    private void CombineEndLinesWithIntersectedLine(AssemblyPiece piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        intersectedLine.AddPiece(piece);
        //We add the endlines as connecting lines to this line
        AssemblyLine loopingEndLine = null;
        foreach(AssemblyLine line in endLines)
        {
            if(LoopDetected(line, intersectedLine))
            {
                line.GetEndPiece().nextPiece = piece;
                loopingEndLine = line;
                break;
            }
        }

        HandleConnectingLines(intersectedLine, endLines.Except(new[] {loopingEndLine}).ToList(), piece);
    }

    private void CombineEndLinesFacingDifferentThanPiece(AssemblyPiece piece, AssemblyPiece intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        AssemblyLine newLine = CreateNewLine(piece);
        //We add the endlines as connecting lines to this line
        HandleConnectingLines(newLine, endLines, piece);

        if(LoopDetected(newLine, intersectedLine))
        {
            piece.nextPiece = intersectedPiece;
            return;
        }
        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { newLine }, intersectedPiece);
    }

    private void CombineEndLineFacingPiece(AssemblyPiece piece, AssemblyPiece intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We add the piece to our end line.
        correctEndLine.AddPiece(piece);
        //And now we need to ensure that all endlines are connecting lines to this line.
        HandleConnectingLines(correctEndLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);

        if(LoopDetected(correctEndLine, intersectedLine))
        {
            piece.nextPiece = intersectedPiece;
            return;
        }

        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { correctEndLine }, intersectedPiece);
        return;
    }

    private void CombineLinesFacingSameWay(AssemblyPiece piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We merge the two lines. The first part of loop detection happens in MergeLines
        AssemblyLine resultingLine = MergeLines(intersectedLine, correctEndLine, piece);
        HandleConnectingLines(resultingLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);
        //Now we have the next loop scenario, if one of the connecting lines that was added piece results in a loop.
        foreach(AssemblyLine line in resultingLine.GetAllConnections())
        {
            if(LoopDetected(line, resultingLine))
            {
                line.GetEndPiece().nextPiece = piece;
                resultingLine.RemoveConnection(line);
                assemblyLines.Add(line);
                break;
            }
        }
    }

    private AssemblyLine GetIntersectedAssemblyLine(AssemblyPiece piece, out AssemblyPiece intersectedPiece)
    {
        foreach(AssemblyLine line in assemblyLines)
        {
            AssemblyLine intersectedLine = RecursiveIntersectionSearch(line, piece, out intersectedPiece);
            if(intersectedLine != null)
            {
                return intersectedLine;
            }
        }
        intersectedPiece = null;
        return null;
    }
    private AssemblyLine RecursiveIntersectionSearch(AssemblyLine line, AssemblyPiece piece, out AssemblyPiece intersectedPiece)
    {
        //if the current line is intersected
        AssemblyLine intersectedLine = line.GetIntersectedAssemblyLine(piece.GetGridCoords() + piece.Movement(), out intersectedPiece);
        if(intersectedLine != null)
        {
            return intersectedLine;
        }
        //Checking this line's connecting lines:
        foreach(AssemblyLine connectingLine in line.GetAllConnections())
        {
            intersectedLine = RecursiveIntersectionSearch(connectingLine, piece, out intersectedPiece);
            if(intersectedLine != null)
            {
                return intersectedLine;
            }
        }

        return null;
    }

    private AssemblyLine CreateNewLine(AssemblyPiece piece)
    {
        AssemblyLine newLine = new AssemblyLine();
        newLine.AddPiece(piece);
        assemblyLines.Add(newLine);
        return newLine;
    }

    private AssemblyLine MergeLines(AssemblyLine startLine, AssemblyLine endLine, AssemblyPiece piece)
    {
        startLine.AddPiece(piece);
        startLine.GetStartPiece().previousPiece = endLine.GetEndPiece();
        endLine.GetEndPiece().nextPiece = startLine.GetStartPiece();

        List<AssemblyPiece> combinedPieces = new();
        combinedPieces.AddRange(endLine.AddAllPieces());
        combinedPieces.AddRange(startLine.AddAllPieces());
        //We create a new AssemblyLine using these pieces. We also need to make sure we're moving the connecting lines.

        
        AssemblyLine newLine = new AssemblyLine(combinedPieces);

        startLine.MoveConnectingLines(newLine);
        endLine.MoveConnectingLines(newLine);
        AssemblyLine nextLine = GetIntersectedAssemblyLine(startLine.GetEndPiece(), out AssemblyPiece i);
        if(LoopDetected(endLine, startLine))
        {
            nextLine.RemoveConnection(startLine);
            assemblyLines.Add(newLine);
        }
        else
        {
            nextLine.ReplaceExistingLine(startLine, newLine, i);
        }

        //We ensure the new lines are recorded and the old ones are disposed of.

        RemoveAssemblyLine(startLine);
        RemoveAssemblyLine(endLine);

        return newLine;
    }

    private void HandleConnectingLines(AssemblyLine mainLine, List<AssemblyLine> connectingLines, AssemblyPiece connectingPiece)
    {
        foreach(AssemblyLine line in connectingLines)
        {
            mainLine.AddConnectingAssemblyLine(line, connectingPiece);
            assemblyLines.Remove(line);
        }
    }
    private List<AssemblyLine> FindLinesPieceIsEndOf(AssemblyPiece piece)
    {
        //Linq returns an empty list if no lines are found and not null.
        return assemblyLines.Where(line => line.IsPieceNewEnd(piece)).ToList();
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, AssemblyPiece intersectedPiece, AssemblyPiece newPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().facing == intersectedPiece.facing && line.GetEndPiece().facing == newPiece.facing);
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, AssemblyPiece intersectedPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().facing == intersectedPiece.facing);
    }

    private bool LoopDetected(AssemblyLine correctEndLine, AssemblyLine intersectedLine)
    {
        HashSet<AssemblyLine> visited = new HashSet<AssemblyLine>();
        return CheckForLoop(correctEndLine, intersectedLine, visited);
    }

    private bool CheckForLoop(AssemblyLine currentLine, AssemblyLine targetLine, HashSet<AssemblyLine> visited)
    {
        if (currentLine == targetLine)
        {
            return true;
        }

        visited.Add(currentLine);

        foreach (var connectedLine in currentLine.GetAllConnections())
        {
            if (!visited.Contains(connectedLine) && CheckForLoop(connectedLine, targetLine, visited))
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveAssemblyLine(AssemblyLine line)
    {
        line.CleanUp();
        assemblyLines.Remove(line);
    }
}
