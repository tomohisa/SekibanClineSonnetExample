using Sekiban.Pure.Aggregates;

namespace SchoolManagement.Domain.Aggregates.Students.Payloads;

[GenerateSerializer]
public record DeletedStudent(
    string Name,
    string StudentId,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber,
    string Address,
    Guid? ClassId = null
) : IAggregatePayload;
