using Haare.Client.UI;
using Haare.Scripts.Client.UI.Animator;
using UnityEditor; 

namespace Haare.Editor.UI
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string[] propertiesToExclude = new string[] {
                "hoverScale",
                "clickPunchScale",
                "hoverDuration",
                "clickDuration",
                "OPTION_HOVERIMAGE",
                "OPTION_ANIMATION",
                "OPTION_HOVERALPHA",
                "INTERACTIABLE",
                "HOVERANIMATION",
                "CLICKANIMATION"
            };
            DrawPropertiesExcluding(serializedObject, propertiesToExclude);
            

            SerializedProperty hoverImageProp = serializedObject.FindProperty("OPTION_HOVERIMAGE");
            SerializedProperty hoverColorProp = serializedObject.FindProperty("OPTION_HOVERALPHA");
            SerializedProperty animationProp = serializedObject.FindProperty("OPTION_ANIMATION");
            
            SerializedProperty clickAnimationFlagProp = serializedObject.FindProperty("CLICKANIMATION");
            SerializedProperty clickDurationProp = serializedObject.FindProperty("clickDuration");
            SerializedProperty clickPunchScaleProp = serializedObject.FindProperty("clickPunchScale");
            
            SerializedProperty hoverAnimationFlagProp = serializedObject.FindProperty("HOVERANIMATION");
            SerializedProperty hoverScaleProp = serializedObject.FindProperty("hoverScale");
            SerializedProperty hoverDurationProp = serializedObject.FindProperty("hoverDuration");
            
            // OPTION_ANIMATION bool 값이 true일 때만 아래 코드가 실행됩니다.
            EditorGUILayout.PropertyField(hoverImageProp);
            EditorGUILayout.PropertyField(hoverColorProp);
            EditorGUILayout.PropertyField(animationProp);

            // 헤더처럼 보이도록 라벨을 추가합니다.
            EditorGUILayout.Space(); // 약간의 공백
            if (animationProp.boolValue)
            {
                EditorGUILayout.LabelField("Animation Options", EditorStyles.boldLabel);

   
                EditorGUILayout.PropertyField(clickAnimationFlagProp);
                if (clickAnimationFlagProp.boolValue)
                {
                    EditorGUILayout.PropertyField(clickPunchScaleProp);
                    EditorGUILayout.PropertyField(clickDurationProp);
                }
                EditorGUILayout.Space(); 
                EditorGUILayout.PropertyField(hoverAnimationFlagProp);
                if (hoverAnimationFlagProp.boolValue)
                {
                    EditorGUILayout.PropertyField(hoverScaleProp);
                    EditorGUILayout.PropertyField(hoverDurationProp);
                }
                
            }

            // 변경된 모든 프로퍼티 값을 실제 변수에 적용합니다. (이 줄이 없으면 값이 저장되지 않음)
            serializedObject.ApplyModifiedProperties();
        }
    }
    [CustomEditor(typeof(CustomImage))]
    public class CustomImageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string[] propertiesToExclude = new string[] {
                "OPTION_ANIMATION",
                "ANIMATION_SLIDE",
                "ANIMATION_POPUP",
                "slideDuration",
                "slideEaseType",
                "onScreenPosition",
                "offScreenPosition",
                "popupDuration",
                "popupEaseType"
            };
            DrawPropertiesExcluding(serializedObject, propertiesToExclude);
            

            SerializedProperty animationProp = serializedObject.FindProperty("OPTION_ANIMATION");
            SerializedProperty animationSlideProp = serializedObject.FindProperty("ANIMATION_SLIDE");
            SerializedProperty animationPopupProp = serializedObject.FindProperty("ANIMATION_POPUP");

            SerializedProperty slideDurationProp = serializedObject.FindProperty("slideDuration");
            SerializedProperty slideEaseTypeProp = serializedObject.FindProperty("slideEaseType");
            SerializedProperty onScreenProp = serializedObject.FindProperty("onScreenPosition");
            SerializedProperty offScreenProp = serializedObject.FindProperty("offScreenPosition");

            
            SerializedProperty popupDurationProp = serializedObject.FindProperty("popupDuration");
            SerializedProperty popupEaseTypeProp = serializedObject.FindProperty("popupEaseType");

            EditorGUILayout.PropertyField(animationProp);

            
            // OPTION_ANIMATION bool 값이 true일 때만 아래 코드가 실행됩니다.
            if (animationProp.boolValue)
            {
                
                // 헤더처럼 보이도록 라벨을 추가합니다.
                EditorGUILayout.Space(); // 약간의 공백
                EditorGUILayout.LabelField("Animation Options", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(animationSlideProp);
                EditorGUILayout.PropertyField(animationPopupProp);

                if (animationSlideProp.boolValue)
                {
                    EditorGUILayout.PropertyField(slideDurationProp);
                    EditorGUILayout.PropertyField(slideEaseTypeProp);
                    EditorGUILayout.PropertyField(onScreenProp);
                    EditorGUILayout.PropertyField(offScreenProp);
                }
                EditorGUILayout.Space(); 
                if (animationPopupProp.boolValue)
                {
                    EditorGUILayout.PropertyField(popupDurationProp);
                    EditorGUILayout.PropertyField(popupEaseTypeProp);
                }

                
            }

            // 변경된 모든 프로퍼티 값을 실제 변수에 적용합니다. (이 줄이 없으면 값이 저장되지 않음)
            serializedObject.ApplyModifiedProperties();
        }
    }
    
}