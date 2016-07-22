namespace qHUD.Controllers
{
    using System;
    using Models;
    using Poe.RemoteMemoryObjects;
    public class AreaController
    {
        private readonly GameController Root;
        public AreaController(GameController gameController) { Root = gameController; }
        public event Action<AreaController> OnAreaChange;
        public AreaInstance CurrentArea { get; private set; }
        public void RefreshState()
        {
            var igsd = Root.Game.IngameState.Data;
            AreaTemplate clientsArea = igsd.CurrentArea;
            int curAreaHash = igsd.CurrentAreaHash;
            if (CurrentArea != null && curAreaHash == CurrentArea.Hash) return;
            CurrentArea = new AreaInstance(clientsArea, curAreaHash, igsd.CurrentAreaLevel);
            OnAreaChange?.Invoke(this);
        }
    }
}