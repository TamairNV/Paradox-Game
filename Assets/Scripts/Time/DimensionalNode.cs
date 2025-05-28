
using System.Collections.Generic;
using System.Numerics;
public class DimensionalNode
{
    public DimensionalNode ForwardDimensionalNode;
    public DimensionalNode BackwardsDimensionalNode;
    public DimensionalNode SplitPoint;
    public LinkedList<DimensionalNode> SameTimeNodes;
    public MomentData data;
    public DimensionalNode DownNode = null;
    public int Time;
    
    public static List<DimensionalNode> Nodes = new List<DimensionalNode>();

    public DimensionalNode(DimensionalNode forwardDimensionalNode, DimensionalNode backwardsDimensionalNode , DimensionalNode splitPoint,LinkedList<DimensionalNode> sameTimeNodes,int time )
    {
        ForwardDimensionalNode = forwardDimensionalNode;
        BackwardsDimensionalNode = backwardsDimensionalNode;
        SplitPoint = splitPoint;
        SameTimeNodes = sameTimeNodes;
        Time = time;
        Nodes.Add(this);
    }
    
}