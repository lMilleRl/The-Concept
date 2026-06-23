using System.Collections;
using UnityEngine;

// Базовый абстрактный проигрыватель катсцен.
// Наследует MonoBehaviour, поэтому поле типа CutscenePlayer сериализуется в инспекторе
// и принимает любого наследника (видео, спрайтовая анимация, Timeline и т.д.).
public abstract class CutscenePlayer : MonoBehaviour
{
    public abstract IEnumerator PlayCutscene(CutsceneData cutscene);
}
