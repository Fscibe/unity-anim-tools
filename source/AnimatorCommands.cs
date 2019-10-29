using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimTools
{
	public static class AnimatorCommands
	{
		private const string DUPLICATE_MENU_PATH = "Assets/AnimTools/Animator/Duplicate";

		/// <summary>
		/// Duplicate an animator and all its animations.
		/// </summary>
		public static AnimatorController Duplicate(AnimatorController animator, string replaceFrom, string replaceTo)
		{
			string animatorPath = AssetDatabase.GetAssetPath(animator);
			string animatorCopyPath = animatorPath.Replace(replaceFrom, replaceTo);
			AnimatorController animatorCopy = null;

			// duplicate animator
			if (AssetDatabase.CopyAsset(animatorPath, animatorCopyPath))
			{
				animatorCopy = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorCopyPath);

				// duplicate animation clips
				int nLayer = animatorCopy.layers.Length;
				for (int iLayer = 0; iLayer < nLayer; ++iLayer)
				{
					AnimatorStateMachine stateMachine = animatorCopy.layers[iLayer].stateMachine;
					int nState = stateMachine.states.Length;
					for (int iState = 0; iState < nState; ++iState)
					{
						AnimatorState state = stateMachine.states[iState].state;
						if (state.motion != null)
						{
							string clipPath = AssetDatabase.GetAssetPath(state.motion);
							string clipCopyPath = clipPath.Replace(replaceFrom, replaceTo);
							AssetDatabase.CopyAsset(clipPath, clipCopyPath);

							Motion clipCopy = AssetDatabase.LoadAssetAtPath<Motion>(clipCopyPath);
							state.motion = clipCopy;
						}
					}
				}

				// save
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			return animatorCopy;
		}

		[MenuItem(DUPLICATE_MENU_PATH, true)]
		static bool Duplicate_ContextMenuValidate(MenuCommand command)
		{
			return Selection.activeObject != null && Selection.activeObject is AnimatorController;
		}

		[MenuItem(DUPLICATE_MENU_PATH, false)]
		static void Duplicate_ContextMenu(MenuCommand command)
		{
			AnimatorController animator = Selection.activeObject as AnimatorController;
			DuplicateAnimatorPopup.Show(animator.name, delegate(string replaceFrom, string replaceTo)
			{
				Duplicate(animator, replaceFrom, replaceTo);
			});
		}
	}
}
