using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace Examples.UTNPhysicsEngine.math
{
    class CordinateSystem
    {
        public Quaternion quaternion;
        public Vector3 position;

        public CordinateSystem(Quaternion quaternion, Vector3 position)
        {
            this.quaternion = quaternion;
            this.quaternion.Normalize();
            this.position = position;
        }

        /**
        * Model Update Transform.
        * This method computes the necessary transform needed in
        * order to transform coordinates from one local frame
        * into another local frame. This utility is useful when
        * one wants to do model updates instead of world updates.
        *
        * In mathematical terms we have two transforms:
        *
        * C1 : H -> G
        * C2 : F -> G
        *
        * And we want to find
        *
        * C3 : H -> F
        *
        * This transform is computed and assigned to this coordinate
        * system.
        *
        * Very important: Note that this method finds the transform A -> B.
        */
        internal CordinateSystem ConvertCordSysFrom(CordinateSystem otherCordSys)
        {
            //---
            //---  p' = RA p + TA         (*1)  from A->WCS
            //---
            //---  p = RB^T (p' - TB)     (*2)  from WCS-B
            //---
            //--- Insert (*1) into (*2)  A -> B
            //---
            //---   p = RB^T ( RA p + TA - TB)
            //---     =  RB^T  RA p + RB^T (TA - TB)
            //--- So
            //---   R = RB^T  RA
            //---   T = RB^T (TA - TB)
            //---
            Quaternion q;
            if (this.quaternion.Equals(otherCordSys.quaternion))
            {
                q = Quaternion.RotationMatrix(Matrix.Identity);
            }
            else
            {
                q = Quaternion.Multiply(Quaternion.Conjugate(otherCordSys.quaternion), this.quaternion);
                q.Normalize();
            }
            //Rotate Vector by Quaternion.
            this.position = Vector3.TransformCoordinate(this.position-otherCordSys.position, Matrix.RotationQuaternion(quaternion));
            this.quaternion = q;
            return this;
        }

        /**
          * This method assumes that the point is in this coordinate system.
          * In other words this method maps local points into non local
          * points:
          *
          * BF -> WCS
          *
          * Let p be the point and let f designate the function of this
          * method then we have
          *
          * [p]_WCS = f(p)
          *
          */
        internal Vector3 fromLocalCoordPoint(Vector3 point)
        {
            Vector3 p = Vector3.TransformCoordinate(point, Matrix.Identity * Matrix.RotationQuaternion(this.quaternion) * Matrix.Translation(this.position));
            return p;
        }

        /**
        * This method assumes that the vector is in this
        * coordinate system. That is it maps the vector
        * from BF into WCS.
        */
        internal Vector3 fromLocalCoordVector(Vector3 vector)
        {
            return Vector3.TransformCoordinate(vector, Matrix.RotationQuaternion(this.quaternion));
        }

        internal Vector3 toLocalCoordsPoint(Vector3 point)
        {
            /*Matrix m1 = Matrix.RotationQuaternion(this.quaternion);
            Matrix m2 = Matrix.RotationQuaternion(Quaternion.Conjugate(this.quaternion));
            bool b = m2.Equals(Matrix.TransposeMatrix(m1));
            Matrix m3 = Matrix.Translation(-this.position);
            Vector3 v1 = Vector3.TransformCoordinate(point-this.position, Matrix.Identity);
            bool b2 = v1.Equals(Vector3.TransformCoordinate(point, m3));*/
            //v -= this.position;
            //m.Translate(new Vector3());
            return Vector3.TransformCoordinate(point, Matrix.Identity * Matrix.Translation(-this.position) * Matrix.RotationQuaternion(Quaternion.Conjugate(this.quaternion)));
        }

        internal Matrix fromLocalCoordMatrix(Matrix R)
        {
            return Matrix.Multiply(Matrix.RotationQuaternion(this.quaternion), R);
        }
    }
}
