using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Collections;

namespace Examples.UTNPhysicsEngine.optimizacion.spatialHash
{
    class SpatialHashBucket
    {
        public ArrayList items;

        public SpatialHashBucket()
        {
            this.items = new ArrayList();
        }

    }
}
