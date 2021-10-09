using System;
using System.Collections.Generic;
using System.Text;

namespace DiepPlugin {
    public class PlayerData {
        public float RotationZ { get; set; }

        public ushort ID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }

        public PlayerData(ushort ID, float x, float y, float radius, BColor color) {
            this.ID = ID;
            this.X = x;
            this.Y = y;
            this.Radius = radius;
            this.ColorR = color.R;
            this.ColorG = color.G;
            this.ColorB = color.B;
        }
    }
}
