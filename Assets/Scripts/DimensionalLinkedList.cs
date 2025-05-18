
using System.Collections.Generic;
using System.Numerics;
public class DimensionalLinkedList
{

    public DimensionalNode CurrentDimensionalNode;
    public List<LinkedList<DimensionalNode>> TransDimensionalLinkStorage;
    public int direction = 1;
    public DimensionalNode LastSplitPoint = null;
    public int CurrentTime = 0;
    
    public DimensionalLinkedList()
    {
        TransDimensionalLinkStorage = new List<LinkedList<DimensionalNode>>();
        TransDimensionalLinkStorage.Add(new LinkedList<DimensionalNode>()); // Initial time step
    }


    public void reverseDirection()
    {
        direction = (direction == 1) ? 0 : 1; // Toggle direction
        LastSplitPoint = CurrentDimensionalNode;

        // Adjust CurrentTime to stay within bounds
        if (direction == 0 && CurrentTime >= TransDimensionalLinkStorage.Count)
        {
            CurrentTime = TransDimensionalLinkStorage.Count - 1;
        }



        CurrentDimensionalNode = new DimensionalNode(
            null, 
            null, 
            LastSplitPoint,
            CurrentDimensionalNode.SameTimeNodes
        );
    }
    

    public void stepNode()
    {
        if (CurrentTime < 0)
        {
            CurrentTime = 0;
        }
        
        if (direction == 1)
        {
            if (CurrentDimensionalNode.ForwardDimensionalNode != null)
            {
                CurrentDimensionalNode = CurrentDimensionalNode.ForwardDimensionalNode;
            }
            else
            {
                CurrentDimensionalNode = new DimensionalNode(null,CurrentDimensionalNode,LastSplitPoint,null);
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
                        TransDimensionalLinkStorage[CurrentTime+ 1]
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
        

        

    }
    
    
}

