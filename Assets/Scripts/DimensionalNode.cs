
using System.Collections.Generic;
using System.Numerics;
public class DimensionalNode
{
    public DimensionalNode ForwardDimensionalNode;
    public DimensionalNode BackwardsDimensionalNode;
    public DimensionalNode SplitPoint;
    public LinkedList<DimensionalNode> SameTimeNodes;
    public MomentData data;
    
    public static List<DimensionalNode> Nodes = new List<DimensionalNode>();

    public DimensionalNode(DimensionalNode forwardDimensionalNode, DimensionalNode backwardsDimensionalNode , DimensionalNode splitPoint,LinkedList<DimensionalNode> sameTimeNodes )
    {
        ForwardDimensionalNode = forwardDimensionalNode;
        BackwardsDimensionalNode = backwardsDimensionalNode;
        SplitPoint = splitPoint;
        SameTimeNodes = sameTimeNodes;
        Nodes.Add(this);
    }
    
}