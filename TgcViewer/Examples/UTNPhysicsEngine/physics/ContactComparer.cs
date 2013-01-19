using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.UTNPhysicsEngine.physics
{
    class ContactComparer : IEqualityComparer<Contact>
    {
        public bool Equals(Contact x, Contact y)
        {
            return x.bodyA.idCode.Equals(y.bodyA.idCode) ||
                    x.bodyB.idCode.Equals(y.bodyB.idCode) ||
                    x.bodyB.idCode.Equals(y.bodyA.idCode) ||
                    x.bodyA.idCode.Equals(y.bodyB.idCode);
        }

        public static bool Distinct(Contact x, Contact y)
        {
            return !x.bodyA.idCode.Equals(y.bodyA.idCode) &&
                   !x.bodyB.idCode.Equals(y.bodyB.idCode) &&
                   !x.bodyB.idCode.Equals(y.bodyA.idCode) &&
                   !x.bodyA.idCode.Equals(y.bodyB.idCode);
        }
        
        public int GetHashCode(Contact obj)
        {
            int hash = 5;
            hash = 13 * hash + obj.bodyA.idCode.GetHashCode();
            hash = 13 * hash + obj.bodyB.idCode.GetHashCode();
            return hash;
        }
    }
}
