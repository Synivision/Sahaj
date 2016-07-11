
namespace Assets.Code.Messaging{
	public class UpdateGamePlayUiMessage : IMessage{
		public int Gold;
		public int ExperiencePoints;
		public int availableGold;
	}
}