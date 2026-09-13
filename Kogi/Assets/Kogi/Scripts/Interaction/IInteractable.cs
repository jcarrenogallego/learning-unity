namespace Kogi.Scripts.Interaction
{
    public interface IInteractable
    {
        string Prompt { get; }
        void Interact();
    }
}
