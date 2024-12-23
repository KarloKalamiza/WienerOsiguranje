﻿namespace Backend.Models;

public class InsurancePolicy
{
    public int InsurancePolicyId { get; }
    public string PolicyNumber { get; set; } = string.Empty;
    public Decimal PolicyAmount { get; set; } = decimal.Zero;
}
