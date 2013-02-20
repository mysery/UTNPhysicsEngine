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
using Examples.UTNPhysicsEngine.physics;

namespace Examples
{
    /// <summary>
    /// UTNPhysicsEngine Main example
    /// </summary>
    public class Ejemplo9 : TgcExample
    {

        public override string getCategory()
        {
            return "Prototipos abstractos";
        }

        public override string getName()
        {
            return "09 ejemplo con mundo de diferente tamaño";
        }

        public override string getDescription()
        {
            return "Ejemplo del UTNPhysicsEngine";
        }

        private Prototipos proto = new Prototipos();

        public void createBodys()
        {

        }

        public override void init()
        {
            proto.init();
            Vector3 WorldSize = new Vector3(300f, 100f, 500f);
            proto.WorldSize = WorldSize;
            proto.world = new World(proto.Bodys, WorldSize, 0.5f);
            proto.limitsWorld = new TgcBoundingBox(WorldSize * -1f,
                                                    WorldSize);
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
