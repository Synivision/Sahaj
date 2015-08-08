using Assets.Code.UnityBehaviours;

public delegate void OnKilledEventHandler();
public delegate void OnCurrentHealthChangedEventHandler(float oldValue, float newValue, float delta);

public class StatsBehaviour : InitializeRequiredBehaviour {
	public StatBlock Block;
	
	private float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            // cap our health values
            if (value < 0)
                value = 0;
            if (value > Block.MaximumHealth)
                value = Block.MaximumHealth;

            // let everyone know
            if (OnCurrentHealthChangedEvent != null)
                OnCurrentHealthChangedEvent(_currentHealth, value, value - _currentHealth);

            // set health
            _currentHealth = value;

            // die if we need to
            if (_currentHealth <= 0)
                Kill();
        }
    }
    public OnCurrentHealthChangedEventHandler OnCurrentHealthChangedEvent;

    public bool IsDead { get; private set; }
    public OnKilledEventHandler OnKilledEvent;

	private float _currentCourage;

    public void Initialize(StatBlock block)
    {
        Block = block;

        _currentHealth = block.MaximumHealth;
        _currentCourage = block.MaximumCourage;

        MarkAsInitialized();
    }

    public void Regenerate(){}
	
	public void Courage(){}
	
	public void UpgradeHealth(float newHeallth){
		Block.MaximumHealth = newHeallth;
	}

    public void Kill()
    {
        IsDead = true;
        if (OnKilledEvent != null)
            OnKilledEvent();
    }
}
