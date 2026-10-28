namespace VeterinaryClinic.Application;

/// <summary>
/// Удаляемая запись используется другими сущностями.
/// </summary>
/// <param name="message">Описание причины отказа.</param>
public class EntityInUseException(string message) : Exception(message);
