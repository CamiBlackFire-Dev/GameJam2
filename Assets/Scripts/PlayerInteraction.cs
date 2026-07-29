using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public bool isInteract;
    public string interactionTag;
    public int pressButton;
    public ObjectInteraction interaction;
    public GameManager gameManager;


    void Update()
    {
        Interact();      
    }

    void Interact()
    {
        if (isInteract && Keyboard.current.eKey.wasPressedThisFrame)
        {
            pressButton ++;
            InteractionTypes(interactionTag, pressButton);
        }
    }

    void InteractionTypes(string tag, int press)
    {
        switch (tag)
        {
            case "Persona":
                interaction.CivilianInteraction();
                gameManager.PlayerScore();
                print("Rescatado");
                break;

            case "Obstaculo":
                interaction.DebrisInteraction(press);
                print("Despejado");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        isInteract = true;
        interactionTag = collision.tag;
        interaction = collision.GetComponent<ObjectInteraction>();
        gameManager = collision.GetComponent<GameManager>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInteract = false;
        interactionTag = null;
        pressButton = 0;
    }
}
