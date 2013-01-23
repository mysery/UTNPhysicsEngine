using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using Examples.UTNPhysicsEngine;
using Examples.UTNPhysicsEngine.physics.body;
using Examples.UTNPhysicsEngine.math;
using TgcViewer;

namespace Examples.UTNPhysicsEngine.physics
{
    class ContactBuilder
    {
        internal static List<Contact> TestCollisionWorld(Body bodyPivot, PlaneBody[] worldLimits) //por el momento las interacciones con el mundo no tienen fisica aplicada (elastico puro)
        {
            List<Contact> resultList = new List<Contact>();
            for (int i = 0; i < worldLimits.Length; i++)
            {
                float insertionDistance;
                if (CollisionAlgoritms.testCollision(bodyPivot, worldLimits[i], out insertionDistance))
                {
                    //en este caso no deberiamos crear contact solvers para planos, que seria una simplifaciacion y no cambiar el body directo.
                    resultList.AddRange(buildContact(bodyPivot, worldLimits[i], insertionDistance));
                }
            }

            return resultList;
        }

        internal static List<Contact> TestCollision(Body bodyPivot, Body bodyNear)
        {
            float insertionDistance;
            if (CollisionAlgoritms.testCollision(bodyPivot, bodyNear, out insertionDistance))
            {
                return buildContact(bodyPivot, bodyNear, insertionDistance);
            }
            return null;
        }

        internal static List<Contact> TestCollisionPick(Body bodyPivot, TgcRay ray) //por el momento las interacciones con el mundo no tienen fisica aplicada (elastico puro)
        {
            Vector3 point;
            if (CollisionAlgoritms.testCollision(bodyPivot, ray, out point))
            {
                //en este caso no deberiamos crear contact solvers para planos, que seria una simplifaciacion y no cambiar el body directo.
                return buildContact(bodyPivot, ray, point);
            }

            return null;
        }

        private static List<Contact> buildContact(Body bodyPivot, TgcRay ray, Vector3 point)
        {
            dynamic bodyA = bodyPivot;
            return buildContact(bodyA, ray, point);
        }

        private static List<Contact> buildContact(BoxBody bodyPivot, TgcRay ray, Vector3 point)
        {
            //dynamic bodyA = bodyPivot;
            return null;
        }
        private static List<Contact> buildContact(SphereBody s, TgcRay ray, Vector3 point)
        {
            Vector3 n = ray.Origin - s.position;
            n.Normalize();

            List<Contact> resultList = new List<Contact>();
            resultList.Add(new Contact(s, new RayBody(ray), -n, point, 0.01f));
            return resultList;
        }


        private static List<Contact> buildContact(Body bodyPivot, Body bodyNear, float insertionDistance)
        {
            dynamic bodyA = bodyPivot;
            dynamic bodyB = bodyNear;
            return buildContact(bodyA, bodyB, insertionDistance);
        }

        private static List<Contact> buildContact(SphereBody body, PlaneBody plane, float insertionDistance)
        {
            //float dist = insertionDistance - (body.radius);
            Vector3 positionContact = body.position - body.radius * plane.Normal;
            List<Contact> resultList = new List<Contact>();
            resultList.Add(new Contact(body, plane, -plane.Normal, positionContact, insertionDistance));
            return resultList;
           
/*            
            

      //--- Compute the coordinate transformation, which transforms from
      //--- the model frame of the sphere (S) into the model frame of the
      //--- plane (P).
      CordinateSystem planeToWCS = plane.wordCordSys();
      CordinateSystem sphereToPlane = body.wordCordSys().ConvertCordSysFrom(planeToWCS);

      //--- Transform sphere center into model frame of plane
      Vector3 p = sphereToPlane.xform_point(body.position);

      //--- The contact point is computed as the projection of
      //--- the sphere center onto the plane.
      p = p - plane.Normal*insertionDistance;

      //--- The penetration distance, must be equal to how deep the sphere
      //--- have sunken down into the plane, so we simply subtract the size
      //--- of the radius from the distance of the center over the plane.
      float dist = insertionDistance - (body.radius);

      //--- Finally convert contact point and contact normal into world
      //--- coordinate system.
      Vector3 ContactInPlaneCords = planeToWCS.xform_point(p);
      Vector3 NormalInPlaneCords = planeToWCS.xform_vector(plane.Normal);
            return new Contact(body, plane, NormalInPlaneCords, ContactInPlaneCords, dist);
      */
        }

