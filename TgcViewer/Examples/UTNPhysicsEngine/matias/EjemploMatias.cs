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
using Examples.UTNPhysicsEngine.physics.body;
using Examples.UTNPhysicsEngine.physics;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;
using Examples.Shaders;

namespace Examples.UTNPhysicsEngine.matias
{
    /// <summary>
    /// Ejemplo Matias
    /// </summary>
    public class EjemploMatias : TgcExample
    {

        //Cancha de basket: 28.65m by 15.24m
        private readonly Vector3 WORLD_EXTENTS = new Vector3(20, 20, 25);
        private readonly Vector3 VIENTO = new Vector3(2f, 0f, 0f);

        private World world;
        private TgcBoundingBox limitsWorld;
        private List<Body> bodys;
        Effect effectSphere;
        List<SphereElement> sphereElements;
        SphereType[] sphereTypes;
        TgcBox lightMesh;
        List<TgcMeshShader> meshesEscenario;
        List<BoxBody> rigidBoxes;
        List<SphereBody> invisibleSpheres;
        BoxBody tapaMovilBody;
        TgcMeshShader tapaMovilMesh;
        MyFpsCamera camera;
        TgcMeshShader pisoMesh;
        List<TgcMeshShader> reflectedMesh = new List<TgcMeshShader>();
        
        
        public override string getCategory()
        {
            return "Prototipo presentación";
        }

        public override string getName()
        {
            return "Cancha de Basquet";
        }

        public override string getDescription()
        {
            return "El prototipo contiene gran cantidad de modelos prefijados, con las variables para agregar cuerpos definidas segun cada objeto, para la presentacion de UTNPhysicsEngine                 "+
                    "Cancha de basket: 28.65m por 15.24m.                                   "+
                    "Pelota de Basket: 24.257 cm diametro, 650g.                              " +
                    "Pelota de Tennis: 6.7cm diametro, 60g.                                        " +
                    "Pelota de Futbol: 22cm diametro, 450g.                                 ";
        }


        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //World
            this.bodys = new List<Body>();
            this.limitsWorld = new TgcBoundingBox(-WORLD_EXTENTS, WORLD_EXTENTS);
            this.world = new World(this.bodys, WORLD_EXTENTS, 0.05f);
            this.world.timeSteps = 0.35f;

            //Cargar shader para esferas
            string compilationErrors;
            string shaderPath = GuiController.Instance.ExamplesMediaDir + "Shaders\\PointLightShader.fx";
            this.effectSphere = Effect.FromFile(GuiController.Instance.D3dDevice, shaderPath, null, null, ShaderFlags.None, null, out compilationErrors);
            if (effectSphere == null)
            {
                throw new Exception("Error al cargar shader: " + shaderPath + ". Errores: " + compilationErrors);
            }


            //Crear tipos de meshes de esfera
            sphereElements = new List<SphereElement>();
            sphereTypes = new SphereType[3];
            SphereType sphereType;
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new CustomMeshShaderFactory();

            //Basket: 24.257 cm diametro, 650g
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Basketball\\Basketball-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 0.24257f / 2;
            sphereType.mass = 0.65f;
            sphereType.restitution = 0.85f;
            sphereTypes[0] = sphereType;

            //Tennis, 6.7cm diametro, 60g
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Tennis\\Tennis-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 0.067f / 2;
            sphereType.mass = 0.06f;
            sphereType.restitution = 0.85f;
            sphereTypes[1] = sphereType;

            //Soccer, 22cm diametro, 450g
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Soccer\\Soccer-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 0.22f / 2;
            sphereType.mass = 0.45f;
            sphereType.restitution = 0.6f;
            sphereTypes[2] = sphereType;



            //Cargar escenario
            TgcSceneLoader loaderEscenario = new TgcSceneLoader();
            loaderEscenario.MeshFactory = new CustomMeshShaderFactory();
            TgcScene scene = loaderEscenario.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "CanchaBasket\\CanchaBasket-TgcScene.xml");
            

