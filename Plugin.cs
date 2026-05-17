using BepInEx;
using HawkNetworking;

namespace WobblyAC
{
    [BepInPlugin("com.wobblyac.plugin", "Wobbly AC", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private bool isMenuVisible = false;

        void Awake()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
                isMenuVisible = !isMenuVisible;
        }
    }
}