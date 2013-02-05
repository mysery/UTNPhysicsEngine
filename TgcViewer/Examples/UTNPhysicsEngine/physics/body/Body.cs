using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Examples.UTNPhysicsEngine.optimizacion.octree;
using Examples.UTNPhysicsEngine.math;
using Examples.UTNPhysicsEngine.optimizacion.spatialHash;

namespace Examples.UTNPhysicsEngine.physics.body
{
    public abstract class Body
    {
        
        /* Constantetes del cuerpo.*/
        public int idCode;
        public float mass;
        private float invMass;
        private bool initialize = false;

        private SpatialHashAABB _aabb;
        public Vector3 lastUpdatePosition;

        private Matrix _Ibody; //Inertia tensor del cuerpo constante durante lo que vive el cuerpo, para las rotaciones.
        private Matrix _InvIbody; //Inversa de inertia tensor del cuerpo constante durante lo que vive el cuerpo, para las rotaciones.
        private float _restitution = 0.9f;
        public float friction = 0.1f;//TODO, verificar buenas constantes de friccion y resitucion.

        /* estado */
        public Vector3 position;
        public Matrix scaling;
        public Quaternion quaternion; //orientacion.
        //public Matrix rotation; //orientacion.
        
        /* derivables */
        public Matrix invInertiaTensor; // calculado con momento y velocidad angular.
        public Vector3 velocity;
        public Vector3 angularVelocity; // w = omega 

        /* Fuerzas */
        public Vector3 aceleracion; // F = momentum //independiente de la rotacion. P
        public Vector3 torque; //tao = angularMomentum //independiente de la traslacion. L
        
        public Body(Vector3 position, Vector3 velocity, Vector3 aceleracion, float mass, bool applyGravity = true)
        {
            this.idCode = BodySecuence.Instance.Next;
            this.position = position;
            this.lastUpdatePosition = position;
            this.quaternion = Quaternion.Identity;// RotationMatrix(Matrix.Identity);
            this.scaling = Matrix.Scaling(1f,1f,1f);
            this.velocity = velocity;
            this.angularVelocity = Vector3.Empty;
            this.aceleracion = aceleracion;
            this.torque = Vector3.Empty;
            this.mass = mass;
            this.invMass = mass != 0f ? 1.0f / mass : 0.0f;
            if (applyGravity)
                this.aceleracion += new Vector3(0.0f, -9.8f, 0.0f) * mass;
        }
        //Esto lo tengo que hacer asi porque en C# no se puede invocar en orden a los constructores. mal...
        public abstract void calculateInertiaBody();
        public Matrix Ibody { get {return _Ibody; } set {_Ibody=value;} }
        public Matrix InvIbody { get { if (!initialize) 
        { calculateInertiaBody(); initialize = true; } 
            return _InvIbody; } set { _InvIbody = value; } }

        public Matrix getTrasform()
        {
            //this.quaternion.Normalize();
            return Matrix.Identity * scaling * Matrix.RotationQuaternion(quaternion) * Matrix.Translation(position);
        }

        internal CordinateSystem wordCordSys()
        {
            return new CordinateSystem(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W), new Vector3(position.X, position.Y, position.Z));
        }

        internal void integrateForceSI(float timeStep)
        {   //Aproximado por euler expli.
            this.velocity.Add(Vector3.Multiply(this.aceleracion, InverseMass * timeStep));            
            Matrix rotation = Matrix.RotationQuaternion(quaternion);
            this.invInertiaTensor = rotation * InvIbody * Matrix.TransposeMatrix(rotation);
            this.angularVelocity.Add(Vector3.TransformNormal(this.torque, this.invInertiaTensor) * timeStep);

            float MAX_ANGVEL = FastMath.PI_HALF;
            /// clamp angular velocity. collision calculations will fail on higher angular velocities, dato sacado de Bullet
            float angvel = this.angularVelocity.Length();
            if (angvel * timeStep > MAX_ANGVEL)
            {
                this.angularVelocity.Multiply((MAX_ANGVEL / timeStep) / angvel);
            }

            

            /*this.angularVelocity.Add(new Vector3(   this.invInertiaTensor.M11 * this.torque.X + this.invInertiaTensor.M12 * this.torque.Y + this.invInertiaTensor.M13 * this.torque.Z,
                                                    this.invInertiaTensor.M21 * this.torque.X + this.invInertiaTensor.M22 * this.torque.Y + this.invInertiaTensor.M23 * this.torque.Z,
                                                    this.invInertiaTensor.M31 * this.torque.X + this.invInertiaTensor.M32 * this.torque.Y + this.invInertiaTensor.M33 * this.torque.Z) * timeStep);*/
        }

