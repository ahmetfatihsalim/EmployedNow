using EmployedNow.Domain.Enums;

namespace EmployedNow.Application.DTOs;

public record CreateConnectionRequest(Guid TargetUserId);
public record ConnectionResponse(Guid Id, Guid RequesterId, string RequesterEmail, Guid TargetId, string TargetEmail, ConnectionStatus Status, DateTime CreatedAt);
