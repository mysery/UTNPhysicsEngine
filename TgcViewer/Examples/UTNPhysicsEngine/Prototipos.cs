using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using Examples.UTNPhysicsEngine;
using Examples.UTNPhysicsEngine.physics.body;
using Examples.UTNPhysicsEngine.physics;

namespace Examples
{
    /// <summary>
    /// UTNPhysicsEngine Main example
    /// </summary>
    public class Prototipos
    {
        private List<Body> bodys;

        internal List<Body> Bodys
        {
            get { return bodys; }
            set { bodys = value; }
        }
        public World world;
        private Device d3dDevice;
        private Mesh sphere;
        private TgcBox box;
        private Material[] sphereMat = new Material[6];
        public TgcBoundingBox limitsWorld;
        private Vector3 worldSize = new Vector3(500f, 500f, 500f);
        private TgcArrow debugContactArrow;

        public Vector3 WorldSize
        {
            get { return worldSize; }
            set { worldSize = value; }
        }

        public void init()
        {
            d3dDevice = GuiController.Instance.D3dDevice;

            Bodys = new List<Body>();
            this.createMesh();
            this.world = new World(Bodys, WorldSize, 0.1f,1f,false);
            this.limitsWorld = new TgcBoundingBox(  WorldSize*-1f,
                                                    WorldSize);
            
            //Crear una fuente de luz direccional en la posición 0.
            d3dDevice.Lights[0].Type = LightType.Directional;
            d3dDevice.Lights[0].Diffuse = Color.White;
            d3dDevice.Lights[0].Ambient = Color.White;
            d3dDevice.Lights[0].Specular = Color.White;
            //d3dDevice.Lights[0].Range = 10;
            d3dDevice.Lights[0].Position = new Vector3(5f, 5f, -10.0f);
            d3dDevice.Lights[0].Direction = new Vector3(0.0f, -1.0f, 1.0f);
            d3dDevice.Lights[0].Enabled = true;
            /*
            d3dDevice.Lights[1].Type = LightType.Point;
            d3dDevice.Lights[1].Diffuse = Color.Gray;
            d3dDevice.Lights[1].Ambient = Color.Gray;
            d3dDevice.Lights[1].Specular = Color.Gray;
            //d3dDevice.Lights[1].Range = 10;
            d3dDevice.Lights[1].Position = new Vector3();
            d3dDevice.Lights[1].Enabled = true;*/
            //Habilitar esquema de Iluminación Dinámica
            d3dDevice.RenderState.Lighting = false; //si lo pongo del inicio los bounding quedan feos.
            d3dDevice.RenderState.ShadeMode = ShadeMode.Gouraud;            

            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 150f;
            GuiController.Instance.FpsCamera.JumpSpeed = 150f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-WorldSize.X, WorldSize.Y, -WorldSize.Z), new Vector3(WorldSize.X, -WorldSize.Y, WorldSize.Z));

            GuiController.Instance.Modifiers.addBoolean(Constant.pause, Constant.pause_text, true);
            GuiController.Instance.Modifiers.addBoolean(Constant.debug, Constant.debug_text, false);
            GuiController.Instance.Modifiers.addFloat(Constant.timeSteps ,0.25f, 10f, 1f);
            GuiController.Instance.Modifiers.addBoolean(Constant.rayImpulse, Constant.rayImpulse_text, true);
            GuiController.Instance.Modifiers.addBoolean(Constant.addMode, Constant.addMode_text, false);
            GuiController.Instance.Modifiers.addFloat(Constant.radio, 1f, 100f, 20f);
            GuiController.Instance.Modifiers.addFloat(Constant.initVel, 0f, 200f, 50f);
            GuiController.Instance.Modifiers.addBoolean(Constant.gravity, Constant.gravity_text, true);
            GuiController.Instance.Modifiers.addVertex3f(Constant.acceleration, new Vector3(-10f, -10f, -10f), new Vector3(10f, 10f, 10f), new Vector3());
            GuiController.Instance.Modifiers.addFloat(Constant.mass, 0f, 10f, 1f);
            GuiController.Instance.Modifiers.addFloat(Constant.restitution, 0.1f, 1f, 1f);
            GuiController.Instance.UserVars.addVar(Constant.objectCount);
            GuiController.Instance.UserVars.addVar(Constant.contactCount);
            //Materials.
            sphereMat[0].Ambient = Color.DarkRed;
            sphereMat[0].Diffuse = Color.Red;
            sphereMat[0].Specular = Color.White;
            sphereMat[1].Ambient = Color.DarkBlue;
            sphereMat[1].Diffuse = Color.Blue;
            sphereMat[1].Specular = Color.White;
            sphereMat[2].Ambient = Color.DarkGreen;
            sphereMat[2].Diffuse = Color.Green;
            sphereMat[2].Specular = Color.White;
            sphereMat[3].Ambient = Color.DarkCyan;
            sphereMat[3].Diffuse = Color.Cyan;
            sphereMat[3].Specular = Color.White;
            sphereMat[4].Ambient = Color.DarkGoldenrod;
            sphereMat[4].Diffuse = Color.Goldenrod;
            sphereMat[4].Specular = Color.White;
            sphereMat[5].Ambient = Color.DarkViolet;
            sphereMat[5].Diffuse = Color.Violet;
            sphereMat[5].Specular = Color.White;

