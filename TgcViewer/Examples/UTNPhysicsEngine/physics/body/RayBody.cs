using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Examples.UTNPhysicsEngine.optimizacion.octree;
using Examples.UTNPhysicsEngine.optimizacion.spatialHash;
using TgcViewer;

namespace Examples.UTNPhysicsEngine.physics.body
{
    public class RayBody : Body
    {
        public RayBody(TgcRay ray)
            : base(new Vector3(), ray.Direction * 7f, new Vector3(), 0f)
        {
        
        }
        
        public override void calculateAABB()
        {
            this.BoundingBox = new SpatialHashAABB();
        }

        public override void calculateInertiaBody()
        {
            this.Ibody = Matrix.Identity;
            this.InvIbody = Matrix.Invert(Ibody);            
        }       
        
    }
}
