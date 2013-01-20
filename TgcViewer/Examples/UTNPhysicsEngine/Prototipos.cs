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
        private World world;
        private Device d3dDevice;
        private Mesh sphere;
        private TgcBox box;
        private Material[] sphereMat = new Material[6];
        private TgcBoundingBox limitsWorld;
        private float worldSize = 500f;

        public float WorldSize
        {
            get { return worldSize; }
            set { worldSize = value; }
        }

        public void init()
        {
            d3dDevice = GuiController.Instance.D3dDevice;

            Bodys = new List<Body>();
            this.createMesh();
            this.world = new World(Bodys, WorldSize);
            this.limitsWorld = new TgcBoundingBox(  new Vector3(-WorldSize, -WorldSize, -WorldSize),
                                                    new Vector3(WorldSize, WorldSize, WorldSize));
            
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
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-WorldSize, WorldSize, -WorldSize), new Vector3(WorldSize, -WorldSize, WorldSize));

            GuiController.Instance.Modifiers.addBoolean("pause", "Pause Simulation", true);
            GuiController.Instance.Modifiers.addBoolean("debug", "Muestra objetos para debug", false);
            GuiController.Instance.Modifiers.addFloat("timeSteps",0.25f, 10f, 1f);
            GuiController.Instance.Modifiers.addBoolean("rayImpulse", "Aplica un impulso al cuerpo", true);
            GuiController.Instance.Modifiers.addBoolean("addBody", "Agrega un cuerpo segun las definiciones", false);

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
        }

        public void createMesh() 
        {
            // sphere
            sphere = Mesh.Sphere(d3dDevice, 1f, 10, 10);

            box = TgcBox.fromSize(new Vector3(1f, 1f, 1f),Color.DarkRed);
            box.AutoTransformEnable = false;            
        }

        private TgcPickingRay pickingRay = new TgcPickingRay();

        public void render(float elapsedTime)
        {
            if (!(bool)GuiController.Instance.Modifiers.getValue("pause"))
            {
                //compute: integrate position, collition detect, contacts solvers
                world.step(elapsedTime);

                if (    GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_RIGHT) &&
                        (bool)GuiController.Instance.Modifiers.getValue("addBody"))
                {
                    pickingRay.updateRay();
                    float radius = 10f;

                    Vector3 pos =  FastMath.clampVector(pickingRay.Ray.Origin, -worldSize + 2 * radius, worldSize - 2 * radius);
                    
                    
                    SphereBody sphereLeft = new SphereBody(radius,
                                                            pos,
                                                            pickingRay.Ray.Direction * 50f,
                                                            new Vector3(0.0f, -9.8f, 0.0f),
                                                            radius/FastMath.TWO_PI);
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
            //world.Octree.render();
        }

        public void close()
        {
            limitsWorld.dispose();
            sphere.Dispose();
            //world.Octree.dispose();
        }


        internal void optimize()
        {
            world.optimize();
        }
    }
}
