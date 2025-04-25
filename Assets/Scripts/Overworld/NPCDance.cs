using UnityEngine;

public class NPCDance : MonoBehaviour
{
    private CharacterDialogueData characterDialogueData;

    private int frameNumber = 0;
    private int index = 0;

    void Start()
    {
        characterDialogueData = GetComponent<CharacterDialogueData>();
    }

    void Update()
    {
        if (frameNumber == 15)
        {
            characterDialogueData.spriteRenderer.sprite = characterDialogueData.faces[index];

            index = (index + 1) % 4;

            frameNumber = 0;
        }

        frameNumber++;
    }
}
