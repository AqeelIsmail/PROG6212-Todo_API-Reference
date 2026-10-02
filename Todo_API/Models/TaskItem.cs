using System;
using System.Collections.Generic;

namespace Todo_API.Models;

public partial class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsComplete { get; set; }
}
