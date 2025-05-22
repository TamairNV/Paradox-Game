
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DimensionalLinkedList
{

    public DimensionalNode CurrentDimensionalNode;
    public List<LinkedList<DimensionalNode>> TransDimensionalLinkStorage;
    public int direction = 1;
    public DimensionalNode LastSplitPoint = null;
    public int CurrentTime = 1;
    [SerializeField] public LineRenderer line;
    private float x = 0;
    private float y = 0;
    public float xStep = 0.1f;
    public float yStep = 0.5f;
    public List<Vector3> linePositions = new List<Vector3>();
    
    public DimensionalLinkedList(float xStep, float yStep,LineRenderer line)
    {
        TransDimensionalLinkStorage = new List<LinkedList<DimensionalNode>>();
        TransDimensionalLinkStorage.Add(new LinkedList<DimensionalNode>()); // Initial time step
        this.xStep = xStep;
        this.yStep = yStep;
        this.line = line;
        

    }

    private int s = 0;
    public void reverseDirection()
    {
        direction = (direction == 1) ? 0 : 1; // Toggle direction
        LastSplitPoint = CurrentDimensionalNode;
        
        // Adjust CurrentTime to stay within bounds
        if (direction == 0 && CurrentTime >= TransDimensionalLinkStorage.Count)
        {
            CurrentTime = TransDimensionalLinkStorage.Count - 1;
        }


        DimensionalNode newNode  = new DimensionalNode(
            null, 
            null, 
            LastSplitPoint,
            CurrentDimensionalNode.SameTimeNodes,
            CurrentTime
        );

        CurrentDimensionalNode.DownNode = newNode;
        CurrentDimensionalNode = newNode;
 

        y -= yStep;
    }
    

    public void stepNode()
    {
        s ++ ;
        if (CurrentTime < 0)
        {
            CurrentTime = 0;
        }
        
        if (direction == 1)
        {
            x += xStep;
            if (CurrentDimensionalNode.ForwardDimensionalNode != null)
            {
                CurrentDimensionalNode = CurrentDimensionalNode.ForwardDimensionalNode;
            }
            else
            {
                CurrentDimensionalNode = new DimensionalNode(null,CurrentDimensionalNode,LastSplitPoint,null,CurrentTime);
                if (TransDimensionalLinkStorage.Count-1 >= CurrentTime && TransDimensionalLinkStorage[CurrentTime] != null)
                {
                    TransDimensionalLinkStorage[CurrentTime].AddLast(CurrentDimensionalNode);

                }
                else
                {
                    LinkedList<DimensionalNode> newLinkedList = new LinkedList<DimensionalNode>();
                    TransDimensionalLinkStorage.Add(newLinkedList);
   
                    newLinkedList.AddFirst(CurrentDimensionalNode);
                }
        
                CurrentDimensionalNode.SameTimeNodes = TransDimensionalLinkStorage[CurrentTime];
                CurrentTime++;
            }
        }

        if (direction == 0)
        {
            x -= xStep;
            if (CurrentTime <= 0) // Prevent underflow
            {
                return;
            }
            CurrentTime--; // Decrement AFTER checking bounds
            if (CurrentDimensionalNode.BackwardsDimensionalNode != null)
            {
                CurrentDimensionalNode = CurrentDimensionalNode.BackwardsDimensionalNode;
            }
            else
            {
                
                // Only create new nodes if within valid time range
                if (CurrentTime >= 0 && CurrentTime < TransDimensionalLinkStorage.Count)
                {
                    CurrentDimensionalNode = new DimensionalNode(
                        CurrentDimensionalNode, 
                        null, 
                        LastSplitPoint,
                        TransDimensionalLinkStorage[CurrentTime+ 1],
                        CurrentTime
                    );
                    TransDimensionalLinkStorage[CurrentTime].AddLast(CurrentDimensionalNode);
                }
                else
                {
                    // Handle out-of-bounds gracefully
                    return;
                }

            }
        }

        if (s == 2)
        {
            linePositions.Add(new Vector3(x,y,0));
            s = 0;
        }

        
        

    }
}