            //Separar meshes segun layer
            this.meshesEscenario = new List<TgcMeshShader>();
            this.rigidBoxes = new List<BoxBody>();
            this.invisibleSpheres = new List<SphereBody>();
            int i = 0;
            foreach (TgcMeshShader m in scene.Meshes)
            {
                //Setear shader
                m.Effect = effectSphere;
                
                //Cajas rigidas
                if (m.Layer == "boundingBox")
                {
                    //Crear Box para colision
                    BoxBody body = new BoxBody(Matrix.Identity, m.BoundingBox.calculateAxisRadius(), m.BoundingBox.calculateBoxCenter(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f);
                    this.world.addBody(body);

                    //Tapa de canasto que se puede remover
                    if (m.Name == "TapaMovil")
                    {
                        this.tapaMovilBody = body;
                        this.tapaMovilMesh = m;
                    }
                    else
                    {
                        this.rigidBoxes.Add(body);

                        //Son visibles
                        this.meshesEscenario.Add(m);
                        if (m.Name == "Plane003")
                        {
                            pisoMesh = m;
                        }
                        if (m.Name == "Box039" || 
                            m.Name == "Box040" || 
                            m.Name == "Box041" || 
                            m.Name == "Box043" || 
                            m.Name == "Box042" || 
                            m.Name == "Box043" || 
                            m.Name == "Box044" || 
                            m.Name == "Box045" || 
                            m.Name == "Box046" ||
                            m.Name == "Box006" ||
                            m.Name == "Box011" ||
                            m.Name == "Box012" ||
                            m.Name == "Box016" ||
                            m.Name == "Box017" ||
                            m.Name == "Box018")
                        {
                            reflectedMesh.Add(m);
                        }                        
                    }


                    
                }
                //Esferas rigidas invisibles
                else if (m.Layer == "boundingSphereInvisibles")
                {
                    //Crear body para esfera
                    SphereBody body = new SphereBody(0.118f, m.BoundingBox.calculateBoxCenter(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, false);
                    body.restitution = 0.8f;
                    this.world.addBody(body);
                    this.invisibleSpheres.Add(body);
                }
                //Pelota visibles y que son SphereBody
                else if (m.Layer == "pelotas")
                {
                    //Crear pelota de basket
                    SphereElement sphereElement = new SphereElement();

                    //Tipo de esfera Basket
                    sphereElement.type = sphereTypes[i++];
                    i = i % 3;
                    //Crear cuerpo de esfera
                    sphereElement.body = new SphereBody(sphereElement.type.radius, m.BoundingBox.calculateBoxCenter(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), sphereElement.type.mass);
                    sphereElement.body.restitution = sphereElement.type.restitution;

                    //Agregar al world
                    this.bodys.Add(sphereElement.body);
                    this.world.addBody(sphereElement.body);

                    //Agregar a lista de elementos
                    this.sphereElements.Add(sphereElement);
                }
                //Demas objetos de adorno del escenario
                else
                {
                    //Son visibles
                    this.meshesEscenario.Add(m);
                    if (m.Name == "Torus001" ||
                       m.Name == "Torus003")
                    {
                        reflectedMesh.Add(m);
                    }  
                }

            }

            //Optimizar objetos estaticos
            //world.optimize();



            //Camara personalizada
            camera = new MyFpsCamera();
            camera.Enable = true;
            camera.MovementSpeed = 0.1f;
            camera.JumpSpeed = 0.1f;
            camera.RotationSpeed = 4f;
            camera.setCamera(new Vector3(0, -WORLD_EXTENTS.Y + 1f, 0), new Vector3(0, -WORLD_EXTENTS.Y + 1f, -1));


            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(0, WORLD_EXTENTS.Y / 2f, 0), new Vector3(1, 1, 1), Color.Red);


            //Modifiers
            GuiController.Instance.Modifiers.addInterval("Pelota", new string[] { "Basket", "Tenis", "Futbol" }, 0);
            GuiController.Instance.Modifiers.addInterval("Cantidad", new string[] { "1", "2", "3", "4", "5"}, 0);
            GuiController.Instance.Modifiers.addBoolean("Tapa de Cajon", "Tapa de Cajon", true);
            GuiController.Instance.Modifiers.addBoolean("Aplicar Viento", "Aplicar Viento", false);
            GuiController.Instance.Modifiers.addBoolean("Aplicar Anti-Gravedad", "Aplicar Anti-Gravedad", false);            

            //Variables.
            GuiController.Instance.UserVars.addVar(Constant.objectCount);
            GuiController.Instance.UserVars.addVar(Constant.contactCount);
        }

