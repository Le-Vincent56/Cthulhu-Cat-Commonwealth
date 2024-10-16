using Tethered.Player;

namespace Tethered.Interactables
{
    public interface IInteractable
    {
        void ShowInteractSymbol();
        void HideInteractSymbol();
        void Interact(PlayerController controller);
    }
}
