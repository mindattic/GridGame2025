//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class TitleManager : MonoBehaviour
//{
//   //External properties
//   public string text
//   {
//       get => label.text;
//       set => label.text = value;
//   }
//   public Color color
//   {
//       get => label.color;
//       set => label.color = value;
//   }
//   #endregion

//   //Fields
//   public TextMeshProUGUI label;
//   float minAlpha = Opacity.Transparent;
//   public float maxAlpha = Opacity.Percent70;


//   //Method which is used for initialization tasks that need to occur before the game starts 
//   void Awake()
//   {
//       label = GameObject.Find(Constants.Title).GetComponent<TextMeshProUGUI>();
//       label.transform.localPosition = new Vector3(0, 680, 0);


//       minAlpha = Opacity.Transparent;
//       maxAlpha = Opacity.Percent70;
//       label.color = new Color(1, 1, 1, minAlpha);
//   }

//   public void UpdateText(string text, Color color = default)
//   {
//       label.text = text;
//       label.color = color;
//   }

//   public void TriggerFadeIn(string text, Color color = default)
//   {
//       if (color == default)
//           color = ColorHelper.Solid.White;

//       StartCoroutine(@@FadeIn(text, color));
//   }


//   public IEnumerator @@FadeIn(string text, Color color = default)
//   {
//       if (color == default)
//           color = ColorHelper.Solid.White;

//       //Before:
//       const float startAngle = -90f;
//       const float endAngle = 0f;
//       const float increment = 4f;
//       float angle = startAngle;
//       transform.eulerAngles = new Vector3(angle, 0, 0);
//       UpdateText(text, color);

//       //During
//       while (angle < endAngle)
//       {
//           angle += increment;
//           angle = Mathf.Clamp(angle, startAngle, endAngle);
//           transform.eulerAngles = new Vector3(angle, 0, 0);
//           yield return Wait.OneTick();
//       }

//       //After:
//       angle = endAngle;
//       transform.eulerAngles = new Vector3(angle, 0, 0);
//   }

//   public void TriggerFadeOut(float delay = 0)
//   {
//       StartCoroutine(FadeOut(delay));
//   }

//   private IEnumerator FadeOut(float delay = 0)
//   {
//       //Begin:
//       float alpha = maxAlpha;
//       label.color = new Color(1f, 1f, 1f, alpha);

//       if (delay != 0)
//           yield return Wait.For(delay);

//       //During:
//       while (alpha > minAlpha)
//       {
//           alpha -= Increment.TenPercent;
//           alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
//           label.color = new Color(1, 1, 1, alpha);
//           yield return Wait.OneTick();
//       }

//       //After:
//   }

//}