        internal void integrateVelocitySI(float timeStep)
        {
            this.position.Add(Vector3.Multiply(this.velocity, timeStep));
            
            float AngularMotionTreshold = 0.5f * FastMath.PI_HALF;
            Vector3 axis;
            float angle = angularVelocity.Length();
            //limit the angular motion
            if (angle * timeStep > AngularMotionTreshold)
            {
                angle = AngularMotionTreshold / timeStep;
            }

            if (angle < 0.001f)
            {
                // use Taylor's expansions of sync function
                axis = angularVelocity * (0.5f * timeStep - (timeStep * timeStep * timeStep) * (0.020833333333f) * angle * angle);
            }
            else
            {
                // sync(fAngle) = sin(c*fAngle)/t
                axis = angularVelocity * ((float)Math.Sin(0.5f * angle * timeStep) / angle);
            }
            Quaternion dorn = new Quaternion(axis.X, axis.Y, axis.Z, (float)Math.Cos(angle * timeStep * 0.5f));
            
            this.quaternion = dorn * this.quaternion;
            this.quaternion.Normalize();
            
            /*Quaternion qAngular = new Quaternion(this.angularVelocity.X * 0.5f * timeStep, this.angularVelocity.Y * 0.5f * timeStep, this.angularVelocity.Z * 0.5f * timeStep, 0f);
            Quaternion newQuaternion = Quaternion.Multiply(qAngular, this.quaternion); //1/2*w*q
            if (newQuaternion.LengthSq() != 0)
            {
                this.quaternion = newQuaternion;
                this.quaternion.Normalize();
            }*/
        }

        public float InverseMass { get { return invMass;} }

        public float restitution { get { return this._restitution; } set { this._restitution = value; } }

        public Vector3 CenterOfMassPosition { get { return this.position; } }

        public void ApplyImpulse(Vector3 relativePosition, Vector3 normalContact, float impulseMagnitude)
		{
            this.velocity.Add(normalContact * InverseMass * impulseMagnitude);

            Vector3 torqueAxisB = Vector3.Cross(relativePosition, impulseMagnitude * normalContact);            
            Vector3 angularComponent = Vector3.TransformNormal(torqueAxisB, this.invInertiaTensor);
/*                    = new Vector3(invInertiaTensor.M11 * torqueAxisB.X + invInertiaTensor.M12 * torqueAxisB.Y + invInertiaTensor.M13 * torqueAxisB.Z,
                                  invInertiaTensor.M21 * torqueAxisB.X + invInertiaTensor.M22 * torqueAxisB.Y + invInertiaTensor.M23 * torqueAxisB.Z,
                                  invInertiaTensor.M31 * torqueAxisB.X + invInertiaTensor.M32 * torqueAxisB.Y + invInertiaTensor.M33 * torqueAxisB.Z);
*/
            this.angularVelocity.Add(angularComponent);
		}

        public Vector3 getVelocityInPosition(Vector3 relPos)
		{
			return this.velocity + Vector3.Cross(this.angularVelocity, relPos);
        }

        public abstract void calculateAABB();
        /*public OctreeBox BoundingBox 
        {
            get
            {
                { calculateAABB();}
                return _aabb;
            }
            set { _aabb = value; }
        }*/
        
        public SpatialHashAABB BoundingBox 
        {
            get
            {
                { calculateAABB();}
                return _aabb;
            }
            set { _aabb = value; }
        }

        public Vector3 LastUpdatePosition { get; set; }
    }
}
