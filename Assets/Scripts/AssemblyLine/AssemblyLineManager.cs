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
        AssemblyPiece intersectingPiece;
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out intersectingPiece);
        List<AssemblyLine> endLines = FindLinesPieceIsEndOf(piece);

        //Should cover all scenarios where we're combining two or more lines.
        if(intersectedLine != null && endLines.Count > 0)
        {
            //We start by considering the scenario where the two lines are facing the same way.
            AssemblyLine correctEndLine = FindCorrectEndLine(endLines, intersectingPiece);
            if(correctEndLine != null)
            {
                //We merge the two lines.
                AssemblyLine resultingLine = MergeLines(intersectedLine, correctEndLine, piece);
                HandleConnectingLines(resultingLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);

                //We could potentially loop here, and therefore we employ a special version of detect loop.
                LoopDetected(resultingLine, intersectedLine, resultingLine.GetAllConnections());
            }
            //Alright we assume the endlines do not face the same way as the intersected line. 
            else
            {
                //In which case we first need to see if the new piece is part of an existing endline.
                correctEndLine = FindCorrectEndLine(endLines, piece);
                if(correctEndLine != null)
                {
                    //We add the piece to our end line.
                    correctEndLine.AddPiece(piece);
                    //And now we need to ensure that all endlines are connecting lines to this line.
                    HandleConnectingLines(correctEndLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);
                    
                    //Here we need to perform loop detection
                    if(LoopDetected(correctEndLine, intersectedLine))
                    {
                        HandleLoop(piece, intersectingPiece);
                        return;
                    }
                    //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
                    HandleConnectingLines(intersectedLine, new List<AssemblyLine>{correctEndLine}, intersectingPiece);
                    return;
                }
                //In case they don't face the same way as the piece, but still intersect with the piece.
                else
                {
                    AssemblyLine newLine = CreateNewLine(piece);
                    //We add the endlines as connecting lines to this line
                    HandleConnectingLines(newLine, endLines, piece);
                    
                    //Here we need to perform loop detection
                    if(LoopDetected(newLine, intersectedLine))
                    {
                        HandleLoop(piece, intersectingPiece);
                        return;
                    }
                    //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
                    HandleConnectingLines(intersectedLine, new List<AssemblyLine>{newLine}, intersectingPiece);
                }
            }
        }
        else if(intersectedLine != null)
        {
            //The case where the new piece is just the new start of an intersected line.
            if(intersectingPiece.facing == piece.facing)
            {
                intersectedLine.AddPiece(piece);
            }
            //If the new piece is intersecting at the middle of a line without being the end of other lines, then it can't possibly create a loop.
            else
            {
                AssemblyLine newLine = CreateNewLine(piece);
                HandleConnectingLines(intersectedLine, new List<AssemblyLine>{newLine}, intersectingPiece);
            }
        }
        //If the new piece doesn't intersect any lines, it can't create a loop.
        else if(endLines.Count > 0)
        {
            AssemblyLine correctEndLine = FindCorrectEndLine(endLines, piece);
            if(correctEndLine != null)
            {
                correctEndLine.AddPiece(piece);
                HandleConnectingLines(correctEndLine, endLines.Except(new[]{correctEndLine}).ToList(), piece);
            }
            else
            {
                AssemblyLine newLine = CreateNewLine(piece);
                HandleConnectingLines(newLine, endLines, piece);
            }
        }
        else
        {
            CreateNewLine(piece);
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
        //We add the piece to startline, which also means it becomes the new start piece of that line.
        startLine.AddPiece(piece);
        //Now we need to align the two lines' gap.
 

        startLine.GetStartPiece().previousPiece = endLine.GetEndPiece();
        endLine.GetEndPiece().nextPiece = startLine.GetStartPiece();

        List<AssemblyPiece> combinedPieces = new();
        combinedPieces.AddRange(endLine.AddAllPieces());
        combinedPieces.AddRange(startLine.AddAllPieces());
        //We create a new AssemblyLine using these pieces. We also need to make sure we're moving the connecting lines.

        AssemblyLine newLine = new AssemblyLine(combinedPieces);

        startLine.MoveConnectingLines(newLine);
        endLine.MoveConnectingLines(newLine);

        //If the previous startline was a connecting line of another line then we also need to replace it with the new line.
        if(startLine.GetEndPiece().nextPiece != null)
        {
            GetIntersectedAssemblyLine(startLine.GetEndPiece(), out AssemblyPiece intersectedPiece).ReplaceExistingLine(startLine, newLine, intersectedPiece);
        }
        else
        {
            //If it wasn't a connecting line, then we need to add it to the list of assembly lines.
            assemblyLines.Add(newLine);
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

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, AssemblyPiece piece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().facing == piece.facing);
    }

    private bool LoopDetected(AssemblyLine currentLine, AssemblyLine intersectedLine, List<AssemblyLine> connectingLines = null)
    {
        if(connectingLines == null)
        {
            HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
            return HasLoop(currentLine, intersectedLine, visitedLines);
        }
        else
        {
            HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
            if(HasLoop(currentLine, intersectedLine, visitedLines))
            {
                //we need to see if visited lines contains any lines from connecting lines here
                foreach(AssemblyLine line in connectingLines)
                {
                    if(visitedLines.Contains(line))
                    {
                        line.RemoveConnection(currentLine);
                        return true;
                    }
                }
            }
            return false;
        }
    }

    private bool HasLoop(AssemblyLine currentLine, AssemblyLine intersectedLine, HashSet<AssemblyLine> visitedLines)
    {
        if(visitedLines.Contains(currentLine))
        {
            return false;
        }

        if(currentLine == intersectedLine)
        {
            return true;
        }

        visitedLines.Add(currentLine);
        foreach(var connectingLine in currentLine.GetAllConnections())
        {
            if(HasLoop(connectingLine, intersectedLine, visitedLines))
            {
                return true;
            }
        }

        return false;
    }
    private void HandleLoop(AssemblyPiece newPiece, AssemblyPiece intersectedPiece)
    {
        newPiece.nextPiece = intersectedPiece;
    }

    private void RemoveAssemblyLine(AssemblyLine line)
    {
        line.CleanUp();
        assemblyLines.Remove(line);
    }
}