        public override void render(float elapsedTime)
        {
            //compute: integrate position, collition detect, contacts solvers
            world.step(elapsedTime);
            
            //Agregar nueva esfera
            if (GuiController.Instance.D3dInput.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                dispararPelota();
            }

            GuiController.Instance.D3dDevice.Clear(ClearFlags.Stencil, 0, 1f, 0);
            GuiController.Instance.D3dDevice.RenderState.StencilPass = StencilOperation.Replace;
            GuiController.Instance.D3dDevice.RenderState.StencilFunction = Compare.Always;
            GuiController.Instance.D3dDevice.RenderState.StencilEnable = true;
            GuiController.Instance.D3dDevice.RenderState.ReferenceStencil = 1;
            GuiController.Instance.D3dDevice.RenderState.ZBufferWriteEnable = false;
            GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.Clockwise;
            pisoMesh.render();
            GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.None;

            GuiController.Instance.D3dDevice.RenderState.StencilPass = StencilOperation.Keep;
            GuiController.Instance.D3dDevice.RenderState.StencilFunction = Compare.Equal;

            bool cajon = (bool)GuiController.Instance.Modifiers["Tapa de Cajon"];
            if (cajon)
            {
                GuiController.Instance.D3dDevice.RenderState.ZBufferWriteEnable = true;
                //-15.1378f ... -10f -19.71213 -39.58f
                tapaMovilMesh.Position = new Vector3(0f, -9.2f, 0f);
                tapaMovilMesh.AlphaBlendEnable = true;
                tapaMovilMesh.ZbufferDisable = false;
                tapaMovilMesh.Effect.Technique = "DIFFUSE_MAP";
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.Clockwise;
                tapaMovilMesh.render();
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.None;
                tapaMovilMesh.AlphaBlendEnable = false;
                tapaMovilMesh.ZbufferDisable = false;
            }

            foreach (TgcMeshShader reflected in reflectedMesh)
            {
                GuiController.Instance.D3dDevice.RenderState.ZBufferWriteEnable = true;
                reflected.Position = new Vector3(0f, -39.58f, 0f); //-12.8f //-13.0f
                reflected.Rotation = new Vector3(0f, 0f, FastMath.PI);
                reflected.AlphaBlendEnable = true;
                reflected.ZbufferDisable = false;
                reflected.Effect.Technique = "DIFFUSE_MAP";
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.Clockwise;
                reflected.render();
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.None;
                reflected.AlphaBlendEnable = false;
                reflected.ZbufferDisable = false;
            }

            foreach (SphereElement sphereElement in this.sphereElements)
            {
                //Aplicar transformacion al mesh                
                Matrix matWorld = sphereElement.body.getTrasform(true, -39.58f);
                sphereElement.type.mesh.Transform = matWorld;
                //matWorld.M22 = -matWorld.M22; no hace falta ya que se tiene el Cull.CounterClockwise da el efecto de rotacion.
                
                //Render mesh
                sphereElement.type.mesh.AlphaBlendEnable = true;
                sphereElement.type.mesh.ZbufferDisable = false;
                sphereElement.type.mesh.Effect.Technique = "DIFFUSE_MAP";
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.CounterClockwise;
                sphereElement.type.mesh.render();
                GuiController.Instance.D3dDevice.RenderState.CullMode = Cull.None;
                sphereElement.type.mesh.AlphaBlendEnable = false;
                sphereElement.type.mesh.ZbufferDisable = false;
            }
            
            //GuiController.Instance.D3dDevice.Clear(ClearFlags.ZBuffer, 0, 1f, 0);
            
            GuiController.Instance.D3dDevice.RenderState.StencilEnable = false;

            GuiController.Instance.D3dDevice.RenderState.ZBufferWriteEnable = true;
            ColorWriteEnable col = GuiController.Instance.D3dDevice.RenderState.ColorWriteEnable;
            GuiController.Instance.D3dDevice.RenderState.ColorWriteEnable = 0;
            pisoMesh.render();
            GuiController.Instance.D3dDevice.RenderState.ColorWriteEnable = col;
            
            if (cajon)
            {
                if (!tapaMovilMesh.Enabled)
                {
                    tapaMovilMesh.Enabled = true;
                    world.addBody(tapaMovilBody);
                }
                tapaMovilMesh.Position = new Vector3();
                tapaMovilMesh.Effect.Technique = "DIFFUSE_MAP";
                tapaMovilMesh.render();
            }
            else
            {
                if (tapaMovilMesh.Enabled)
                {
                    tapaMovilMesh.Enabled = false;
                    world.removeBody(tapaMovilBody);
                }
            }

            foreach (TgcMeshShader reflected in reflectedMesh)
            {
                reflected.Position = Vector3.Empty;
                reflected.Rotation = Vector3.Empty;
                reflected.Effect.Technique = "DIFFUSE_MAP";
                reflected.render();
            }

            ////Cargar variables shader de la luz
            effectSphere.SetValue("lightColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightMesh.Position));
            effectSphere.SetValue("lightIntensity", 2f);
            effectSphere.SetValue("lightAttenuation", 0.2f);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            effectSphere.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
            effectSphere.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.FromArgb(110, Color.White)));
            effectSphere.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("materialSpecularExp", 20f);

