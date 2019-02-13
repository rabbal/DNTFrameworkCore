using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Hooks
{
    public class HookEntityMetadata
    {
        private EntityState _state;

        public HookEntityMetadata(EntityState state)
        {
            _state = state;
        }

        public EntityState State
        {
            get => _state;
            set
            {
                if (_state == value) return;

                _state = value;
                HasStateChanged = true;
            }
        }

        public bool HasStateChanged { get; private set; }
    }
}