using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace Examples.UTNPhysicsEngine.optimizacion.spatialHash
{
    class SpatialHashKey
    {
        public long cell_x;
        public long cell_y;
        public long cell_z;
        public Vector3 cellSize;
        
        public SpatialHashKey(Vector3 _maxSize, float cellSize, Vector3 point)
        {
            this.cellSize = new Vector3(cellSize, cellSize, cellSize);
            this.cell_x = (long)(point.X / this.cellSize.X) + (long)_maxSize.X / 2;
            this.cell_y = (long)(point.Y / this.cellSize.Y) + (long)_maxSize.Y / 2;
            this.cell_z = (long)(point.Z / this.cellSize.Z) + (long)_maxSize.Z / 2;
        }

        public SpatialHashKey(Vector3 _maxSize, Vector3 cellSize, Vector3 point)
        {
            this.cellSize = cellSize;
            this.cell_x = (long)(point.X / this.cellSize.X) + (long)_maxSize.X / 2;
            this.cell_y = (long)(point.Y / this.cellSize.Y) + (long)_maxSize.Y / 2;
            this.cell_z = (long)(point.Z / this.cellSize.Z) + (long)_maxSize.Z / 2;
        }

        public SpatialHashKey(long cx, long cy, long cz)
        {
            this.cell_x = cx;
            this.cell_y = cy;
            this.cell_z = cz;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SpatialHashKey objCast = (SpatialHashKey)obj;
            return (cell_x.Equals(objCast.cell_x) &&
                    cell_y.Equals(objCast.cell_y) &&
                    cell_z.Equals(objCast.cell_z));
            
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return 
            this.cell_x.GetHashCode() +
            this.cell_y.GetHashCode() +
            this.cell_z.GetHashCode();
        }

    }
}
