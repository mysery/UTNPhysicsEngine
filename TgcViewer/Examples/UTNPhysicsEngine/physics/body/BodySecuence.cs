using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.UTNPhysicsEngine.physics.body
{
    class BodySecuence
    {
        #region Singleton
        private static volatile BodySecuence instance;
        private static volatile int seq = 0;

        public static BodySecuence Instance
        {
            get
            { 
              if (instance == null)
                instance = new BodySecuence();
              return instance; 
            }
        }
        #endregion

        public int Next
        {
            get { return seq++; }
        }
    }
}
