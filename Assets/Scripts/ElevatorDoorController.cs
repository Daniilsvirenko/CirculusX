using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ElevatorDoorController : MonoBehaviour
{
    private Animation anim;
    private bool isPlaying = false;
    private bool hasOpened = false; // Флаг, чтобы двери не открывались дважды

    public float skipSecondsAtStart = 0f; // Укажите здесь задержку (в секундах), которую нужно пропустить
    public float stopAnimationAtSecond = 1.5f; // Укажите здесь секунду, на которой двери максимально открыты
    public bool openOnStart = false; // Поставьте галочку, чтобы лифт открывался сам в начале игры

    void Start()
    {
        anim = GetComponent<Animation>();
        anim.playAutomatically = false;
        anim.wrapMode = WrapMode.Once;

        // Если это начальный лифт, открываем двери сразу при запуске
        if (openOnStart)
        {
            OpenDoors();
        }
    }

    void Update()
    {
        if (isPlaying && anim != null && anim.clip != null)
        {
            // Если анимация дошла до нужной секунды (двери открылись), ставим её на паузу
            if (anim[anim.clip.name].time >= stopAnimationAtSecond)
            {
                anim[anim.clip.name].speed = 0; // Останавливаем анимацию навсегда
                isPlaying = false; // Больше не проверяем
            }
        }
    }

    public void OpenDoors()
    {
        if (anim != null && !isPlaying && !hasOpened)
        {
            isPlaying = true;
            hasOpened = true; // Запоминаем, что мы уже нажали кнопку
            
            // Начинаем проигрывать
            anim.Play();
            
            // Пропускаем пустые кадры в начале (если анимация тупит)
            anim[anim.clip.name].time = skipSecondsAtStart;
            anim[anim.clip.name].speed = 1; // Убеждаемся, что скорость нормальная
        }
    }
}
