using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;

namespace TgcViewer.Utils.TgcGeometry
{
    /// <summary>
    /// Clase que representa el volumen del Frustum.
    /// Las normales de los planos del Frustum apuntan hacia adentro.
    /// </summary>
    public class TgcFrustum
    {
        Plane[] frustumPlanes = new Plane[6];
        /// <summary>
        /// Los 6 planos que componen el Frustum.
        /// Estan en el siguiente orden: 
        ///     Left, Right, Top, Bottom, Near, Far
        /// Estan normalizados.
        /// Sus normales hacia adentro.
        /// </summary>
        public Plane[] FrustumPlanes
        {
            get { return frustumPlanes; }
        }

        /// <summary>
        /// Tipos de planos del Frustum
        /// </summary>
        public enum PlaneTypes
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
            Near = 4,
            Far = 5,
        }

        /// <summary>
        /// Left plane
        /// </summary>
        public Plane LeftPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Left]; }
        }

        /// <summary>
        /// Right plane
        /// </summary>
        public Plane RightPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Right]; }
        }

        /// <summary>
        /// Top plane
        /// </summary>
        public Plane TopPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Top]; }
        }

        /// <summary>
        /// Bottom plane
        /// </summary>
        public Plane BottomPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Bottom]; }
        }

        /// <summary>
        /// Near plane
        /// </summary>
        public Plane NearPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Near]; }
        }

        /// <summary>
        /// Far plane
        /// </summary>
        public Plane FarPlane
        {
            get { return frustumPlanes[(int)PlaneTypes.Far]; }
        }


        public TgcFrustum()
        {
            frustumPlanes = new Plane[6];
        }


        /// <summary>
        /// Actualiza los planos que conforman el volumen del Frustum.
        /// Los planos se calculan con las normales apuntando hacia adentro
        /// </summary>
        /// <param name="viewMatrix">View matrix</param>
        /// <param name="projectionMatrix">Projection matrix</param>
        public void updateVolume(Matrix viewMatrix, Matrix projectionMatrix)
        {
            Matrix viewProjection = viewMatrix * projectionMatrix;

            
            //Left plane 
            frustumPlanes[0].A = viewProjection.M14 + viewProjection.M11;
            frustumPlanes[0].B = viewProjection.M24 + viewProjection.M21;
            frustumPlanes[0].C = viewProjection.M34 + viewProjection.M31;
            frustumPlanes[0].D = viewProjection.M44 + viewProjection.M41;

            //Right plane 
            frustumPlanes[1].A = viewProjection.M14 - viewProjection.M11;
            frustumPlanes[1].B = viewProjection.M24 - viewProjection.M21;
            frustumPlanes[1].C = viewProjection.M34 - viewProjection.M31;
            frustumPlanes[1].D = viewProjection.M44 - viewProjection.M41;

            //Top plane 
            frustumPlanes[2].A = viewProjection.M14 - viewProjection.M12;
            frustumPlanes[2].B = viewProjection.M24 - viewProjection.M22;
            frustumPlanes[2].C = viewProjection.M34 - viewProjection.M32;
            frustumPlanes[2].D = viewProjection.M44 - viewProjection.M42;

            //Bottom plane 
            frustumPlanes[3].A = viewProjection.M14 + viewProjection.M12;
            frustumPlanes[3].B = viewProjection.M24 + viewProjection.M22;
            frustumPlanes[3].C = viewProjection.M34 + viewProjection.M32;
            frustumPlanes[3].D = viewProjection.M44 + viewProjection.M42;

            //Near plane 
            frustumPlanes[4].A = viewProjection.M13;
            frustumPlanes[4].B = viewProjection.M23;
            frustumPlanes[4].C = viewProjection.M33;
            frustumPlanes[4].D = viewProjection.M43;

            //Far plane 
            frustumPlanes[5].A = viewProjection.M14 - viewProjection.M13;
            frustumPlanes[5].B = viewProjection.M24 - viewProjection.M23;
            frustumPlanes[5].C = viewProjection.M34 - viewProjection.M33;
            frustumPlanes[5].D = viewProjection.M44 - viewProjection.M43;
            

            //Normalize planes 
            for ( int i = 0; i < 6; i++ ) 
            {
                frustumPlanes[i] = Plane.Normalize( frustumPlanes[i] );
            }
        }


        /*
        float ANG2RAD = 3.14159265358979323846f / 180.0f;

        //Crear coreners del Frustum para poder dibujarlo
        private void computeFrustumCorners(Vector3 p, Vector3 l, float ratio, float nearD, float farD, float angle, Vector3 retUp)
        {
            float tang = (float)System.Math.Tan(ANG2RAD * angle * 0.5);
            float nh = nearD * tang;
            float nw = nh * ratio;
            float fh = farD * tang;
            float fw = fh * ratio;


            // compute the Z axis of camera
            // this axis points in the opposite direction from 
            // the looking direction
            Vector3 Z = Vector3.Subtract(p, l);
            Z.Normalize();

            // X axis of camera with given "up" vector and Z axis
            Vector3 X = Vector3.Cross(retUp, Z);
            X.Normalize();

            // the real "up" vector is the cross product of Z and X
            Vector3 Y = Vector3.Cross(Z, X);

            // compute the centers of the near and far planes
            Vector3 nc = p - Z * nearD;
            Vector3 fc = p - Z * farD;

            // compute the 4 corners of the frustum on the near plane
            Vector3 ntl = nc + Y * nh - X * nw;
            Vector3 ntr = nc + Y * nh + X * nw;
            Vector3 nbl = nc - Y * nh - X * nw;
            Vector3 nbr = nc - Y * nh + X * nw;

            // compute the 4 corners of the frustum on the far plane
            Vector3 ftl = fc + Y * fh - X * fw;
            Vector3 ftr = fc + Y * fh + X * fw;
            Vector3 fbl = fc - Y * fh - X * fw;
            Vector3 fbr = fc - Y * fh + X * fw;

        }
        */


    }
}
