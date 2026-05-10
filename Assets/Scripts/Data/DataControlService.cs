using Assets.Scripts.Managers;

namespace Assets.Scripts.Data
{
    public class DataControlService
    {
        private readonly SaveManager _saveManager;
        private AllData _current;
        public AllData Current => _current;

        public DataControlService(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }

        public void Load()
        {
            _current = _saveManager.Load();
        }

        public void Save()
        {
            _saveManager.Save(_current);
        }

        public void Update(AllData data) 
        {
            _current = data;
        }
    }
}
