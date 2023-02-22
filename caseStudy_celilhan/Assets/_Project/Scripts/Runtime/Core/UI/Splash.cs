using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Runtime.Core.UI
{
    public class Splash : UIBase
    {
        public override async UniTask SetUI(bool state)
        {
            await UniTask.Yield();
        }
    }
}