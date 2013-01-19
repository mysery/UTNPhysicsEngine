using System;
using System.Collections;
using Examples.UTNPhysicsEngine.optimizacion.octree.Vector;

namespace  Examples.UTNPhysicsEngine.optimizacion.octree
{
    /// <summary>
    /// This interface organizes the access to Octree class.
    /// </summary>
    public interface IOctree
    {

        /// <summary> Add a object into the organizer at a location.</summary>
        /// <param name="x">up-down location x/y/z</param>
        /// <param name="y">left-right location x/y/z</param>
        /// <param name="z">front-back location x/y/z</param>
        /// <returns> true if the insertion worked. </returns>
        bool AddNode(float x, float y, float z, object obj);
        
        bool AddNode(Vector3f vector, object obj);
        
        /// <summary> Remove a object out of the organizer at a location.</summary>
        /// <param name="x">up-down location (x, y)</param>
        /// <param name="y">left-right location (y, x)</param>
        /// <returns> the object removed, null if the object not found.</returns>
        object RemoveNode(float x, float y, float z, object obj);
      
        object RemoveNode(Vector3f vector, object obj);
      
        /// <summary>Clear the octree.</summary>
        void Clear();

        /// <summary> Find an object closest to point x/y/z.</summary>
        /// <param name="x">up-down location in Octree grid</param>
        /// <param name="y">left-right location in Octree grid</param>
        /// <param name="z">front-back location in Octree grid</param>
        /// <returns> the object that is closest to point x/y/z.</returns>
        object GetNode(float x, float y, float z);
        object GetNode(Vector3f vector);
        
        /// <summary> Find an object closest to point x/y/z within a distance.</summary>
        /// <param name="x">up-down location in Octree grid</param>
        /// <param name="y">left-right location in Octree grid</param>
        /// <param name="z">front-back location in Octree grid</param>
        /// <param name="withinDistance">maximum distance to have a hit.</param>
        /// <returns> the object that is closest to the x/y/z, within the given distance.</returns>
        object GetNode(float x, float y, float z, double withinDistance);
        object GetNode(Vector3f vector, double withinDistance);
        
        /// <summary> Find an set of objects closest to point x/y/z within a given radius.</summary>
        /// <param name="x">up-down location in Octree grid</param>
        /// <param name="y">left-right location in Octree grid</param>
        /// <param name="z">front-back location in Octree grid</param>
        /// <param name="radius">search radius</param>
        /// <returns> the object that is closest to point x/y/z, within the given distance.</returns>
        ArrayList GetNodes(float x, float y, float z, double radius);
        ArrayList GetNodes(Vector3f vector, double radius);
        
        ArrayList GetNodes(float x, float y, float z, double MinRadius, double MaxRadius);
        ArrayList GetNodes(Vector3f vector, double MinRadius, double MaxRadius);
        
        // <summary> Find an object closest to point x/y/z, within a cube.</summary>
        ArrayList GetNode(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin);
    }
}