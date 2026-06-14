using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ElevatorDoorController : MonoBehaviour
{
    private Animation anim;
    private bool isPlaying = false;
    private bool hasOpened = false; // Флаг, чтобы двери не открывались дважды

    public float skipSecondsAtStart = 0f; // Укажите здесь задержку (в секундах), которую нужно пропустить (если число отрицательное, например -1, это будет задержка перед стартом)
    public float stopAnimationAtSecond = 1.5f; // Укажите здесь секунду, на которой двери максимально открыты
    public bool openOnStart = false; // Поставьте галочку, чтобы лифт открывался сам в начале игры

    [Header("Audio Settings")]
    public AudioSource doorAudioSource;
    [Tooltip("С какой секунды звукового файла начать воспроизведение (обрезаем начало)")]
    public float audioStartSecond = 0f;
    [Tooltip("Через сколько секунд выключить звук, если он слишком длинный (0 = играть до конца)")]
    public float audioDuration = 0f;

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

        // Логика обрезки звука в конце
        if (doorAudioSource != null && doorAudioSource.isPlaying && audioDuration > 0)
        {
            if (doorAudioSource.time >= (audioStartSecond + audioDuration))
            {
                doorAudioSource.Stop();
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
            
            // Пропускаем пустые кадры в начале (если анимация тупит), или делаем задержку
            anim[anim.clip.name].time = skipSecondsAtStart;
            anim[anim.clip.name].speed = 1; // Убеждаемся, что скорость нормальная

            // Запускаем звук с нужной секунды
            if (doorAudioSource != null && doorAudioSource.clip != null)
            {
                doorAudioSource.time = audioStartSecond;
                doorAudioSource.Play(); // Возвращаем мгновенный старт звука
            }
        }
    }

    public void ResetDoors()
    {
        if (anim != null && anim.clip != null)
        {
            // Сбрасываем анимацию в самое начало (двери закрыты)
            anim.Stop();
            anim.Rewind();
            anim.Play();
            anim[anim.clip.name].time = 0;
            anim.Sample();
            anim.Stop();

            isPlaying = false;
            hasOpened = false;

            // Если это начальный лифт, он снова должен открыться сам
            if (openOnStart)
            {
                OpenDoors();
            }
            else
            {
                if (doorAudioSource != null)
                {
                    doorAudioSource.Stop();
                }
            }
        }
    }
}
