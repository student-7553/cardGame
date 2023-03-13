using UnityEngine;

namespace SingularityGroup.HotReload.Demo {
    public interface IDemo {
        bool IsServerRunning();
        void OpenHotReloadWindow();
        void OpenScriptFile(TextAsset textAsset, int line, int column);
    }
    
    public static class Demo {
        public static IDemo I = new FallbackDemo();
    }
    
    public class FallbackDemo : IDemo {
        public bool IsServerRunning() {
            return true;
        }

        public void OpenHotReloadWindow() {
            //no-op
        }

        public void OpenScriptFile(TextAsset textAsset, int line, int column) {
            //no-op
        }
    }
}