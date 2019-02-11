using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using CacheService.Storages;
using CacheService.Exceptions;

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

            Mock<IStorage<int, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act
            cacheService.AddItem(keyCache, valueCache, lifeTimeInSecond);

            // Assert
            storageMock.Verify(s => s.Add(It.Is<int>(key => key == keyCache),
                                          It.Is<string>(value => value == valueCache),
                                          It.Is<int>(time => time == lifeTimeInSecond)), Times.Once);

            return cachedItems.Count;
        }

        [TestCase(1, 2, "test", 160)]
        public void AdItem_AddItemInFullCach_LastAccessedItemIsDeleted(int cacheCapacity, int keyCache, string valueCache, int lifeTimeInSecond)
        {
            // Arrange 
            var cachedItems = new Dictionary<int, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int, string>> storageMock = GetStorageMock(cachedItems);

            Mock<StorageFactory<int, string>> storageFactoryMock = GetStorageFactoryMock(storageMock);

            var cacheService = new CacheService<int, string>(cacheCapacity, storageFactoryMock.Object);

            // Act
            cacheService.AddItem(keyCache, valueCache, lifeTimeInSecond);

            // Assert
            storageMock.Verify(s => s.Remove(It.Is<int>(key => key == 1)), Times.Once);

            Assert.AreEqual(1, cachedItems.Count);

        }

        [TestCase(null, "test", 160)]
        public void AdItem_AddItemWithNullKey_ArgumentNullException(int? keyCache, string valueCache, int lifeTimeInSecond)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int?, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act - Assert
            Assert.Throws<ArgumentNullException>(() => cacheService.AddItem(keyCache, valueCache, lifeTimeInSecond));

            storageMock.Verify(s => s.Add(It.Is<int?>(key => key == keyCache), 
                                          It.Is<string>(value => value == valueCache), 
                                          It.Is<int>(time => time == lifeTimeInSecond)), Times.Never);

        }

        [TestCase(12, "test", 0)]
        public void AdItem_AddItemWithTimeLessOne_ArgumentOutOfRangeException(int? keyCache, string valueCache, int lifeTimeInSecond)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int?, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act - Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => cacheService.AddItem(keyCache, valueCache, lifeTimeInSecond));

            storageMock.Verify(s => s.Add(It.Is<int?>(key => key == keyCache),
                                          It.Is<string>(value => value == valueCache),
                                          It.Is<int>(time => time == lifeTimeInSecond)), Times.Never);

        }

        #endregion AddItem tests

        #region GetItem tests

        [TestCase(1)]
        public void GetItem_ValidKey_ReturnValue(int? keyCache)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int?, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act
            var cachedValue = cacheService.GetItem(keyCache);

            // Assert
            Assert.AreEqual("test", cachedValue);

            storageMock.Verify(s => s.Get(It.Is<int?>(key => key == keyCache)), Times.Once);
        }

        [TestCase(null)]
        public void GetItem_NullKey_ArgumentNullException(int? keyCache)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int?, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act - Assert       
            Assert.Throws<ArgumentNullException>(() => cacheService.GetItem(keyCache));

            storageMock.Verify(s => s.Get(It.Is<int?>(key => key == keyCache)), Times.Never);
        }

        [TestCase(11)]
        public void GetItem_NonexistentKey_CachedItemNotFoundException(int? keyCache)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) }
            };

            Mock<IStorage<int?, string>> storageMock;

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act - Assert       
            Assert.Throws<CachedItemNotFoundException>(() => cacheService.GetItem(keyCache));

            storageMock.Verify(s => s.Get(It.Is<int?>(key => key == keyCache)), Times.Never);
        }

        [TestCase(2)]
        public void GetItem_KeyOfExpiredValue_ExpirationTimeException(int? keyCache)
        {
            // Arrange 
            var cachedItems = new Dictionary<int?, CacheItemModel<string>>()
            {
                [1] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(120) },
                [2] = new CacheItemModel<string>() { Value = "test", LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(1) }
            };

            Mock<IStorage<int?, string>> storageMock; 

            var cacheService = GetCacheServiceWithDefaultCapacity(cachedItems, out storageMock);

            // Act - Assert 
            Task.Delay(1000).Wait();

            Assert.Throws<ExpirationTimeException>(() => cacheService.GetItem(keyCache));

            storageMock.Verify(s => s.Get(It.Is<int?>(key => key == keyCache)), Times.Once);
        }

        #endregion GetItem tests

        private CacheService<TKey, TValue> GetCacheServiceWithDefaultCapacity<TKey, TValue>(Dictionary<TKey, CacheItemModel<TValue>> cachedItems, out Mock<IStorage<TKey, TValue>> storageMock)
        {
            storageMock = GetStorageMock(cachedItems);

            Mock<StorageFactory<TKey, TValue>>  storageFactoryMock = GetStorageFactoryMock(storageMock);

            return new CacheService<TKey, TValue>(storageFactoryMock.Object);
        }

        private Mock<StorageFactory<TKey, TValue>> GetStorageFactoryMock<TKey, TValue>(Mock<IStorage<TKey, TValue>> storageMock)
        {
            Mock<StorageFactory<TKey, TValue>> storageFactoryMock = new Mock<StorageFactory<TKey, TValue>>();     

            storageFactoryMock.Setup(factory => factory.CreateStorage(It.IsAny<int>(), EqualityComparer<TKey>.Default)).Returns(storageMock.Object);

            return storageFactoryMock;
        }

        private Mock<IStorage<TKey, TValue>> GetStorageMock<TKey, TValue>(Dictionary<TKey, CacheItemModel<TValue>> cachedItems)
        {
            Mock<IStorage<TKey, TValue>> storageMock = new Mock<IStorage<TKey, TValue>>();

            storageMock.SetupGet(s => s.CachedItemCount).Returns(cachedItems.Count);

            storageMock.Setup(s => s.Add(It.IsAny<TKey>(), It.IsAny<TValue>(), It.IsAny<int>()))
                .Callback<TKey, TValue, int>((key, value, lifeTime) => cachedItems.Add(key, new CacheItemModel<TValue>() { Value = value, LastAccessTime = DateTime.Now, ExpirationTime = DateTime.Now.AddSeconds(lifeTime) }));

            storageMock.Setup(s => s.Remove(It.IsAny<TKey>())).Callback<TKey>(key => cachedItems.Remove(key));

            storageMock.Setup(s => s.IsCachedItem(It.IsAny<TKey>())).Returns<TKey>(key => cachedItems.ContainsKey(key));

            storageMock.Setup(s => s.Get(It.IsAny<TKey>())).Returns<TKey>(key => cachedItems[key]);

            storageMock.Setup(s => s.GetAll()).Returns(() => cachedItems);

            return storageMock;
        }
    }
}
