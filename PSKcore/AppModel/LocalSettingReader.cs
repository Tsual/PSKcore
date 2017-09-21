using PSKcore.DbModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PSKcore.AppModel
{
    public class LocalSettingReader
    {
        private LocalSettingReader() { }

        static LocalSettingReader()
        {
            APPDbContext db = new APPDbContext();
            db.Database.EnsureCreated();
            foreach(var t in db.LocalSettings)
            {
                LocalSetting set = t;
                set.LocalSettingChanged += Set_LocalSettingChanged;
                _settings.Add(set);
            }
            _settings.CollectionChanged += _settings_CollectionChanged;



        }

        private static void _settings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            APPDbContext db = new APPDbContext();
            if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var t in e.NewItems)
                    db.Entry((t as LocalSetting)?.dbsetting).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            }
            else if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var t in e.OldItems)
                    db.Entry((t as LocalSetting)?.dbsetting).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            else if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                var _old = db.LocalSettings.Find((e.OldItems[0] as LocalSetting)?.ID);
                var _listnew = e.NewItems[0] as LocalSetting;
                _old.Key = _listnew?.Key;
                _old.Value = _listnew?.Value;
                db.Entry(_old).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            db.SaveChanges();
        }

        private static void Set_LocalSettingChanged(LocalSetting setting)
        {
            APPDbContext db = new APPDbContext();
            db.Entry(setting.dbsetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
        }

        static ObservableCollection<LocalSetting> _settings = new ObservableCollection<LocalSetting>();
        private static LocalSettingReader _Current = new LocalSettingReader();

        public IList<LocalSetting> Settings { get => _settings; }
        public static LocalSettingReader Current { get => _Current;  }

        public LocalSetting this[string Key]
        {
            get {
                var linq=from t in _settings
                           where t.Key == Key
                           select t;
                if (linq.Count() > 0) return linq.ElementAt(0);
                return null;
            }
        }

        public bool ContainsKey(string key)
        {
            return (from t in _settings
                    where t.Key == key &&t.Value!=""&&t.Value!=null
                    select t).Count() > 0;
        }

        public bool ContainsValue(string value)
        {
            return (from t in _settings
                    where t.Value == value
                    select t).Count() > 0;
        }


    }


}
