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

namespace Examples
{
    /// <summary>
    /// UTNPhysicsEngine Main example
    /// </summary>
    public class Ejemplo7 : TgcExample
    {

        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "07 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Septimo ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
            #region Spheres

            // creo una grilla de cuerpos.
            const int numberSpheresPerSide = 10;
            const int maxValueRandom = 60;
            const float radius = 10.0f;
            const float separationBetweenSpheres = 2.0f;

            const float xCentre = 0.0f;
            const float zCentre = -120.0f;
            const float yLocation = 30.0f;
            Random random = new Random();

            const float initialX = xCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);
            const float initialZ = zCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);

            for (int x = 0; x < numberSpheresPerSide; ++x)
            {
                for (int z = 0; z < numberSpheresPerSide; ++z)
                {
                    SphereBody sphere = new SphereBody(radius,
                                                        new Vector3(initialX + (x * ((radius * 2) + separationBetweenSpheres)),
                                                                    yLocation + random.Next(maxValueRandom),
                                                                    initialZ + (z * ((radius * 2) + separationBetweenSpheres))),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        1.0f);
                    
                    proto.Bodys.Add(sphere);
                }
            }

            #endregion

            #region BigSphere

            const float radiusBigYLocation = 10f;
            const float radiusBig = 150.0f;
            SphereBody bigSphere = new SphereBody(radiusBig,
                                                    new Vector3(xCentre,
                                                                -radiusBig + radiusBigYLocation,
                                                                    zCentre),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        0f);
            proto.Bodys.Add(bigSphere);
            #endregion

        }

        public override void init()
        {
            proto.init();
            this.createBodys();
            proto.optimize();
        }

        public override void close()
        {
            proto.close();
        }

        public override void render(float elapsedTime)
        {
            proto.render(elapsedTime);
        }
    }
}
