using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore.InMemory;

namespace NewsAggregation.Tests.Unit.Services
{
    public class CategoryServiceTests
    {
        private readonly NewsAggregationContext _context;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            var options = new DbContextOptionsBuilder<NewsAggregationContext>()
                .UseInMemoryDatabase(databaseName: "CategoryServiceTestsDb")
                .Options;
            _context = new NewsAggregationContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _categoryService = new CategoryService(_context);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllActiveCategories()
        {
            // Arrange
            _context.Categories.AddRange(
                new Category { Name = "Tech", IsActive = true },
                new Category { Name = "Sports", IsActive = true },
                new Category { Name = "Politics", IsActive = false }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, c => Assert.True(c.IsActive));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var category = new Category { Name = "Tech", IsActive = true };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _categoryService.GetCategoryByIdAsync(999);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidData_ReturnsCreatedCategory()
        {
            // Arrange
            var category = new Category { Name = "New Category", IsActive = true };

            // Act
            var result = await _categoryService.CreateCategoryAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Category", result.Name);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WithValidData_ReturnsUpdatedCategory()
        {
            // Arrange
            var category = new Category { Name = "Old Name", IsActive = true };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            category.Name = "Updated Name";

            // Act
            var result = await _categoryService.UpdateCategoryAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var category = new Category { Name = "ToDelete", IsActive = true };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoryService.DeleteCategoryAsync(category.Id);

            // Assert
            Assert.True(result);
            var deleted = await _categoryService.GetCategoryByIdAsync(category.Id);
            Assert.NotNull(deleted);
            Assert.False(deleted.IsActive);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithInvalidId_ReturnsFalse()
        {
            // Act
            var result = await _categoryService.DeleteCategoryAsync(999);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCategoryKeywordsAsync_WithValidId_UpdatesKeywords()
        {
            // Arrange
            var category = new Category { Name = "KeywordCat", IsActive = true };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            var keywords = "news,tech,ai";

            // Act
            var result = await _categoryService.UpdateCategoryKeywordsAsync(category.Id, keywords);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(keywords, result.Keywords);
        }
    }
} 