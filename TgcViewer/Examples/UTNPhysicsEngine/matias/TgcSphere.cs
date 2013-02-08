using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace Examples.UTNPhysicsEngine.matias
{
    /// <summary>
    /// Herramienta para crear una Esfera 3D de tamaño variable, con color y Textura
    /// </summary>
    public class TgcSphere : IRenderObject, ITransformObject
    {


        CustomVertex.PositionNormalTextured[] vertices;
        VertexBuffer vertexBuffer;
        VertexDeclaration vertexDeclaration;
        int slices;
        int stacks;


        float radius;
        /// <summary>
        /// Radio de la esfera
        /// </summary>
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        /// <summary>
        /// Escala de la caja. Siempre es (1, 1, 1).
        /// Utilizar Radius
        /// </summary>
        public Vector3 Scale
        {
            get { return new Vector3(1, 1, 1); }
            set { ; }
        }

        Color color;
        /// <summary>
        /// Color de los vértices de la caja
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        TgcTexture texture;
        /// <summary>
        /// Textura de la caja
        /// </summary>
        public TgcTexture Texture
        {
            get { return texture; }
        }

        protected Effect effect;
        /// <summary>
        /// Shader del mesh
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        protected string technique;
        /// <summary>
        /// Technique que se va a utilizar en el effect.
        /// Cada vez que se llama a render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return technique; }
            set { technique = value; }
        }

        Matrix transform;
        /// <summary>
        /// Matriz final que se utiliza para aplicar transformaciones a la malla.
        /// Si la propiedad AutoTransformEnable esta en True, la matriz se reconstruye en cada cuadro
        /// en base a los valores de: Position, Rotation, Scale.
        /// Si AutoTransformEnable está en False, se respeta el valor que el usuario haya cargado en la matriz.
        /// </summary>
        public Matrix Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        bool autoTransformEnable;
        /// <summary>
        /// En True hace que la matriz de transformacion (Transform) de la malla se actualiza en
        /// cada cuadro en forma automática, según los valores de: Position, Rotation, Scale.
        /// En False se respeta lo que el usuario haya cargado a mano en la matriz.
        /// Por default está en True.
        /// </summary>
        public bool AutoTransformEnable
        {
            get { return autoTransformEnable; }
            set { autoTransformEnable = value; }
        }

        private Vector3 translation;
        /// <summary>
        /// Posicion absoluta del centro de la esfera
        /// </summary>
        public Vector3 Position
        {
            get { return translation; }
            set
            {
                translation = value;
                updateBoundingSphere();
            }
        }

        private Vector3 rotation;
        /// <summary>
        /// Rotación absoluta de la esfera
        /// </summary>
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private bool enabled;
        /// <summary>
        /// Indica si la esfera esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        private TgcBoundingSphere boundingSphere;
        /// <summary>
        /// BoundingBox de la caja
        /// </summary>
        public TgcBoundingSphere BoundingBox
        {
            get { return boundingSphere; }
        }

        private bool alphaBlendEnable;
        /// <summary>
        /// Habilita el renderizado con AlphaBlending para los modelos
        /// con textura o colores por vértice de canal Alpha.
        /// Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable
        {
            get { return alphaBlendEnable; }
            set { alphaBlendEnable = value; }
        }

        Vector2 uvOffset;
        /// <summary>
        /// Offset UV de textura
        /// </summary>
        public Vector2 UVOffset
        {
            get { return uvOffset; }
            set { uvOffset = value; }
        }

        Vector2 uvTiling;
        /// <summary>
        /// Tiling UV de textura
        /// </summary>
        public Vector2 UVTiling
        {
            get { return uvTiling; }
            set { uvTiling = value; }
        }


        /// <summary>
        /// Crea una esfera vacia
        /// </summary>
        public TgcSphere()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;


            //Valores por defecto
            this.radius = 50;
            this.slices = 15;
            this.stacks = 10;

            int stripVertCount = (slices * 2 + 2) * stacks;
            int expandVertCount = (stripVertCount - 2) * 3;
            vertices = new CustomVertex.PositionNormalTextured[expandVertCount];
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), vertices.Length, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
            vertexDeclaration = new VertexDeclaration(d3dDevice, TgcSphere.PositionNormalTextured_VertexElements);

            this.autoTransformEnable = true;
            this.transform = Matrix.Identity;
            this.translation = new Vector3(0, 0, 0);
            this.rotation = new Vector3(0, 0, 0);
            this.enabled = true;
            this.color = Color.White;
            this.alphaBlendEnable = false;
            this.uvOffset = new Vector2(0, 0);
            this.uvTiling = new Vector2(1, 1);

            //BoundingSphere
            boundingSphere = new TgcBoundingSphere();



            /*
            //Shader
            this.effect = GuiController.Instance.Shaders.VariosShader;
            this.technique = TgcShaders.T_POSITION_COLORED;
             */
        }

        /// <summary>
        /// Actualiza la esfera en base a los valores configurados
        /// </summary>
        public void updateValues()
        {

            //Basado en: http://xith3d.svn.sourceforge.net/viewvc/xith3d/trunk/src/org/xith3d/scenegraph/primitives/Sphere.java?revision=1887&view=markup

            int stackLen = slices * 2 + 2;
            int c = color.ToArgb();
            Vector3[] coords = new Vector3[(slices + 1) * (stacks + 1)];
            Vector3[] normals = new Vector3[(slices + 1) * (stacks + 1)];
            Vector2[] texCoords2 = new Vector2[(slices + 1) * (stacks + 1)];
            int[] indices = new int[stackLen * stacks];


            /*
            float u = uvTiling.X;
            float v = uvTiling.Y;
            float offsetU = uvOffset.X;
            float offsetV = uvOffset.Y;
            */

            //Generar vertices indexados con TRIANGLE_STRIP, para esfera centrada en el origen
            for (int j = 0; j < stacks + 1; j++)
            {
                float angleXZl = ((float)j - ((float)stacks / 2.0f)) * FastMath.PI / (float)stacks;
                float low = FastMath.Sin(angleXZl);

                for (int i = 0; i < slices + 1; i++)
                {
                    float angleXY = (float)i * FastMath.TWO_PI / (float)slices;

                    float x = FastMath.Cos(angleXY);
                    float y = FastMath.Sin(angleXY);
                    float cl = FastMath.Cos(angleXZl);

                    int k = (j * (slices + 1)) + i;
                    coords[k] = new Vector3(x * cl * radius, low * radius, -y * cl * radius);

                    if (j < stacks)
                    {
                        int idx = (j * stackLen) + i * 2;
                        indices[idx + 0] = k;
                        indices[idx + 1] = k + slices + 1;
                    }

                    normals[k] = coords[k];
                    normals[k].Normalize();

                    float tx = (float)i * 1.0f / (float)slices;
                    texCoords2[k] = new Vector2(tx, (float)(j + 0) * 1.0f / (float)stacks);
                }
            }

            //Expandir vertices indexados
            int vertIdx = 0;
            for (int i = 2; i < indices.Length; i++)
            {
                int i1 = indices[i - 2];
                int i2 = indices[i - 1];
                int i3 = indices[i];

                vertices[vertIdx] = new CustomVertex.PositionNormalTextured(coords[i1], normals[i1], texCoords2[i1].X, texCoords2[i1].Y);
                vertices[vertIdx + 1] = new CustomVertex.PositionNormalTextured(coords[i2], normals[i2], texCoords2[i2].X, texCoords2[i2].Y);
                vertices[vertIdx + 2] = new CustomVertex.PositionNormalTextured(coords[i3], normals[i3], texCoords2[i3].X, texCoords2[i3].Y);
                vertIdx += 3;
            }


            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }



        /// <summary>
        /// Configurar textura de la pared
        /// </summary>
        public void setTexture(TgcTexture texture)
        {
            if (this.texture != null)
            {
                this.texture.dispose();
            }
            this.texture = texture;
        }


        /// <summary>
        /// Renderizar la caja
        /// </summary>
        public void render()
        {
            if (!enabled)
                return;

            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //transformacion
            if (autoTransformEnable)
            {
                this.transform = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(translation);
            }

            //Activar AlphaBlending
            activateAlphaBlend();

            //renderizar
            if (texture != null)
            {
                texturesManager.shaderSet(effect, "texDiffuseMap", texture);
            }
            else
            {
                texturesManager.clear(0);
            }
            texturesManager.clear(1);


            this.setShaderMatrix(this.effect, this.transform);
            d3dDevice.VertexDeclaration = this.vertexDeclaration;
            effect.Technique = this.technique;
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);

            //Render con shader
            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.vertices.Length / 3);
            effect.EndPass();
            effect.End();

            //Desactivar AlphaBlend
            resetAlphaBlend();
        }

        /// <summary>
        /// Cargar todas la matrices generales que necesita el shader
        /// </summary>
        public void setShaderMatrix(Effect effect, Matrix world)
        {
            Device device = GuiController.Instance.D3dDevice;

            Matrix matWorldView = world * device.Transform.View;
            Matrix matWorldViewProj = matWorldView * device.Transform.Projection;
            effect.SetValue("matWorld", world);
            effect.SetValue("matWorldView", matWorldView);
            effect.SetValue("matWorldViewProj", matWorldViewProj);
            effect.SetValue("matInverseTransposeWorld", Matrix.TransposeMatrix(Matrix.Invert(world)));
        }

        /// <summary>
        /// Activar AlphaBlending, si corresponde
        /// </summary>
        protected void activateAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            if (alphaBlendEnable)
            {
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;
            }
        }

        /// <summary>
        /// Desactivar AlphaBlending
        /// </summary>
        protected void resetAlphaBlend()
        {
            Device device = GuiController.Instance.D3dDevice;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
        }

        /// <summary>
        /// Liberar los recursos de la cja
        /// </summary>
        public void dispose()
        {
            if (texture != null)
            {
                texture.dispose();
            }
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
            boundingSphere.dispose();
        }

        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(Vector3 v)
        {
            this.move(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Desplaza la malla la distancia especificada, respecto de su posicion actual
        /// </summary>
        public void move(float x, float y, float z)
        {
            this.translation.X += x;
            this.translation.Y += y;
            this.translation.Z += z;

            updateBoundingSphere();
        }

        /// <summary>
        /// Mueve la malla en base a la orientacion actual de rotacion.
        /// Es necesario rotar la malla primero
        /// </summary>
        /// <param name="movement">Desplazamiento. Puede ser positivo (hacia adelante) o negativo (hacia atras)</param>
        public void moveOrientedY(float movement)
        {
            float z = (float)Math.Cos((float)rotation.Y) * movement;
            float x = (float)Math.Sin((float)rotation.Y) * movement;

            move(x, 0, z);
        }

        /// <summary>
        /// Obtiene la posicion absoluta de la malla, recibiendo un vector ya creado para
        /// almacenar el resultado
        /// </summary>
        /// <param name="pos">Vector ya creado en el que se carga el resultado</param>
        public void getPosition(Vector3 pos)
        {
            pos.X = translation.X;
            pos.Y = translation.Y;
            pos.Z = translation.Z;
        }

        /// <summary>
        /// Rota la malla respecto del eje X
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateX(float angle)
        {
            this.rotation.X += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Y
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateY(float angle)
        {
            this.rotation.Y += angle;
        }

        /// <summary>
        /// Rota la malla respecto del eje Z
        /// </summary>
        /// <param name="angle">Ángulo de rotación en radianes</param>
        public void rotateZ(float angle)
        {
            this.rotation.Z += angle;
        }

        /// <summary>
        /// Actualiza el BoundingBox de la caja.
        /// No contempla rotacion
        /// </summary>
        private void updateBoundingSphere()
        {
            boundingSphere.setValues(this.Position, this.radius);
        }

        /// <summary>
        /// Convierte el box en un TgcMesh
        /// </summary>
        /// <param name="meshName">Nombre de la malla que se va a crear</param>
        public TgcMesh toMesh(string meshName)
        {
            throw new Exception();
        }

        /// <summary>
        /// Crear un nuevo TgcBox igual a este
        /// </summary>
        /// <returns>Box clonado</returns>
        public TgcBoundingSphere clone()
        {
            throw new Exception();
        }



        /// <summary>
        /// FVF para formato de vertice PositionNormalTextured
        /// </summary>
        public static readonly VertexElement[] PositionNormalTextured_VertexElements = new VertexElement[]
        {
            new VertexElement(0, 0, DeclarationType.Float3,
                                    DeclarationMethod.Default,
                                    DeclarationUsage.Position, 0),
            
            new VertexElement(0, 12, DeclarationType.Float3,
                                     DeclarationMethod.Default,
                                     DeclarationUsage.Normal, 0),

            new VertexElement(0, 24, DeclarationType.Float2,
                                     DeclarationMethod.Default,
                                     DeclarationUsage.TextureCoordinate, 0),

            VertexElement.VertexDeclarationEnd 
        };



    }
}
