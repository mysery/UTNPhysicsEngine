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
    public class Ejemplo11 : TgcExample
    {

        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "11 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Decimo ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();
        
        public void createBodys()
        {
            #region Box
            Matrix m = new Matrix();
            m.M11 = 1f; m.M12 = 0f;m.M13 = 0f;
            m.M21 = 0f; m.M22 = 1f;m.M23 = 0f;
            m.M31 = 0f; m.M32 = 0f;m.M33 = 1f;
            Vector3 halfExtend = new Vector3(20f, 20f, 20f);
            // creo una grilla de cuerpos.
            int numberBoxPerSide = 15;            
            float separationBetweenBox = 4.0f;

            float xCentre = 0f;
            float zCentre = 0f;
            float yLocation = 0f;
            float initialX = xCentre - (((numberBoxPerSide - 1) * ((halfExtend.X * 2.0f) + separationBetweenBox)) / 2.0f) - (separationBetweenBox / 2.0f);
            float initialZ = zCentre - (((numberBoxPerSide - 1) * ((halfExtend.Y * 2.0f) + separationBetweenBox)) / 2.0f) - (separationBetweenBox / 2.0f);

            for (int x = 0; x < numberBoxPerSide; ++x)
            {
                for (int z = 0; z < numberBoxPerSide; ++z)
                {
                    BoxBody box = new BoxBody(m, halfExtend,
                                              new Vector3(initialX + (x * ((halfExtend.X * 2) + separationBetweenBox)),
                                                          yLocation,
                                                          initialZ + (z * ((halfExtend.Z * 2) + separationBetweenBox))),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        0f);

                    proto.Bodys.Add(box);
                }
            }

            #endregion
            #region Spheres

            // creo una grilla de cuerpos.
            const int numberSpheresPerSide = 10;
            const float radius = 10.0f;
            const float separationBetweenSpheres = 2.0f;

            xCentre = 0.0f;
            zCentre = 0.0f;
            yLocation = 100.0f;
            initialX = xCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);
            initialZ = zCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);

            for (int x = 0; x < numberSpheresPerSide; ++x)
            {
                for (int z = 0; z < numberSpheresPerSide; ++z)
                {
                    SphereBody sphere = new SphereBody(radius,
                                                        new Vector3(initialX + (x * ((radius * 2) + separationBetweenSpheres)),
                                                                    yLocation,
                                                                    initialZ + (z * ((radius * 2) + separationBetweenSpheres))),
                                                        new Vector3(),
                                                        new Vector3(),
                                                        1.0f);

                    proto.Bodys.Add(sphere);
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
            proto.render(elapsedTime*0.5f);
        }
    }
}