            debugContactArrow = TgcArrow.fromExtremes(new Vector3(), TgcArrow.ORIGINAL_DIR*5f, Color.Red, Color.Red, 0.1f, new Vector2(0.3f, 0.6f));
        }

        public void createMesh() 
        {
            // sphere
            sphere = Mesh.Sphere(d3dDevice, 1f, 15, 15);

            box = TgcBox.fromSize(new Vector3(1f, 1f, 1f),Color.DarkRed);
            box.AutoTransformEnable = false;            
        }

        private TgcPickingRay pickingRay = new TgcPickingRay();

        public void render(float elapsedTime)
        {
            world.applyRay = (bool)GuiController.Instance.Modifiers.getValue(Constant.rayImpulse);
            world.debugMode = (bool)GuiController.Instance.Modifiers.getValue(Constant.debug);
            world.timeSteps = (float)GuiController.Instance.Modifiers.getValue(Constant.timeSteps);

            if (!(bool)GuiController.Instance.Modifiers.getValue(Constant.pause))
            {
                //compute: integrate position, collition detect, contacts solvers
                world.step(elapsedTime);

                if (    GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT) &&
                        (bool)GuiController.Instance.Modifiers.getValue(Constant.addMode))
                {
                    pickingRay.updateRay();
                    float radius = (float)GuiController.Instance.Modifiers.getValue(Constant.radio);
                    Vector3 rV = new Vector3(radius, radius, radius)*2f;
                    Vector3 pos = FastMath.clamp(pickingRay.Ray.Origin, -WorldSize + rV, worldSize - rV);
                    
                    Vector3 acc = (Vector3)GuiController.Instance.Modifiers.getValue(Constant.acceleration);
                    
                    SphereBody sphereLeft = new SphereBody(radius,
                                                            pos,
                                                            pickingRay.Ray.Direction * (float)GuiController.Instance.Modifiers.getValue(Constant.initVel),
                                                            acc,
                                                            (float)GuiController.Instance.Modifiers.getValue(Constant.mass),
                                                            (bool)GuiController.Instance.Modifiers.getValue(Constant.gravity));
                    sphereLeft.restitution = (float)GuiController.Instance.Modifiers.getValue(Constant.restitution);
                    this.Bodys.Add(sphereLeft);
                    this.world.addBody(sphereLeft);
                }
                
            }
            int i = 0;
            d3dDevice.RenderState.Lighting = true;
            foreach (Body body in Bodys)
            {
                if (body is SphereBody)
                {
                    //Renderizar malla
                    d3dDevice.Material = sphereMat[i];
                    d3dDevice.Transform.World = body.getTrasform();
                    sphere.DrawSubset(0);
                    i++;
                    if (i > 5) i = 0;
                }
                else if (body is BoxBody)
                {
                    d3dDevice.RenderState.Lighting = false;
                    box.Transform = body.getTrasform();
                    box.render();
                    //Vector3 aabbMin, aabbMax;
                    //((BoxBody)body).getAABB(out aabbMin, out aabbMax);
                    //box.setExtremes(aabbMin, aabbMax);
                    box.render();
                    d3dDevice.RenderState.Lighting = true;
                }
            }
            d3dDevice.RenderState.Lighting = false;
            limitsWorld.render();
            
            //Debug del world:
            if ((bool)TgcViewer.GuiController.Instance.Modifiers.getValue(Constant.debug))
            {
                foreach (Contact c in world.debugContacts)
                {
                    //Obtener matriz de rotacion respecto del vector de la linea
                    Vector3 lineVec = c.normalContact;
                    float angle = FastMath.Acos(Vector3.Dot(TgcArrow.ORIGINAL_DIR, lineVec));
                    Vector3 axisRotation = Vector3.Cross(TgcArrow.ORIGINAL_DIR, lineVec);
                    axisRotation.Normalize();
                    debugContactArrow.transform = Matrix.RotationAxis(axisRotation, angle) * Matrix.Translation(c.positionContact);
                    debugContactArrow.render();
                }
                if (world.debugContacts.Count > 1000)
                {
                    world.debugContacts.RemoveRange(0, world.debugContacts.Count/8);
                }
            }

            GuiController.Instance.UserVars.setValue(Constant.objectCount, world.bodys.Count);
            GuiController.Instance.UserVars.setValue(Constant.contactCount, world.contacts.Count);
        }

        public void close()
        {
            limitsWorld.dispose();
            sphere.Dispose();
            debugContactArrow.dispose();
        }


        internal void optimize()
        {
            world.optimize();
        }
    }
}
