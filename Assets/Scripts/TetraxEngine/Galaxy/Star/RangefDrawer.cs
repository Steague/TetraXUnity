using UnityEditor;
using UnityEngine;

//Totes stole this from SO
namespace TetraxEngine.Galaxy.Star
{
    [CustomPropertyDrawer(typeof(RangeF))]
    public class RangeFDrawer : PropertyDrawer
    {
        private SerializedProperty _min, _max;
        private string _name;
        private bool _cache;
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Screen.width < 333 ? (16f + 18f) : 16f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_cache)
            {
                //get the name before it's gone
                _name = property.displayName;

                //get the X and Y values
                property.Next(true);
                _min = property.Copy();
                property.Next(true);
                _max = property.Copy();

                _cache = true;
            }

            Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(_name));

            //Check if there is enough space to put the name on the same line (to save space)
            if (position.height > 16f)
            {
                position.height = 16f;
                EditorGUI.indentLevel += 1;
                contentPosition = EditorGUI.IndentedRect(position);
                contentPosition.y += 18f;
            }

            float half = contentPosition.width / 2;
            GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

            //show the X and Y from the point
            EditorGUIUtility.labelWidth = 27f;
            contentPosition.width *= 0.5f;
            EditorGUI.indentLevel = 0;

            // Begin/end property & change check make each field
            // behave correctly when multi-object editing.
            EditorGUI.BeginProperty(contentPosition, label, _min);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("Min"), _min.floatValue);
                if (EditorGUI.EndChangeCheck())
                    _min.floatValue = newVal;
            }
            EditorGUI.EndProperty();

            contentPosition.x += half;

            EditorGUI.BeginProperty(contentPosition, label, _max);
            {
                EditorGUI.BeginChangeCheck();
                float newVal = EditorGUI.FloatField(contentPosition, new GUIContent("Max"), _max.floatValue);
                if (EditorGUI.EndChangeCheck())
                    _max.floatValue = newVal;
            }
            EditorGUI.EndProperty();
        }
    }
}