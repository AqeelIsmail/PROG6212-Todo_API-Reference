using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_API.Controllers;
using Todo_API.Models;

namespace Todo_API.Tests;

public class BookingsControllerTests
{
    private static ToDoDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ToDoDbContext(options);
    }

    [Fact]
    public void GetAllTasks_ReturnsAllTasks()
    {
        // Arrange
        using var db = CreateContext();

        db.TaskItems.AddRange(
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Learn EF Core",
                IsComplete = false
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Build API",
                IsComplete = true
            }
        );

        db.SaveChanges();

        var controller = new BookingsController(db);

        // Act
        var result = controller.GetAllTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var tasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(
            okResult.Value);

        Assert.Equal(2, tasks.Count());
    }


    [Fact]
    public void GetTask_ExistingId_ReturnsOk()
    {
        // Arrange
        using var db = CreateContext();

        var id = Guid.NewGuid();

        db.TaskItems.Add(new TaskItem
        {
            Id = id,
            Title = "Test Task",
            IsComplete = false
        });

        db.SaveChanges();

        var controller = new BookingsController(db);

        // Act
        var result = controller.GetTask(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var task = Assert.IsType<TaskItem>(okResult.Value);

        Assert.Equal(id, task.Id);
        Assert.Equal("Test Task", task.Title);
        Assert.False(task.IsComplete);
    }


    [Fact]
    public void GetTask_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        using var db = CreateContext();

        var controller = new BookingsController(db);

        var id = Guid.NewGuid();

        // Act
        var result = controller.GetTask(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }


    [Fact]
    public void CreateTask_AddsTaskToDatabase()
    {
        // Arrange
        using var db = CreateContext();

        var controller = new BookingsController(db);

        var task = new TaskItem
        {
            Title = "New Task",
            IsComplete = false
        };

        // Act
        var result = controller.CreateTask(task);

        // Assert
        Assert.IsType<OkResult>(result);

        var savedTask = db.TaskItems.FirstOrDefault();

        Assert.NotNull(savedTask);
        Assert.Equal("New Task", savedTask.Title);
        Assert.False(savedTask.IsComplete);
        Assert.NotEqual(Guid.Empty, savedTask.Id);
    }


    [Fact]
    public void DeleteTask_ExistingTask_RemovesTask()
    {
        // Arrange
        using var db = CreateContext();

        var id = Guid.NewGuid();

        db.TaskItems.Add(new TaskItem
        {
            Id = id,
            Title = "Delete Me",
            IsComplete = false
        });

        db.SaveChanges();

        var controller = new BookingsController(db);

        // Act
        var result = controller.DeleteTask(id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var task = db.TaskItems.FirstOrDefault(t => t.Id == id);

        Assert.Null(task);
    }


    [Fact]
    public void DeleteTask_NonExistingTask_ReturnsNotFound()
    {
        // Arrange
        using var db = CreateContext();

        var controller = new BookingsController(db);

        var id = Guid.NewGuid();

        // Act
        var result = controller.DeleteTask(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }


    [Fact]
    public void UpdateTask_ExistingTask_UpdatesStatus()
    {
        // Arrange
        using var db = CreateContext();

        var id = Guid.NewGuid();

        db.TaskItems.Add(new TaskItem
        {
            Id = id,
            Title = "Complete Me",
            IsComplete = false
        });

        db.SaveChanges();

        var controller = new BookingsController(db);

        // Act
        var result = controller.UpdateTask(id, true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var updatedTask = Assert.IsType<TaskItem>(okResult.Value);

        Assert.True(updatedTask.IsComplete);
        Assert.Equal(id, updatedTask.Id);
    }


    [Fact]
    public void UpdateTask_NonExistingTask_ReturnsNotFound()
    {
        // Arrange
        using var db = CreateContext();

        var controller = new BookingsController(db);

        var id = Guid.NewGuid();

        // Act
        var result = controller.UpdateTask(id, true);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}