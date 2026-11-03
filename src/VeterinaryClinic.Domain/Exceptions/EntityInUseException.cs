namespace VeterinaryClinic.Domain.Exceptions;

/// <summary>
/// Удаляемая запись используется другими сущностями.
/// </summary>
/// <param name="message">Описание причины отказа.</param>
public class EntityInUseException(string message) : Exception(message);
