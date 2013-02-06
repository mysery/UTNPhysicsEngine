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

        private World world;
        private TgcBoundingBox limitsWorld;
        private List<Body> bodys;
        Effect effectSphere;
        List<SphereElement> sphereElements;
        SphereType[] sphereTypes;
        TgcBox lightMesh;
        int currentType = 0;
        List<TgcMeshShader> meshesEscenario;


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
            float worldSize = 300f;
            this.world = new World(this.bodys, worldSize);
            this.limitsWorld = new TgcBoundingBox(new Vector3(-worldSize, -worldSize, -worldSize), new Vector3(worldSize, worldSize, worldSize));
            world.optimize();

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
            this.meshesEscenario = new List<TgcMeshShader>();
            foreach (TgcMeshShader m in scene.Meshes)
            {
                m.Effect = effectSphere;
                this.meshesEscenario.Add(m);
            }



            //Crear tipos de meshes de esfera
            sphereElements = new List<SphereElement>();
            sphereTypes = new SphereType[3];
            SphereType sphereType;
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new CustomMeshShaderFactory();
            
            //Basket
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Basketball\\Basketball-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 20f;
            sphereType.mass = 20f;
            sphereType.restitution = 0.8f;
            sphereTypes[0] = sphereType;

            //Tennis
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Tennis\\Tennis-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 5f;
            sphereType.mass = 5f;
            sphereType.restitution = 0.9f;
            sphereTypes[1] = sphereType;

            //Soccer
            sphereType = new SphereType();
            sphereType.mesh = (TgcMeshShader)loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "Balls\\Soccer\\Soccer-TgcScene.xml").Meshes[0];
            sphereType.mesh.AutoTransformEnable = false;
            sphereType.mesh.Effect = effectSphere;
            sphereType.radius = 16f;
            sphereType.mass = 16f;
            sphereType.restitution = 0.7f;
            sphereTypes[2] = sphereType;
            


            //Camera
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 150f;
            GuiController.Instance.FpsCamera.JumpSpeed = 150f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-worldSize, worldSize, -worldSize), new Vector3(worldSize, -worldSize, worldSize));


            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(0, 250, 0), new Vector3(10, 10, 10), Color.Red);

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
                Vector3 acceleration = new Vector3(0, 0, 0);

                //Crear cuerpo de esfera
                sphereElement.body = new SphereBody(sphereElement.type.radius, position, velocity, acceleration, sphereElement.type.mass, true);
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
            effectSphere.SetValue("lightIntensity", 20f);
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
            limitsWorld.render();

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
