using System.Collections.Generic;
using System.Text;
using System;

namespace DiepPlugin {
    internal struct BulletData {
        public ushort ownerID;
        public float X, Y;
        public float vX, vY;
        public byte ColorR;
        public byte ColorG;
        public byte ColorB;
        public float radius;
    }
}
