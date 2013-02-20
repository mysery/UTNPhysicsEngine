using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples.UTNPhysicsEngine.physics
{
    static class ContactSolver
    {
        private static float EPSILON_FOR_IMPULSE = 0.0001f;

        internal static void solveSimpleContact(Contact c, float timeStep)
        {
            Vector3 relPosA = c.positionContact - c.bodyA.CenterOfMassPosition;
            Vector3 relPosB = c.positionContact - c.bodyB.CenterOfMassPosition;

            //float normalImpulse = 0;            

            float relVel;
            float velADotn = Vector3.Dot(c.normalContact, c.bodyA.velocity)
                        + Vector3.Dot(Vector3.Cross(c.bodyA.angularVelocity, relPosA), c.normalContact);
            //body->v + (body->omega criss (p - body->x));
            float velBDotn = Vector3.Dot(c.normalContact, c.bodyB.velocity)
                        + Vector3.Dot(Vector3.Cross(c.bodyB.angularVelocity, relPosB), c.normalContact);
            relVel = velADotn - velBDotn;
            if (relVel > EPSILON_FOR_IMPULSE)
                return; //Se esta alejando ya, si se le aplica nuevaente el impulso hace que los cuerpos se aceleren.
//            if (relVel > -EPSILON_FOR_IMPULSE)
//                return; //Caso de penetracion, hay que realizar "resting contact" problema cuadratico.

            float numerator = -(1 + c.combinedRestitution) * relVel;
            float denom = c.bodyA.InverseMass + c.bodyB.InverseMass
                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosA, c.normalContact), c.bodyA.invInertiaTensor), relPosA))
                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosB, c.normalContact), c.bodyB.invInertiaTensor), relPosB));

            float impulseMagnitude = numerator / denom;

            float penVel = -c.insertionDistance / timeStep;
            float penetrationImpulse = 0;
            if (numerator < penVel)
            {
                penetrationImpulse = c.penetration / denom;
            }
            float fixedImpulse = penetrationImpulse + impulseMagnitude;

            if (c.bodyA.InverseMass != 0)
            {
                c.bodyA.ApplyImpulse(relPosA, c.normalContact, fixedImpulse);
            }
            if (c.bodyB.InverseMass != 0)
            {
                c.bodyB.ApplyImpulse(relPosB, c.normalContact, -fixedImpulse);
            }

            
        }

        //DEPRECADO, no lo uso porque trae errores.
        internal static void solveSimpleFrictionContact(Contact c)
        {
            Vector3 relPosA = c.positionContact - c.bodyA.CenterOfMassPosition;
            Vector3 relPosB = c.positionContact - c.bodyB.CenterOfMassPosition;

            //float normalImpulse = 0;

            float relVel;
            float velADotn = Vector3.Dot(c.normalContact, c.bodyA.velocity)
                        + Vector3.Dot(Vector3.Cross(c.bodyA.angularVelocity, relPosA), c.normalContact);
            //body->v + (body->omega criss (p - body->x));
            float velBDotn = Vector3.Dot(c.normalContact, c.bodyB.velocity)
                        + Vector3.Dot(Vector3.Cross(c.bodyB.angularVelocity, relPosB), c.normalContact);
            relVel = velADotn - velBDotn;
            if (relVel > EPSILON_FOR_IMPULSE)
                return; //Se esta alejando ya, si se le aplica nuevaente el impulso hace que los cuerpos se aceleren.
            if (relVel > -EPSILON_FOR_IMPULSE)
                return; //Caso de penetracion, hay que realizar "resting contact" rigid body II

            //////////////// FRCICION ///////////////
            Vector3 frictionTangentialA = new Vector3(), frictionTangentialB = new Vector3();
            PlaneSpace1(c.normalContact, ref frictionTangentialA, ref frictionTangentialB);
            float relaxation = relVel * c.combinedFriction;
            float limit = c.combinedFriction; //impulseMagnitude * c.combinedFriction

            float denomFricctionA = c.bodyA.InverseMass + c.bodyB.InverseMass
                                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosA, frictionTangentialA), c.bodyA.invInertiaTensor), relPosA))
                                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosB, frictionTangentialA), c.bodyB.invInertiaTensor), relPosB));
            float impulseFictionA = relaxation / (denomFricctionA); //impulse frictional

            if (limit < impulseFictionA)
                impulseFictionA = limit;
            if (impulseFictionA < -limit)
                impulseFictionA = -limit;

            if (c.bodyA.InverseMass != 0)
            {
                c.bodyA.ApplyImpulse(relPosA, frictionTangentialA, impulseFictionA);
            }
            if (c.bodyB.InverseMass != 0)
            {
                c.bodyB.ApplyImpulse(relPosB, frictionTangentialA, -impulseFictionA);
            }


            float denomFricctionB = c.bodyA.InverseMass + c.bodyB.InverseMass
                                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosA, frictionTangentialB), c.bodyA.invInertiaTensor), relPosA))
                                        + Vector3.Dot(c.normalContact, Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(relPosB, frictionTangentialB), c.bodyB.invInertiaTensor), relPosB));
            float impulseFictionB = relaxation / (denomFricctionB); //impulse frictional

            //limit = (impulseMagnitude+impulseFictionA) * c.combinedFriction;
            if (limit < impulseFictionB)
                impulseFictionB = limit;
            if (impulseFictionA < -limit)
                impulseFictionB = -limit;

            if (c.bodyA.InverseMass != 0)
            {
                c.bodyA.ApplyImpulse(relPosA, frictionTangentialB, impulseFictionB);
            }
            if (c.bodyB.InverseMass != 0)
            {
                c.bodyB.ApplyImpulse(relPosB, frictionTangentialB, -impulseFictionB);
            }
        }

        internal const float Sqrt12 = 0.7071067811865475244008443621048490f;
        internal static void PlaneSpace1(Vector3 n, ref Vector3 p, ref Vector3 q)
        {
            if (Math.Abs(n.Z) > Sqrt12)
            {
                // choose p in y-z plane
                float a = n.Y * n.Y + n.Z * n.Z;
                float k = 1f / (float)Math.Sqrt(a);
                p.X = 0;
                p.Y = -n.Z * k;
                p.Z = n.Y * k;
                // set q = n x p
                q.X = a * k;
                q.Y = -n.X * p.Z;
                q.Z = n.X * p.Y;
            }
            else
            {
                // choose p in x-y plane
                float a = n.X * n.X + n.Y * n.Y;
                float k = 1f / (float)Math.Sqrt(a);
                p.X = -n.Y * k;
                p.Y = n.X * k;
                p.Z = 0;
                // set q = n x p
                q.X = -n.Z * p.Y;
                q.Y = n.Z * p.X;
                q.Z = a * k;
            }
        }

        /**
    * Compute Collision Matrix.
    * This method computes the collision matrix, K, which is part
    * of the impulse-momemtum relation between two bodies in a
    * single point collision.
    *
    *    K = (1/Ma + 1/Mb) Identity  - ( star(ra) Ia^-1 star(ra)  +  star(rb) Ib^-1 star(rb))
    *
    * Where star() is the cross product matrix and Identity is a
    * diagonal matrix of ones.
    *
    * The computations have been optimized to exploit symmetry and zero pattern's as
    * much as possible. It is still possible to optimize further by exploiting
    * common sub-terms in the computations.
    *
    * @param m_a  Inverse mass of object A.
    * @param I_a  Inverse Intertia Tensor of object A.
    * @param r_a  The arm from center of mas of object A to the point of contact.
    * @param m_b  Inverse mass of object B.
    * @param I_b  Inverse Intertia Tensor of object B.
    * @param r_b  The arm from center of mas of object B to the point of contact.
    *
    * @return     The collision matrix K.
    */
        private static Matrix compute_collision_matrix(
      float m_a
      , Matrix I_a
      , Vector3 r_a
      , float m_b
      , Matrix I_b
      , Vector3 r_b
      )
        {
            Matrix K = Matrix.Identity;
            float K00 = r_a.Z * I_a.M22 * r_a.Z - 2 * r_a.Z * I_a.M23 * r_a.Y + r_a.Y * I_a.M33 * r_a.Y;
            float K01 = -r_a.Z * I_a.M12 * r_a.Z + r_a.Y * I_a.M13 * r_a.Z + r_a.Z * I_a.M23 * r_a.X - r_a.Y * I_a.M33 * r_a.X;
            float K02 = r_a.Z * I_a.M12 * r_a.Y - r_a.Y * I_a.M13 * r_a.Y - r_a.Z * I_a.M22 * r_a.X + r_a.Y * I_a.M23 * r_a.X;
            float K11 = r_a.Z * I_a.M11 * r_a.Z - 2 * r_a.X * I_a.M13 * r_a.Z + r_a.X * I_a.M33 * r_a.X;
            float K12 = -r_a.Z * I_a.M11 * r_a.Y + r_a.Z * I_a.M12 * r_a.X + r_a.X * I_a.M13 * r_a.Y - r_a.X * I_a.M23 * r_a.X;
            float K22 = r_a.Y * I_a.M11 * r_a.Y - 2 * r_a.X * I_a.M12 * r_a.Y + r_a.X * I_a.M22 * r_a.X;
            K.M11 = m_a + K00;
            K.M12 = K01;
            K.M13 = K02;
            K.M21 = K01;
            K.M22 = m_a + K11;
            K.M23 = K12;
            K.M31 = K02;
            K.M32 = K12;
            K.M33 = m_a + K22;
            K00 = r_b.Z * I_b.M22 * r_b.Z - 2 * r_b.Z * I_b.M23 * r_b.Y + r_b.Y * I_b.M33 * r_b.Y;
            K01 = -r_b.Z * I_b.M12 * r_b.Z + r_b.Y * I_b.M13 * r_b.Z + r_b.Z * I_b.M23 * r_b.X - r_b.Y * I_b.M33 * r_b.X;
            K02 = r_b.Z * I_b.M12 * r_b.Y - r_b.Y * I_b.M13 * r_b.Y - r_b.Z * I_b.M22 * r_b.X + r_b.Y * I_b.M23 * r_b.X;
            K11 = r_b.Z * I_b.M11 * r_b.Z - 2 * r_b.X * I_b.M13 * r_b.Z + r_b.X * I_b.M33 * r_b.X;
            K12 = -r_b.Z * I_b.M11 * r_b.Y + r_b.Z * I_b.M12 * r_b.X + r_b.X * I_b.M13 * r_b.Y - r_b.X * I_b.M23 * r_b.X;
            K22 = r_b.Y * I_b.M11 * r_b.Y - 2 * r_b.X * I_b.M12 * r_b.Y + r_b.X * I_b.M22 * r_b.X;
            K.M11 += m_b + K00;
            K.M12 += K01;
            K.M13 += K02;
            K.M21 += K01;
            K.M22 += m_b + K11;
            K.M23 += K12;
            K.M31 += K02;
            K.M32 += K12;
            K.M33 += m_b + K22;
            return K;
        }

        private static float[,] amat;
        internal static void solveRestingContacts(List<Contact> restingContacts)
        {
            Contact[] contacts = restingContacts.ToArray();
            int ncontacts = restingContacts.Count;
            //float[,] amat;
            float[] bvec;
            float[] fvec;

            int notZero;
            int notZeroCols;
            /* Compute ai j and bi coefficients */
            computeAMatrix(contacts, ncontacts, out amat, out notZero, out notZeroCols);
            computeBVector(contacts, ncontacts, out bvec);
            /* Solve for f j's */
            //formula de lib q+cTx+1/2*xT*Hx para esto no hay componente lineal. c=0
            /*NagLibrary.E04.e04nq(   "C",
                                    new NagLibrary.E04.E04NQ_QPHX(qphx), //hessiano.
                                    ncontacts,//m
                                    ncontacts,//n
                                    notZero, //no cero n
                                    1,//defuult
                                    0, //c componente lineal.
                                    notZeroCols,
                                    0,// c rows
                                    0, //q
                                    "",
                                    ????? que parametro es este!!!!!
            */


            qpSolve(amat, bvec, ncontacts, out fvec); //problema cuadratrico.
            /* Now add the resting contact forces we just computed into
            the `force' and `torque' field of each rigid body. */

            for (int i = 0; i < ncontacts; i++)
            {
                float f = fvec[i]; /* fi */
                Vector3 n = contacts[i].normalContact; /* O ni.t0/ */
                Body bodyA = contacts[i].bodyA; /* body A */
                Body bodyB = contacts[i].bodyB; /* body B */
                /* apply the force `f n' positively to A... */
                bodyA.aceleracion += f * n;
                bodyA.torque += Vector3.Cross((contacts[i].positionContact - bodyA.position), (f * n));
                /* and negatively to B */
                bodyB.aceleracion -= f * n;
                bodyB.torque -= Vector3.Cross((contacts[i].positionContact - bodyB.position), (f * n));
            }
        }

        public static void qphx(int ncolh, double[] x, double[] hx, int nstate)
        {
            //
            //      Routine to compute H*x. (In this version of QPHX, the Hessian
            //      matrix h is not referenced explicitly.)
            //
            double two = 2.00e+0;

            for (int i = 0; i < ncolh; i++)
            {
                for (int j = 0; j < ncolh; j++)
                {
                    hx[i] += two * amat[j, i] * x[i]; // es por dos por la api que tiene esto.
                }
            }
        }


        private static void qpSolve(float[,] amat, float[] bvec, int ncontacts, out float[] fvec)
        {
            fvec = new float[ncontacts];
            throw new NotImplementedException();
        }

        private static void computeBVector(Contact[] contacts, int ncontacts, out float[] bvec)
        {
            bvec = new float[ncontacts];

            for (int i = 0; i < ncontacts; i++)
            {
                Contact c = contacts[i];
                Body A = c.bodyA;
                Body B = c.bodyB;
                Vector3 n = c.normalContact; /* O ni.t0/ */
                Vector3 ra = c.positionContact - A.position; /* p-xa.t0/ */
                Vector3 rb = c.positionContact - B.position; /* p-xb.t0/ */
                /*Gettheexternalforcesandtorques*/
                Vector3 f_ext_a = A.aceleracion;
                Vector3 f_ext_b = B.aceleracion;
                Vector3 t_ext_a = A.torque;
                Vector3 t_ext_b = B.torque;
                Vector3 a_ext_part, a_vel_part, b_ext_part, b_vel_part;
                /*Operators:''isforcrossproduct,`*',isfor
                dotproducts(betweentwotriples),ormatrix-vector
                multiplication(betweenamatrixandatriple).*/
                /*Computethepartof R pa.t0/ duetotheexternal
                forceandtorque,andsimilarlyfor R pb.t0/.*/
                a_ext_part = f_ext_a * A.InverseMass + (Vector3.Cross(Vector3.TransformNormal(t_ext_a, A.invInertiaTensor), ra)); //A.invInertiaTensor*t_ext_a
                b_ext_part = f_ext_b * B.InverseMass + (Vector3.Cross(Vector3.TransformNormal(t_ext_b, B.invInertiaTensor), rb));
                /*Computethepartof R pa.t0/ duetovelocity,
                andsimilarlyfor R pb.t0/.*/
                a_vel_part = Vector3.Cross(A.angularVelocity, Vector3.Cross(A.angularVelocity, ra)) +
                            Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(A.torque, A.angularVelocity), A.invInertiaTensor), ra);
                b_vel_part = Vector3.Cross(B.angularVelocity, Vector3.Cross(B.angularVelocity, rb)) +
                            Vector3.Cross(Vector3.TransformNormal(Vector3.Cross(B.torque, B.angularVelocity), B.invInertiaTensor), rb);
                /*Combinetheaboveresults,anddotwith O ni.t0/ */
                float k1 = Vector3.Dot(n, ((a_ext_part + a_vel_part) - (b_ext_part + b_vel_part)));
                Vector3 ndot = compute_ndot(c);
                /*Seesection8for`pt_velocity'definition*/
                Vector3 pt_velocity_a = c.bodyA.velocity + Vector3.Cross(c.bodyA.angularVelocity, (c.positionContact - c.bodyA.CenterOfMassPosition));
                Vector3 pt_velocity_b = c.bodyB.velocity + Vector3.Cross(c.bodyB.angularVelocity, (c.positionContact - c.bodyB.CenterOfMassPosition));

                float k2 = 2 * Vector3.Dot(ndot, (pt_velocity_a - pt_velocity_b));
                bvec[i] = k1 + k2;
            }
        }

        private static void computeAMatrix(Contact[] contacts, int ncontacts, out float[,] amat, out int notZero, out int notZeroCols)
        {
            amat = new float[ncontacts, ncontacts];
            notZero = 0;
            notZeroCols = 0;
            bool hasNonZero = false;
            for (int i = 0; i < ncontacts; i++)
            {
                for (int j = 0; j < ncontacts; j++)
                {
                    amat[i, j] = compute_aij(contacts[i], contacts[j]);
                    if (amat[i, j] != 0)
                    {
                        notZero++;
                        hasNonZero = true;
                    }
                }
                if (hasNonZero)
                {
                    notZeroCols++;
                    hasNonZero = false;
                }

            }
        }

        private static float compute_aij(Contact ci, Contact cj)
        {
            /*If the bodies involved in the ith and jth contact are distinct,then aij is zero.*/
            if (ContactComparer.Distinct(ci, cj))
                return 0.0f;
            Body A = ci.bodyA;
            Body B = ci.bodyB;
            Vector3 ni = ci.normalContact; /* O ni.t0/ */
            Vector3 nj = cj.normalContact; /* O nj.t0/ */
            Vector3 pi = ci.positionContact; /*ith contact point location*/
            Vector3 pj = cj.positionContact; /* jth contact point location*/
            Vector3 ra = pi - A.CenterOfMassPosition;
            Vector3 rb = pi - B.CenterOfMassPosition;
            /*What force and torque does contact j exertonbody A?*/
            Vector3 force_on_a = new Vector3();
            Vector3 torque_on_a = new Vector3();
            if (cj.bodyA.idCode.Equals(ci.bodyA.idCode)) //cj.a = ci.a
            {
                /*force direction of jth contact force on A*/
                force_on_a = nj;
                /*torque direction*/
                torque_on_a = Vector3.Cross((pj - A.CenterOfMassPosition), nj);
            }
            else if (cj.bodyB.idCode.Equals(ci.bodyA.idCode))//cj.b==ci.a)
            {
                force_on_a = -nj;
                torque_on_a = Vector3.Cross((pj - A.CenterOfMassPosition), nj);
            }
            /*What force and torque does contact j exert on body B?*/
            Vector3 force_on_b = new Vector3();
            Vector3 torque_on_b = new Vector3();
            if (cj.bodyA.idCode.Equals(ci.bodyB.idCode))//cj.a==ci.b)
            {
                /*forcedirectionof jthcontactforceon B*/
                force_on_b = nj;
                /*torquedirection*/
                torque_on_b = Vector3.Cross((pj - B.CenterOfMassPosition), nj);
            }
            else if (cj.bodyB.idCode.Equals(ci.bodyB.idCode))//cj.b==ci.b)
            {
                force_on_b = -nj;
                torque_on_b = Vector3.Cross((pj - B.CenterOfMassPosition), nj);
            }
            /*Now compute how the jth contact force affects the linear
            and angular acceleration of the contact point on body A*/
            Vector3 a_linear = force_on_a * A.InverseMass;
            Vector3 a_angular = Vector3.Cross(Vector3.TransformNormal(torque_on_a, A.invInertiaTensor), ra);
            /*Samefor B*/
            Vector3 b_linear = force_on_b * B.InverseMass;
            Vector3 b_angular = Vector3.Cross(Vector3.TransformNormal(torque_on_b, B.invInertiaTensor), rb);
            return Vector3.Dot(ni, ((a_linear + a_angular) - (b_linear + b_angular)));
        }

        /*return the derivative of the normal vector*/
        private static Vector3 compute_ndot(Contact c)
        {
            return Vector3.Cross(c.bodyB.angularVelocity, c.normalContact);

            /*if(c.isVertexFace()) //vertex/facecontact
            {
                //The vector`n'is attached to B,so...
                return Vector3.Cross(c.bodyB.angularVelocity, c.normalContact);
            }
            /*else
            {
            /*This is alittle trickier.The unit normal 'n'is
            n^ = ea * eb / | ea * eb |
            Differentiating n^ with respect to time is left
            asanexercise...buthere's some code*/
            //Vector3 eadot =c.bodyA.angularVelocity /*cross c.ea*/; /* TODO tengo que obtener ea en la colision */
            //Vector3 ebdot =c.bodyB.angularVelocity /*cross c.eb*/; /* TODO tengo que obtener eb en la colision */
            /*Vector3 n1 = new Vector3();//ea*eb,
            Vector3 z = new Vector3();//eadot*eb+ea*ebdot;
            float l = n1.Length();
            n1=n1/l; //normalize
            return(z-((z*n1)*n1))/l;
            }*/
        }
    }
}
