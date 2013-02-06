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
    public class Ejemplo1 : TgcExample
    {

        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "01 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Primer ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
            const float radius = 20.0f;

            Random r = new Random(123456);
            for (int i = 0; i < 7; ++i)
	        {
		        for (int j = 0; j < 7; j++)
		        {
                    for (int k = 0; k < 7; k++)
                    {
                        SphereBody sphereLeft = new SphereBody( radius,
                                                                new Vector3(-proto.WorldSize / 2 + i * radius * 4 + r.Next(2), -proto.WorldSize / 2 + j * radius * 4 + r.Next(2), -proto.WorldSize / 2 + k * radius * 4 + r.Next(2)),
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
