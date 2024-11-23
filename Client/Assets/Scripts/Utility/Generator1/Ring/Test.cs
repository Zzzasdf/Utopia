using UnityEngine;
using UnityEngine.UI;

namespace Generator.Ring111
{
    public class Test : MonoBehaviour
    {
        private Generator generator;
        [SerializeField] private int count = 10;

        private void Awake()
        {
            generator = GetComponent<Generator>();
            generator.AddListener(OnItemRender, OnItemSelected, OnItemFinalSelected);
        }

        private void OnItemRender(int index, Transform itemTra)
        {
            Text lbTxt = itemTra.Find("lbTxt").GetComponent<Text>();
            lbTxt.text = index.ToString();
        }

        private void OnItemSelected(int index)
        {
            Debug.Log($"选中 => {index}");
        }

        private void OnItemFinalSelected(int index)
        {
            Debug.LogWarning($"最终选中 => {index}");
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                generator.SetCount(count, generator.CurSelectedIndex());
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                generator.SetIndex(8);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                generator.SetScrollUntilIndex(4);
            }
        }
    }
}