﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer.Utils;
using System.Drawing;
using TgcViewer;
using Examples.UTNPhysicsEngine.physics.body;
using Examples.UTNPhysicsEngine.optimizacion.octree;
using Examples.UTNPhysicsEngine.optimizacion.spatialHash;

namespace Examples.UTNPhysicsEngine.physics
{
    public class World
    {
        //private Octree _octree;
        private SpatialHash _spatialHash;

        /*public Octree Octree
        {
            get { return _octree; }
        }*/
        public SpatialHash SpatialHash
        {
            get { return _spatialHash; }
        }

        private const int MAX_STEPS = 50;
        private const float FIXED_TIME_STEP = 1f / 60f;
        private float localTime = FIXED_TIME_STEP;
        
        private List<Body> bodys;
        private Vector3 worldSize;
        //private ISet<Contact> contacts = new HashSet<Contact>(new ContactComparer());
        private ArrayList contacts = new ArrayList();//<Contact>();//new ContactComparer());
        private PlaneBody[] planesLimits = new PlaneBody[6];
        private TgcPickingRay pickingRay;

        public float timeSteps = 1f;
        public bool debugMode = false;
        public bool applyRay = false;

        public World(List<Body> bodys, float worldSize, float indexSizeCell = 0.1f, float indexScale = 1f)
        {
            this.bodys = bodys;
            this.worldSize = new Vector3(worldSize, worldSize, worldSize);
            //Iniciarlizar PickingRay
            this.pickingRay = new TgcPickingRay();

            //Up
            planesLimits[0] = new PlaneBody(new Plane(0, 1, 0, worldSize),new Vector3(0, worldSize, 0), new Vector3(), new Vector3(), 0f);
            //Down
            planesLimits[1] = new PlaneBody(new Plane(0, -1, 0, -worldSize), new Vector3(0, -worldSize, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[1].idCode = planesLimits[0].idCode;
            //Front
            planesLimits[2] = new PlaneBody(new Plane(0, 0, 1, worldSize), new Vector3(0, 0, worldSize), new Vector3(), new Vector3(), 0f);
            planesLimits[2].idCode = planesLimits[0].idCode;
            //Back
            planesLimits[3] = new PlaneBody(new Plane(0, 0, -1, -worldSize), new Vector3(0, 0, -worldSize), new Vector3(), new Vector3(), 0f);
            planesLimits[3].idCode = planesLimits[0].idCode;
            //Right
            planesLimits[4] = new PlaneBody(new Plane(1, 0, 0, worldSize), new Vector3(worldSize, 0, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[4].idCode = planesLimits[0].idCode;
            //Left
            planesLimits[5] = new PlaneBody(new Plane(-1, 0, 0, -worldSize), new Vector3(-worldSize, 0, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[5].idCode = planesLimits[0].idCode;

            this._spatialHash = new SpatialHash(this.worldSize * 2, this.worldSize * indexSizeCell * indexScale, indexScale);
        }

        public World(List<Body> bodys, Vector3 worldSize, float indexSizeCell=0.1f, float indexScale=1f)
        {
            this.bodys = bodys;
            this.worldSize = worldSize;
            //Iniciarlizar PickingRay
            this.pickingRay = new TgcPickingRay();

            //Up
            planesLimits[0] = new PlaneBody(new Plane(0, 1, 0, worldSize.Y), new Vector3(0, worldSize.Y, 0), new Vector3(), new Vector3(), 0f);
            //Down
            planesLimits[1] = new PlaneBody(new Plane(0, -1, 0, -worldSize.Y), new Vector3(0, -worldSize.Y, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[1].idCode = planesLimits[0].idCode;
            //Front
            planesLimits[2] = new PlaneBody(new Plane(0, 0, 1, worldSize.Z), new Vector3(0, 0, worldSize.Z), new Vector3(), new Vector3(), 0f);
            planesLimits[2].idCode = planesLimits[0].idCode;
            //Back
            planesLimits[3] = new PlaneBody(new Plane(0, 0, -1, -worldSize.Z), new Vector3(0, 0, -worldSize.Z), new Vector3(), new Vector3(), 0f);
            planesLimits[3].idCode = planesLimits[0].idCode;
            //Right
            planesLimits[4] = new PlaneBody(new Plane(1, 0, 0, worldSize.X), new Vector3(worldSize.X, 0, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[4].idCode = planesLimits[0].idCode;
            //Left
            planesLimits[5] = new PlaneBody(new Plane(-1, 0, 0, -worldSize.X), new Vector3(-worldSize.X, 0, 0), new Vector3(), new Vector3(), 0f);
            planesLimits[5].idCode = planesLimits[0].idCode;

            this._spatialHash = new SpatialHash(new Vector3(this.worldSize.X / (this.worldSize.X * indexSizeCell) * 2 + 4,
                                                            this.worldSize.Y/(this.worldSize.Y*indexSizeCell)*2 + 4,
                                                            this.worldSize.Z/(this.worldSize.Z*indexSizeCell)*2 + 4), 
                                                            this.worldSize * indexSizeCell * indexScale, indexScale);
        }
        
        internal void optimize()
        {
            this._spatialHash.clear(bodys.Count);
            foreach (Body body in bodys)
            {
                //_octree.AddNode(body.position, body, body.BoundingBox);
                _spatialHash.add(body.BoundingBox.aabbMin, body.BoundingBox.aabbMax, body);
            }            
        }

        internal void addBody(Body body)
        {
            _spatialHash.add(body.BoundingBox.aabbMin, body.BoundingBox.aabbMax, body);            
        }

        internal void removeBody(Body body)
        {
            _spatialHash.remove(body.BoundingBox.aabbMin, body.BoundingBox.aabbMax, body);
            this.bodys.Remove(body);
        }

        internal void step(float timeStep)
        {
            int numSimulationSubSteps = 1;
            localTime += timeStep;
            float fixedTime = FIXED_TIME_STEP * timeSteps;
            if (localTime >= fixedTime)
            {
                numSimulationSubSteps = (int)(localTime / fixedTime);
                localTime -= numSimulationSubSteps * fixedTime;
            }
            
            int clampedSimulationSteps = (numSimulationSubSteps > MAX_STEPS) ? MAX_STEPS : numSimulationSubSteps;
            for (int i = 0; i < clampedSimulationSteps; i++)
            {
                this.integrateForce(fixedTime);
                this.doCollision();
                this.doSolveContacts(fixedTime);
                this.integrateVelocity(fixedTime);
            }
        }

        private void integrateForce(float timeStep)
        {
            foreach (Body body in bodys)
            {
                if (body.mass.Equals(0f) || float.IsNaN(body.velocity.X))
                {
                    continue;
                }
                
                body.integrateForceSI(timeStep);                
            }
        }

        private void integrateVelocity(float timeStep)
        {
            foreach (Body body in bodys)
            {
                if (body.mass.Equals(0f) || float.IsNaN(body.velocity.X))
                {
                    continue;
                }
                //Vector3 oldLocation = 
                    
                SpatialHashAABB oldAabb = body.BoundingBox;
                body.integrateVelocitySI(timeStep);
                body.position = FastMath.clamp(body.position, -worldSize, worldSize);
                SpatialHashAABB newAabb = body.BoundingBox;
                _spatialHash.update(oldAabb, newAabb, body);
                //_spatialHash.add(newAabb.aabbMin, newAabb.aabbMax, body);

                /*if ((body.LastUpdatePosition - body.position).LengthSq() > updatePostionDistance)
                {
                    _octree.RemoveHashNode(body.LastUpdatePosition, body);
                    _octree.AddNode(body.position, body, body.BoundingBox);
                    body.LastUpdatePosition = body.position;
                    if (!_octree.RemoveHashNode(body.LastUpdatePosition, body))
                        Logger.logInThread("No se puede borrar", Color.DarkRed);
                    if (!_octree.AddNode(body.position, body, body.BoundingBox))
                        Logger.logInThread("No se pudo agregar", Color.DarkRed);
                    
                }*/
            }
        }

        private void doCollision()
        {
            contacts.Clear();
            bool picking = false;
            //picking
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();
                picking = true;
            }

            foreach (Body bodyPivot in bodys)
            {
                if (bodyPivot.mass.Equals(0f) || float.IsNaN(bodyPivot.velocity.X))
                {
                    continue;
                }

                List<Contact> contactWorld = ContactBuilder.TestCollisionWorld(bodyPivot,
                                                                     planesLimits);

                //las coliciones con el mundo deben ser resultas tambien.
                //if (contactWorld != null)
                    contacts.AddRange(contactWorld);
                SpatialHashAABB aabb = bodyPivot.BoundingBox;
                ArrayList nearList = _spatialHash.getNeighbors(aabb.aabbMin, aabb.aabbMax);
                foreach (Body bodyNear in nearList)
                {
                    if (bodyPivot.Equals(bodyNear) ||
                        (bodyPivot.mass.Equals(0f)
                        && bodyNear.mass.Equals(0f)))
                    {
                        continue;
                    }
                    List<Contact> contact = ContactBuilder.TestCollision(bodyPivot,
                                                                     bodyNear);


                    //contact == null No hay colision.
                    if (contact != null)
                        contacts.AddRange(contact);
                        //if (!contacts.Add(contact))
                        // Logger.logInThread("El contacto esta repetido.", Color.DarkGoldenrod);
                }

                if (picking)
                {
                    if (applyRay)
                    {
                        List<Contact> contactPick = ContactBuilder.TestCollisionPick(bodyPivot,
                                                                                    pickingRay.Ray);
                        if (contactPick != null)
                            contacts.AddRange(contactPick);
                    }
                }
            }
            picking = false;
        }

        private void doSolveContacts(float timeStep)
        {
            foreach (Contact c in contacts)
			{
                if (debugMode)
                {
                    debugContacts.Add(c);
                }
                else
                {
                    debugContacts.Clear();
                }

                ContactSolver.solveSimpleContact(c, timeStep);
			}
        }

        public ArrayList debugContacts = new ArrayList(2000);
    }
}
