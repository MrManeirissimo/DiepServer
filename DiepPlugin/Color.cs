using System.Collections.Generic;
using System.Text;
using System;

namespace DiepPlugin {
    public class BaseColor<T> {
        public T R { get; set; }
        public T G { get; set; }
        public T B { get; set; }

        public BaseColor() {

        }
        public BaseColor(T r, T g, T b) {
            R = r; G = g; B = b;
        }
    }

    public class Color : BaseColor<int> {
        public Color(int r, int g, int b) : base(r, g, b) {

        }
    }

    public class BColor : BaseColor<byte> {
        public BColor(byte r, byte g, byte b) : base(r, g, b) {
        }
    }
}