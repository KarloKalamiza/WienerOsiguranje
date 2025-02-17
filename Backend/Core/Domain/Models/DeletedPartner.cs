﻿using Backend.Core.Domain.Enums;

namespace Backend.Core.Domain.Models;

public class DeletedPartner
{
    public int PartnerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PartnerNumber { get; set; } = string.Empty;
    public string? CroatianPIN { get; set; } = string.Empty;
    public PartnerType PartnerTypeId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedByUser { get; set; } = string.Empty;
    public bool IsForeign { get; set; }
    public string ExternalCode { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DeletedByUser { get; set; } = string.Empty;
    public DateTime DeletedAtUtc { get; set; } = DateTime.UtcNow;
}
