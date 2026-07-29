using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private bool isInteract;
    [SerializeField] private string interactionTag;


    void Start()
    {
        
    }

    void Update()
    {
        Interact();
       
    }

    void Interact()
    {
        if (isInteract && Keyboard.current.eKey.wasPressedThisFrame)
        {
            InteractionTypes(interactionTag);
        }
    }

    void InteractionTypes(string tag)
    {
        switch (tag)
        {
            case "Persona":
                print("Rescatado");
                break;

            case "Obstaculo":
                print("Despejado");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        isInteract = true;
        interactionTag = collision.tag;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInteract = false;
        interactionTag = null;
    }
}