            //Render balas
            foreach (SphereElement sphereElement in this.sphereElements)
            {
                //Hack rotacion.
                Vector3 rotacion = Vector3.Normalize(new Vector3(sphereElement.body.velocity.Z, 0, -sphereElement.body.velocity.X));
                float angulo = (new Vector3(sphereElement.body.position.X, 0, sphereElement.body.position.Z) - new Vector3(sphereElement.body.lastUpdatePosition.X, 0, sphereElement.body.lastUpdatePosition.Z)).Length() / sphereElement.body.Radius;
                if (rotacion.Length() > float.Epsilon && angulo > float.Epsilon)
                    sphereElement.body.lastRotation = sphereElement.body.lastRotation * Matrix.RotationAxis(rotacion, angulo);
                else
                    sphereElement.body.lastRotation = Matrix.Identity;

                //Aplicar transformacion al mesh
                Matrix matWorld = sphereElement.body.getTrasform();
                sphereElement.type.mesh.Transform = matWorld;
                //Render mesh
                sphereElement.type.mesh.Effect.Technique = "DIFFUSE_MAP";
                sphereElement.type.mesh.render();

                //Aplica una fuerza de viento.
                if ((bool)GuiController.Instance.Modifiers["Aplicar Viento"])
                {
                    sphereElement.body.externalForce = VIENTO;
                }
                
                //Aplica una fuerza anti gravedad.
                if ((bool)GuiController.Instance.Modifiers["Aplicar Anti-Gravedad"])
                {
                    if (!(bool)GuiController.Instance.Modifiers["Aplicar Viento"])
                        sphereElement.body.externalForce = sphereElement.body.aceleracion*-1f; //Solo aplica gravedad
                    else
                        sphereElement.body.externalForce = sphereElement.body.aceleracion * -1f + VIENTO; // aplica gravedad y viendo 
                }
                else
                {
                    if (!(bool)GuiController.Instance.Modifiers["Aplicar Viento"])
                        sphereElement.body.externalForce = Vector3.Empty;
                }
            }


            //Render escenario
            foreach (TgcMeshShader mesh in this.meshesEscenario)
            {
                if (mesh != pisoMesh &&  !reflectedMesh.Contains(mesh))
                    mesh.render();
                
            }

            //Renderizar mesh de luz
            lightMesh.render();

