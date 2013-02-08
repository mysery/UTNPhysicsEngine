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
    public class Ejemplo4 : TgcExample
    {

        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "04 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Cuarto ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
            float[] radius = { 40f, 30.0f, 20.0f };

            Random r = new Random(123456);
            for (int i = 0; i < 3; ++i)
	        {
                for (int j = 0; j < 3; j++)
		        {
                    for (int k = 0; k < 3; k++)
                    {
                        SphereBody sphereLeft = new SphereBody( radius[i],
                                                                new Vector3(-proto.WorldSize.X / 2 + i * radius[i] * 4 + r.Next(2), -proto.WorldSize.Y / 2 + j * radius[i] * 4 + r.Next(2), -proto.WorldSize.Z / 2 + k * radius[i] * 4 + r.Next(2)),
                                                                new Vector3(),
                                                                //new Vector3((float)r.Next(-2, 2), (float)r.Next(-2, 2), (float)r.Next(-2, 2)),
                                                                new Vector3(),
                                                                1f);
                        proto.Bodys.Add(sphereLeft);
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
