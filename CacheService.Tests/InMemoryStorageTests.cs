using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CacheService.Storages;
using CacheService.Exceptions;

namespace CacheService.Tests
{
    [TestFixture]
    public class InMemoryStorageTests
    {
        #region Ctor tests

        [TestCase(2)]
        public void Ctor_CapacityMoreZero_CreateNewInstanceOfInMemoryStorage(int capacity)
        {
            // Arrange - Act
            InMemoryStorage<int, string> inMemoryStorage = new InMemoryStorage<int, string>(capacity, EqualityComparer<int>.Default);

            // Assert
            Assert.IsNotNull(inMemoryStorage);
        }

        [TestCase(0)]
        public void Ctor_CapacityLessOne_ArgumentOutOfRangeException(int capacity)
        {
            InMemoryStorage<int, string> inMemoryStorage;

            // Arrange - Act - Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => inMemoryStorage = new InMemoryStorage<int, string>(capacity, EqualityComparer<int>.Default));   
        }

        [TestCase(1)]
        public void Ctor_EqualityComparerIsNull_ArgumentNullException(int capacity)
        {
            InMemoryStorage<int, string> inMemoryStorage;

            // Arrange - Act - Assert
            Assert.Throws<ArgumentNullException>(() => inMemoryStorage = new InMemoryStorage<int, string>(capacity, null));
        }

        #endregion Ctor tests

        #region Add tests

        [TestCase(1, "test1", 30)]
        public void Add_ValidData_CountItemOnCacheIsOne(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            // Act
            inMemoryStorage.Add(key, value, lifeTime);

            // Assert
            Assert.AreEqual(1, inMemoryStorage.CachedItemCount);
        }

        [TestCase(1, "test1", 30)]
        public void Add_AddExcistedItem_ArgumentException(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            // Act
            inMemoryStorage.Add(key, value, lifeTime);

            // Assert
            Assert.Throws<ArgumentException>(() => inMemoryStorage.Add(key, value, lifeTime));
        }

        [TestCase(null, "test1", 30)]
        public void Add_KeyIsNull_ArgumentNullException(int? key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int?, string>(2, EqualityComparer<int?>.Default);

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => inMemoryStorage.Add(key, value, lifeTime));
        }

        [TestCase(1, "test1", 0)]
        public void Add_LifetimeIsLessOne_ArgumentOutOfRangeException(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            // Act - Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => inMemoryStorage.Add(key, value, lifeTime));
        }

        #endregion Add tests

        #region Remove tetst

        [TestCase(1, "test1", 30)]
        public void Remove_ValidData_CountItemOnCacheIsZero(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            inMemoryStorage.Add(key, value, lifeTime);

            // Act
            inMemoryStorage.Remove(key);

            // Assert
            Assert.AreEqual(0, inMemoryStorage.CachedItemCount);
        }

        [TestCase(1)]
        public void Remove_NotExistedKey_ArgumentException(int key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            // Act - Assert
            Assert.Throws<ArgumentException>(() => inMemoryStorage.Remove(key));           
        }

        [TestCase(null)]
        public void Remove_KeyIsNull_ArgumentNullException(int? key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int?, string>(2, EqualityComparer<int?>.Default);

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => inMemoryStorage.Remove(key));
        }

        #endregion Remove tests

        #region Get tests

        [TestCase(1, "test1", 30)]
        public void Get_ValidData_ReturnItem(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            inMemoryStorage.Add(key, value, lifeTime);

            // Act
            var item =  inMemoryStorage.Get(key);

            // Assert
            Assert.AreEqual(value, item.Value);
        }
        
        [TestCase(null)]
        public void Get_KeyIsNull_ArgumentNullException(int? key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int?, string>(2, EqualityComparer<int?>.Default);           

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => inMemoryStorage.Get(key));
        }

        [TestCase(1)]
        public void Get_NonexistentKey_CachedItemNotFoundException(int key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            // Act - Assert
            Assert.Throws<CachedItemNotFoundException>(() => inMemoryStorage.Get(key));
        }

        #endregion Get tests

        #region GetAll tests

        [TestCase(new int[] { 1, 2 })]
        public void GetAll_AddItemTwoTimes_ReturnTwoItems(int [] keys)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(4, EqualityComparer<int>.Default);

            foreach (int key in keys)
            {
                inMemoryStorage.Add(key, "test", 60);
            }            

            // Act
            var items = inMemoryStorage.GetAll();

            // Assert
            Assert.AreEqual(keys.Length, items.Count());     
        }

        [TestCase]
        public void GetAll_EmptyStorage_ReturnZeroItems()
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(4, EqualityComparer<int>.Default);

             // Act
            var items = inMemoryStorage.GetAll();

            // Assert
            Assert.AreEqual(0, items.Count());
        }

        #endregion GetAll tests

        #region IsCachedItem tests

        [TestCase(1, "test1", 30)]
        public void IsCachedItem_CachedItem_ReturnTrue(int key, string value, int lifeTime)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);

            inMemoryStorage.Add(key, value, lifeTime);

            // Act
            bool isCached = inMemoryStorage.IsCachedItem(key);

            // Assert
            Assert.AreEqual(true, isCached);
        }

        [TestCase(1)]
        public void IsCachedItem_NonCachedItem_ReturnFalse(int key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<int, string>(2, EqualityComparer<int>.Default);           

            // Act
            bool isCached = inMemoryStorage.IsCachedItem(key);

            // Assert
            Assert.AreEqual(false, isCached);
        }

        [TestCase(null)]
        public void IsCachedItem_KeyIsNull_ArgumentNullException(string key)
        {
            // Arrange
            var inMemoryStorage = new InMemoryStorage<string, string>(2, EqualityComparer<string>.Default);

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => inMemoryStorage.IsCachedItem(key));      
        }

        #endregion IsCachedItem tests
    }
}
