using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Examples.UTNPhysicsEngine.optimizacion.octree;
using Examples.UTNPhysicsEngine.optimizacion.spatialHash;

namespace Examples.UTNPhysicsEngine.physics.body
{
    class PlaneBody : Body
    {
        private Plane plane;
        private Vector3 normal;
        private Matrix direction;

        public PlaneBody(Plane plane, Vector3 position, Vector3 velocity, Vector3 aceleracion, float mass)
            : base(position, velocity, aceleracion, mass)
        {
            //this.restitution = 1f;
            this.plane = plane;
            this.normal = new Vector3(plane.A, plane.B, plane.C);
            this.normal.Normalize();
            //this.computeBase();
        }
        
        public override void calculateAABB()
        {
            this.BoundingBox = new SpatialHashAABB();
        }

        public override void calculateInertiaBody()
        {
            this.Ibody = Matrix.Identity;
            this.InvIbody = Matrix.Invert(Ibody);            
            //FIXME.
        }

        public Vector3 Normal { get {
            return normal;
        }
            set { normal = value; }
        }

        public float D { get{ return plane.D; } }

        
      private void computeBase()
      {
        Vector3 ty;
        Vector3 tx;
        //--- Use ty as temporary storage for computing a projection axe (which is temporarily stored in tx)
        Vector3  i = new Vector3(1f, 0f, 0f);
        Vector3  j = new Vector3(0f, 1f, 0f);
        Vector3  k = new Vector3(0f, 0f, 1f);

        ty = Vector3.Cross(normal,i);
        if(ty.LengthSq() > 0)
        {
          tx = i;
        }
        else
        {
          ty = Vector3.Cross(normal,j);
          if(ty.LengthSq() > 0)
          {
              tx = j;
          }
          else
          {
            ty = Vector3.Cross(normal,k);
            if(ty.LengthSq() > 0)
            {
                tx = k;
            }
            else
            {
              throw new Exception("No se encontro vector paralelo al eje normal");
            }
          }
        }
        
        ty = Vector3.Cross(normal,tx);
        ty.Normalize();
        tx = Vector3.Cross(ty,normal);
        tx.Normalize();

        this.direction = Matrix.Identity;
        this.direction.M11 = tx.X;
        this.direction.M12 = tx.Y;
        this.direction.M13 = tx.Z;
        this.direction.M21 = ty.X;
        this.direction.M22 = ty.Y;
        this.direction.M23 = ty.Z;
        this.quaternion = Quaternion.RotationMatrix(direction);
        this.quaternion.Normalize();
      }
    }
}
