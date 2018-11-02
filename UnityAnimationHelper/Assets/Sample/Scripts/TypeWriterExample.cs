using AnimationHelpers;
using TMPro;
using UnityEngine;

namespace Assets.Sample.Scripts
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypeWriterExample : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TMP_Text>().TypeWriteText(0.1f, 1f).Execute();
        }
    }
}