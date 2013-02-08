using System;
using System.Collections.Generic;
using System.Text;
using Examples.UTNPhysicsEngine;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Examples.UTNPhysicsEngine.physics.body;
using Examples.UTNPhysicsEngine.math;

namespace Examples.UTNPhysicsEngine.physics
{
    class CollisionAlgoritms
    {
        /// <summary>
        /// Testeo de coliciones de todos los objetos del dominio.
        /// multimethod o multi dispach pathern, encontre que se puede implementar con .net4
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="insertionDistance"></param>
        /// <returns></returns>
        public static bool testCollision(Body x, Body y, out float insertionDistance)
        {
            dynamic a = x;
            dynamic b = y;
            return testCollision(a, b, out insertionDistance);
        }

        private static bool testCollision(SphereBody s, PlaneBody p, out float insertionDistance)
        {
            // For a normalized plane (|p.n| = 1), evaluating the plane equation
            // for a point gives the signed distance of the point to the plane
            insertionDistance = Vector3.Dot(s.Center, p.Normal) - FastMath.Abs(p.D);
            // If sphere center within +/-radius from plane, plane intersects sphere
            insertionDistance = FastMath.Abs(insertionDistance);
            if (insertionDistance <= s.Radius + s.margin)
            {
                insertionDistance -= s.Radius - s.margin;
                return true;
            }
            else
                return false;
        }

        private static bool testCollision(PlaneBody p, SphereBody s, out float insertionDistance)
        {
            return testCollision(s, p, out insertionDistance);
        }
        /// <summary>
        /// En este caso el testeo de colision devuelve un insertionDistance al cuadrado, por optimizacion.
        /// </summary>
        /// <param name="s1">Esfera 1</param>
        /// <param name="s2">Esfera 2</param>
        /// <param name="insertionDistance">distancia que se solapan al cuadrado</param>
        /// <returns>true si hay colision</returns>
        private static bool testCollision(SphereBody s1, SphereBody s2, out float insertionDistance)
        {
            // Calculate squared distance between centers
            Vector3 d = s1.Center - s2.Center;
            float dist2 = Vector3.Dot(d, d);
            // Spheres intersect if squared distance is less than squared sum of radii
            float radiusSum = s1.Radius + s2.Radius;

            insertionDistance = dist2;
            return dist2 <= radiusSum * radiusSum;            
        }

        private static bool testCollision(BoxBody b, SphereBody s, out float insertionDistance)
        {
            Examples.UTNPhysicsEngine.optimizacion.spatialHash.SpatialHashAABB box1 = b.BoundingBox;
            Examples.UTNPhysicsEngine.optimizacion.spatialHash.SpatialHashAABB box2 = s.BoundingBox;

            //Para optimizar la colision no usamos la insersionDistance para este algo.
            insertionDistance = 0f;

            //insertionDistance = GetSphereDistance(b, out pOnBox, out pOnSphere, sphereCenter, radius);
            if (((box1.aabbMin.X <= box2.aabbMin.X && box1.aabbMax.X >= box2.aabbMax.X) ||
               (box1.aabbMin.X >= box2.aabbMin.X && box1.aabbMin.X <= box2.aabbMax.X) ||
               (box1.aabbMax.X >= box2.aabbMin.X && box1.aabbMax.X <= box2.aabbMax.X)) &&
              ((box1.aabbMin.Y <= box2.aabbMin.Y && box1.aabbMax.Y >= box2.aabbMax.Y) ||
               (box1.aabbMin.Y >= box2.aabbMin.Y && box1.aabbMin.Y <= box2.aabbMax.Y) ||
               (box1.aabbMax.Y >= box2.aabbMin.Y && box1.aabbMax.Y <= box2.aabbMax.Y)) &&
              ((box1.aabbMin.Z <= box2.aabbMin.Z && box1.aabbMax.Z >= box2.aabbMax.Z) ||
               (box1.aabbMin.Z >= box2.aabbMin.Z && box1.aabbMin.Z <= box2.aabbMax.Z) ||
               (box1.aabbMax.Z >= box2.aabbMin.Z && box1.aabbMax.Z <= box2.aabbMax.Z)))
            {
                if ((box1.aabbMin.X <= box2.aabbMin.X) &&
                   (box1.aabbMin.Y <= box2.aabbMin.Y) &&
                   (box1.aabbMin.Z <= box2.aabbMin.Z) &&
                   (box1.aabbMax.X >= box2.aabbMax.X) &&
                   (box1.aabbMax.Y >= box2.aabbMax.Y) &&
                   (box1.aabbMax.Z >= box2.aabbMax.Z))
                {
                    return true;
                }
                else if ((box1.aabbMin.X > box2.aabbMin.X) &&
                         (box1.aabbMin.Y > box2.aabbMin.Y) &&
                         (box1.aabbMin.Z > box2.aabbMin.Z) &&
                         (box1.aabbMax.X < box2.aabbMax.X) &&
                         (box1.aabbMax.Y < box2.aabbMax.Y) &&
                         (box1.aabbMax.Z < box2.aabbMax.Z))
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }            
        }

