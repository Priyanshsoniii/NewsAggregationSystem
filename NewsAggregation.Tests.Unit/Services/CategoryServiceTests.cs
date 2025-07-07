using Moq;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Technology", IsHidden = false },
                new Category { Id = 2, Name = "Sports", IsHidden = false },
                new Category { Id = 3, Name = "Politics", IsHidden = true }
            };

            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetAllAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Contains(resultList, c => c.Name == "Technology");
            Assert.Contains(resultList, c => c.Name == "Sports");
            Assert.Contains(resultList, c => c.Name == "Politics");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCategory()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Technology", IsHidden = false };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal("Technology", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var categoryId = 999;

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryService.GetByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ReturnsCreatedCategory()
        {
            // Arrange
            var category = new Category { Name = "New Category", IsHidden = false };
            var createdCategory = new Category { Id = 1, Name = "New Category", IsHidden = false };

            _mockCategoryRepository.Setup(x => x.CreateAsync(category)).ReturnsAsync(createdCategory);

            // Act
            var result = await _categoryService.CreateAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Category", result.Name);
            _mockCategoryRepository.Verify(x => x.CreateAsync(category), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ReturnsUpdatedCategory()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Updated Category", IsHidden = false };
            var updatedCategory = new Category { Id = 1, Name = "Updated Category", IsHidden = false };

            _mockCategoryRepository.Setup(x => x.UpdateAsync(category)).ReturnsAsync(updatedCategory);

            // Act
            var result = await _categoryService.UpdateAsync(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Updated Category", result.Name);
            _mockCategoryRepository.Verify(x => x.UpdateAsync(category), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var categoryId = 1;

            _mockCategoryRepository.Setup(x => x.DeleteAsync(categoryId)).ReturnsAsync(true);

            // Act
            var result = await _categoryService.DeleteAsync(categoryId);

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(x => x.DeleteAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var categoryId = 999;

            _mockCategoryRepository.Setup(x => x.DeleteAsync(categoryId)).ReturnsAsync(false);

            // Act
            var result = await _categoryService.DeleteAsync(categoryId);

            // Assert
            Assert.False(result);
            _mockCategoryRepository.Verify(x => x.DeleteAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetActiveCategoriesAsync_ReturnsOnlyActiveCategories()
        {
            // Arrange
            var allCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Technology", IsHidden = false },
                new Category { Id = 2, Name = "Sports", IsHidden = false },
                new Category { Id = 3, Name = "Politics", IsHidden = true },
                new Category { Id = 4, Name = "Entertainment", IsHidden = false }
            };

            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(allCategories);

            // Act
            var result = await _categoryService.GetActiveCategoriesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.All(resultList, c => Assert.False(c.IsHidden));
            Assert.Contains(resultList, c => c.Name == "Technology");
            Assert.Contains(resultList, c => c.Name == "Sports");
            Assert.Contains(resultList, c => c.Name == "Entertainment");
            Assert.DoesNotContain(resultList, c => c.Name == "Politics");
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var category = new Category { Name = "Test Category" };

            _mockCategoryRepository.Setup(x => x.CreateAsync(category))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _categoryService.CreateAsync(category));
        }

        [Fact]
        public async Task UpdateAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Test Category" };

            _mockCategoryRepository.Setup(x => x.UpdateAsync(category))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _categoryService.UpdateAsync(category));
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockCategoryRepository.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _categoryService.GetAllAsync());
        }
    }
} 