using UnityEngine;
using UnityEditor;
using System;

namespace AnimTools
{
	public class CreateAnimationPopup : EditorWindow
	{
		private Action<string, int> _callbackAccept;
		private string				_animationName;
		private int					_fps;
		private string				_spritePath;

		public static void Show(string defaultAnimationName, int defaultFPS, Action<string, int> callbackAccept)
		{
			CreateAnimationPopup window = CreateInstance<CreateAnimationPopup>();
			window._callbackAccept = callbackAccept;
			window._animationName = defaultAnimationName;
			window._fps = defaultFPS;
			window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 160);
			window.titleContent.text = "Create animation";
			window.ShowUtility();
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("Create Animator & AnimationClip with name:");
			_animationName = EditorGUILayout.TextField(_animationName);
			_fps = EditorGUILayout.IntField("FPS:", _fps);

			_spritePath = EditorGUILayout.TextField("GameObject path (optional)", _spritePath);

			GUILayout.Space(24);

			EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_animationName));
			if (GUILayout.Button("create"))
			{
				Close();
				_callbackAccept?.Invoke(_animationName, _fps);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
