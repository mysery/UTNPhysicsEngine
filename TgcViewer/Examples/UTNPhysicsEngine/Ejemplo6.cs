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
    public class Ejemplo6 : TgcExample
    {

        public override string getCategory()
        {
            return "Prototipos abstractos";
        }

        public override string getName()
        {
            return "06 ejemplo esferas en piramide";
        }

        public override string getDescription()
        {
            return "Sexto ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
            const float radius = 20.0f;
            const float separationDistance = 50.0f;
            const int numberOfSpheresPerBaseLayer = 8;
            const float initialYLocation = -50.0f;
            const float zOffset = -10f;

            for (int y = 0; y < numberOfSpheresPerBaseLayer; ++y)
            {
                for (int x = 0; x < numberOfSpheresPerBaseLayer - y; ++x)
                {
                    for (int z = 0; z < numberOfSpheresPerBaseLayer - y; ++z)
                    {
                        SphereBody sphares = new SphereBody(radius,
                                                            new Vector3((radius * 2f * x) + (y * radius),
                                                                        initialYLocation + (separationDistance * y),
                                                                        zOffset + (radius * 2 * z) + (y * radius)),
                                                            new Vector3(),
                                                            new Vector3(),
                                                            y != 0 ? 1.0f : 0f);
                        sphares.restitution = 0.5f;
                        /*if (y != 0)
                            sphares.aceleracion = new Vector3(0.0f, -9.8f, 0.0f);*/

                        proto.Bodys.Add(sphares);
                    }
                }
            }

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