        private static List<Contact> buildContact(SphereBody s1, SphereBody s2, float insertionDistance)
        {/*
            //insertionDistance viene al cuadrado por optimizacion de la colision.
            float insertionDistanceSqrt = FastMath.Sqrt(insertionDistance);
            //Si dist es negativo significa que se metieron adentro.
            float dist = insertionDistanceSqrt - (s1.radius + s2.radius);
            Vector3 normalContactOnSurface = FastMath.divVector(s1.position - s2.position, insertionDistanceSqrt);

            Vector3 positionContactS1 = s1.position - s1.radius * normalContactOnSurface; //no es necesario calcular con un contacto alcanza.
            //Vector3 positionContactS2 = s2.position + s2.radius * normalContactOnSurface;
            return new Contact(s1, s2, normalContactOnSurface, positionContactS1, dist);
            */
            //--- The contact normal is simply the normalized vector between
            //--- the sphere centers.
            //real_type squared_radius_sum = radius_sum*radius_sum;
            Vector3 n = s1.position - s2.position;
            float length = FastMath.Sqrt(insertionDistance);
            n.Multiply(1/length);

            //--- The contact point is kind of a midpoint weighted by the radius of the
            //--- spheres. That is the contact point, p, is computed  according to the
            //--- proportionality equation
            //---
            //---  |p-cA| /|p-cB| = rA / rB
            //---
            //--- We know
            //---
            //---   dA = |p-cA|
            //---   dB = |p-cB|
            //---    d = |cB-cA| = dA + dB
            //---
            //---        dA = dB (rA/rB)
            //---   dB + dA = dB + dB (rA/rB)   /*  add dB to both sides */
            //---         d = dB(1+rA/rB)       /*  use d=dA+dB          */
            //---
            //---        dB = d/(1+rA/rB)
            //---      d-dA = d/(1+rA/rB)       /*  use d=dA+dB          */
            //---        dA = d - d/(1+rA/rB)
            //---
            float dB = (length / ((s1.radius / s2.radius) + 1.0f));
            float dA = length - dB;
            float distance = length - (s1.radius + s2.radius);
            Vector3 p = n * dA + s1.position;
            List<Contact> resultList = new List<Contact>();
            resultList.Add(new Contact(s1, s2, n, p, distance));
            return resultList;
        }

        private static List<Contact> buildContact(BoxBody b, SphereBody s, float insertionDistance)
        {
            return null;
        }

        private static List<Contact> buildContact(SphereBody s, BoxBody b, float insertionDistance)
        {
            return buildContact(b, s, insertionDistance);
        }

