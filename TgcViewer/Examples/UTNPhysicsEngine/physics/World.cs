using System;
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
using System.Threading.Tasks;

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

        private const int MAX_STEPS = 20;
        private const float FIXED_TIME_STEP = 1f / 60f;
        private float localTime = 0f;        
        
        public List<Body> bodys;
        private Vector3 worldSize;
        //private ISet<Contact> contacts = new HashSet<Contact>(new ContactComparer());
        public List<Contact> contacts = new List<Contact>();//<Contact>();//new ContactComparer());
        private PlaneBody[] planesLimits = new PlaneBody[6];
        private TgcPickingRay pickingRay;

        public float timeSteps = 1f;
        public bool debugMode = false;
        public bool applyRay = false;
        public bool fixedWhitLerp = false;

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

            this._spatialHash = new SpatialHash(new Vector3(this.worldSize.X / (this.worldSize.X * indexSizeCell) * 2 + 6,
                                                            this.worldSize.Y / (this.worldSize.Y * indexSizeCell) * 2 + 6,
                                                            this.worldSize.Z / (this.worldSize.Z * indexSizeCell) * 2 + 6), 
                                                            this.worldSize * indexSizeCell * indexScale, indexScale);
        }

        public World(List<Body> bodys, Vector3 worldSize, float indexSizeCell = 0.1f, float indexScale = 1f, bool fixedWhitLerp=false)
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

            this._spatialHash = new SpatialHash(new Vector3(this.worldSize.X / (this.worldSize.X * indexSizeCell) * 2 + 6,
                                                            this.worldSize.Y/(this.worldSize.Y*indexSizeCell)*2 + 6,
                                                            this.worldSize.Z/(this.worldSize.Z*indexSizeCell)*2 + 6), 
                                                            this.worldSize * indexSizeCell * indexScale, indexScale);
            this.fixedWhitLerp = fixedWhitLerp;

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

        float acumulador = 0;
        internal void step(float timeStep)
        {
            if (fixedWhitLerp){
                float dt = FIXED_TIME_STEP * timeSteps;
                //Fix encontrado en http://gafferongames.com/game-physics/fix-your-timestep/
          
    //            if (timeStep > 0.25f)
    //                timeStep = 0.25f;	  // note: max frame time to avoid spiral of death
                acumulador += timeStep;
                bool cleanState = true;
                while (acumulador >= dt)
                {
                    //float fixedTime = FastMath.Min(timeStep, dt);
                    this.fixedStep(dt, cleanState);
                    acumulador -= dt;
                    cleanState = false;
                }
                //this.fixedStep(acumulador);
                //lerp(acumulador / dt);
            }
            else {
                float fixedTime = FIXED_TIME_STEP * timeSteps;
                localTime += timeStep;
                int numSimulationSubSteps = 1;

                if (localTime >= fixedTime)
                {
                    numSimulationSubSteps = (int)(localTime / fixedTime);
                    localTime -= numSimulationSubSteps * fixedTime;
                }

                int clampedSimulationSteps = (numSimulationSubSteps > MAX_STEPS) ? MAX_STEPS : numSimulationSubSteps;
                for (int i = 0; i < clampedSimulationSteps; i++)
                {
                    this.fixedStep(fixedTime, i==0);
                }
            }
        }
        
        private void lerp(float alpha)
        {
            if (alpha > float.Epsilon)
            {
                foreach (Body body in bodys)
                {
                    if (body.mass.Equals(0f) || float.IsNaN(body.velocity.X))
                    {
                        continue;
                    }

                    body.lerp(alpha);
                    //state.x = current.x * alpha + previous.x * (1 - alpha);
                    //state.v = current.v * alpha + previous.v * (1 - alpha);
                }
            }
        }

        private void fixedStep(float fixedTime, bool cleanState = false)
        {
            this.integrateForce(fixedTime);
            this.doCollision();
            this.doSolveContacts(fixedTime);
            this.integrateVelocity(fixedTime, cleanState);
        }

        private void integrateForce(float timeStep)
        {
            //foreach (Body body in bodys)
            Parallel.ForEach(bodys, body =>
            {
                if (!body.mass.Equals(0f) && !float.IsNaN(body.velocity.X))
                {
                    body.integrateForceSI(timeStep);
                }

            }
            );
        }

        private void integrateVelocity(float timeStep, bool cleanState)
        {
            object resultsLock = new object(); // globally visible
            //foreach (Body body in bodys)
            Parallel.ForEach(bodys, body =>
            {
                if (!body.mass.Equals(0f) && !float.IsNaN(body.velocity.X))
                {
                    SpatialHashAABB oldAabb = body.BoundingBox;
                    body.integrateVelocitySI(timeStep, cleanState);
                    body.position = FastMath.clamp(body.position, -worldSize, worldSize);
                    SpatialHashAABB newAabb = body.BoundingBox;
                    lock (resultsLock)
                    {
                        _spatialHash.update(oldAabb, newAabb, body);
                    }
                }

            }
            );
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
            object resultsLock = new object(); // globally visible
            //foreach (Body bodyPivot in bodys)
            Parallel.ForEach(bodys, bodyPivot =>
            {
                if (!bodyPivot.mass.Equals(0f) && !float.IsNaN(bodyPivot.velocity.X))
                {
                List<Contact> contactWorld = ContactBuilder.TestCollisionWorld(bodyPivot,
                                                                     planesLimits);

                //las coliciones con el mundo deben ser resultas tambien.
                if (contactWorld != null)
                    lock (resultsLock)
                    {
                        contacts.AddRange(contactWorld);
                    }
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
                        lock (resultsLock)
                        {
                            contacts.AddRange(contact);
                        }
                }
                    
                if (picking)
                {
                    if (applyRay)
                    {
                        List<Contact> contactPick = ContactBuilder.TestCollisionPick(bodyPivot,
                                                                                    pickingRay.Ray);
                        if (contactPick != null)
                            lock (resultsLock)
                            {
                                contacts.AddRange(contactPick);
                            }
                    }
                }
                }
            }
            );
            picking = false;
        }

        private void doSolveContacts(float timeStep)
        {
            foreach (Contact c in contacts)
            //Parallel.ForEach(contacts, c => No se puede paralelizar sin agregar potencias de velocidad.
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
