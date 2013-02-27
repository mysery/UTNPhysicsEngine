using System;
using System.Collections.Generic;
using System.Text;
using Examples.UTNPhysicsEngine;
using Microsoft.DirectX;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples.UTNPhysicsEngine.physics
{
    public class Contact
    {
        private const float MAX_FRICTION_SUPORTED = 10f;
        public Body bodyA;
        public Body bodyB;
        public Vector3 positionContact;
        public Vector3 normalContact;
        public float insertionDistance;
        public float combinedFriction;
        public float combinedRestitution;
        public float penetration;

        public Contact(Body bodyA, Body bodyB, Vector3 normalContact, Vector3 positionContact, float insertionDistance)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            this.positionContact = positionContact;
            this.normalContact = normalContact;
            this.insertionDistance = insertionDistance;
            float friction = bodyA.friction * bodyB.friction;
            if (friction < -MAX_FRICTION_SUPORTED)
                friction = -MAX_FRICTION_SUPORTED;
            if (friction > MAX_FRICTION_SUPORTED)
                friction = MAX_FRICTION_SUPORTED;
			this.combinedFriction = friction;
            this.combinedRestitution = bodyA.restitution * bodyB.restitution;
            this.penetration = insertionDistance * -0.4f;
        }

        internal bool isVertexFace()
        {
            return true;
        }
    }
}
