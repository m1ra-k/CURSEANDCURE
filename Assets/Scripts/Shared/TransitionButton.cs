using UnityEngine;
using UnityEngine.UI;

public class TransitionButton : MonoBehaviour
{
    public GameProgressionManager GameProgressionManagerInstance;
    public string buttonType;
    private Button progressionButton;

    void Awake()
    {
        GameProgressionManagerInstance = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManager>();
    }

    void Start()
    {
        progressionButton = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            progressionButton.onClick.Invoke();
        }
    }

    public void Transition()
    {
        GameProgressionManagerInstance.TransitionScene(buttonType);
    }
}
