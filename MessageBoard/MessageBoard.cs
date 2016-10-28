using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SQLite;

namespace MessageBoard
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}

	public class App : Application
	{
		public App()
		{
			// The root page of your application
			MainPage = new NavigationPage(new EditPage())
			{
			};
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}

	[Table("History")]
	public class Message
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set;}
		public string text { get; set; }
	};


	//モデル
	public class MessageStorage
	{
		//Singleton
		private static MessageStorage _instance = new MessageStorage();
		SQLiteConnection database;
		public static MessageStorage getInstance()
		{
			return _instance;
		}

		//member
		public List<Message> messages = new List<Message>();

		//init
		public MessageStorage()
		{
			database = DependencyService.Get<ISQLite>().GetConnection();
			database.CreateTable<Message>();

			var table = database.Table<Message>();
			foreach (var s in table)
			{
				messages.Add(s);
			}
		}

		//methods
		public void addMessage(Message msg)
		{
			try
			{
				database.Insert(msg);
			}
			catch (SQLiteException e)
			{
				return;
			}
			this.messages.Add(msg);
		}

		public void clear()
		{
			messages.Clear();
			database.DeleteAll<Message>();
		}
		//public void removeMessage(int i)
		//{
		//	this.messages.RemoveAt(i);
		//}
	}


	//文字表示用page
	class DisplayPage : CarouselPage
	{
		public DisplayPage()
		{
			//nav bar抑止
			NavigationPage.SetHasNavigationBar(this, false);

			Children.Clear();

			//追加
			for (int i = 0; i < MessageStorage.getInstance().messages.Count; i++)
			{
				Children.Add(new LabelPage(MessageStorage.getInstance().messages[i].text));
			}
			//初期ページ
			Children.Add(new LabelPage("つくばライフは超楽しい"));

			//表示ページへの移動
			if (MessageStorage.getInstance().messages.Count == 0)
			{
				CurrentPage = Children[0];
			}
			else
			{
				CurrentPage = Children[MessageStorage.getInstance().messages.Count - 1];
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}

	//文字表示用　個別page
	class LabelPage : ContentPage
	{
		public LabelPage(String text)
		{
			//表示Label
			Content = new Label
			{
				BackgroundColor = Color.Black,
				TextColor = Color.FromHex("#ffffff"),
				FontSize = 45,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				FontFamily = "Hiragino Kaku Gothic ProN W6",
				FontAttributes = FontAttributes.Bold,
				Text = text
			};

			//タップ遷移
			var gr = new TapGestureRecognizer(); // ←1
			gr.Tapped += (s, e) =>
			{
				Navigation.PopAsync();
			};
			Content.GestureRecognizers.Add(gr);

		}

	}

	//編集page
	class EditPage : ContentPage
	{
		private Entry textfield;

		public EditPage()
		{
			//nav bar抑止
			NavigationPage.SetHasNavigationBar(this, false);

			//textfield
			textfield = new Entry
			{
				Placeholder = "Message",
			};
			//表示時にtextfieldに自動フォーカス
			Appearing += (sender, e) =>
			{
				textfield.Focus();
			};
			//エンターで表示pageへ
			textfield.Completed += EntryCompleted;

			//ボタン
			var clearButton = new Button()
			{
				Text = "履歴を消去します"
			};
			clearButton.Clicked += async (s, e) =>
			{
				//DisplayAlertの表示
				var result = await DisplayAlert("履歴を消去します", "よろしいですか？", "OK", "キャンセル");
				if (result == true)
				{
					textfield.Text = "";
					MessageStorage.getInstance().clear();
					//実行後にフォーカス合わせ
					textfield.Focus();
				}
			};

			//Layout
			Content = new StackLayout
			{
				Children = { textfield, clearButton }
			};
		}

		//エンター時の挙動
		void EntryCompleted(object sender, EventArgs e)
		{
			if (textfield.Text != null && textfield.Text.Length != 0)
			{
				var newMesssage = new Message();
				newMesssage.text = textfield.Text;
				MessageStorage.getInstance().addMessage(newMesssage);
			}
			Navigation.PushAsync(new DisplayPage());
		}
	}
}