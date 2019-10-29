using UnityEngine;
using UnityEditor;
using System;

namespace AnimTools
{
	public class DuplicateAnimatorPopup : EditorWindow
	{
		private Action<string, string> _callbackAccept;
		private string _animatorName;
		private string _replaceFrom;
		private string _replaceBy;

		public static void Show(string animatorName, Action<string, string> callbackAccept)
		{
			DuplicateAnimatorPopup window = CreateInstance<DuplicateAnimatorPopup>();
			window._callbackAccept = callbackAccept;
			window._animatorName = animatorName;
			window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 200);
			window.titleContent.text = "Duplicate '" + animatorName + "'";
			window.ShowUtility();
		}

		private void OnGUI()
		{
			EditorGUILayout.LabelField("rename all anims by replacing:");
			_replaceFrom = EditorGUILayout.TextField(_replaceFrom);
			EditorGUILayout.LabelField("with:");
			_replaceBy = EditorGUILayout.TextField(_replaceBy);
			GUILayout.Space(24);

			EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_replaceFrom)
				|| string.IsNullOrWhiteSpace(_replaceBy)
				|| !_animatorName.Contains(_replaceFrom));
			if (GUILayout.Button("duplicate"))
			{
				Close();
				_callbackAccept?.Invoke(_replaceFrom, _replaceBy);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
