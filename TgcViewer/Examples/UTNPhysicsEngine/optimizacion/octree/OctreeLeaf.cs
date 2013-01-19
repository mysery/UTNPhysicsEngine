using System;

namespace  Examples.UTNPhysicsEngine.optimizacion.octree
{

    [Serializable]
    public class OctreeLeaf
    {

        private float fx, fy, fz;
        private object objectValue;

        public OctreeLeaf(float x, float y, float z, object obj)
        {
            fx = x;
            fy = y;
            fz = z;
            objectValue = obj;
        }

        public object LeafObject
        {
            get
            {
                return objectValue;
            }
        }

        public float X
        {
            get
            {
                return fx;
            }
            set
            {
                fx = value; ;
            }
        }
        public float Y
        {
            get
            {
                return fy;
            }
            set
            {
                fy = value; ;
            }
        }
        public float Z
        {
            get
            {
                return fz;
            }
            set
            {
                fz = value; ;
            }
        }

    }
}