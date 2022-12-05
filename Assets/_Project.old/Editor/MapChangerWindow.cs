using Blobby.UserInterface;
using UnityEditor;
using UnityEngine;

namespace Blobby._Project.Editor
{
    public class MapChangerWindow : EditorWindow
    {
        [MenuItem("Window/Map Changer")]
        public static void ShowWindow()
        {
            GetWindow<MapChangerWindow>("Map Changer");
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Menu"))
            {
                MapHelper.ChangeMap(Blobby.Map.Menu);
            }

            if (GUILayout.Button("Gym"))
            {
                MapHelper.ChangeMap(Blobby.Map.Gym);
            }

            GUILayout.EndHorizontal();
        }
    }
}
