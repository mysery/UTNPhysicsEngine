using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Examples.UTNPhysicsEngine.optimizacion.octree;
using Examples.UTNPhysicsEngine.optimizacion.spatialHash;

namespace Examples.UTNPhysicsEngine.physics.body
{
    class BoxBody : Body
    {
        public float margin = 0.004f; //Para tener un margen de error en las collisiones.
        public Vector3 localCenter;
        public Matrix direction; // Local xyz en local space, M(1,:) en Eje X ; M(2,:) en Eje Y ; M(3,:) en Eje Z. conforma una base.
        public Vector3 extendend; // Positive halfwidth extents of OBB along each axis
        // Region R = { x|x=c+r*u[0]+s*u[1]+t*u[2] }, |r|<=e[0], |s|<=e[1], |t|<=e[2]

        public BoxBody(Matrix direction, Vector3 extendend, Vector3 position, Vector3 velocity, Vector3 aceleracion, float mass)
            : base(position, velocity, aceleracion, mass)
        {
            Vector3 u0 = new Vector3(direction.M11, direction.M12, direction.M13);
            u0.Normalize();
            Vector3 u1 = new Vector3(direction.M21, direction.M22, direction.M23);
            u1.Normalize();
            Vector3 u2 = new Vector3(direction.M31, direction.M32, direction.M33);
            u2.Normalize();
            this.direction = Matrix.Identity;
            this.direction.M11 = u0.X;
            this.direction.M12 = u0.Y;
            this.direction.M13 = u0.Z;
            this.direction.M21 = u1.X;
            this.direction.M22 = u1.Y;
            this.direction.M23 = u1.Z;
            this.direction.M31 = u2.X;
            this.direction.M32 = u2.Y;
            this.direction.M33 = u2.Z;
            this.quaternion = Quaternion.RotationMatrix(direction);
            this.quaternion.Normalize();
            this.extendend = extendend;
            this.scaling = Matrix.Scaling(extendend.X * 2f, extendend.Y * 2f, extendend.Z * 2f);
            this.localCenter = new Vector3();
        }

        public Vector3 Center
        {
            get { return localCenter; }
        }

        public override void integrateVelocitySI(float timeStep)
        {
            base.integrateVelocitySI(timeStep);
            direction = Matrix.RotationQuaternion(quaternion);
        }
        
        public override void calculateInertiaBody()
        {
            float lx = 2f * (extendend.X);
            float ly = 2f * (extendend.Y);
            float lz = 2f * (extendend.Z);

            Matrix m = Matrix.Identity;
            m.M11 = mass / (12.0f) * (ly * ly + lz * lz);
            m.M22 = mass / (12.0f) * (lx * lx + lz * lz);
            m.M33 = mass / (12.0f) * (lx * lx + ly * ly);
            
            this.Ibody = m;
            this.InvIbody = Matrix.Invert(this.Ibody);
        }

        public override void calculateAABB()
        {
            Vector3 row1 = new Vector3(FastMath.Abs(direction.M11), FastMath.Abs(direction.M12), FastMath.Abs(direction.M13));
            Vector3 row2 = new Vector3(FastMath.Abs(direction.M21), FastMath.Abs(direction.M22), FastMath.Abs(direction.M23));
            Vector3 row3 = new Vector3(FastMath.Abs(direction.M31), FastMath.Abs(direction.M32), FastMath.Abs(direction.M33));
            Vector3 extent = new Vector3(Vector3.Dot(row1, extendend),
                                         Vector3.Dot(row2, extendend),
                                         Vector3.Dot(row3, extendend));
            extent += new Vector3(margin, margin, margin);

            Vector3 aabbMin = position - extent; //en WordSpace.
            Vector3 aabbMax = position + extent; //en WordSpace.

            this.BoundingBox = new SpatialHashAABB(aabbMax, aabbMin);
        }

        public void getAABB(out Vector3 aabbMin, out Vector3 aabbMax)
        {
            Vector3 row1 = new Vector3(FastMath.Abs(direction.M11), FastMath.Abs(direction.M12), FastMath.Abs(direction.M13));
            Vector3 row2 = new Vector3(FastMath.Abs(direction.M21), FastMath.Abs(direction.M22), FastMath.Abs(direction.M23));
            Vector3 row3 = new Vector3(FastMath.Abs(direction.M31), FastMath.Abs(direction.M32), FastMath.Abs(direction.M33));
            Vector3 extent = new Vector3(Vector3.Dot(row1, extendend),
                                         Vector3.Dot(row2, extendend),
                                         Vector3.Dot(row3, extendend));
            extent += new Vector3(margin, margin, margin);

            aabbMin = position - extent; //en WordSpace.
            aabbMax = position + extent; //en WordSpace.
        }

        public float getEnvelope()
        {
            Vector3 row1 = new Vector3(FastMath.Abs(direction.M11), FastMath.Abs(direction.M12), FastMath.Abs(direction.M13));
            Vector3 row2 = new Vector3(FastMath.Abs(direction.M21), FastMath.Abs(direction.M22), FastMath.Abs(direction.M23));
            Vector3 row3 = new Vector3(FastMath.Abs(direction.M31), FastMath.Abs(direction.M32), FastMath.Abs(direction.M33));
            Vector3 extent = new Vector3(Vector3.Dot(row1, extendend),
                                         Vector3.Dot(row2, extendend),
                                         Vector3.Dot(row3, extendend));
            extent += new Vector3(margin, margin, margin);
            return extent.Length();
        }
    }
}
