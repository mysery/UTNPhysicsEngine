using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace TgcViewer.Utils.Input
{
    /// <summary>
    /// Cámara en primera persona personalizada
    /// </summary>
    public class MyFpsCamera : TgcCamera
    {
        static readonly Vector3 UP_VECTOR = new Vector3(0, 1, 0);

        //Constantes de movimiento
        const float DEFAULT_ROTATION_SPEED = 2f;
        const float DEFAULT_MOVEMENT_SPEED = 100f;
        const float DEFAULT_JUMP_SPEED = 100f;

        #region Getters y Setters

        bool enable;
        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;

                //Si se habilito la camara, cargar como la cámara actual
                if (value)
                {
                    GuiController.Instance.CurrentCamera = this;
                }
            }
        }

        float rotationSpeed;
        /// <summary>
        /// Velocidad de rotacion
        /// </summary>
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        float movementSpeed;
        /// <summary>
        /// Velocidad de movimiento
        /// </summary>
        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = value; }
        }

        float jumpSpeed;
        /// <summary>
        /// Velocidad de salto
        /// </summary>
        public float JumpSpeed
        {
            get { return jumpSpeed; }
            set { jumpSpeed = value; }
        }

        Vector3 position;
        /// <summary>
        /// Posicion de la camara
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector3 lookAt;
        /// <summary>
        /// Punto al que mira la camara
        /// </summary>
        public Vector3 LookAt
        {
            get { return lookAt; }
            set { lookAt = value; }
        }

  
        #endregion


        float currentRotY;
        float currentRotXZ;
        Matrix viewMatrix;

        /// <summary>
        /// Crea la cámara con valores iniciales.
        /// </summary>
        public MyFpsCamera()
        {
            resetValues();
        }

        /// <summary>
        /// Carga los valores default de la camara
        /// </summary>
        public void resetValues()
        {
            this.rotationSpeed = DEFAULT_ROTATION_SPEED;
            this.movementSpeed = DEFAULT_MOVEMENT_SPEED;
            this.jumpSpeed = DEFAULT_JUMP_SPEED;
            this.position = new Vector3(0, 0, 0);
            this.lookAt = new Vector3(0, 0, 0);
            this.currentRotY = 0;
            this.currentRotXZ = 0;
            this.viewMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Configura la posicion de la cámara
        /// </summary>
        public void setCamera(Vector3 pos, Vector3 lookAt)
        {
            this.position = pos;
            this.lookAt = lookAt;
        }

        /// <summary>
        /// Actualiza los valores de la camara
        /// </summary>
        public void updateCamera()
        {
            //Si la camara no está habilitada, no procesar el resto del input
            if (!enable)
            {
                return;
            }

            float elapsedTime = GuiController.Instance.ElapsedTime;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            float forwardMovement = 0;
            float strafeMovement = 0;
            float jumpMovement = 0;
            float xzRotation = 0;
            float yRotation = 0;
            bool moving = false;
            bool rotating = false;

            //Analizar input de teclado
            if (d3dInput.keyDown(Key.W))
            {
                forwardMovement = this.movementSpeed;
                moving = true;
            }
            else if (d3dInput.keyDown(Key.S))
            {
                forwardMovement = -this.movementSpeed;
                moving = true;
            }
            if (d3dInput.keyDown(Key.A))
            {
                strafeMovement = this.movementSpeed;
                moving = true;
            }
            else if (d3dInput.keyDown(Key.D))
            {
                strafeMovement = -this.movementSpeed;
                moving = true;
            }
            if (d3dInput.keyDown(Key.Space))
            {
                jumpMovement = this.jumpSpeed;
                moving = true;
            }
            else if (d3dInput.keyDown(Key.LeftControl))
            {
                jumpMovement = -this.jumpSpeed;
                moving = true;
            }

            //Analizar mouse
            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                yRotation = d3dInput.XposRelative * this.rotationSpeed;
                xzRotation = -d3dInput.YposRelative * this.rotationSpeed;
                rotating = true;
            }

            //Acumular rotacion
            this.currentRotY += yRotation * elapsedTime;
            this.currentRotXZ += xzRotation * elapsedTime;

            //Clamp de rotacion XZ entre [-PI/2, PI/2]
            this.currentRotXZ = this.currentRotXZ > FastMath.PI_HALF ? FastMath.PI_HALF : this.currentRotXZ;
            this.currentRotXZ = this.currentRotXZ < -FastMath.PI_HALF ? -FastMath.PI_HALF : this.currentRotXZ;

            //Wrap de rotacion Y entre [0, 2PI]
            this.currentRotY = this.currentRotY > FastMath.TWO_PI ? this.currentRotY - FastMath.TWO_PI : this.currentRotY;
            this.currentRotY = this.currentRotY < 0 ? FastMath.TWO_PI + this.currentRotY : this.currentRotY;



            //Obtener angulos de direccion segun rotacion en Y y en XZ
            var dirX = FastMath.Sin(this.currentRotY);
            var dirZ = FastMath.Cos(this.currentRotY);
            var dirY = FastMath.Sin(this.currentRotXZ);

            //Direcciones de movimiento
            var movementDir = new Vector3(dirX, 0, dirZ);
            //vec3.normalize(movementDir);
            var strafeDir = Vector3.Cross(movementDir, MyFpsCamera.UP_VECTOR);


            //Movimiento adelante-atras
            movementDir.Scale(forwardMovement);

            //Movimiento strafe
            strafeDir.Scale(strafeMovement);

            //Sumar movimiento
            this.position += movementDir + strafeDir;
            this.position.Y += jumpMovement;

            //Hacia donde mirar
            Vector3 lookAtDir = new Vector3(dirX, dirY, dirZ);
            lookAtDir.Normalize();
            this.lookAt = this.position + lookAtDir;

            //Crear matriz de view
            this.viewMatrix = Matrix.LookAtLH(this.position, this.lookAt, MyFpsCamera.UP_VECTOR);
        }

        /// <summary>
        /// Actualiza la ViewMatrix, si es que la camara esta activada
        /// </summary>
        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }

        

        public Vector3 getPosition()
        {
            return this.position;
        }

        public Vector3 getLookAt()
        {
            return this.lookAt;
        }

        /// <summary>
        /// String de codigo para setear la camara desde GuiController, con la posicion actual y direccion de la camara
        /// </summary>
        internal string getPositionCode()
        {
            //TODO ver de donde carajo sacar el LookAt de esta camara
            Vector3 lookAt = this.lookAt;

            return "GuiController.Instance.setCamera(new Vector3(" +
                TgcParserUtils.printFloat(position.X) + "f, " + TgcParserUtils.printFloat(position.Y) + "f, " + TgcParserUtils.printFloat(position.Z) + "f), new Vector3(" +
                TgcParserUtils.printFloat(lookAt.X) + "f, " + TgcParserUtils.printFloat(lookAt.Y) + "f, " + TgcParserUtils.printFloat(lookAt.Z) + "f));";
        }



    }
}
