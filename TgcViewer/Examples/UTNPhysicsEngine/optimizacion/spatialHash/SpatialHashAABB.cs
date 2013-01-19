using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.UTNPhysicsEngine.optimizacion.spatialHash
{
    public class SpatialHashAABB
    {
        public Microsoft.DirectX.Vector3 aabbMax;
        public Microsoft.DirectX.Vector3 aabbMin;

        public SpatialHashAABB(Microsoft.DirectX.Vector3 aabbMax, Microsoft.DirectX.Vector3 aabbMin)
        {
            this.aabbMax = aabbMax;
            this.aabbMin = aabbMin;
        }

        public SpatialHashAABB()
        {
            this.aabbMax = new Microsoft.DirectX.Vector3();
            this.aabbMin = new Microsoft.DirectX.Vector3();
        }
    }
}
