using System;

namespace  Examples.UTNPhysicsEngine.optimizacion.octree
{
    /// <summary>
    /// x - left/right
    /// y - front/back
    /// z - top/bottom
    /// </summary>
    [Serializable]
    public class OctreeBox
    {

        private float top;
        private float bottom;
        private float left;
        private float right;
        private float front;
        private float back;

        public OctreeBox()
        {
            right = 0f;
            left = 0f;
            front = 0f;
            back = 0f;
            top = 0f;
            bottom = 0f;
        }
        public OctreeBox(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin)
        {
            right = xMax;
            left = xMin;
            front = yMax;
            back = yMin;
            top = zMax;
            bottom = zMin;
        }

        public bool within(OctreeBox Box)
        {
            return within(Box.Top, Box.Left, Box.Bottom, Box.Right, Box.Front, Box.Back);
        }

        public bool within(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin)
        {
            if (xMin >= Right ||
                xMax < Left ||
                yMin >= Front ||
                yMax < Back ||
                zMin >= Top ||
                zMax < Bottom)
                return false;

            return true;
        }

        public bool pointWithinBounds(float x, float y, float z)
        {
            OctreeBox box1 = new OctreeBox(x, x, y, y, z, z);
            return this.boundsWithinBounds(box1);
            /*if (x <= Right && 
                x > Left &&  
                y <= Front && 
                y > Back && 
                z <= Top && 
                z > Bottom)
                return true;
            else
                return false;*/
        }

        /// <summary>
        /// nos dice si un Occtreebox esta en contacto con este. Las opciones de clasificacion son:
        /// box1 el que se quiere clasificar y box2 es esta instancia.
        /// <para># Adentro: box1 se encuentra completamente dentro de la box2</para> true;
        /// <para># Afuera: box2 se encuentra completamente afuera de box1</para> false;
        /// <para># Atravesando: box2 posee una parte dentro de box1 y otra parte fuera de la box1</para> true;
        /// <para># Encerrando: box2 esta completamente adentro a la box1, es decir, la box2 se encuentra dentro
        ///     de la box1. Es un caso especial de que box2 esté afuera de box1</para> false;
        /// </summary>
        public bool boundsWithinBounds(OctreeBox box1)
        {
            if (((this.Left <= box1.Left && this.Right >= box1.Right) ||
                (this.Left >= box1.Left && this.Left <= box1.Right) ||
                (this.Right >= box1.Left && this.Right <= box1.Right)) &&
               ((this.Back <= box1.Back && this.Front >= box1.Front) ||
                (this.Back >= box1.Back && this.Back <= box1.Front) ||
                (this.Front >= box1.Back && this.Front <= box1.Front)) &&
               ((this.Bottom <= box1.Bottom && this.Top >= box1.Top) ||
                (this.Bottom >= box1.Bottom && this.Bottom <= box1.Top) ||
                (this.Top >= box1.Bottom && this.Top <= box1.Top)))
            {
                if ((this.Left <= box1.Left) &&
                   (this.Back <= box1.Back) &&
                   (this.Bottom <= box1.Bottom) &&
                   (this.Right >= box1.Right) &&
                   (this.Front >= box1.Front) &&
                   (this.Top >= box1.Top))
                {
                    return true;//BoxBoxResult.Adentro;
                }
                else if ((this.Left > box1.Left) &&
                         (this.Back > box1.Back) &&
                         (this.Bottom > box1.Bottom) &&
                         (this.Right < box1.Right) &&
                         (this.Front < box1.Front) &&
                         (this.Top < box1.Top))
                {
                    return false;//BoxBoxResult.Encerrando;
                }
                else
                {
                    return true;//BoxBoxResult.Atravesando;
                }
            }
            else
            {
                return false;//BoxBoxResult.Afuera;
            }
        }

        /// <summary> A utility method to figure out the closest distance of a border
        /// to a point. If the point is inside the rectangle, return 0.
        /// </summary>
        /// <param name="x">up-down location in Octree Grid (x, y)</param>
        /// <param name="y">left-right location in Octree Grid (y, x)</param>
        /// <returns> closest distance to the point. </returns>
        public double borderDistance(float x, float y, float z)
        {
            double nsdistance;
            double ewdistance;
            double fbdistance;

            if (Left <= x && x <= Right)
                ewdistance = 0;
            else
                ewdistance = Math.Min((Math.Abs(x - Right)), (Math.Abs(x - Left)));

            if (Front <= y && y <= Back)
                fbdistance = 0;
            else
                fbdistance = Math.Min(Math.Abs(y - Back), Math.Abs(y - Front));

            if (Bottom <= z && z <= Top)
                nsdistance = 0;
            else
                nsdistance = Math.Min(Math.Abs(z - Top), Math.Abs(z - Bottom));

            return Math.Sqrt(nsdistance * nsdistance +
                             ewdistance * ewdistance +
                             fbdistance * fbdistance);
        }


        /// <summary>
        /// Scalar Product
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static OctreeBox operator *(OctreeBox ob, float s)
        {
            return new OctreeBox(s * ob.Right,
                                s * ob.Left,
                                s * ob.Front,
                                s * ob.Back,
                                s * ob.Top,
                                s * ob.Bottom);            
        }


        public float Right
        {
            get { return this.right; }
            set { this.right = value; }
        }
        public float Left
        {
            get { return this.left; }
            set { this.left = value; }
        }
        public float Front
        {
            get { return this.front; }
            set { this.front = value; }
        }
        public float Back
        {
            get { return this.back; }
            set { this.back = value; }
        }
        public float Top
        {
            get { return this.top; }
            set { this.top = value; }
        }
        public float Bottom
        {
            get { return this.bottom; }
            set { this.bottom = value; }
        }


        internal OctreeBox setBounds(float xMax, float xMin, float yMax, float yMin, float zMax, float zMin)
        {
            right = xMax;
            left = xMin;
            front = yMax;
            back = yMin;
            top = zMax;
            bottom = zMin;
            return this;
        }
    }
}