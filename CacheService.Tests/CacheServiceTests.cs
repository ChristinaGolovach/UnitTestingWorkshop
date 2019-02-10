using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using CacheService.Storages;

namespace CacheService.Tests
{
    [TestFixture]
    public class CacheServiceTests
    {
        #region Ctor tests

        [TestCase(2)]
        public void CtorWithCapacity_CapacityMoreZero_CreateNewInstanceOfCacheService(int capacity)
        {
            // Arrange - Act
            CacheService<int, string> cacheService = new CacheService<int, string>(capacity, new InMemoryStorageFactory<int, string>());

            // Assert
            Assert.IsNotNull(cacheService);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void CtorWithCapacity_CapacityLessOne_ArgumentOutOfRangeException(int capacity)
        {
            CacheService<int, string> cacheService;

            // Arrange - Act - Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => cacheService = new CacheService<int, string>(capacity, new InMemoryStorageFactory<int, string>()));
        }

        [TestCase(1, null)]
        public void CtorWithCapacityAndEqualityComparer_EqualityComparerIsNull_ArgumentNullException(int capacity, IEqualityComparer<int> keyEqualityComparer)
        {
            CacheService<int, string> cacheService;

            // Arrange - Act - Assert
            Assert.Throws<ArgumentNullException>
                (() => cacheService = new CacheService<int, string>(capacity, keyEqualityComparer, new InMemoryStorageFactory<int, string>()));
        }

        [TestCase(1, null)]
        public void CtorWithCapacityAndEqualityComparerAndStorageFactory_StorageFactoryIsNull_ArgumentNullException(int capacity, StorageFactory<int, string> storageFactory)
        {
            // Arrange
            CacheService<int, string> cacheService;
            IEqualityComparer<int> keyEqualityComparer = EqualityComparer<int>.Default;

            // Act - Assert
            Assert.Throws<ArgumentNullException>
                (() => cacheService = new CacheService<int, string>(capacity, keyEqualityComparer, storageFactory));
        }

        #endregion Ctor tetst

        #region AddItem tests      

        [TestCase(2, "test", 60, ExpectedResult = 2)]
        [TestCase(2, "testNew", 60, ExpectedResult = 2)]
        [TestCase(11, "test", 700, ExpectedResult = 2)]
        public int AddItem_ValidArguments_ItemInCache(int keyCache, string valueCache, int lifeTimeInSecond)
        {
            // Arrange 
            var cachedItems = new Dictionary<int, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<StorageFactory<int, string>> storageFactoryMock = new Mock<StorageFactory<int, string>>();

            Mock<IStorage<int, string>> storageMock = GetStorageMock(cachedItems);
  
            storageFactoryMock.Setup(factory => factory.CreateStorage(It.IsAny<int>(), EqualityComparer<int>.Default)).Returns(storageMock.Object);

            var cacheService = new CacheService<int, string>(storageFactoryMock.Object);

            // Act
            cacheService.AddItem(keyCache, valueCache, lifeTimeInSecond);

            // Assert
            storageMock.Verify(s => s.Add(It.Is<int>(key => key == keyCache), It.Is<string>(value => value == valueCache), It.Is<int>(time => time == lifeTimeInSecond)), Times.Once);

            return cachedItems.Count;
        }



        private Mock<IStorage<int, string>> GetStorageMock(Dictionary<int, CacheItemModel<string>> cachedItems)
        {          
            Mock<IStorage<int, string>> storageMock = new Mock<IStorage<int, string>>();

            storageMock.SetupGet(s => s.CachedItemCount).Returns(cachedItems.Count);

            storageMock.Setup(s => s.Add(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Callback<int, string, int>((key, value, lifeTime) => cachedItems.Add(key, new CacheItemModel<string>() { Value = value, LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(lifeTime) }));

            storageMock.Setup(s => s.IsCachedItem(It.IsAny<int>())).Callback<int>(key => cachedItems.ContainsKey(key));

            storageMock.Setup(s => s.Remove(It.IsAny<int>())).Callback<int>(key => cachedItems.Remove(key));

            return storageMock;
        }

        #endregion AddItem tests

        #region GetItem tests
        #endregion GetItem tests
    }
}
