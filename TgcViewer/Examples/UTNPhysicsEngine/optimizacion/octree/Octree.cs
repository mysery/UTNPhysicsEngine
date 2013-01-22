using System;
using System.Collections;
using System.Collections.Generic;
using  Examples.UTNPhysicsEngine.optimizacion.octree.Vector;
using Microsoft.DirectX;
using System.Text;
using TgcViewer.Utils;

namespace  Examples.UTNPhysicsEngine.optimizacion.octree
{

    /// <summary> The Octree lets you organize objects in a grid, that redefines
    /// itself and focuses more gridding when more objects appear in a
    /// certain area.
    /// </summary>
    [Serializable]
    public class Octree : IOctree
    {

        protected internal OctreeNode top;


        public Octree()
            : this(1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 20, OctreeNode.NO_MIN_SIZE)
        {
        }

        public Octree(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, int maxItems)
            : this(xMax, xMin, yMax, yMin, zMax, zMin, maxItems, OctreeNode.NO_MIN_SIZE)
        {
        }

        public Octree(int up, int left, int down, int right, int Front, int Back, int maxItems)
            : this((float)up, (float)left, (float)down, (float)right, (float)Front, (float)Back, maxItems, OctreeNode.DEFAULT_MIN_SIZE)
        {
        }

        public Octree(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, int maxItems, float minSize)
        {
            top = new OctreeNode(xMax, xMin, yMax, yMin, zMax, zMin, maxItems, minSize, null);
        }

        #region Add Node

        /// <summary> Add a object into the tree at a location.
        /// </summary>
        /// <param name="x">up-down location in Octree Grid</param>
        /// <param name="y">left-right location in Octree Grid</param>
        /// <param name="z">front-back location in Octree Grid</param>
        /// <returns> true if the insertion worked. </returns>
        public bool AddNode(float x, float y, float z, object obj)
        {
            return top.AddNode(x, y, z, obj);
        }
       
        public bool AddNode(Vector3f vector, object obj)
        {
            return top.AddNode(vector.x, vector.y, vector.z, obj);
        }
       
        public bool AddNode(Vector3 vector, Object obj)
        {
            return top.AddNode(vector.X, vector.Y, vector.Z, obj);
        }
        
        public bool AddNode(Vector3 vector, Object obj, OctreeBox aabb)
        {
            return top.AddNode(vector.X, vector.Y, vector.Z, obj, aabb);
        }

        #endregion

        #region Remove Node
        /// <summary> Remove a object out of the tree at a location. </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <returns> the object removed, null if the object not found.
        /// </returns>
        public object RemoveNode(float x, float y, float z, object obj)
        {
            return top.RemoveNode(x, y, z, obj);
        }
        public object RemoveNode(Vector3f vector, object obj)
        {
            return top.RemoveNode(vector.x, vector.y, vector.z, obj);
        }
        public object RemoveNode(Vector3 vector, object obj)
        {
            return top.RemoveNode(vector.X, vector.Y, vector.Z, obj);
        }
        public bool RemoveHashNode(Vector3 vector, object obj)
        {
            return top.RemoveHashNode(vector.X, vector.Y, vector.Z, obj);
        }
        #endregion

        #region Get Node

        /// <summary> Get an object closest to a x/y. </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <returns> the object that was found.</returns>
        public object GetNode(float x, float y, float z)
        {
            return top.GetNode(x, y, z);
        }
        public object GetNode(Vector3f vector)
        {
            return top.GetNode(vector.x, vector.y, vector.z);
        }
        public object GetNode(Vector3 vector)
        {
            return top.GetNode(vector.X, vector.Y, vector.Z);
        }

        /// <summary> Get an object closest to a x/y, within a maximum distance.
        /// 
        /// </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)
        /// </param>
        /// <param name="y">left-right location in Octree Grid (y, x)
        /// </param>
        /// <param name="withinDistance">the maximum distance to get a hit, in
        /// decimal degrees.
        /// </param>
        /// <returns> the object that was found, null if nothing is within
        /// the maximum distance.
        /// </returns>
        public object GetNode(float x, float y, float z, double withinDistance)
        {
            return top.GetNode(x, y, z, withinDistance);
        }
        public object GetNode(Vector3f vector, double withinDistance)
        {
            return top.GetNode(vector.x, vector.y, vector.z, withinDistance);
        }
        
        /// <summary> Get all the objects within a bounding box.
        /// 
        /// </summary>
        /// <param name="Top">top location in Octree Grid (x, y)
        /// </param>
        /// <param name="Left">left location in Octree Grid (y, x)
        /// </param>
        /// <param name="Bottom">lower location in Octree Grid (x, y)
        /// </param>
        /// <param name="Right">right location in Octree Grid (y, x)
        /// </param>
        /// <returns> ArrayList of objects.
        /// </returns>
        public ArrayList GetNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin)
        {
            return GetNode(xMax, xMin, yMax, yMin, zMax, zMin, ArrayList.Synchronized(new ArrayList(100)));
        }

        public ArrayList getNeighbors(OctreeBox ob)
        {
            return top.getNeighbors(ob, 
                                    top, 
                                    top.bounds.Right,
                                    top.bounds.Left,
                                    top.bounds.Front,
                                    top.bounds.Back,
                                    top.bounds.Top,
                                    top.bounds.Bottom,
                                    ArrayList.Synchronized(new ArrayList(100)));
        }

