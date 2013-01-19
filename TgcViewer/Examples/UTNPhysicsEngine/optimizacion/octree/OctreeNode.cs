using System;
using System.Collections;
using  Examples.UTNPhysicsEngine.optimizacion.octree.Vector;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Collections.Generic;
using Examples.UTNPhysicsEngine.physics.body;

namespace  Examples.UTNPhysicsEngine.optimizacion.octree
{

    /// <summary> The OctreeNode is the part of the Octree that either holds
    /// branch nodes, or objects as leaves. Currently, the nodes that
    /// have branch do not hold items that span across branch
    /// boundaries, since this was designed to handle point data.
    /// </summary>

    [Serializable]
    public class OctreeNode : IOctree
    {
 
        public const float NO_MIN_SIZE = -1;
        public const float DEFAULT_MIN_SIZE = 10;

        protected internal OctreeNode parent;
        protected internal Dictionary<int, List<OctreeNode>> nodeMap;

        protected internal ArrayList items;
        protected internal OctreeNode[] branch;
        protected internal int maxItems;
        protected internal float minSize;

        public OctreeBox bounds;
        /// <summary> Added to avoid problems when a node is completely filled with a
        /// single point value.
        /// </summary>
        protected internal bool allTheSamePoint;
        protected internal float firstX;
        protected internal float firstY;
        protected internal float firstZ;

        protected internal bool allTheSameBounds;
        protected internal OctreeBox firstBounds;

        private TgcBoundingBox bondingBox;

        /// <summary> Constructor to use if you are going to store the objects in
        /// x/y space, and there is really no smallest node size.</summary>
        /// <param name="Top">northern border of node coverage.</param>
        /// <param name="Left">western border of node coverage.</param>
        /// <param name="Bottom">southern border of node coverage.</param>
        /// <param name="Right">eastern border of node coverage.</param>
        /// <param name="maximumItems">number of items to hold in a node before
        /// splitting itself into four branch and redispensing the
        /// items into them.</param>
        public OctreeNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, int maximumItems, OctreeNode parent)
            : this(xMax, xMin, yMax, yMin, zMax, zMin, maximumItems, NO_MIN_SIZE, parent)
        {
        }

