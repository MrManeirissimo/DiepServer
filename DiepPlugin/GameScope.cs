using System;
using System.Collections.Generic;
using System.Text;

namespace DiepPlugin {
    public static class GameScope {
        public static DiepPlayerManager PlayerManager { get; set; }
        public static BulletManager BulletManager { get; set; }
    }
}
