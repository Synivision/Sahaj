
public delegate void OnMaximumHealthChangedEventHandler(float oldValue, float newValue, float delta);
public delegate void OnMaximumCourageChangedEventHandler(float oldValue, float newValue, float delta);
public delegate void OnMaximumDamageChangedEventHandler(float oldValue, float newValue, float delta);

public class StatBlock
{
    private float _maximumHealth;
    public float MaximumHealth
    {
        get
        {
            return _maximumHealth;
        }
        set
        {
            _maximumHealth = value;

            if (OnMaximumHealthChangedEvent != null)
                OnMaximumHealthChangedEvent(_maximumHealth, value, value - _maximumHealth);
        }
    }
    public OnMaximumHealthChangedEventHandler OnMaximumHealthChangedEvent;

    private float _maximumCourage;
    public float MaximumCourage
    {
        get
        {
            return _maximumCourage;
        }
        set
        {
            _maximumCourage = value;

            if (OnMaximumCourageChangedEvent != null)
                OnMaximumCourageChangedEvent(_maximumCourage, value, value - _maximumCourage);
        }
    }
    public OnMaximumCourageChangedEventHandler OnMaximumCourageChangedEvent;

    private float _maximumDamage;
    public float MaximumDamage
    {
        get { return _maximumDamage; }
        set
        {
            _maximumDamage = value;

            if (OnMaximumDamageChangedEvent != null)
                OnMaximumDamageChangedEvent(_maximumDamage, value, value - _maximumDamage);
        }
    }
    public OnMaximumDamageChangedEventHandler OnMaximumDamageChangedEvent;
}