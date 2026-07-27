namespace TextBox
{
    /// <summary>
    /// Правила формата команд в link-тегах TMP.
    /// 
    /// ФОРМАТ: <link="commandName=param1:param2:...">text</link>
    /// 
    /// РАЗДЕЛИТЕЛИ:
    ///   '=' — отделяет имя команды от блока параметров
    ///   ':' — отделяет параметры друг от друга
    /// 
    /// КОМАНДЫ И ПАРАМЕТРЫ (порядок фиксирован):
    /// 
    ///   pause=seconds
    ///     Пауза перед следующим символом.
    ///     Params[0] = seconds (float)
    ///     Пример: <link="pause=0.5">.</link>
    /// 
    ///   speed=charsPerSecond
    ///     Изменить скорость печати.
    ///     Params[0] = charsPerSecond (float)
    ///     Пример: <link="speed=10">slow text</link>
    /// 
    ///   mute
    ///     Заглушить голос. Без параметров.
    ///     Пример: <link="mute">...</link>
    /// 
    ///   resume
    ///     Возобновить голос. Без параметров.
    ///     Пример: <link="resume">text</link>
    /// 
    ///   wave=amplitude:frequency
    ///     Волновой эффект на вершинах.
    ///     Params[0] = amplitude (float)
    ///     Params[1] = frequency (float)
    ///     Пример: <link="wave=2:0.5">wobbly</link>
    /// 
    ///   shake=intensity:frequency:decay
    ///     Тряска символов.
    ///     Params[0] = intensity (float)
    ///     Params[1] = frequency (float)
    ///     Params[2] = decay (float, опционально, 0 = без затухания)
    ///     Пример: <link="shake=3:10:0.5">BOOM</link>
    /// 
    ///   distortion=intensity
    ///     Искажение вершин.
    ///     Params[0] = intensity (float)
    ///     Пример: <link="distortion=1.5">glitch</link>
    /// 
    ///   event=eventID
    ///     Вызов внешнего события по ID.
    ///     Params[0] = eventID (float, используется как int-идентификатор)
    ///     Пример: <link="event=1">trigger</link>
    /// 
    /// ПРИМЕЧАНИЯ:
    ///   - Имена команд регистронезависимы (Enum.TryParse с ignoreCase=true)
    ///   - Если параметров нет, блок после '=' опускается
    ///   - Десятичный разделитель — точка (InvariantCulture)
    ///   - text между тегами link определяет StartCharIndex и CharLength
    /// </summary>
    public static class TextBoxCommandRules { }
}
