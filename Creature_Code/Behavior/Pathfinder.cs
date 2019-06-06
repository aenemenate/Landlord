using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class Pathfinder
    {
        public static List<Point> FindPath( Point worldIndex, int currentFloor, Point start, Point end )
        {
            List<Point> path = new List<Point>();
            if (!PointIsTraversable(end, worldIndex, currentFloor))
                return path;
            List<Node> openList = new List<Node>() { new Node( start, 0, 0 ) };
            List<Node> closedList = new List<Node>();

            bool CheckOpenList( Node node ) {
                foreach (Node otherNode in openList)
                    if (otherNode.Pos.Equals( node.Pos ))
                        if (otherNode.F <= node.F)
                            return true;
                return false;
            }

            bool CheckClosedList( Node node ) {
                foreach (Node otherNode in closedList)
                    if (otherNode.Pos.Equals( node.Pos ))
                        if (otherNode.F <= node.F)
                            return true;
                return false;
            }

            int count = 0;
            while (openList.Count > 0 || count > 500) {
                count++;
                Node currentNode = GetSmallestNode( openList );
                if (currentNode.Pos.Equals( new Point() ))
                    return path;
                List<Node> adjacentNodes = GetAdjacentNodes( currentNode, worldIndex, currentFloor);

                for (int i = adjacentNodes.Count - 1; i >= 0; i--)
                {
                    Node node = adjacentNodes[i];
                    if (node.Pos.Equals( end ))
                    {
                        Node pathNode = node;
                        while (pathNode.Pos.Equals( start ) == false)
                        {
                            path.Insert( 0, pathNode.Pos );
                            pathNode = pathNode.ParentNode;
                        }
                        return path;
                    } else
                    {
                        node.G = currentNode.G + node.Pos.DistFrom( currentNode.Pos );
                        node.H = node.Pos.DistFrom( end );

                        if (CheckOpenList( node ))
                            continue;

                        if (CheckClosedList( node ))
                            continue;

                        openList.Add( node );
                    }
                }
                closedList.Add( currentNode );
            }
            return path;
        }

        private static Node GetSmallestNode( List<Node> list )
        {
            Node smallestNode = new Node( new Point(), 100, 100 );
            foreach (Node node in list)
                if (node.F < smallestNode.F)
                    smallestNode = node;
            try {
                list.RemoveAt( list.FindIndex( n => n.Pos.Equals( smallestNode.Pos ) ) );
            } catch { }

            return smallestNode;
        }

        private static List<Node> GetAdjacentNodes( Node node, Point worldIndex, int currentFloor )
        {
            List<Node> adjacentNodes = new List<Node>();

            for (int i = Math.Max( 0, node.Pos.X - 1 ); i <= Math.Min( node.Pos.X + 1, Program.WorldMap[worldIndex.X, worldIndex.Y].Width - 1 ); i++)
                for (int j = Math.Max( 0, node.Pos.Y - 1 ); j <= Math.Min( node.Pos.Y + 1, Program.WorldMap[worldIndex.X, worldIndex.Y].Height - 1 ); j++)
                {
                    if (PointIsTraversable( new Point(i, j), worldIndex, currentFloor ) && new Point[i, j].Equals( node.Pos ) == false)
                        adjacentNodes.Add( new Node( new Point( i, j ), 0, 0, node ) );
                }

            return adjacentNodes;
        }

        private static bool DetermineIfCanReachEnd(Point end, Point worldIndex, int currentFloor)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;
            if (!blocks[end.X * width + end.Y].Solid)
                return true;
            else
            {
                List<Point> adjacentPoints = end.GetAdjacentWalkablePoints(worldIndex, currentFloor);
                if (adjacentPoints.Count > 0)
                {
                    Point closestPoint = new Point();
                    foreach (Point point in adjacentPoints)
                        if (point.DistFrom( Program.Player.Position ) < closestPoint.DistFrom( Program.Player.Position ))
                            closestPoint = point;
                    end = closestPoint;
                    return true;
                }
            }
            return false;

        }

        private static bool PointIsTraversable( Point point, Point worldIndex, int currentFloor ) {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;
            return !blocks[point.X * width + point.Y].Solid || blocks[point.X * width + point.Y] is Chest || blocks[point.X * width + point.Y] is Door;
        }
    }


    static class Riverfinder
    {
        public static List<Point> FindPath( Point start, Point end, float[,] heightMap ) {
            List<Point> path       = new List<Point>();
            List<Node>  openList   = new List<Node>() { new Node( start, 0, 0 ) };
            List<Node>  closedList = new List<Node>();

            Point startIndex = new Point(start.X / 100, start.Y / 100), 
                    endIndex = new Point( end.X / 100, end.Y / 100 ); //world map indexes
            Point currentPoint = new Point(start.X, start.Y);
            Point GetNextTempGoal()
            {
                Point currentIndex = new Point( currentPoint.X / 100, currentPoint.Y / 100 );
                Point tempIndex = new Point();
                if (currentIndex.Equals( endIndex ))
                    return end;
                for (int i = Math.Max(0, currentIndex.X - 1); i <= Math.Min( 4, currentIndex.X + 1 ); i++)
                    for (int j = Math.Max( 0, currentIndex.Y - 1 ); j <= Math.Min( 4, currentIndex.Y + 1 ); j++) {
                        if (new Point( i, j ).DistFrom( endIndex ) < tempIndex.DistFrom( endIndex ))
                            tempIndex = new Point( i, j );
                    }
                bool xDeltaNon0 = (tempIndex.X - currentIndex.X) == 1 || (tempIndex.X - currentIndex.X) == -1;
                bool yDeltaNon0 = (tempIndex.Y - currentIndex.Y) == 1 || (tempIndex.Y - currentIndex.Y) == -1;
                if (xDeltaNon0 && yDeltaNon0) {
                    if (Program.RNG.Next( 0, 100 ) < 50) xDeltaNon0 = false;
                    else yDeltaNon0 = false;
                }
                Point tempGoal = 
                    new Point( currentPoint.X + (( (tempIndex.X - currentIndex.X) == 1 ? 100 : -1) - currentPoint.X % 100) * (xDeltaNon0 ? 1 : 0),
                                currentPoint.Y + (( ( tempIndex.Y - currentIndex.Y ) == 1 ? 100 : -1) - currentPoint.Y % 100) * (yDeltaNon0 ? 1 : 0));
                Point returnGoal = new Point(tempGoal.X, tempGoal.Y);
                if (xDeltaNon0) {
                    for (int j = ( tempGoal.Y / 100 ) * 100 + 10; j < ( tempGoal.Y / 100 ) * 100 + 90; j++) {
                        if (new Point( tempGoal.X, j ).DistFrom( end ) + heightMap[tempGoal.X, j] < returnGoal.DistFrom( end ) + heightMap[end.X, end.Y] && false == heightMap[tempGoal.X, j] >= WorldMapGeneration.StoneCutoff)
                            returnGoal = new Point( tempGoal.X, j );
                    }
                } else if (yDeltaNon0) {
                    for (int i = ( tempGoal.X / 100 ) * 100 + 10; i < ( tempGoal.X / 100 ) * 100 + 90; i++) {
                        if (new Point( i, tempGoal.Y ).DistFrom( end ) + heightMap[i, tempGoal.Y] < returnGoal.DistFrom( end ) + heightMap[end.X, end.Y] && false == heightMap[i, tempGoal.Y] >= WorldMapGeneration.StoneCutoff)
                            returnGoal = new Point( i, tempGoal.Y);
                    }
                }
                
                return returnGoal;
            }

            bool CheckOpenList( Node node )
            {
                foreach (Node otherNode in openList)
                    if (otherNode.Pos.Equals( node.Pos ))
                        if (otherNode.F <= node.F)
                            return true;
                return false;
            }
            bool CheckClosedList( Node node )
            {
                foreach (Node otherNode in closedList)
                    if (otherNode.Pos.Equals( node.Pos ))
                        if (otherNode.F <= node.F)
                            return true;
                return false;
            }

            while (path.Exists(point => point.Equals(end)) == false)
            {
                Point nextGoal = GetNextTempGoal();
                while (openList.Count > 0)
                {
                    Node currentNode = GetSmallestNode( openList );
                    List<Node> adjacentNodes = GetAdjacentNodes( currentNode, new Point(currentPoint.X / 100, currentPoint.Y / 100) );
                    for (int i = adjacentNodes.Count - 1; i >= 0; i--)
                    {
                        Node node = adjacentNodes[i];
                        if (heightMap[node.Pos.X, node.Pos.Y] >= WorldMapGeneration.StoneCutoff && !( heightMap[currentNode.Pos.X, currentNode.Pos.Y] >= WorldMapGeneration.StoneCutoff ))
                            continue;
                        if (node.Pos.Equals( nextGoal ))
                        {
                            Node pathNode = node;
                            while (pathNode.Pos.Equals( currentPoint ) == false)
                            {
                                path.Insert( 0, pathNode.Pos );
                                pathNode = pathNode.ParentNode;
                            }
                            goto Finish;
                        } else
                        {
                            node.G = currentNode.G + node.Pos.DistFrom( currentNode.Pos )
                                + Math.Max( -2, Math.Min( 2, .3 * ( heightMap[node.Pos.X, node.Pos.Y] - heightMap[currentNode.Pos.X, currentNode.Pos.Y] ) ) );
                            node.H = Math.Abs( node.Pos.X - nextGoal.X ) + Math.Abs( node.Pos.Y - nextGoal.Y );
                            if (CheckOpenList( node ))
                                continue;

                            if (CheckClosedList( node ))
                                continue;

                            openList.Add( node );
                        }
                    }
                    closedList.Add( currentNode );
                }

                Finish:
                closedList = new List<Node>();
                openList = new List<Node>() { new Node( nextGoal, 0, 0 ) };
                currentPoint = new Point(nextGoal.X, nextGoal.Y);
            }
            return path;
        }

        private static Node GetSmallestNode( List<Node> list )
        {
            Node smallestNode = new Node( new Point(), 99999, 99999 );
            foreach (Node node in list)
                if (node.F < smallestNode.F)
                    smallestNode = node;

            list.RemoveAt( list.FindIndex( n => n.Pos.Equals( smallestNode.Pos ) ) );

            return smallestNode;
        }

        private static List<Node> GetAdjacentNodes( Node node, Point currentIndex )
        {
            List<Node> adjacentNodes = new List<Node>();
            if (node.Pos.X - 1 >= Math.Max( 0, currentIndex.X * 100 - 1 ))
                adjacentNodes.Add( new Node( new Point( node.Pos.X - 1, node.Pos.Y ), 0, 0, node ) );
            if (node.Pos.X + 1 <= Math.Min( 499, currentIndex.X * 100 + 100 ))
                adjacentNodes.Add( new Node( new Point( node.Pos.X + 1, node.Pos.Y ), 0, 0, node ) );
            if (node.Pos.Y - 1 >= Math.Max( 0, currentIndex.Y * 100 - 1 ))
                adjacentNodes.Add( new Node( new Point( node.Pos.X, node.Pos.Y - 1 ), 0, 0, node ) );
            if (node.Pos.Y + 1 <= Math.Min( 499, currentIndex.Y * 100 + 100 ))
                adjacentNodes.Add( new Node( new Point( node.Pos.X, node.Pos.Y + 1 ), 0, 0, node ) );
            return adjacentNodes;
        }
    }

    class Node
    {
        private Point pos;
        private double g, h;
        private Node parentNode;

        public Node(Point pos, int g, int h, Node parentNode = null)
        {
            this.pos = pos;
            this.g = g;
            this.parentNode = parentNode;
        }


        public Point Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        public double G
        {
            get { return g; }
            set { g = value; }
        }

        public double H
        {
            get { return h; }
            set { h = value; }
        }

        public double F { get { return g + h; } }

        public Node ParentNode
        {
            get { return parentNode; }
            set { parentNode = value; }
        }
    }
}
