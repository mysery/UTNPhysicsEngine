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
    public class SphereBody : Body
    {
        public float radius;
        public float margin = 0.04f; //Para tener un margen de error en las collisiones.

        public SphereBody(float radius, Vector3 position, Vector3 velocity, Vector3 aceleracion, float mass, bool applyGravity = true)
            : base(position, velocity, aceleracion, mass, applyGravity)
        {
            this.radius = radius;
            this.scaling = Matrix.Scaling(radius, radius, radius);

            this.calculateAABB();
        }

        public Vector3 Center
        {
            get { return position; }
        }

        public float Radius { get {return radius;} }


        public override void calculateInertiaBody()
        {
            float elem = 0.4f * mass * radius * radius; //Mass Moment of Inertia = 2/5*mass*r^2
            Matrix m = Matrix.Identity;
            m.M11 = elem;
            m.M22 = elem;
            m.M33 = elem;
            this.Ibody = m;
            this.InvIbody = Matrix.Invert(this.Ibody);
        }

        public override void calculateAABB()
        {
            //this.BoundingBox = new OctreeBox(position.X + radius, position.X - radius, position.Y + radius, position.Y - radius, position.Z + radius, position.Z - radius);
            Vector3 rv = new Vector3(radius, radius, radius);
            Vector3 aabbMin = position - rv;
            Vector3 aabbMax = position + rv;
            this.BoundingBox = new SpatialHashAABB(aabbMax, aabbMin);
        }

    }
}
