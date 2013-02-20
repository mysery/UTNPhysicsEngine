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
    public class Ejemplo10 : TgcExample
    {

        public override string getCategory()
        {
            return "Prototipos abstractos";
        }

        public override string getName()
        {
            return "10 ejemplo con cajas rotadas y esferas";
        }

        public override string getDescription()
        {
            return "Decimo ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();
        
        public void createBodys()
        {
            Matrix m = new Matrix();
            m.M11 = 1f; m.M12 = 0f;m.M13 = 0f;
            m.M21 = 0f; m.M22 = 1f;m.M23 = 0f;
            m.M31 = 0f; m.M32 = 0f;m.M33 = 1f;
            Vector3 halfExtend = new Vector3(20f, 20f, 20f);
            //m.M12 = 1f;
            //m.M32 = -1f;
            BoxBody box = new BoxBody(m, halfExtend, new Vector3(), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);
            box = new BoxBody(m, halfExtend, box.position + new Vector3(halfExtend.X * 2, 0f, halfExtend.Z * 2), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);
            box = new BoxBody(m, halfExtend, box.position - new Vector3(halfExtend.X * 4, 0f, halfExtend.Z * 4), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);
            halfExtend = new Vector3(20f, 30f, 20f);
            box = new BoxBody(m, halfExtend, box.position - new Vector3(halfExtend.X * 2, 0f, 0f), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);
            halfExtend = new Vector3(40f, 20f, 20f);
            box = new BoxBody(m, halfExtend, box.position + new Vector3(halfExtend.X * 4, 0f, halfExtend.Z * 2), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);

            m.M12 = 1f;
            m.M32 = -1f;
            halfExtend = new Vector3(20f, 20f, 20f);
            box = new BoxBody(m, halfExtend, new Vector3(80f, 0f, -80f), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);

            m.M12 = 0f;
            m.M32 = 0f;
            m.M13 = 1f;
            m.M33 = 1f;
            box = new BoxBody(m, halfExtend, new Vector3(40f, 0f, -40f), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);

            m.M12 = -1f;
            m.M32 = -1f;
            m.M13 = -1f;
            m.M33 = -1f;
            box = new BoxBody(m, halfExtend, new Vector3(40f, 50f, 40f), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);

            m.M11 = 1f;
            m.M22 = 1f;
            m.M33 = 1f;
            box = new BoxBody(m, halfExtend, new Vector3(140f, 0, -140f), new Vector3(), new Vector3(), 0f);
            proto.Bodys.Add(box);

            #region Spheres

            // creo una grilla de cuerpos.
            const int numberSpheresPerSide = 10;
            const float radius = 10.0f;
            const float separationBetweenSpheres = 2.0f;

            const float xCentre = 0.0f;
            const float zCentre = 0.0f;
            const float yLocation = 100.0f;
            const float initialX = xCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);
            const float initialZ = zCentre - (((numberSpheresPerSide - 1) * ((radius * 2.0f) + separationBetweenSpheres)) / 2.0f) - (separationBetweenSpheres / 2.0f);

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
