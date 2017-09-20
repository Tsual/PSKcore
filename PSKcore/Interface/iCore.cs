using PSKcore.AppModel;

namespace PSKcore.Interface
{
    public interface ICore
    {
        void Regist(CurrentUser user);
        void Unsubscribe();
        CurrentUser CurrentUser { get; }
        bool isRegisted { get; }
        void DeleteUser();
        ATT_INFO Databridge { get; set; }
    }
}