            GuiController.Instance.UserVars.setValue(Constant.objectCount, world.bodys.Count);
            GuiController.Instance.UserVars.setValue(Constant.contactCount, world.contacts.Count);

            //Render con alfablending y sin Zbuffer
            //GuiController.Instance.D3dDevice.RenderState.ZBufferEnable = false;
            //GuiController.Instance.D3dDevice.RenderState.AlphaBlendEnable = false;
           
            //GuiController.Instance.D3dDevice.RenderState.AlphaBlendEnable = true;
            //GuiController.Instance.D3dDevice.RenderState.ZBufferEnable = true;

            

            //Dibujar limites del escenario
            //limitsWorld.render();

        }

        /// <summary>
        /// Accion de disparar una nueva esfera
        /// </summary>
        private void dispararPelota()
        {
            Vector3 cameraPos = camera.getPosition();
            Vector3 cameraLookAt = camera.getLookAt();

            //Tipo de esfera
            string tipoObjeto = (string)GuiController.Instance.Modifiers["Pelota"];
            SphereType sphereType;
            if (tipoObjeto == "Basket") sphereType = sphereTypes[0];
            else if (tipoObjeto == "Tenis") sphereType = sphereTypes[1];
            else sphereType = sphereTypes[2];

            //Crear n esferas
            int cantidad = int.Parse((string)GuiController.Instance.Modifiers["Cantidad"]);
            for (int i = 0; i < cantidad; i++)
            {
                //Crear elemento de esfera
                SphereElement sphereElement = new SphereElement();
                sphereElement.type = sphereType;

                Vector3 direccionTiro = cameraLookAt - cameraPos;
                //Parametros de esfera
                float initialX = cameraPos.X - (cantidad - 1) * sphereElement.type.radius;
                float initialZ = cameraPos.Z - (cantidad - 1) * sphereElement.type.radius;
                Vector3 position = new Vector3(initialX + i * sphereElement.type.radius * 2,
                                        cameraPos.Y,
                                        initialZ + i * sphereElement.type.radius * 2);
                position = FastMath.clamp(position, -WORLD_EXTENTS, WORLD_EXTENTS);
                //Vector3 position = cameraPos + new Vector3(i * sphereElement.type.radius * 2f, i * sphereElement.type.radius * 2f, 0);
                Vector3 velocity = Vector3.Normalize(direccionTiro) * 10;
                Vector3 acceleration = new Vector3();// por default es la gravedad. (0.0f, -9.8f, 0.0f)*masa;

                //Crear cuerpo de esfera
                sphereElement.body = new SphereBody(sphereElement.type.radius, position, velocity, acceleration, sphereElement.type.mass);
                sphereElement.body.restitution = sphereElement.type.restitution;

                //Agregar al world
                this.bodys.Add(sphereElement.body);
                this.world.addBody(sphereElement.body);

                //Agregar a lista de elementos
                this.sphereElements.Add(sphereElement);
            }

            if (this.bodys.Count > 200)
            {
                for (int i = 0; i < cantidad; i++)
                {
                    //Agregar al world
                    SphereElement sphereElement = this.sphereElements[0];
                    this.world.removeBody(sphereElement.body);
                    this.bodys.Remove(sphereElement.body);
                    this.sphereElements.Remove(sphereElement);
                }
            }

        }

        public override void close()
        {
            limitsWorld.dispose();
            lightMesh.dispose();
            foreach (TgcMeshShader mesh in this.meshesEscenario)
            {
                mesh.dispose();
            }
            foreach (SphereType t in this.sphereTypes)
            {
                t.mesh.dispose();
            }
        }


        /// <summary>
        /// Estructura auxiliar para almacenar cada objeto esfera creado
        /// </summary>
        public class SphereElement
        {
            public SphereType type;
            public SphereBody body;
        }


        /// <summary>
        /// Distintos tipos de esferas (textura, masa, etc) para reutilizar
        /// </summary>
        public class SphereType
        {
            public TgcMeshShader mesh;
            public float radius;
            public float mass;
            public float restitution;
        }


    }
}
