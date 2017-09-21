using PSKcore.AppModel;

namespace PSKcore.Interface
{
    public interface ICore
    {
        CurrentUser CurrentUser { get; }
        ATT_INFO Databridge { get; set; }
        bool isRegisted { get; }

        void DeleteUser();
        void Regist(CurrentUser user);
        void Unsubscribe();
    }
}