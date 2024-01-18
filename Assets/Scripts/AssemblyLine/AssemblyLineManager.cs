using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssemblyLineManager
{
    private List<AssemblyLine> assemblyLines;
    private AssemblyLineSystem assemblyLineSystem;
    public AssemblyLineManager(List<AssemblyLine> assemblyLines, AssemblyLineSystem assemblyLineSystem)
    {
        this.assemblyLines = assemblyLines;
        this.assemblyLineSystem = assemblyLineSystem;
    }
    public void PlaceTransportablePiece(ITransportable newPiece)
    {
        if(newPiece != null)
        {
            AddPieceToAssemblyLine(newPiece);
        }
    }

    private void AddPieceToAssemblyLine(ITransportable piece)
    {
        ITransportable intersectedPiece;
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

    private void HandleEndConnections(ITransportable piece, List<AssemblyLine> endLines)
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

    private void HandleSingleIntersection(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine)
    {
        //The case where the new piece is just the new start of an intersected line.
        if (intersectedPiece.Facing == piece.Facing)
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

    private void HandleMultipleIntersections(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
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
            correctEndLine = FindCorrectEndLine(endLines, piece);
            //First case is that the two lines are facing each other. 
            if(correctEndLine != null && FacingExtentions.GetOppositeFacing(piece.Facing) == intersectedPiece.Facing)
            {
                endLines.Remove(correctEndLine);
                CombineOppositeFacingLines(piece, intersectedPiece, correctEndLine, intersectedLine, endLines);
            }
            //Second case is that the new piece is facing the same way as an end line, but not the intersected line.
            else if (correctEndLine != null)
            {
                endLines.Remove(correctEndLine);
                CombineEndLineFacingPiece(piece, intersectedPiece, intersectedLine, endLines, correctEndLine);
            }
            //However the new piece might still face the same way as the intersected line, without having a same facing end line.
            else if(piece.Facing == intersectedPiece.Facing)
            {
                CombineEndLinesWithIntersectedLine(piece, intersectedLine, endLines);
            }
            //In case they don't face the same way as the piece, and the piece doesn't face the same way as the line.
            else
            {
                CombineEndLinesFacingDifferentThanPiece(piece, intersectedPiece, intersectedLine, endLines);
            }
        }
    }
    private void CombineLinesFacingSameWay(ITransportable piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We merge the two lines. The first part of loop detection happens in MergeLines
        AssemblyLine resultingLine = MergeLines(intersectedLine, correctEndLine, piece);
        HandleConnectingLines(resultingLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);
        //Now we have the next loop scenario, if one of the connecting lines that was added piece results in a loop.
        foreach(AssemblyLine line in resultingLine.GetAllConnections())
        {
            if(LoopDetected(line, resultingLine))
            {
                resultingLine.RemoveConnection(line);
                assemblyLines.Add(line);
                GetIntersectedAssemblyLine(line.GetEndPiece(), out ITransportable i);
                resultingLine.HandleLoop(line, i);
                break;
            }
        }
    }
    private void CombineOppositeFacingLines(ITransportable piece, ITransportable intersectedPiece, AssemblyLine correctEndLine, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        correctEndLine.AddPiece(piece);
        //The two lines should become connecting lines of each other without removing from AssemblyLines.
        intersectedLine.AddConnectingAssemblyLine(correctEndLine, intersectedPiece);
        correctEndLine.AddConnectingAssemblyLine(intersectedLine, piece);
        //We add the endlines as connecting lines to the line with the piece being placed.
        HandleConnectingLines(correctEndLine, endLines, piece);
        //This can't possibly loop. 
    }
    private void CombineEndLineFacingPiece(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We add the piece to our end line.
        correctEndLine.AddPiece(piece);
        //And now we need to ensure that all endlines are connecting lines to this line.
        HandleConnectingLines(correctEndLine, endLines, piece);

        if(LoopDetected(correctEndLine, intersectedLine))
        {
            intersectedLine.HandleLoop(correctEndLine, intersectedPiece);
            return;
        }

        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { correctEndLine }, intersectedPiece);
        return;
    }
    private void CombineEndLinesWithIntersectedLine(ITransportable piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        intersectedLine.AddPiece(piece);
        //We add the endlines as connecting lines to this line
        AssemblyLine loopingEndLine = null;
        foreach(AssemblyLine line in endLines)
        {
            if(LoopDetected(line, intersectedLine))
            {
                loopingEndLine = line;
                intersectedLine.HandleLoop(line, piece);
                break;
            }
        }

        HandleConnectingLines(intersectedLine, endLines.Except(new[] {loopingEndLine}).ToList(), piece);
    }

    private void CombineEndLinesFacingDifferentThanPiece(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        AssemblyLine newLine = CreateNewLine(piece);
        //We add the endlines as connecting lines to this line
        HandleConnectingLines(newLine, endLines, piece);

        if(LoopDetected(newLine, intersectedLine))
        {
            intersectedLine.HandleLoop(newLine, intersectedPiece);
            return;
        }
        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { newLine }, intersectedPiece);
    }

    

    

    private AssemblyLine GetIntersectedAssemblyLine(ITransportable piece, out ITransportable intersectedPiece)
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
    private AssemblyLine RecursiveIntersectionSearch(AssemblyLine line, ITransportable piece, out ITransportable intersectedPiece)
    {
        //if the current line is intersected
        AssemblyLine intersectedLine = line.GetIntersectedAssemblyLine(piece.GetNextCellCoords(), out intersectedPiece);
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

    private AssemblyLine CreateNewLine(ITransportable piece)
    {
        AssemblyLine newLine = new AssemblyLine(assemblyLineSystem);
        newLine.AddPiece(piece);
        assemblyLines.Add(newLine);
        return newLine;
    }

    private AssemblyLine MergeLines(AssemblyLine startLine, AssemblyLine endLine, ITransportable piece)
    {
        // Add the connecting piece to the endLine (which will now be the start of the merged line).
        startLine.AddPiece(piece);
        //Moves all nodes from startLine to endLine
        startLine.AppendLine(endLine);
        //Moves all connecting lines from startLine to endLine
        startLine.MoveConnectingLines(endLine);

        AssemblyLine nextLine = GetIntersectedAssemblyLine(endLine.GetEndPiece(), out ITransportable i);
        if (nextLine != null)
        {
            if (LoopDetected(startLine, endLine))
            {
                assemblyLines.Add(nextLine);
            }
            nextLine.ReplaceExistingLine(startLine, endLine, i);
            assemblyLines.Remove(endLine);
        }
        RemoveAssemblyLine(startLine);
        return endLine;
    }

    

    private void HandleConnectingLines(AssemblyLine mainLine, List<AssemblyLine> connectingLines, ITransportable connectingPiece)
    {
        foreach(AssemblyLine line in connectingLines)
        {
            mainLine.AddConnectingAssemblyLine(line, connectingPiece);
            assemblyLines.Remove(line);
        }
    }
    private List<AssemblyLine> FindLinesPieceIsEndOf(ITransportable piece)
    {
        //Linq returns an empty list if no lines are found and not null.
        return assemblyLines.Where(line => line.IsPieceNewEnd(piece)).ToList();
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, ITransportable intersectedPiece, ITransportable newPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().Facing == intersectedPiece.Facing && line.GetEndPiece().Facing == newPiece.Facing);
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, ITransportable intersectedPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().Facing == intersectedPiece.Facing);
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