        public ArrayList GetNode(OctreeBox ob)
        {
            return GetNode(ob, ArrayList.Synchronized(new ArrayList(100)));
        }

        public ArrayList GetNode(OctreeBox ob, ArrayList nodes)
        {
            if (ob.Left > ob.Right || (Math.Abs(ob.Left - ob.Right) < 1e-6))
                return top.GetNode(ob, top.GetNode(ob.Right, 0, ob.Front, ob.Back, ob.Top, ob.Bottom, nodes));
            else
                return top.GetNode(ob, nodes);
        }
        
        /// <summary> Get all the objects within a bounding box, and return the
        /// objects within a given Vector.
        /// 
        /// </summary>
        /// <param name="Top">top location in Octree Grid (x, y)
        /// </param>
        /// <param name="Left">left location in Octree Grid (y, x)
        /// </param>
        /// <param name="Bottom">lower location in Octree Grid (x, y)
        /// </param>
        /// <param name="Right">right location in Octree Grid (y, x)
        /// </param>
        /// <param name="vector">a vector to add objects to.
        /// </param>
        /// <returns> ArrayList of objects.
        /// </returns>
        public ArrayList GetNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin, ArrayList nodes)
        {
            if (nodes == null)
                nodes = ArrayList.Synchronized(new ArrayList(10));
            
            if (xMin > xMax || (Math.Abs(xMin - xMax) < 1e-6))
                return top.GetNode(xMax, xMin, yMax, yMin, zMax, zMin, top.GetNode(xMax, 0, yMax, yMin, zMax, zMin, nodes));
            else
                return top.GetNode(xMax, xMin, yMax, yMin, zMax, zMin, nodes);
        }
        
        #endregion

        #region Get Nodes

        public ArrayList GetChildsNodes(Vector3 vector, float radius)
        {
            return top.GetChildsNodes(vector.X, vector.Y, vector.Z, radius);
        }
        

                /// <summary> Get an object closest to a x/y, within a maximum distance.
        /// 
        /// </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)
        /// </param>
        /// <param name="y">left-right location in Octree Grid (y, x)
        /// </param>
        /// <param name="withinDistance">the maximum distance to get a hit, in
        /// decimal degrees.
        /// </param>
        /// <returns> the objects that were found  within the maximum radius.
        /// </returns>
        public ArrayList GetNodes(float x, float y, float z, double radius)
        {
            return top.GetNodes(x, y, z, radius);
        }
        public ArrayList GetNodes(Vector3f vector, double radius)
        {
            return top.GetNodes(vector.x, vector.y, vector.z, radius);
        }
        public ArrayList GetNodes(Vector3 vector, float radius)
        {
            return top.GetNodes(vector.X, vector.Y, vector.Z, radius);
        }

        /// <summary> Get an object closest to a x/y, within a maximum distance./// </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <param name="withinDistance">the maximum distance to get a hit, in
        /// decimal degrees.</param>
        /// <returns> the objects that were found  within the maximum radius.</returns>
        public ArrayList GetNodes(float x, float y, float z, double MinRadius, double MaxRadius)
        {
            return top.GetNodes(x, y, z, MinRadius, MaxRadius);
        }
        public ArrayList GetNodes(Vector3f vector, double MinRadius, double MaxRadius)
        {
            return top.GetNodes(vector.x, vector.y, vector.z, MinRadius, MaxRadius);
        }

        public ArrayList GetNodes(Vector3 location, float MinRadius, float MaxRadius)
        {
            return top.GetNodes(location.X, location.Y, location.Z, MinRadius, MaxRadius);
        }

        #endregion

        /// <summary>Clear the tree. </summary>
        public void Clear()
        {
            top.Clear();
        }

        /// <summary>Clear the tree. </summary>
        private int renderCount = 0;
        public void render()
        {
            if (!(bool)TgcViewer.GuiController.Instance.Modifiers.getValue(Constant.debug))
            {
                top.render();
            }
            if (renderCount++ > 1000)
            {
                //printDebugOctree(top);
                renderCount = 0;
                top.deleteEmptyNodes(top.branch);
            }
        }
        

        public void dispose()
        {
            top.dispose();
        }

        /// <summary>
        /// Imprime por consola la generacion del Octree
        /// </summary>
        private void printDebugOctree(OctreeNode rootNode)
        {
            Logger.logInThread("########## Octree DEBUG ##########", System.Drawing.Color.Black);
            StringBuilder sb = new StringBuilder();
            doPrintDebugOctree(rootNode, 0, sb);
            Logger.logInThread(sb.ToString(), System.Drawing.Color.Black);
            Logger.logInThread("########## FIN Octree DEBUG ##########", System.Drawing.Color.Black);
        }

        /// <summary>
        /// Impresion recursiva
        /// </summary>
        private void doPrintDebugOctree(OctreeNode node, int index, StringBuilder sb)
        {
            if (node == null)
            {
                return;
            }

            String lineas = "";
            for (int i = 0; i < index; i++)
            {
                lineas += "-";
            }

            if (node.branch == null)
            {
                if (node.items.Count > 0)
                {
                    sb.Append(lineas + "Models [" + node.items.Count + "]" + "\n");                    
                }
                else
                {
                    sb.Append(lineas + "[0]" + "\n");
                }

            }
            else
            {
                sb.Append(lineas + "\n");
                index++;
                for (int i = 0; i < node.branch.Length; i++)
                {
                    doPrintDebugOctree(node.branch[i], index, sb);
                }
            }
        }
    }
}