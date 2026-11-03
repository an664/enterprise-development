namespace VeterinaryClinic.Domain.Exceptions;

/// <summary>
/// Кабинет занят другим приёмом в указанное время.
/// </summary>
public class AppointmentConflictException() : Exception("На это время в кабинете уже назначен приём.");
