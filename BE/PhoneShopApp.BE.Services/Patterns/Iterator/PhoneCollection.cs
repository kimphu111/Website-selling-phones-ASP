using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Services.Patterns.Iterator;

public class PhoneIterator : IIterator<Phone>
{
    private readonly List<Phone> _phones;
    private int _position = 0;

    public PhoneIterator(List<Phone> phones)
    {
        _phones = phones;
    }

    public bool HasNext()
    {
        return _position < _phones.Count;
    }

    public Phone Next()
    {
        return _phones[_position++];
    }
}

public class PhoneCollection : IPhoneCollection
{
    private readonly List<Phone> _phones;

    public PhoneCollection(List<Phone> phones)
    {
        _phones = phones;
    }

    public IIterator<Phone> CreateIterator()
    {
        return new PhoneIterator(_phones);
    }
}