        private static bool testCollision(SphereBody s, BoxBody b, out float insertionDistance)
        {
            return testCollision(b, s, out insertionDistance);
        }

        private static bool testCollision(BoxBody box, PlaneBody plane, out float insertionDistance)
        {
            //--- Transform box center and orientation into model frame of plane geometry
/*            CordinateSystem BtoP = box.wordCordSys().ConvertCordSysFrom(plane.wordCordSys());

            Vector3 c = box.Center;
            c = BtoP.xform_point(c);
            Matrix R = box.direction;
            R = BtoP.xform_matrix(R);

            //--- Find closest point between plane and box
            Vector3 n = plane.Normal;

            Vector3 i, j, k;
            i.X = R.M11; i.Y = R.M12; i.Z = R.M13;
            j.X = R.M21; j.Y = R.M22; j.Z = R.M23;
            k.X = R.M31; k.Y = R.M32; k.Z = R.M33;

            Vector3 p = c;
            Vector3 a = box.extendend;
            Vector3 delta;//--- keep signs so we know which corner of box that was the clostest point.

            if (Vector3.Dot(n, i) > 0)
            {
                p -= i * a.X;
                delta.X = -1;
            }
            else
            {
                p += i * a.X;
                delta.X = +1;
            }
            if (Vector3.Dot(n, j) > 0)
            {
                p -= j * a.Y;
                delta.Y = -1;
            }
            else
            {
                p += j * a.Y;
                delta.Y = +1;
            }
            if (Vector3.Dot(n, k) > 0)
            {
                p -= k * a.Z;
                delta.Z = -1;
            }
            else
            {
                p += k * a.Z;
                delta.Z = +1;
            }
            float w = plane.D;
            insertionDistance = Vector3.Dot(n, p) - w;

            //--- If closest points on box is longer away than epsilon, then there
            //--- is no chance for any contacts.
//            if(distance>envelope)
//                 return false;
            if (FastMath.Abs(insertionDistance) > 0.01f)
                return false;
            else
                return true;

*/
            // Compute the projection interval radius of b onto L(t) = b.c+t*p.n
            float r =   box.extendend.X * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M11, box.direction.M12, box.direction.M13))) +
                        box.extendend.Y * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M21, box.direction.M22, box.direction.M23))) +
                        box.extendend.Z * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M31, box.direction.M32, box.direction.M33)));
            // Compute distance of box center from plane
            float s = Vector3.Dot(plane.Normal, box.position) - FastMath.Abs(plane.D);
            // Intersection occurs when distance s falls within [-r,+r] interval
            insertionDistance = FastMath.Abs(s) - FastMath.Abs(r);
            return FastMath.Abs(s) <= FastMath.Abs(r);
        }

        private static bool testCollision(PlaneBody p, BoxBody b, out float insertionDistance)
        {
            return testCollision(b, p, out insertionDistance);
        }

        private static bool testCollision(BoxBody b1, BoxBody b2, out float insertionDistance)
        {
            insertionDistance = 0;
            return false;
        }

        /**
         * 
         * PARA PICKING
         * 
         **/
        public static bool testCollision(Body bodyPivot, TgcRay ray, out Vector3 point)
        {
            dynamic a = bodyPivot;
            return testCollision(a, ray, out point);
        }

        public static bool testCollision(SphereBody sphere, TgcRay ray, out Vector3 point)
        {
            point = new Vector3();
            Vector3 m = ray.Origin - sphere.Center;
            float b = Vector3.Dot(m, ray.Direction);
            float c = Vector3.Dot(m, m) - sphere.Radius * sphere.Radius;
            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f) 
                return false;
            float discr = b * b - c;
            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0.0f) 
                return false;
            // Ray now found to intersect sphere, compute smallest t value of intersection
            float t = -b - FastMath.Sqrt(discr);
            // If t is negative, ray started inside sphere so clamp t to zero
            if (t < 0.0f) t = 0.0f;
            point = ray.Origin + t * ray.Direction;
            return true;
        }

        public static bool testCollision(BoxBody box, TgcRay ray, out Vector3 point)
        {
            point = new Vector3();
            return false;
        }

        public static float GetSphereDistance(BoxBody boxShape, out Vector3 pointOnBox, out Vector3 pointOnSphere, Vector3 sphereCenter, float radius)
        {
            pointOnBox = new Vector3();
            pointOnSphere = new Vector3();

            float margins;
            Vector3[] bounds = new Vector3[2];            

            bounds[0] = -boxShape.extendend;
            bounds[1] = boxShape.extendend;

            margins = 0f;// boxShape.margin; //also add sphereShape margin?

            CordinateSystem boxWSCoord = boxShape.wordCordSys();

            Vector3[] boundsVec = new Vector3[2];
            float penetration;

            boundsVec[0] = bounds[0];
            boundsVec[1] = bounds[1];

            Vector3 marginsVec = new Vector3(margins, margins, margins);

            // add margins
            bounds[0] += marginsVec;
            bounds[1] -= marginsVec;

            /////////////////////////////////////////////////

            Vector3 prel, normal, v3P;
            Vector3[] n = new Vector3[6];
            float sep = float.PositiveInfinity, sepThis;

            n[0] = new Vector3(-1.0f, 0.0f, 0.0f);
            n[1] = new Vector3(0.0f, -1.0f, 0.0f);
            n[2] = new Vector3(0.0f, 0.0f, -1.0f);
            n[3] = new Vector3(1.0f, 0.0f, 0.0f);
            n[4] = new Vector3(0.0f, 1.0f, 0.0f);
            n[5] = new Vector3(0.0f, 0.0f, 1.0f);

            // convert  point in local space
            prel = boxWSCoord.toLocalCoordsPoint(sphereCenter);

            bool found = false;

            v3P = prel;

            for (int i = 0; i < 6; i++)
            {
                int j = i < 3 ? 0 : 1;
                if ((sepThis = (Vector3.Dot(v3P - bounds[j], n[i]))) > 0.0f)
                {
                    v3P = v3P - n[i] * sepThis;
                    found = true;
                }
            }

            //

            if (found)
            {
                bounds[0] = boundsVec[0];
                bounds[1] = boundsVec[1];

                normal = Vector3.Normalize(prel - v3P);
                pointOnBox = v3P + normal * margins;
                pointOnSphere = prel - normal * radius;

                if ((Vector3.Dot(pointOnSphere - pointOnBox, normal)) > 0.0f)
                {
                    return 1.0f;
                }

                // transform back in world space
                pointOnBox = boxWSCoord.fromLocalCoordPoint(pointOnBox);
                pointOnSphere = boxWSCoord.fromLocalCoordPoint(pointOnSphere);

                float seps2 = (pointOnBox - pointOnSphere).LengthSq();

                //if this fails, fallback into deeper penetration case, below
                if (seps2 > float.Epsilon)
                {
                    sep = -(float)Math.Sqrt(seps2);
                    normal = (pointOnBox - pointOnSphere);
                    normal *= 1f / sep;
                }
                return sep;
            }

            //////////////////////////////////////////////////
            // Deep penetration case

            penetration = GetSpherePenetration(boxShape, ref pointOnBox, ref pointOnSphere, sphereCenter, radius, bounds[0], bounds[1]);

            bounds[0] = boundsVec[0];
           bounds[1] = boundsVec[1];

            if (penetration <= 0.0f)
                return (penetration - margins);
            else
                return 1.0f;
        }

        public static float GetSpherePenetration(BoxBody boxObject, ref Vector3 pointOnBox, ref Vector3 pointOnSphere, Vector3 sphereCenter, float radius, Vector3 aabbMin, Vector3 aabbMax)
        {
            Vector3[] bounds = new Vector3[2];
            bounds[0] = aabbMin;
            bounds[1] = aabbMax;

            Vector3 p0 = new Vector3(), prel, normal = new Vector3();
            Vector3[] n = new Vector3[6];
            float sep = -10000000.0f, sepThis;

            n[0] = new Vector3(-1.0f, 0.0f, 0.0f);
            n[1] = new Vector3(0.0f, -1.0f, 0.0f);
            n[2] = new Vector3(0.0f, 0.0f, -1.0f);
            n[3] = new Vector3(1.0f, 0.0f, 0.0f);
            n[4] = new Vector3(0.0f, 1.0f, 0.0f);
            n[5] = new Vector3(0.0f, 0.0f, 1.0f);

            CordinateSystem boxWSCoord = boxObject.wordCordSys();

            // convert point in local space
            prel = boxWSCoord.toLocalCoordsPoint(sphereCenter);

            ///////////

            for (int i = 0; i < 6; i++)
            {
                int j = i < 3 ? 0 : 1;
                if ((sepThis = (Vector3.Dot(prel - bounds[j], n[i])) - radius) > 0.0f) return 1.0f;
                if (sepThis > sep)
                {
                    p0 = bounds[j];
                    normal = n[i];
                    sep = sepThis;
                }
            }

            pointOnBox = prel - normal * (Vector3.Dot(normal, (prel - p0)));
            pointOnSphere = pointOnBox + normal * sep;

            // transform back in world space
            pointOnBox = boxWSCoord.fromLocalCoordPoint(pointOnBox);
            pointOnSphere = boxWSCoord.fromLocalCoordPoint(pointOnSphere);
            normal = Vector3.Normalize(pointOnBox - pointOnSphere);

            return sep;
        }
    }
}
