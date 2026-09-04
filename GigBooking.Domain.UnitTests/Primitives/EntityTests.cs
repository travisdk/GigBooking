using GigBooking.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GigBooking.Domain.UnitTests.Primitives;

public class EntityTests
{
    public class MyEntity : Entity<Guid>
    {
        public MyEntity(Guid id) : base(id)

        {
        }

    }
    public class AnotherEntity : Entity<Guid>
    {
        public AnotherEntity(Guid id) : base(id)

        {
        }

    }
    [Fact]
    public void EqualOperator_WhenEntitiesAreEqual_ReturnsTrue() {

        var baseGuid = Guid.NewGuid();
        MyEntity a = new MyEntity(baseGuid);
        MyEntity b = new MyEntity(baseGuid);

        Assert.True(a == b);
        Assert.True(b == a);

    }
    [Fact]
    public void EqualOperator_WhenEntitiesAreNotEqual_ReturnsFalse()
    {

        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        MyEntity a = new MyEntity(guid1);
        MyEntity b = new MyEntity(guid2);

        Assert.False(a == b);
        Assert.False(b == a);

    }

    [Fact]
    public void NotEqualOperator_WhenEntitiesAreEqual_ReturnsFalse()
    {

        var baseGuid = Guid.NewGuid();
        MyEntity a = new MyEntity(baseGuid);
        MyEntity b = new MyEntity(baseGuid);

        Assert.False(a != b);
        Assert.False(b != a);

    }
    [Fact]
    public void NotEqualOperator_WhenEntitiesAreNotEqual_ReturnsTrue()
    {

        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        MyEntity a = new MyEntity(guid1);
        MyEntity b = new MyEntity(guid2);

        Assert.True(a != b);
        Assert.True(b != a);

    }

    [Fact]
    public void EqualsMethod_WhenEntitiesAreEqual_ReturnsTrue()
    {
        var baseGuid = Guid.NewGuid();
        MyEntity a = new MyEntity(baseGuid);
        MyEntity b = new MyEntity(baseGuid);

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));

    }

    [Fact]
    public void EqualsMethod_WhenEntitiesAreNotEqual_ReturnsFalse()
    {

        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        MyEntity a = new MyEntity(guid1);
        MyEntity b = new MyEntity(guid2);

        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));

    }
    [Fact]
    public void GetHasCode_WhenEntitiesAreEqual_ReturnsSameValue()
    {
        var guid = Guid.NewGuid();

        MyEntity a = new MyEntity(guid);
        MyEntity b = new MyEntity(guid);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());

    }

    [Fact]
    public void Equality_WhenComparingWithNull_ReturnsFalse()
    {

        MyEntity myEntity = new MyEntity(Guid.NewGuid());
        MyEntity? myNullEntity = null;

        Assert.False(myEntity == myNullEntity);
        Assert.False(myNullEntity == myEntity);
        Assert.True(myEntity != myNullEntity);
        Assert.True(myNullEntity != myEntity);

        Assert.False(myEntity.Equals((object?)null));
        Assert.False(myEntity.Equals((MyEntity?)null));
    }


    [Fact]
    public void Equality_WhenBothAreNull_ReturnsTrue() {
        
        MyEntity? myEntity1 = null;
        MyEntity? myEntity2 = null;
        Assert.True(myEntity1 == myEntity2);
        Assert.False(myEntity1 != myEntity2);
        
    }

    [Fact]

    public void Equals_WhenDifferentTypes_ReturnsFalse()
    {
        MyEntity myEntity = new MyEntity(new Guid());
        string testObject = "A TEST";
        Assert.False(myEntity.Equals(testObject));

    }


    [Fact]
    public void Equals_WhenSameInstance_ReturnsTrue()
    {
        MyEntity myEntity = new MyEntity(new Guid());
        Assert.True(myEntity.Equals(myEntity));

    }


    [Fact]
    public void Equals_WhenDifferentEntitiesWithSameId_ReturnsFalse()
    {
        var guid = Guid.NewGuid();
        MyEntity myEntity = new MyEntity(guid);
        AnotherEntity anotherEntity = new AnotherEntity(guid);

        Assert.False(myEntity == anotherEntity);
        Assert.True(myEntity != anotherEntity);
        Assert.False(myEntity.Equals(anotherEntity));
    
    }
    [Fact]
    public void Id_ReturnsCorrectValue()
    {
        var expectedId = Guid.NewGuid();
        MyEntity myEntity = new MyEntity(expectedId);
        Assert.Equal(expectedId, myEntity.Id);

    }
}
