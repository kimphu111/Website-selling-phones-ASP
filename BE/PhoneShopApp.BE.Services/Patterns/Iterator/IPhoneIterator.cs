namespace PhoneShopApp.BE.Services.Patterns.Iterator;

public interface IIterator<T>
{
    bool HasNext();
    T Next();
}

public interface IPhoneCollection
{
    IIterator<PhoneShopApp.BE.Core.Entities.Phone> CreateIterator();
}