        private static List<Contact> buildContact(BoxBody box, PlaneBody plane, float insertionDistance)
        {
            Vector3 c = box.position;
            Matrix R = box.direction;
            
            //--- Find closest point between plane and box
            Vector3 n = plane.Normal;

            Vector3 i,j,k;
			i.X = R.M11;    i.Y = R.M12;    i.Z = R.M13;
            j.X = R.M21;    j.Y = R.M22;    j.Z = R.M23;
            k.X = R.M31;    k.Y = R.M32;    k.Z = R.M33;

            Vector3 p = c;
            Vector3 a = box.extendend;
            Vector3 delta;//--- keep signs so we know which corner of box that was the clostest point.

            if(Vector3.Dot(n,i)>0)
            {
            p += i*a.X;
            delta.X = +1;
            }
            else
            {
            p -= i*a.X;
            delta.X = -1;
            }
            if(Vector3.Dot(n,j)>0)
            {
            p += j*a.Y;
            delta.Y = +1;
            }
            else
            {
            p -= j*a.Y;
            delta.Y = -1;
            }
            if(Vector3.Dot(n,k)>0)
            {
            p += k*a.Z;
            delta.Z = +1;
            }
            else
            {
            p -= k*a.Z;
            delta.Z = -1;
            }
            float w = FastMath.Abs(plane.D);
            float distance = Vector3.Dot(n, p) - w; //HASTA ACA Venimos perfectos, pero despues tira cualquiera los puntos de contactos.
            
            //--- If closest points on box is longer away than epsilon, then there
            //--- is no chance for any contacts.
//            if(FastMath.Abs(distance)<box.margin)
//                return null;

            //--- The closest point was within envelope, so it will be the first contact point
            //--- All contacts will have the same normal dictated by the plane, so there is
            //--- no need for storing a normal for each contact point.

            //--- Now find three other corners of the box by going along the edges meeting at the clostest point.
            CordinateSystem boxCoords = box.wordCordSys();
            Vector3 localPoint = boxCoords.toLocalCoordsPoint(p);
            //localPoint = Vector3.TransformCoordinate(localPoint, box.direction);
            Vector3 b;
            b.X = 1f * a.X * delta.X;
            b.Y = 1f * a.Y * delta.Y;
            b.Z = 1f * a.Z * delta.Z;
            Vector3 bt = Vector3.TransformCoordinate(b, Matrix.Identity*box.direction);
            Vector3[] corner = {new Vector3(localPoint.X + bt.X, localPoint.Y, localPoint.Z),
                                new Vector3(localPoint.X, localPoint.Y + bt.Y, localPoint.Z),
                                new Vector3(localPoint.X, localPoint.Y, localPoint.Z + bt.Z)};

            //--- Pick the two corner points with smallest signed distance, and
            //--- if distance below envelope add them as contacts.
            float[] dist = {   FastMath.Abs( Vector3.Dot(boxCoords.fromLocalCoordPoint(corner[0]),n) - w),//+ Vector3.Dot(plane.Normal, i)*box.extendend.X
                               FastMath.Abs( Vector3.Dot(boxCoords.fromLocalCoordPoint(corner[1]),n) - w),
                               FastMath.Abs( Vector3.Dot(boxCoords.fromLocalCoordPoint(corner[2]),n) - w)};

            //--- Use insertion sort to find contacts with smallest distance
            int []permutation = {0,1,2};
            for(int s=1; s<3; ++s)
            {
            for(int t=s; t>0; --t)
            {
                if(dist[permutation[t]] < dist[permutation[t-1]])
                {
                int tmp = permutation[t-1];
                permutation[t-1] = permutation[t];
                permutation[t] = tmp;
                }
            }
            }
            
            //if(dist[permutation[0]]>envelope)
            //{
//            p = plane.wordCordSys().xform_point(p);
//            n = plane.wordCordSys().xform_vector(n);
            // Compute the projection interval radius of b onto L(t) = b.c+t*p.n
            /*float r = box.extendend.X * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M11, box.direction.M12, box.direction.M13))) +
                        box.extendend.Y * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M21, box.direction.M22, box.direction.M23))) +
                        box.extendend.Z * FastMath.Abs(Vector3.Dot(plane.Normal, new Vector3(box.direction.M31, box.direction.M32, box.direction.M33)));
            // Compute distance of box center from plane
            float sa = Vector3.Dot(plane.Normal, box.Center) - plane.D;
            // Intersection occurs when distance s falls within [-r,+r] interval
            distance = FastMath.Abs(sa) - FastMath.Abs(r);
            //float dist = insertionDistance - (body.radius);
            p = box.position - box.extendend.X * plane.Normal;
            //return new Contact(box, plane, -plane.Normal, positionContact, dist);
            */
            List<Contact> resultList = new List<Contact>();
            resultList.Add(new Contact(box, plane, -n, boxCoords.fromLocalCoordPoint(corner[permutation[0]]), insertionDistance));
            if (dist[permutation[1]] < box.margin + distance)
                resultList.Add(new Contact(box, plane, -n, boxCoords.fromLocalCoordPoint(corner[permutation[1]]), insertionDistance));
            if (dist[permutation[2]] < box.margin + distance)
                resultList.Add(new Contact(box, plane, -n, boxCoords.fromLocalCoordPoint(corner[permutation[2]]), insertionDistance));
            //}
/*            p[1] = corner[permutation[0]];
            distance[1] = dist[permutation[0]];
            
            if(dist[permutation[1]]>envelope)
            {
            PtoWCS.xform_point(p);
            PtoWCS.xform_point(p[1]);
            PtoWCS.xform_vector(n);
            return 2;
            }
            p[2] = corner[permutation[1]];
            distance[2] = dist[permutation[1]];

            //---- fourth contact can be generated, but often not necessary
            //p[3] = p[1] + (p[2]-p);
            //distance[3] = n*p[3] - w;
            //PtoWCS.xform_point(p[3]);

            PtoWCS.xform_point(p);
            PtoWCS.xform_point(p[1]);
            PtoWCS.xform_point(p[2]);
            PtoWCS.xform_vector(n);
            return 3;
             }
             */
            return resultList;
        }

    public float GetSpherePenetration(BoxBody boxObject, ref Vector3 pointOnBox, ref Vector3 pointOnSphere, Vector3 sphereCenter, float radius, Vector3 aabbMin, Vector3 aabbMax)
		{
			Vector3[] bounds = new Vector3[2];

			bounds[0] = aabbMin;
			bounds[1] = aabbMax;

			Vector3 p0 = new Vector3(), tmp, prel, normal = new Vector3();
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
				if ((sepThis = (Vector3.Dot(prel - bounds[j], n[i])) /*- radius*/) > 0.0f) return 1.0f;
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
			pointOnBox = boxWSCoord.fromLocalCoordVector(pointOnBox);
			pointOnBox = boxWSCoord.fromLocalCoordVector(pointOnSphere);
			normal = Vector3.Normalize(pointOnBox - pointOnSphere);

			return sep;
		}

        private static Contact buildContact(BoxBody b1, BoxBody b2, float insertionDistance)
        {
            return null;
        }
    }
}
