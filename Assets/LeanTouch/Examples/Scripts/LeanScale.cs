using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to scale the current GameObject
	public class LeanScale : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers;

		[Tooltip("Allows you to force rotation with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("Does scaling require an object to be selected?")]
		public LeanSelectable RequiredSelectable;

		[Tooltip("Should the scaling be performanced relative to the finger center?")]
		public bool Relative;
		
		protected virtual void Update()
		{
			// If we require a selectable and it isn't selected, cancel scaling
			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			// Get the fingers we want to use
			var fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);

			// Calculate the scaling values based on these fingers
			var scale        = LeanGesture.GetPinchScale(fingers);
			var screenCenter = LeanGesture.GetScreenCenter(fingers);

			// Perform the scaling
			Scale(scale, screenCenter);
		}

		private void Scale(float scale, Vector2 screenCenter)
		{
			// Make sure the scale is valid
			if (scale > 0.0f)
			{
				if (Relative == true)
				{
					// Screen position of the transform
					var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			
					// Push the screen position away from the reference point based on the scale
					screenPosition.x = screenCenter.x + (screenPosition.x - screenCenter.x) * scale;
					screenPosition.y = screenCenter.y + (screenPosition.y - screenCenter.y) * scale;
			
					// Convert back to world space
					transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
			
					// Grow the local scale by scale
					transform.localScale *= scale;
				}
				else
				{
					// Grow the local scale by scale
					transform.localScale *= scale;
				}
			}
		}
	}
}