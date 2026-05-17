using System;
using UnityEngine;
using System.Collections.Generic;

namespace WobblyMenu.UI
{
    public static class Drawer
    {
        private const float WIDTH = 200f;

        public static void DrawObject(object obj, string search)
        {
            if (obj == null)
            {
                return;
            }

            var fields = FieldUtility.GetFields(obj);

            foreach (var field in fields)
            {
                if (!string.IsNullOrEmpty(search) &&
                    !field.name.ToLower().Contains(search.ToLower()))
                    continue;

                DrawField(field);
            }
        }

        private static void DrawField(EditableField field)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(WIDTH));

            GUILayout.Label(field.name, GUILayout.Width(90));

            float w = WIDTH - 90;

            object value = field.Get();

            if (value == null)
            {
                GUILayout.Label("null", GUILayout.Width(w));
                GUILayout.EndHorizontal();
                return;
            }

            Type t = field.type;

            if (t == typeof(string))
            {
                string v = GUILayout.TextField((string)value, GUILayout.Width(w));
                field.Set(v);
            }
            else if (t == typeof(int))
            {
                int.TryParse(GUILayout.TextField(value.ToString(), GUILayout.Width(w)), out int v);
                field.Set(v);
            }
            else if (t == typeof(float))
            {
                float.TryParse(GUILayout.TextField(value.ToString(), GUILayout.Width(w)), out float v);
                field.Set(v);
            }
            else if (t == typeof(bool))
            {
                bool v = GUILayout.Toggle((bool)value, "", GUILayout.Width(w));
                field.Set(v);
            }
            else if (t == typeof(Vector3))
            {
                Vector3 v3 = (Vector3)value;

                GUILayout.BeginVertical(GUILayout.Width(w));

                float.TryParse(GUILayout.TextField(v3.x.ToString(), GUILayout.Width(w)), out float x);
                float.TryParse(GUILayout.TextField(v3.y.ToString(), GUILayout.Width(w)), out float y);
                float.TryParse(GUILayout.TextField(v3.z.ToString(), GUILayout.Width(w)), out float z);

                field.Set(new Vector3(x, y, z));

                GUILayout.EndVertical();
            }
            else if (t.IsEnum)
            {
                var names = Enum.GetNames(t);

                int index = Array.IndexOf(names, value.ToString());
                if (index < 0) index = 0;

                int newIndex = GUILayout.SelectionGrid(index, names, 1, GUILayout.Width(w));

                field.Set(Enum.Parse(t, names[newIndex]));
            }
            else
            {
                GUILayout.Label("Unsupported", GUILayout.Width(w));
            }

            GUILayout.EndHorizontal();
        }
    }
}