using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Blobby.UserInterface;

namespace BlobbyEditor
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

            if (GUILayout.Button("Beach"))
            {
                MapHelper.ChangeMap(Blobby.Map.Beach);
            }

            if (GUILayout.Button("Moon"))
            {
                MapHelper.ChangeMap(Blobby.Map.Moon);
            }

            GUILayout.EndHorizontal();
        }
    }
}
