using Game.State;

namespace Game.Logic
{
    [SubSystem]
    public class System : IBootable
    {
        public RootState State { get; private set; } = new RootState();

        public NewGameLogic NewGameLogic { get; private set; }
        public CellLogic CellLogic { get; private set; }
        public OperationsLogic OperationsLogic { get; private set; }
        public TimeLogic TimeLogic { get; private set; }
        public RemainingBombLogic RemainingBombLogic { get; private set; }

        public void Boot()
        {
            NewGameLogic = F.Create<NewGameLogic>(this);
            CellLogic = F.Create<CellLogic>(this);
            OperationsLogic = F.Create<OperationsLogic>(this);
            TimeLogic = F.Create<TimeLogic>(this);
            RemainingBombLogic = F.Create<RemainingBombLogic>(this);
        }
    }
}