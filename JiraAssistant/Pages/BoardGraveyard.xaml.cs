using JiraAssistant.Logic.ViewModels;

namespace JiraAssistant.Pages
{
   public partial class BoardGraveyard
   {

      public BoardGraveyard(BoardGraveyardViewModel viewModel)
      {
         InitializeComponent();

         DataContext = viewModel;
      }

      public override void OnNavigatedTo()
      {
         (DataContext as BoardGraveyardViewModel).RefreshGraveyard();
      }

      public override string Title { get { return "Issues graveyard"; } }
   }
}
