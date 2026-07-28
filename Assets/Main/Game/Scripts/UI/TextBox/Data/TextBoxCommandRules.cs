namespace TextBox
{
    /// <summary>
    /// Правила формата команд в link-тегах TMP.
    /// 
    /// ФОРМАТ: &lt;link="commandName=param1:param2:..."&gt;text&lt;/link&gt;
    /// 
    /// РАЗДЕЛИТЕЛИ:
    ///   '=' — отделяет имя команды от блока параметров
    ///   ':' — отделяет параметры друг от друга
    /// 
    /// ═══════════════════════════════════════════════════
    /// КОМАНДЫ (влияют на TypeRunner / VoiceSpeaker):
    /// ═══════════════════════════════════════════════════
    /// 
    ///   pause=seconds
    ///     Пауза перед следующим символом.
    ///     Params[0] = seconds (float)
    ///     Получатель: TypeRunner.SetPause
    ///     Пример: &lt;link="pause=0.5"&gt;.&lt;/link&gt;
    /// 
    ///   speed=charsPerSecond
    ///     Изменить скорость печати.
    ///     Params[0] = charsPerSecond (float)
    ///     Получатель: TypeRunner.SetSpeed
    ///     Пример: &lt;link="speed=10"&gt;slow text&lt;/link&gt;
    /// 
    ///   mute
    ///     Заглушить голос. Без параметров.
    ///     Получатель: VoiceSpeaker.Mute
    ///     Пример: &lt;link="mute"&gt;&lt;/link&gt;
    /// 
    ///   resume
    ///     Возобновить голос. Без параметров.
    ///     Получатель: VoiceSpeaker.Resume
    ///     Пример: &lt;link="resume"&gt;&lt;/link&gt;
    /// 
    ///   event=eventID
    ///     Вызов внешнего события по ID.
    ///     Params[0] = eventID (float, используется как int)
    ///     Получатель: внешний слушатель через делегат
    ///     Пример: &lt;link="event=1"&gt;&lt;/link&gt;
    /// 
    /// ═══════════════════════════════════════════════════
    /// ЭФФЕКТЫ (влияют на TextChanger, применяются к символам внутри тега):
    /// ═══════════════════════════════════════════════════
    /// 
    ///   wave=intensity:speed
    ///     Волновой эффект на вершинах.
    ///     Params[0] = intensity (float) — амплитуда смещения
    ///     Params[1] = speed (float) — скорость анимации
    ///     Получатель: TextChanger.AddEffect → ITextEffect (Wave)
    ///     Пример: &lt;link="wave=2:3"&gt;wobbly&lt;/link&gt;
    /// 
    ///   shake=intensity:speed
    ///     Тряска символов.
    ///     Params[0] = intensity (float) — сила тряски
    ///     Params[1] = speed (float) — частота обновления
    ///     Получатель: TextChanger.AddEffect → ITextEffect (Shake)
    ///     Пример: &lt;link="shake=3:10"&gt;BOOM&lt;/link&gt;
    /// 
    ///   distortion=intensity:speed
    ///     Искажение вершин через шум.
    ///     Params[0] = intensity (float) — сила искажения
    ///     Params[1] = speed (float) — скорость шума
    ///     Получатель: TextChanger.AddEffect → ITextEffect (Distortion)
    ///     Пример: &lt;link="distortion=1.5:2"&gt;glitch&lt;/link&gt;
    /// 
    /// ═══════════════════════════════════════════════════
    /// ПРИМЕЧАНИЯ:
    /// ═══════════════════════════════════════════════════
    ///   - Имена команд регистронезависимы (Enum.TryParse с ignoreCase=true)
    ///   - Если параметров нет, блок после '=' опускается
    ///   - Десятичный разделитель — точка (InvariantCulture)
    ///   - text между тегами link определяет StartCharIndex и CharLength
    ///   - Команды срабатывают при достижении StartCharIndex парсером
    ///   - Эффекты применяются к диапазону [StartCharIndex, StartCharIndex + CharLength)
    /// </summary>
    public static class TextBoxCommandRules { }
}
