using System;

namespace Dodo1000Bot.Models.PublicApi;

public class UnitInfo
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public string Name { get; set; }

    public DepartmentState DepartmentState { get; set; }

    public UnitState State { get; set; }

    public UnitType Type { get; set; }

    public Location Location { get; set; }

    public string Address { get; set; }

    public AddressDetails AddressDetails { get; set; }

    public DateOnly BeginDateWork { get; set; }
}