using System;
using CodeHub.iOS.Elements;
using CodeFramework.ViewControllers;
using CodeHub.Core.ViewModels;
using UIKit;
using CodeFramework.iOS.Utils;
using MvvmCross.Binding.BindingContext;

namespace CodeHub.iOS.Views.Repositories
{
    public class RepositoriesExploreView : ViewModelCollectionDrivenDialogViewController
    {
		private Hud _hud;

		public RepositoriesExploreView()
        {
            AutoHideSearch = false;
            //EnableFilter = true;
            NoItemsText = "No Repositories".t();
            Title = "Explore".t();
        }

        protected override IUISearchBarDelegate CreateSearchDelegate()
        {
            return null;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			_hud = new Hud(View);
			var vm = (RepositoriesExploreViewModel)ViewModel;
            var search = (UISearchBar)TableView.TableHeaderView;

			var set = this.CreateBindingSet<RepositoriesExploreView, RepositoriesExploreViewModel>();
			set.Bind(search).For(x => x.Text).To(x => x.SearchText);
			set.Apply();

			search.SearchButtonClicked += (sender, e) =>
			{
				search.ResignFirstResponder();
				vm.SearchCommand.Execute(null);
			};

			vm.Bind(x => x.IsSearching, x =>
			{
				if (x)
					_hud.Show("Searching...");
				else
					_hud.Hide();
			});

			BindCollection(vm.Repositories, repo =>
            {
				var description = vm.ShowRepositoryDescription ? repo.Description : string.Empty;
                var imageUrl = repo.Owner?.AvatarUrl;
				var sse = new RepositoryElement(repo.Name, repo.StargazersCount, repo.ForksCount, description, repo.Owner.Login, imageUrl) { ShowOwner = true };
				sse.Tapped += () => vm.GoToRepositoryCommand.Execute(repo);
                return sse;
            });
        }
    }
}
