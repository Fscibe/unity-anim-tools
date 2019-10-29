using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimTools
{
	public static class AnimationClipCommands
	{
		private const string CREATE_FROM_SPRITES_MENU_PATH = "Assets/AnimTools/Animation/CreateSpriteAnimation";

		public static void CreateSpriteAnimation(int fps, string folderPath, string animName, string spriteNodePath, Texture2D[] frames)
		{
			if (frames.Length <= 0)
				return;

			// sort by name
			System.Array.Sort(frames, (a, b) => string.Compare(a.name, b.name));

			// get sprite of each texture
			List<Sprite> sprites = new List<Sprite>();
			foreach (Texture2D tex in frames)
			{
				string path = AssetDatabase.GetAssetPath(tex);
				Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
				foreach (Object subAsset in assets)
				{
					if (subAsset is Sprite)
					{
						sprites.Add((Sprite)subAsset);
					}
				}
			}

			// animation
			bool isNew = false;
			string animFileName = folderPath + "/" + animName + ".anim";
			AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animFileName);
			if (animClip == null)
			{
				animClip = new AnimationClip();
				isNew = true;
			}
			animClip.name = animName;
			animClip.frameRate = fps;

			EditorCurveBinding curveBinding = new EditorCurveBinding();
			curveBinding.type = typeof(SpriteRenderer);
			curveBinding.propertyName = "m_Sprite";
			curveBinding.path = spriteNodePath;

			ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Count];
			for (int frameIndex = 0; frameIndex < keyframes.Length; ++frameIndex)
			{
				keyframes[frameIndex] = new ObjectReferenceKeyframe();
				keyframes[frameIndex].time = (1.0f / animClip.frameRate) * frameIndex;
				keyframes[frameIndex].value = sprites[frameIndex];
			}
			AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyframes);

			if (isNew)
			{
				AssetDatabase.CreateAsset(animClip, animFileName);
			}

			// controller
			string controllerPath = folderPath + "/" + animName + ".controller";
			AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
			if (controller == null)
			{
				controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
			}
			AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
			if (!FindState(rootStateMachine, animName, out AnimatorState state))
			{
				state = rootStateMachine.AddState(animName);
			}
			state.motion = animClip;

			// save
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		private static bool FindState(AnimatorStateMachine stateMachine, string name, out AnimatorState result)
		{
			foreach (var state in stateMachine.states)
			{
				if (state.state.name.Equals(name))
				{
					result = state.state;
					return true;
				}
			}
			result = null;
			return false;
		}
		
		[MenuItem(CREATE_FROM_SPRITES_MENU_PATH, true)]
		static bool CreateFromSprites_ContextMenuValidate(MenuCommand command)
		{
			Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
			return textures.Length > 0;
		}

		[MenuItem(CREATE_FROM_SPRITES_MENU_PATH, false)]
		static void CreateFromSprites_ContextMenu(MenuCommand command)
		{
			Texture2D[] frames = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
			if (frames.Length > 0)
			{
				Texture2D firstFrame = frames[0];
				string firstFramePath = AssetDatabase.GetAssetPath(firstFrame);
				string folderPath = Path.GetDirectoryName(firstFramePath);
				string defaultAnimName = Path.GetFileNameWithoutExtension(firstFramePath);
				int defaultFPS = 24;

				CreateAnimationPopup.Show(defaultAnimName, defaultFPS, delegate (string animName, int fps)
				{
					CreateSpriteAnimation(fps, folderPath, animName, "", frames);
				});
			}
		}
	}
}
