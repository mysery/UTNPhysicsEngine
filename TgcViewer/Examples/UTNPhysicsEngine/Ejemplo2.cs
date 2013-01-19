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
using TgcViewer.Utils.TgcSceneLoader;
using Examples.UTNPhysicsEngine.physics.body;

namespace Examples
{
    /// <summary>
    /// UTNPhysicsEngine Main example
    /// </summary>
    public class Ejemplo2 : TgcExample
    {
        public override string getCategory()
        {
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "02 ejemplo del UTNPhysicsEngine";
        }

        public override string getDescription()
        {
            return "Segundo ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();
        
        public void createBodys()
        {
            const float radius = 20.0f;

            Random r = new Random(123456);
            for (int i = 0; i < 3; ++i)
	        {
		        for (int j = 0; j < 3; j++)
		        {
                    for (int k = 0; k < 3; k++)
                    {
                        SphereBody sphereLeft = new SphereBody( radius,
                                                                new Vector3(-proto.WorldSize / 2 + i * radius * 4, -proto.WorldSize / 2 + j * radius * 4, -proto.WorldSize / 2 + k * radius * 4),
                                                                new Vector3((float)r.Next(-5, 5), (float)r.Next(-5, 5), (float)r.Next(-5, 5)),
                                                                new Vector3(0.0f, -9.8f, 0.0f),
                                                                /*(float)r.NextDouble() * 5*/1f);
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
