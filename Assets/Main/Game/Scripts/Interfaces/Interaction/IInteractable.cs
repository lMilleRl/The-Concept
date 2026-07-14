/// <summary>
/// Чтобы временно запретить взаимодействие с объектом — отключи компонент MonoBehaviour (enabled = false).
/// PlayerInteraction пропускает disabled-компоненты.
/// </summary>
public interface IInteractable
{
    void Activate();
}
