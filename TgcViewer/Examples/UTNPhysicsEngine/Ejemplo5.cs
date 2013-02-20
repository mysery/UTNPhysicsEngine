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
    public class Ejemplo5 : TgcExample
    {

        public override string getCategory()
        {
            return "Prototipos abstractos";
        }

        public override string getName()
        {
            return "05 ejemplo escalera de esferas";
        }

        public override string getDescription()
        {
            return "Quinto ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
            #region Spheres en grilla

            // creo una grilla de cuerpos.
            int numberSpheresPerSide = 5;
            float radius = 10f;
            float separationBetweenSpheres = 5.0f;

            float xCentre = 0.0f;
            float zCentre = -120.0f;
            float yLocation = 30.0f;

            float initialX = xCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);
            float initialZ = zCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);

            for (int x = 0; x < numberSpheresPerSide; ++x)
            {
                for (int z = 0; z < numberSpheresPerSide; ++z)
                {
                    SphereBody sp1 = new SphereBody(radius,
                                                        new Vector3(initialX + (x * ((radius * 2) + separationBetweenSpheres)),
                                                                    yLocation,
                                                                    initialZ + (z * ((radius * 2) + separationBetweenSpheres))),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        1.0f);
                    proto.Bodys.Add(sp1);
                }
            }

            #endregion

            #region Spheres en escalera

            numberSpheresPerSide = 20;
            radius = 5.0f;
            separationBetweenSpheres = 2f;

            xCentre = 0.0f;
            zCentre = -120.0f;
            yLocation = 0.0f;

            initialX = xCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);
            initialZ = zCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);

            for (int x = 0; x < numberSpheresPerSide; ++x)
            {
                yLocation -= separationBetweenSpheres;
                for (int z = 0; z < numberSpheresPerSide; ++z)
                {
                    SphereBody sp2 = new SphereBody(radius,
                                                        new Vector3(initialX + (x * ((radius * 2) + separationBetweenSpheres)),
                                                                    yLocation,
                                                                    initialZ + (z * ((radius * 2) + separationBetweenSpheres))),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        0f);
                    proto.Bodys.Add(sp2);
                }
            }

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
