using UnityEngine;
using UnityEngine.UIElements;

public class PartyDataScreen : MonoBehaviour
{
    [SerializeField] private string knightName;
    private VisualElement rootVisualElement;
    
    private void Awake()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;

        rootVisualElement.Q<Label>("NameLabel").text = "冒险家";
        rootVisualElement.Query<Label>("NameLabel").AtIndex(1).text = knightName;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rootVisualElement.style.display =
                rootVisualElement.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
