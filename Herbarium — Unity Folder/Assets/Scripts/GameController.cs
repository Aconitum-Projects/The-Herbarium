using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject[] characters;
    public int currentCharacterIndex = 0;
    private CharacterMovement[] characterMovements;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        characterMovements = new CharacterMovement[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            characterMovements[i] = characters[i].GetComponent<CharacterMovement>();
            characterMovements[i].SetSelectionEffect(false);
        }
        UpdateCharacterControl();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCharacter();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                int clickedIndex = System.Array.IndexOf(characters, clickedObject);
                if (clickedIndex >= 0 && clickedIndex != currentCharacterIndex)
                {
                    DeselectAllCharacters();
                    currentCharacterIndex = clickedIndex;
                    UpdateCharacterControl();
                }
            }
        }
    }

    public void SwitchCharacter()
    {
        characterMovements[currentCharacterIndex].SetControl(false);
        characterMovements[currentCharacterIndex].SetSelectionEffect(false);

        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;

        UpdateCharacterControl();
    }

    private void UpdateCharacterControl()
    {
        characterMovements[currentCharacterIndex].SetControl(true);
        characterMovements[currentCharacterIndex].SetSelectionEffect(true);
    }

    private void DeselectAllCharacters()
    {
        for (int i = 0; i < characterMovements.Length; i++)
        {
            characterMovements[i].SetControl(false);
            characterMovements[i].SetSelectionEffect(false);
        }
    }
}