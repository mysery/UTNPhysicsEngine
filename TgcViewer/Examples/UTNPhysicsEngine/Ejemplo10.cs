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
            return "UTNPhysicsEngine";
        }

        public override string getName()
        {
            return "10 ejemplo del UTNPhysicsEngine";
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
            BoxBody box = new BoxBody(m, halfExtend, new Vector3(), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);
            box = new BoxBody(m, halfExtend, new Vector3(100f, 0f, 100f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);
            box = new BoxBody(m, halfExtend, new Vector3(-100f, 0f, -100f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);
            halfExtend = new Vector3(20f, 30f, 20f);
            box = new BoxBody(m, halfExtend, new Vector3(0f, 0f, -100f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);
            halfExtend = new Vector3(40f, 20f, 20f);
            box = new BoxBody(m, halfExtend, new Vector3(-100f, 0f, 100f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);

            m.M12 = 1f;
            m.M32 = -1f;
            halfExtend = new Vector3(20f, 20f, 20f);
            box = new BoxBody(m, halfExtend, new Vector3(200f, 0f, 200f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);

            m.M12 = 0f;
            m.M32 = 0f;
            m.M13 = 1f;
            m.M33 = 1f;
            box = new BoxBody(m, halfExtend, new Vector3(-200f, 0f, -200f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);

            m.M12 = -1f;
            m.M32 = -1f;
            m.M13 = -1f;
            m.M33 = -1f;
            box = new BoxBody(m, halfExtend, new Vector3(0f, 0f, -200f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);

            m.M11 = 0f;
            m.M22 = 0f;
            m.M33 = 0f;            
            box = new BoxBody(m, halfExtend, new Vector3(0f, 0f, 200f), new Vector3(), new Vector3(0f, -0.1f, 0f), 1f);
            proto.Bodys.Add(box);
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
