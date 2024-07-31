using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using son.Controllers;
using son.Data;
using son.Models;
using Xunit;

namespace son.Tests.Controllers
{
    public class CoursesControllerTests : IDisposable
    {
        private readonly sonContext _context;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            var options = new DbContextOptionsBuilder<sonContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new sonContext(options);

            _context.Courses.AddRange(
                new Course { CourseId = 1, CourseName = "Course 1", CourseDescription = "Description 1", Credits = 3 },
                new Course { CourseId = 2, CourseName = "Course 2", CourseDescription = "Description 2", Credits = 4 }
            );
            _context.SaveChanges();

            _controller = new CoursesController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAListOfCourses()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Course>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithCourse()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(1, model.CourseId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            // Act
            var result = await _controller.Details(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var course = new Course { CourseId = 3, CourseName = "Course 3", CourseDescription = "Description 3", Credits = 5 };

            // Act
            var result = await _controller.Create(course);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(3, _context.Courses.CountAsync().Result);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithCourse()
        {
            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(1, model.CourseId);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Delete(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithCourse()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
            Assert.Equal(1, model.CourseId);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex()
        {
            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(1, _context.Courses.CountAsync().Result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