        /// <summary> Constructor to use if you are going to store the objects in x/y
        /// space, and there is a smallest node size because you don't want
        /// the nodes to be smaller than a group of pixels.</summary>
        /// <param name="Top">northern border of node coverage.</param>
        /// <param name="Left">western border of node coverage.</param>
        /// <param name="Bottom">southern border of node coverage.</param>
        /// <param name="Right">eastern border of node coverage.</param>
        /// <param name="maximumItems">number of items to hold in a node before
        /// splitting itself into four branch and redispensing the items into them.</param>
        /// <param name="minimumSize">the minimum difference between the boundaries of the node.</param>
        public OctreeNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, int maximumItems, float minimumSize, OctreeNode parent)
        {
            bounds = new OctreeBox(xMax, xMin, yMax, yMin, zMax, zMin);
            bondingBox = new TgcBoundingBox(new Vector3(xMin, yMin, zMin), new Vector3(xMax, yMax, zMax));
            maxItems = maximumItems;
            minSize = minimumSize;
            items = ArrayList.Synchronized(new ArrayList(10));
            nodeMap = new Dictionary<int, List<OctreeNode>>(50);
            this.parent = parent;
        }

        /// <summary>Return true if the node has branch. </summary>
        public bool hasChildren()
        {
            if (branch != null)
                return true;
            else
                return false;
        }

        protected internal bool branching()
        {
           // Make sure we're bigger than the minimum, if we care,
            if (minSize != NO_MIN_SIZE)
                if (Math.Abs(bounds.Top - bounds.Bottom) < minSize &&
                    Math.Abs(bounds.Right - bounds.Left) < minSize &&
                    Math.Abs(bounds.Front - bounds.Back) < minSize)
                    return false;

            float nsHalf = (float)(bounds.Top - (bounds.Top - bounds.Bottom) * 0.5);
            float ewHalf = (float)(bounds.Right - (bounds.Right - bounds.Left) * 0.5);
            float fbHalf = (float)(bounds.Front - (bounds.Front - bounds.Back) * 0.5);

            branch = new OctreeNode[8];

            branch[0] = new OctreeNode(ewHalf, bounds.Left, bounds.Front, fbHalf, bounds.Top, nsHalf, maxItems, minSize, this); //left-front-top
            branch[1] = new OctreeNode(bounds.Right, ewHalf, bounds.Front, fbHalf, bounds.Top, nsHalf, maxItems, minSize, this);
            branch[2] = new OctreeNode(ewHalf, bounds.Left, bounds.Front, fbHalf, nsHalf, bounds.Bottom, maxItems, minSize, this);
            branch[3] = new OctreeNode(bounds.Right, ewHalf, bounds.Front, fbHalf, nsHalf, bounds.Bottom, maxItems, minSize, this);

            branch[4] = new OctreeNode(ewHalf, bounds.Left, fbHalf, bounds.Back, bounds.Top, nsHalf, maxItems, minSize, this); //left-back-top
            branch[5] = new OctreeNode(bounds.Right, ewHalf, fbHalf, bounds.Back, bounds.Top, nsHalf, maxItems, minSize, this);
            branch[6] = new OctreeNode(ewHalf, bounds.Left, fbHalf, bounds.Back, nsHalf, bounds.Bottom, maxItems, minSize, this);
            branch[7] = new OctreeNode(bounds.Right, ewHalf, fbHalf, bounds.Back, nsHalf, bounds.Bottom, maxItems, minSize, this);

            return true;
        }
        /// <summary> This method splits the node into four branch, and disperses
        /// the items into the branch. The split only happens if the
        /// boundary size of the node is larger than the minimum size (if
        /// we care). The items in this node are cleared after they are put
        /// into the branch.
        /// </summary>
        protected internal void split()
        {
            if (!branching())
                return;

            ArrayList temp = (ArrayList)items.Clone();
            items.Clear();
            IEnumerator things = temp.GetEnumerator();
            while (things.MoveNext())
            {
                if (!(this.getChild((Body)((OctreeLeaf)things.Current).LeafObject)).Remove(this))
                    throw new Exception("whoa");
                AddNode((OctreeLeaf)things.Current);
            }
        }
        /// <summary> This method splits the node into four branch, and disperses
        /// the items into the branch. The split only happens if the
        /// boundary size of the node is larger than the minimum size (if
        /// we care). The items in this node are cleared after they are put
        /// into the branch.
        /// </summary>
        protected internal void split(OctreeBox aabb)
        {
             if (!branching())
                return;

            ArrayList temp = (ArrayList)items.Clone();
            items.Clear();
            IEnumerator things = temp.GetEnumerator();
            while (things.MoveNext())
            {
                if (!(this.getChild((Body)((OctreeLeaf)things.Current).LeafObject)).Remove(this))
                     throw new Exception("whoa");
                
                AddNode((OctreeLeaf)things.Current, aabb);
            }
        }
        

        /// <summary> Get the node that covers a certain x/y pair.</summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <returns> node if child covers the point, null if the point is
        /// out of range.</returns>
        protected internal OctreeNode getChild(float x, float y, float z)
        {
            if (bounds.pointWithinBounds(x, y, z))
            {
                if (branch != null)
                {
                    for (int i = 0; i < branch.Length; i++)
                        if (branch[i].bounds.pointWithinBounds(x, y, z))
                            return branch[i].getChild(x, y, z);

                }
                else
                    return this;
            }
            return null;
        }

        /// <summary> obtiene los nodos que contienen a un octreebox.</summary>
        /// <param name="aabb">Octree box</param>
        /// <param name="nodes">Lista syncronica de nodos.</param>
        /// <returns> node if child covers the point, lista vacia si esta fuera.</returns>
        protected internal ArrayList getChild(OctreeBox aabb, ArrayList nodes)
        {
            if (bounds.boundsWithinBounds(aabb))
            {
                if (branch != null)
                {
                    for (int i = 0; i < branch.Length; i++)
                    {
                        if (branch[i].bounds.boundsWithinBounds(aabb))
                            branch[i].getChild(aabb, nodes);
                    }
                } else
                    nodes.Add(this);
            }
            return nodes;
        }

        /// <summary>rapidamente.</summary>
        /// <param name="aabb">Octree box</param>
        /// <param name="nodes">Lista syncronica de nodos.</param>
        /// <returns> node if child covers the point, lista vacia si esta fuera.</returns>
        public List<OctreeNode> getChild(Body body)
        {
            OctreeNode tmpNode = this;
            while (tmpNode.parent != null)
            {
                tmpNode = tmpNode.parent;
            }

            if (tmpNode.nodeMap.ContainsKey(body.idCode))
                return tmpNode.nodeMap[body.idCode];
            else
                return new List<OctreeNode>();            
        }

        #region Add Node

        /// <summary>rapidamente.</summary>
        /// <param name="aabb">Octree box</param>
        /// <param name="nodes">Lista syncronica de nodos.</param>
        /// <returns> node if child covers the point, lista vacia si esta fuera.</returns>
        public void addHashChild(Body body, OctreeNode node)
        {
            OctreeNode tmpNode = this;
            while (tmpNode.parent != null)
            {
                tmpNode = tmpNode.parent;
            }

            if (tmpNode.nodeMap.ContainsKey(body.idCode))
                tmpNode.nodeMap[body.idCode].Add(node);
            else
            {
                tmpNode.nodeMap.Add(body.idCode, new List<OctreeNode>());
                tmpNode.nodeMap[body.idCode].Add(node);
            }
        }

        /// <summary> Add a object into the tree at a location.</summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <param name="obj">object to add to the tree.</param>
        /// <returns> true if the pution worked.</returns>
        public bool AddNode(float x, float y, float z, object obj)
        {
            return AddNode(new OctreeLeaf(x, y, z, obj));
        }
        public bool AddNode(Vector3f vector, object obj)
        {
            return AddNode(new OctreeLeaf(vector.x, vector.y, vector.z, obj));
        }
        public bool AddNode(float x, float y, float z, object obj, OctreeBox aabb)
        {
            return AddNode(new OctreeLeaf(x, y, z, obj), aabb);
        }

        /// <summary> Add a OctreeLeaf into the tree at a location.</summary>
        /// <param name="leaf">object-location composite</param>
        /// <returns> true if the pution worked.</returns>
        public bool AddNode(OctreeLeaf leaf)
        {
            if (branch == null)
            {
                this.addHashChild((Body)leaf.LeafObject, this);
                this.items.Add(leaf);
                if (this.items.Count == 1)
                {
                    this.allTheSamePoint = true;
                    this.firstX = leaf.X;
                    this.firstY = leaf.Y;
                    this.firstZ = leaf.Z;
                }
                else
                {
                    if (this.firstX != leaf.X || this.firstY != leaf.Y || this.firstZ != leaf.Z)
                    {
                        this.allTheSamePoint = false;
                    }
                }

                if (this.items.Count > maxItems && !this.allTheSamePoint)
                    split();
                return true;
            }
            else
            {
                OctreeNode node = getChild(leaf.X, leaf.Y, leaf.Z);
                if (node != null)
                {
                    return node.AddNode(leaf);
                }
            }
            return false;
        }

        /// <summary> Add a OctreeLeaf into the tree at a location.</summary>
        /// <param name="leaf">object-location composite</param>
        /// <returns> true if the pution worked.</returns>
        public bool AddNode(OctreeLeaf leaf, OctreeBox aabb)
        {
            if (branch == null)
            {
                this.addHashChild((Body)leaf.LeafObject, this);
                this.items.Add(leaf);
                if (this.items.Count == 1)
                {
                    this.allTheSameBounds = true;
                    this.firstBounds = aabb;
                }
                else
                {
                    if (!firstBounds.boundsWithinBounds(aabb))
                    {
                        this.allTheSameBounds = false;
                    }
                }

                if (this.items.Count > maxItems && !this.allTheSameBounds)
                    split(aabb);
                return true;
            }
            else
            {
                ArrayList nodes = getChild(aabb, ArrayList.Synchronized(new ArrayList(10)));
                bool added = false;
                foreach (OctreeNode oneLeaf in nodes)
                {
                    added = oneLeaf.AddNode(leaf, aabb) || added;
                }
                return added;
            }
        }

        #endregion

        #region Remove Node

        /// <summary> Remove a object out of the tree at a location.
        /// 
        /// </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)

        /// <param name="y">left-right location in Octree Grid (y, x)

        /// <returns> the object removed, null if the object not found.
        /// </returns>
        public object RemoveNode(float x, float y, float z, object obj)
        {
            return RemoveNode(new OctreeLeaf(x, y, z, obj));
        }
        
        public object RemoveNode(Vector3f vector, object obj)
        {
            return RemoveNode(new OctreeLeaf(vector.x, vector.y, vector.z, obj));
        }


        /// <summary> Remove a OctreeLeaf out of the tree at a location.
        /// 
        /// </summary>
        /// <param name="leaf">object-location composite

        /// <returns> the object removed, null if the object not found.
        /// </returns>
        public object RemoveNode(OctreeLeaf leaf)
        {
            if (branch == null)
            {
                // This must be the node that has it...
                for (int i = 0; i < items.Count; i++)
                {
                    OctreeLeaf qtl = (OctreeLeaf)items[i];
                    if (leaf.LeafObject == qtl.LeafObject)
                    {
                        items.RemoveAt(i);
                        return qtl.LeafObject;
                    }
                }
            }
            else
            {
                OctreeNode node = getChild(leaf.X, leaf.Y, leaf.Z);
                if (node != null)
                {
                    return node.RemoveNode(leaf);
                }
            }
            return null;
        }

        public bool RemoveHashNode(float x, float y, float z, object obj)
        {
            return RemoveHashNode(new OctreeLeaf(x, y, z, obj));
        }

        public bool RemoveHashNode(Vector3f vector, object obj)
        {
            return RemoveHashNode(new OctreeLeaf(vector.x, vector.y, vector.z, obj));
        }

        /// <summary> Remove a OctreeLeaf out of the tree at a location.
        /// 
        /// </summary>
        /// <param name="leaf">object-location composite

        /// <returns> the object removed, null if the object not found.
        /// </returns>
        public bool RemoveHashNode(OctreeLeaf leaf)
        {
            List<OctreeNode> nodes = getChild((Body)leaf.LeafObject);
            ArrayList cloneNodes = new ArrayList();
            cloneNodes.AddRange(nodes);
            bool sucess = false;
            foreach (OctreeNode node in cloneNodes)
            {
                if (node.branch == null)
                {// This must be the node that has it...
                    for (int i = 0; i < node.items.Count; i++)
                    {
                        OctreeLeaf qtl = (OctreeLeaf)node.items[i];
                        if (leaf.LeafObject == qtl.LeafObject)
                        {
                            node.items.RemoveAt(i);
                            sucess = true;
                        }
                    }
                }
                //else
                //{
                sucess = node.RemoveHashNode((Body)leaf.LeafObject) || sucess;
                //}
            }
            return sucess;
        }

        /// <summary>rapidamente.</summary>
        /// <param name="aabb">Octree box</param>
        /// <param name="nodes">Lista syncronica de nodos.</param>
        /// <returns> node if child covers the point, lista vacia si esta fuera.</returns>
        public bool RemoveHashNode(Body body)
        {
            OctreeNode tmpNode = this;
            while (tmpNode.parent != null)
            {
                tmpNode = tmpNode.parent;
            }

            if (tmpNode.nodeMap.ContainsKey(body.idCode))
                return tmpNode.nodeMap[body.idCode].Remove(this);
            
            return false;
        }

        #endregion

        #region Get Node

        /// <summary> Get an object closest to a x/y.</summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <returns> the object that matches the best distance, null if no objects were found. </returns>
        public object GetNode(float x, float y, float z)
        {
            return GetNode(x, y, z, Double.PositiveInfinity);
        }
        public object GetNode(Vector3f vector)
        {
            return GetNode(vector.x, vector.y, vector.z, Double.PositiveInfinity);
        }

        /// <summary> Get an object closest to a x/y/z. If there are branches at
        /// this node, then the branches are searched. The branches are
        /// checked first, to see if they are closer than the best distance
        /// already found. If a closer object is found, bestDistance will
        /// be updated with a new Double object that has the new distance.</summary>
        /// <param name="x">left-right location in Octree Grid</param>
        /// <param name="y">up-down location in Octree Grid</param>
        /// <param name="z">front-nack location in Octree Grid</param>
        /// <param name="bestDistance">the closest distance of the object found so far.</param>
        /// <returns> the object that matches the best distance, null if no closer objects were found.</returns>
        public object GetNode(float x, float y, float z, double ShortestDistance)
        {
            double distance;
            object closest = null;
            if (branch == null)
            {
                foreach (OctreeLeaf leaf in this.items)
                {
                    distance = Math.Sqrt(
                                Math.Pow(x - leaf.X, 2.0) +
                                Math.Pow(y - leaf.Y, 2.0) +
                                Math.Pow(z - leaf.Z, 2.0));

                    if (distance < ShortestDistance)
                    {
                        ShortestDistance = distance;
                        closest = leaf.LeafObject;
                    }
                }
                return closest;
            }
            else
            {
                // Check the distance of the bounds of the branch,
                // versus the bestDistance. If there is a boundary that
                // is closer, then it is possible that another node has an
                // object that is closer.
                for (int i = 0; i < branch.Length; i++)
                {
                    double childDistance = branch[i].bounds.borderDistance(x, y, z);
                    if (childDistance < ShortestDistance)
                    {
                        object test = branch[i].GetNode(x, y, z, ShortestDistance);
                        if (test != null)
                            closest = test;
                    }
                }
            }
            return closest;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="ShortestDistance"></param>
        /// <returns></returns>
        public object GetNode(Vector3f vector, double ShortestDistance)
        {
            return GetNode(vector.x, vector.y, vector.z, ShortestDistance);
        }

        /// <summary> Get all the objects within a bounding box.</summary>
        /// <param name="Top">top location in Octree Grid (x, y)</param>
        /// <param name="Left">left location in Octree Grid (y, x)</param>
        /// <param name="Bottom">lower location in Octree Grid (x, y)</param>
        /// <param name="Right">right location in Octree Grid (y, x)</param>
        /// <returns> Vector of objects. </returns>
        public ArrayList GetNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin)
        {
            return GetNode(new OctreeBox(xMax, xMin, yMax, yMin, zMax, zMin), ArrayList.Synchronized(new ArrayList(10)));
        }

        /// <summary> Get all the objects within a bounding box.</summary>
        /// <param name="Top">top location in Octree Grid (x, y)</param>
        /// <param name="Left">left location in Octree Grid (y, x)</param>
        /// <param name="Bottom">lower location in Octree Grid (x, y)</param>
        /// <param name="Right">right location in Octree Grid (y, x)</param>
        /// <param name="vector">current vector of objects.</param>
        /// <returns> Vector of objects. </returns>
        public ArrayList GetNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, ArrayList nodes)
        {
            return GetNode(new OctreeBox(xMax, xMin, yMax, yMin, zMax, zMin), nodes);
        }

        /// <summary> Get all the objects within a bounding box.</summary>
        /// <param name="rect">boundary of area to fill.</param>
        /// <param name="vector">current vector of objects.</param>
        /// <returns> updated Vector of objects.</returns>
        public ArrayList GetNode(OctreeBox rect, ArrayList nodes)
        {
            if (branch == null)
            {
                IEnumerator things = this.items.GetEnumerator();
                while (things.MoveNext())
                {
                    OctreeLeaf qtl = (OctreeLeaf)things.Current;
                    if (rect.pointWithinBounds(qtl.X, qtl.Y, qtl.Z))
                        nodes.Add(qtl.LeafObject);
                }
            }
            else
            {
                for (int i = 0; i < branch.Length; i++)
                {
                    if (branch[i].bounds.boundsWithinBounds(rect))
                        branch[i].GetNode(rect, nodes);
                }
            }
            return nodes;
        }

        #endregion

        #region Get Nodes

        public ArrayList GetChildsNodes(float x, float y, float z, float radius)
        {
            ArrayList Nodes = new ArrayList();
            if (branch == null)
            {
                foreach (OctreeLeaf leaf in this.items)
                {
                    Nodes.Add(leaf.LeafObject);
                }
                
                return Nodes;
            }
            else
            {
                OctreeNode node = getChild(x, y, y);
                if (node != null)
                {
                    if (node.parent != null)
                    {
                        Nodes.AddRange(node.parent.GetNodes(x, y, z, radius));
                    }
                }
            }

            return Nodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public ArrayList GetNodes(float x, float y, float z, double radius)
        {
            ArrayList Nodes = new ArrayList();
            double distance;
            if (branch == null)
            {
                foreach (OctreeLeaf leaf in this.items)
                {
                    distance = Math.Sqrt(
                                Math.Pow(x - leaf.X, 2.0) +
                                Math.Pow(y - leaf.Y, 2.0) +
                                Math.Pow(z - leaf.Z, 2.0));

                    if (distance < radius)
                        Nodes.Add(leaf.LeafObject);

                }
                return Nodes;
            }
            else
            {
                // Check the distance of the bounds of the branch,
                // versus the bestDistance. If there is a boundary that
                // is closer, then it is possible that another node has an
                // object that is closer.
                for (int i = 0; i < branch.Length; i++)
                {
                    double childDistance = branch[i].bounds.borderDistance(x, y, z);

                    if (childDistance < radius)
                    {
                        object test = branch[i].GetNode(x, y, z, radius);
                        if (test != null)
                            Nodes.Add(test);
                    }

                }
            }
            return Nodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public ArrayList GetNodes(Vector3f vector, double radius)
        {
            return GetNodes(vector.x, vector.y, vector.z, radius);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="MinRadius"></param>
        /// <param name="MaxRadius"></param>
        /// <returns></returns>
        public ArrayList GetNodes(float x, float y, float z, double MinRadius, double MaxRadius)
        {
            ArrayList Nodes = new ArrayList();
            double distance = 1e32;
            if (branch == null)
            {
                foreach (OctreeLeaf leaf in this.items)
                {
                    distance = Math.Sqrt(
                                Math.Pow(x - leaf.X, 2.0) +
                                Math.Pow(y - leaf.Y, 2.0) +
                                Math.Pow(z - leaf.Z, 2.0));

                    if (distance >= MinRadius && distance < MaxRadius)
                    {
                        //if (distance <= minDistance) //closest object first
                        //{
                       //     Nodes.Insert(0, leaf.LeafObject);
                       //     minDistance = Math.Min(minDistance, distance);
                       // }
                       // else
                            Nodes.Add(leaf.LeafObject);
                    }

                }
                return Nodes;
            }
            else
            {
                // Check the distance of the bounds of the branch,
                // versus the bestDistance. If there is a boundary that
                // is closer, then it is possible that another node has an
                // object that is closer.
                for (int i = 0; i < branch.Length; i++)
                {
                    double childDistance = branch[i].bounds.borderDistance(x, y, z);

                    if (childDistance > MinRadius && childDistance <= MaxRadius)
                    {
                        object test = branch[i].GetNode(x, y, z, MinRadius);
                        if (test != null)
                            Nodes.Add(test);
                    }

                }
            }
            return Nodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="MinRadius"></param>
        /// <param name="MaxRadius"></param>
        /// <returns></returns>
        public ArrayList GetNodes(Vector3f vector, double MinRadius, double MaxRadius)
        {
            return GetNodes(vector.x, vector.y, vector.z, MinRadius, MaxRadius);
        }

        #endregion

        /// <summary>Clear the tree below this node. </summary>
        public void Clear()
        {
            this.items.Clear();
            if (branch != null)
            {
                for (int i = 0; i < branch.Length; i++)
                {
                    branch[i].Clear();
                }
                branch = null;
            }
        }

        public void render()
        {
            bondingBox.render();
            if (hasChildren())
                foreach (OctreeNode octreeNode in branch)
                {
                    octreeNode.render();
                }
        }

        internal void dispose()
        {
            bondingBox.dispose();
            if (hasChildren())
                foreach (OctreeNode octreeNode in branch)
                {
                    octreeNode.dispose();
                }
        }

        #region Optimizacion
        /// <summary>
        /// Se quitan padres cuyos nodos no tengan ningun triangulo
        /// </summary>
        public void deleteEmptyNodes(OctreeNode[] children)
        {
            if (children == null)
            {
                return;
            }

            for (int i = 0; i < children.Length; i++)
            {
                OctreeNode childNode = children[i];
                OctreeNode[] childNodeChildren = childNode.branch;
                if (childNodeChildren != null && hasEmptyChilds(childNode))
                {
                    childNode.branch = null;                    
                }
                else
                {
                    deleteEmptyNodes(childNodeChildren);
                }
            }
        }

        /// <summary>
        /// Se fija si los hijos de un nodo no tienen mas hijos y no tienen ningun triangulo
        /// </summary>
        private bool hasEmptyChilds(OctreeNode node)
        {
            OctreeNode[] children = node.branch;
            for (int i = 0; i < children.Length; i++)
            {
                OctreeNode childNode = children[i];
                if (childNode.branch != null || childNode.items.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion


        /// <summary>
        /// Recorrer recursivamente el Octree para encontrar los nodos visibles
        /// </summary>
        public ArrayList getNeighbors(OctreeBox aabb, OctreeNode node,
            float xMax, float xMin, float yMax, float yMin, float zMax, float zMin,
            ArrayList bodys)
        {
            OctreeNode[] children = node.branch;

            //es hoja, cargar todos los meshes
            if (children == null)
            {
                getNeighborsBody(node, bodys);
            }

            //recursividad sobre hijos
            else
            {
                float ewHalf = (float)(xMax - (xMax - xMin) * 0.5);
                float fbHalf = (float)(yMax - (yMax - yMin) * 0.5);
                float nsHalf = (float)(zMax - (zMax - zMin) * 0.5);

                getChildNeighbors(aabb, children[0], ewHalf, xMin, yMax, fbHalf, zMax, nsHalf, bodys);
                getChildNeighbors(aabb, children[1], xMax, ewHalf, yMax, fbHalf, zMax, nsHalf, bodys);
                getChildNeighbors(aabb, children[2], ewHalf, xMin, yMax, fbHalf, nsHalf, zMin, bodys);
                getChildNeighbors(aabb, children[3], xMax, ewHalf, yMax, fbHalf, nsHalf, zMin, bodys);

                getChildNeighbors(aabb, children[4], ewHalf, xMin, fbHalf, yMin, zMax, nsHalf, bodys);
                getChildNeighbors(aabb, children[5], xMax, ewHalf, fbHalf, yMin, zMax, nsHalf, bodys);
                getChildNeighbors(aabb, children[6], ewHalf, xMin, fbHalf, yMin, nsHalf, zMin, bodys);
                getChildNeighbors(aabb, children[7], xMax, ewHalf, fbHalf, yMin, nsHalf, zMin, bodys);
                
            }

            return bodys;
        }

        private OctreeBox tempOctree = new OctreeBox();
        /// <summary>
        /// Hacer visible las meshes de un nodo si es visible por el Frustum
        /// </summary>
        private void getChildNeighbors(OctreeBox aabb, OctreeNode childNode,
                float xMax, float xMin, float yMax, float yMin, float zMax, float zMin,
                ArrayList bodys)
        {
            if (childNode == null)
            {
                return;
            }

            if (tempOctree.setBounds(xMax, xMin, yMax, yMin, zMax, zMin).boundsWithinBounds(aabb))
            {
                getNeighbors(aabb, childNode, xMax, xMin, yMax, yMin, zMax, zMin, bodys);
            }            
        }

        /// <summary>
        /// Hacer visibles todas las meshes de un nodo
        /// </summary>
        private void getNeighborsBody(OctreeNode node, ArrayList bodys)
        {
            foreach (OctreeLeaf leaf in node.items)
            {
                bodys.Add(leaf.LeafObject);
            }
        }

    }
}