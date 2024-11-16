using UnityEngine;
using UnityEngine.UI;

public class RingScrollTest : MonoBehaviour
{
    private RingScrollGenerator ringScrollGenerator;

    private void Awake()
    {
        ringScrollGenerator = GetComponent<RingScrollGenerator>();
        ringScrollGenerator.AddListener(OnItemRender, OnItemSelected, OnItemFinalSelected);
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
            ringScrollGenerator.SetCount(10, 5);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ringScrollGenerator.SetCount(3, 1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ringScrollGenerator.SetCount(20, 10);
        }
    }
}