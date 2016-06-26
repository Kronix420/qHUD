namespace qHUD.Controllers
{
    using System.Collections.Generic;
    using Framework;
    using Models;
    using Poe.RemoteMemoryObjects;
    public class GameController
    {
        public GameController(Memory memory)
        {
            Memory = memory;
            Area = new AreaController(this);
            EntityListWrapper = new EntityListWrapper(this);
            Window = new GameWindow(memory.Process);
            Game = new TheGame(memory);
            Files = new FsController(memory);
        }

        public EntityListWrapper EntityListWrapper { get; }
        public GameWindow Window { get; private set; }
        public TheGame Game { get; }
        public AreaController Area { get; }
        public Memory Memory { get; private set; }
        public IEnumerable<EntityWrapper> Entities => EntityListWrapper.Entities;
        public EntityWrapper Player => EntityListWrapper.Player;
        public bool InGame => Game.IngameState.InGame;
        public FsController Files { get; private set; }
        public void RefreshState()
        {
            if (!InGame) return;
            EntityListWrapper.RefreshState();
            Area.RefreshState();
        }
    }
}