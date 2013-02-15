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


        private World world;
        private TgcBoundingBox limitsWorld;
        private List<Body> bodys;
        Effect effectSphere;
        List<SphereElement> sphereElements;
        SphereType[] sphereTypes;
        TgcBox lightMesh;
        int currentType = 0;
        List<TgcMeshShader> meshesEscenario;
        List<BoxBody> rigidBoxes;
        List<SphereBody> invisibleSpheres;


        public override string getCategory()
        {
            return "Matias";
        }

        public override string getName()
        {
            return "Ejemplo matias";
        }

        public override string getDescription()
        {
            return "Ejemplo matias";
        }


        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //World
            this.bodys = new List<Body>();
            this.limitsWorld = new TgcBoundingBox(-WORLD_EXTENTS, WORLD_EXTENTS);
            this.world = new World(this.bodys, Vector3.Scale(WORLD_EXTENTS, 2));

            //Cargar shader para esferas
            string compilationErrors;
            string shaderPath = GuiController.Instance.ExamplesMediaDir + "Shaders\\PointLightShader.fx";
            this.effectSphere = Effect.FromFile(GuiController.Instance.D3dDevice, shaderPath, null, null, ShaderFlags.None, null, out compilationErrors);
            if (effectSphere == null)
            {
                throw new Exception("Error al cargar shader: " + shaderPath + ". Errores: " + compilationErrors);
            }


            //Cargar escenario
            TgcSceneLoader loaderEscenario = new TgcSceneLoader();
            loaderEscenario.MeshFactory = new CustomMeshShaderFactory();
            TgcScene scene = loaderEscenario.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "CanchaBasket\\CanchaBasket-TgcScene.xml");
            

            //Separar meshes segun layer
            this.meshesEscenario = new List<TgcMeshShader>();
            this.rigidBoxes = new List<BoxBody>();
            this.invisibleSpheres = new List<SphereBody>();
            foreach (TgcMeshShader m in scene.Meshes)
            {
                m.Effect = effectSphere;
                
                //Cajas rigidas
                if (m.Layer == "boundingBox")
                {
                    //Crear Box para colision
                    BoxBody body = new BoxBody(Matrix.Identity, m.BoundingBox.calculateAxisRadius(), m.BoundingBox.calculateBoxCenter(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f);
                    this.world.addBody(body);
                    this.rigidBoxes.Add(body);

                    //Son visibles
                    this.meshesEscenario.Add(m);
                }
                //Esferas rigidas invisibles
                else if (m.Layer == "boundingSphereInvisibles")
                {
                    SphereBody body = new SphereBody(0.118f, m.BoundingBox.calculateBoxCenter(), new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, false);
                    body.restitution = 0.8f;
                    this.world.addBody(body);
                    this.invisibleSpheres.Add(body);
                }
                //Demas objetos de adorno del escenario
                else
                {
                    //Son visibles
                    this.meshesEscenario.Add(m);
                }

            }

            //Optimizar objetos estaticos
            world.optimize();




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
            


            //Camera
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 5;
            GuiController.Instance.FpsCamera.JumpSpeed = 5f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(0, -WORLD_EXTENTS.Y + 1f, 0), new Vector3(0, -WORLD_EXTENTS.Y + 1f, 1));


            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(0, WORLD_EXTENTS.Y / 2f, 0), new Vector3(1, 1, 1), Color.Red);

        }

        public override void render(float elapsedTime)
        {
            //compute: integrate position, collition detect, contacts solvers
            world.step(elapsedTime);

            Vector3 cameraPos = GuiController.Instance.FpsCamera.getPosition();
            Vector3 cameraLookAt = GuiController.Instance.FpsCamera.getLookAt();

            //Agregar nueva esfera
            if (GuiController.Instance.D3dInput.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Crear elemento de esfera
                SphereElement sphereElement = new SphereElement();

                //Tipo de esfera
                sphereElement.type = sphereTypes[++currentType % sphereTypes.Length];

                //Parametros de esfera
                Vector3 position = cameraPos;
                Vector3 velocity = Vector3.Normalize(cameraLookAt - cameraPos) * 10;
                Vector3 acceleration = new Vector3();// por default es la gravedad. (0.0f, -9.8f, 0.0f)*masa;

                //Crear cuerpo de esfera
                sphereElement.body = new SphereBody(sphereElement.type.radius, position, velocity, acceleration, sphereElement.type.mass);//no hace falta el true, tiene valor default.
                sphereElement.body.restitution = sphereElement.type.restitution;

                
                

                //Agregar al world
                this.bodys.Add(sphereElement.body);
                this.world.addBody(sphereElement.body);

                //Agregar a lista de elementos
                this.sphereElements.Add(sphereElement);
            }


            //Cargar variables shader de la luz
            effectSphere.SetValue("lightColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightMesh.Position));
            effectSphere.SetValue("lightIntensity", 2f);
            effectSphere.SetValue("lightAttenuation", 0.2f);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            effectSphere.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.FromArgb(50, 50, 50)));
            effectSphere.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            effectSphere.SetValue("materialSpecularExp", 20f);


            //Render balas
            foreach (SphereElement sphereElement in this.sphereElements)
            {
                //Aplicar transformacion al mesh
                Matrix matWorld = sphereElement.body.getTrasform();
                sphereElement.type.mesh.Transform = matWorld;

                //Render mesh
                sphereElement.type.mesh.render();

            }



            //Render escenario
            foreach (TgcMeshShader mesh in this.meshesEscenario)
            {
                mesh.render();
            }

            //Renderizar mesh de luz
            lightMesh.render();

            //Dibujar limites del escenario
            //limitsWorld.render();

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
