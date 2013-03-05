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
    public class Ejemplo0 : TgcExample
    {

        public override string getCategory()
        {
            return "Prototipos abstractos";
        }

        public override string getName()
        {
            return "00 Ejemplo vacio";
        }

        public override string getDescription()
        {
            return "Primer ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {
/*            const float radius = 1.0f;
            for (int i = 0; i < 6; i++)
            {
                SphereBody sphereLeft = new SphereBody(radius,
                                                        new Vector3(i-2,i,i+2),
                                                        new Vector3(6f, 7f, 8f),
                                                        new Vector3(),
                                                                   1f);
                proto.Bodys.Add(sphereLeft);
            }
*/                    
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
            if (proto.Bodys.Count == 1500)
            {
                Body body = proto.Bodys[0];
                proto.world.removeBody(body); //El proto y el world tienen la misma lista, con lo cual alcanza con sacarlo de una.
            }

        }
    }
}
